[**简体中文**](DEPLOYMENT.zh.md) | [English](DEPLOYMENT.md)

# Nginx 反向代理部署模板

本文提供 `Web.Admin`、主 API 和文件服务的通用 Nginx HTTPS 反向代理模板。示例中的项目名称、域名、服务器地址、证书路径、日志路径和站点目录均为脱敏占位值，不对应任何真实生产环境。

> 本模板仅作为部署参考，不能替代针对实际网络、权限、证书、跨域、上传限制和安全策略的独立审核。生产使用前请阅读项目的[免责声明与合规要求](../README.zh.md#免责声明与合规要求)，并通过 `nginx -t` 校验最终配置。

## 默认服务映射

| 外部路径        | 默认上游                       | 说明                             |
| --------------- | ------------------------------ | -------------------------------- |
| `/`             | `/var/www/fast-admin`          | `Web.Admin` 构建后的静态文件     |
| `/api/`         | `http://127.0.0.1:38081/`      | 主 API，代理时移除 `/api/` 前缀  |
| `/api/file/`    | `http://127.0.0.1:38082/file/` | 文件服务，必须放在 `/api/` 之前  |
| `/api/hubs/`    | `http://127.0.0.1:38081/hubs/` | SignalR/WebSocket                |
| `/CallbackApi/` | `http://127.0.0.1:38081/`      | 可选回调入口，不需要时可整段删除 |

端口取自当前仓库的默认启动配置。部署时建议只允许 Nginx 所在主机或受信网络访问上游服务，不要将应用端口直接暴露到公网。

## 配置模板

将以下内容保存为站点配置文件，例如 `/etc/nginx/conf.d/fast-admin.conf`。`map` 指令必须位于 Nginx 的 `http` 上下文中；常见发行版的 `conf.d/*.conf` 默认就在该上下文内加载。

```nginx
# WebSocket Connection 请求头映射，必须位于 http 上下文。
map $http_upgrade $connection_upgrade {
    default upgrade;
    ''      close;
}

# HTTP 统一跳转到 HTTPS。
server {
    listen 80;
    listen [::]:80;
    server_name admin.example.com;

    return 301 https://$host$request_uri;
}

server {
    listen 443 ssl;
    listen [::]:443 ssl;
    server_name admin.example.com;

	charset utf-8;
    server_tokens off;

    access_log /var/log/nginx/fast-admin.access.log;
    error_log /var/log/nginx/fast-admin.error.log warn;

    ssl_certificate /etc/nginx/ssl/admin.example.com/fullchain.pem;
    ssl_certificate_key /etc/nginx/ssl/admin.example.com/privkey.pem;
    ssl_session_timeout 30m;
    ssl_session_cache shared:FAST_ADMIN_SSL:10m;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers TLS_AES_128_GCM_SHA256:TLS_AES_256_GCM_SHA384:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers on;

    # 按实际业务和文件上传限制调整。
    client_max_body_size 100m;

    # HTTPS 下关闭实时压缩，仅提供构建阶段生成的 .gz 预压缩文件。
    gzip off;
    gzip_static on;

    # 公共反向代理请求头。
	proxy_http_version 1.1;
	proxy_set_header Host $host;
	proxy_set_header X-Real-IP $remote_addr;
	proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
	proxy_set_header X-Forwarded-Host $host;
	proxy_set_header X-Forwarded-Proto $scheme;

    # SignalR/WebSocket，必须放在通用 /api/ 规则之前。
    location ^~ /api/hubs/ {
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Host $host;
        proxy_set_header X-Forwarded-Proto $scheme;

		proxy_set_header Upgrade $http_upgrade;
		proxy_set_header Connection $connection_upgrade;

		proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 3600s;
        proxy_send_timeout 3600s;

        proxy_pass http://127.0.0.1:38081/hubs/;
    }

    # 可选回调接口，不使用时删除此 location。
    location ^~ /CallbackApi/ {
        proxy_pass http://127.0.0.1:38081/;
    }

    # 文件服务必须放在通用 /api/ 规则之前。
    location ^~ /api/file/ {
        proxy_pass http://127.0.0.1:38082/file/;
    }

    # 主 API。
    location ^~ /api/ {
        proxy_pass http://127.0.0.1:38081/;
    }

    # Web.Admin 单页应用。
    location / {
        root /var/www/fast-admin;
        index index.html;
        try_files $uri $uri/ /index.html;
    }

    error_page 500 502 503 504 /50x.html;
    location = /50x.html {
        root /usr/share/nginx/html;
    }
}
```

## 必须替换或确认的内容

- 将 `admin.example.com` 替换为已完成 DNS 解析的实际域名。
- 将证书和私钥路径替换为服务器上的真实路径，并限制私钥文件读取权限。
- 将 `/var/www/fast-admin` 替换为 `Web.Admin` 构建产物的实际部署目录。
- 如果 API 与 Nginx 不在同一主机，将 `127.0.0.1` 替换为受信内网地址，并通过防火墙或安全组限制访问来源。
- 根据部署配置确认主 API、文件服务端口及 `/api/file/` 的路径重写规则。
- 根据允许上传的文件类型和大小调整 `client_max_body_size`，同时保持前后端限制一致。
- 只有构建产物包含 `.gz` 文件且 Nginx 支持 `gzip_static` 模块时，才保留 `gzip_static on`。

## 跨域与安全说明

模板没有添加全局 `Access-Control-Allow-Origin: *`。同域部署通常不需要额外 CORS 响应头；跨域部署应在服务端配置明确的允许来源、方法、请求头和凭据策略，避免由 Nginx 与应用重复返回或放宽跨域头。

不要在仓库、Issue、日志或公开截图中提交真实证书私钥、Token、密码、内网拓扑、数据库连接信息或其他生产凭据。

## 启用与验证

```bash
sudo nginx -t
sudo systemctl reload nginx
```

部署后至少验证：

- HTTP 是否正确跳转到 HTTPS，证书链和域名是否匹配。
- 刷新任意前端路由时是否仍返回 `index.html`。
- `/api/`、`/api/file/` 和可选回调接口的路径重写是否正确。
- `/api/hubs/chatHub` 是否能够建立并保持 WebSocket 连接。
- 大文件上传、超时、日志轮转、防火墙和上游服务异常时的行为是否符合预期。
