[简体中文](DEPLOYMENT.zh.md) | [**English**](DEPLOYMENT.md)

# Nginx Reverse Proxy Deployment Template

This document provides a generic Nginx HTTPS reverse proxy template for `Web.Admin`, the main API, and the file service. Project names, domains, server addresses, certificate paths, log paths, and site directories in the example are sanitized placeholders and do not refer to a real production environment.

> This template is deployment guidance only. It does not replace an independent review of the target network, permissions, certificates, CORS policy, upload limits, and security controls. Before production use, read the project [Disclaimer and compliance requirements](../README.md#disclaimer-and-compliance-requirements) and validate the final configuration with `nginx -t`.

## Default Service Mapping

| Public path     | Default upstream               | Purpose                                             |
| --------------- | ------------------------------ | --------------------------------------------------- |
| `/`             | `/var/www/fast-admin`          | Static files built from `Web.Admin`                 |
| `/api/`         | `http://127.0.0.1:38081/`      | Main API with the `/api/` prefix removed            |
| `/api/file/`    | `http://127.0.0.1:38082/file/` | File service; must precede the general `/api/` rule |
| `/api/hubs/`    | `http://127.0.0.1:38081/hubs/` | SignalR/WebSocket                                   |
| `/CallbackApi/` | `http://127.0.0.1:38081/`      | Optional callback entry; remove when unused         |

The ports come from the repository's current default launch settings. In deployment, allow upstream access only from the Nginx host or a trusted network instead of exposing application ports directly to the Internet.

## Configuration Template

Save the following content as a site configuration such as `/etc/nginx/conf.d/fast-admin.conf`. The `map` directive must be loaded inside the Nginx `http` context; common distributions load `conf.d/*.conf` there by default.

```nginx
# WebSocket Connection header mapping. This must be in the http context.
map $http_upgrade $connection_upgrade {
    default upgrade;
    ''      close;
}

# Redirect HTTP traffic to HTTPS.
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

    # Adjust to match the intended business and file-upload limits.
    client_max_body_size 100m;

    # Disable real-time compression on HTTPS and serve precompressed .gz assets.
    gzip off;
    gzip_static on;

    # Common reverse proxy headers.
	proxy_http_version 1.1;
	proxy_set_header Host $host;
	proxy_set_header X-Real-IP $remote_addr;
	proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
	proxy_set_header X-Forwarded-Host $host;
	proxy_set_header X-Forwarded-Proto $scheme;

    # SignalR/WebSocket; keep this before the general /api/ rule.
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

    # Optional callback entry. Remove this location when unused.
    location ^~ /CallbackApi/ {
        proxy_pass http://127.0.0.1:38081/;
    }

    # The file-service rule must precede the general /api/ rule.
    location ^~ /api/file/ {
        proxy_pass http://127.0.0.1:38082/file/;
    }

    # Main API.
    location ^~ /api/ {
        proxy_pass http://127.0.0.1:38081/;
    }

    # Web.Admin single-page application.
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

## Values to Replace or Confirm

- Replace `admin.example.com` with the real domain after its DNS record is ready.
- Replace the certificate and private-key paths with their actual server paths, and restrict private-key read permissions.
- Replace `/var/www/fast-admin` with the actual directory containing the `Web.Admin` build output.
- If the API and Nginx run on different hosts, replace `127.0.0.1` with a trusted private address and restrict access through a firewall or security group.
- Confirm the main API and file-service ports and the `/api/file/` rewrite behavior against the deployed configuration.
- Adjust `client_max_body_size` for the allowed upload types and sizes, keeping frontend and backend limits aligned.
- Keep `gzip_static on` only when the build output contains `.gz` files and Nginx includes the `gzip_static` module.

## CORS and Security Notes

The template does not add a global `Access-Control-Allow-Origin: *` header. Same-origin deployment normally needs no extra CORS response header. For cross-origin deployment, configure explicit allowed origins, methods, headers, and credential behavior in the server application so that Nginx and the application do not return duplicate or overly permissive headers.

Never commit real certificate private keys, tokens, passwords, internal network topology, database connection details, or other production credentials to the repository, an issue, a log, or a public screenshot.

## Enable and Verify

```bash
sudo nginx -t
sudo systemctl reload nginx
```

After deployment, verify at least the following:

- HTTP redirects to HTTPS and the certificate chain matches the domain.
- Refreshing any frontend route still returns `index.html`.
- Path rewriting for `/api/`, `/api/file/`, and the optional callback entry is correct.
- `/api/hubs/chatHub` can establish and maintain a WebSocket connection.
- Large uploads, timeouts, log rotation, firewall rules, and upstream failures behave as intended.
