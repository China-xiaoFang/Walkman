# 概念随听官网

## 文件说明

- `index.html`：部署版首页，引用 `assets` 目录中的本地资源。
- `css/index.css`：跨主题共享的默认基础、布局和组件样式。
- `css/dark.css`：默认深色主题调色板。
- `css/light.css`：跟随系统浅色偏好的主题覆盖样式。
- `css/responsive.css`：按视口宽度生效的响应式样式与减少动画适配。
- `js/index.js`：页面菜单、交互与渐入动画逻辑。
- `favicon.ico`：浏览器图标。
- `assets/logo.png`：网站与微信小程序品牌 Logo。
- `assets/gongan.png`：公安联网备案通用标准图标。
- `assets/android.svg`：重新设计的 Android 品牌机器人图标。

## 部署

将 `index.html`、`css`、`js`、`assets`、`favicon.ico` 一起部署到网站根目录。

## 跳转与下载配置

`js/index.js` 顶部的 `siteConfig` 用于配置微信小程序、Android 和 iOS 入口：

```javascript
const siteConfig = Object.freeze({
  miniProgramUrlScheme: "",
  androidApkUrl: "",
  iosAppStoreUrl: "",
});
```

交付预览中三个地址均保持为空。地址为空时，按钮不跳转、不弹出配置提示，也不展示或复制小程序 AppID。部署前自行填写实际地址。

### 微信小程序

- `miniProgramUrlScheme`：填写微信后台生成的 URL Scheme，例如 `weixin://dl/business/?t=...`。
- 页面通过 URL Scheme 直接唤起微信小程序，不使用 URL Link，也不依赖公众号或微信 JS-SDK。

### Android

- `androidApkUrl`：填写 APK 文件的直接下载地址，例如 `/downloads/concept-listening.apk`。
- 点击“Android 版”下载按钮后直接发起 APK 下载，不应填写应用介绍页地址。
- 建议服务器返回 `Content-Type: application/vnd.android.package-archive`；需要强制下载时可同时配置 `Content-Disposition: attachment`。
- 当前入口是 APK 直下而不是 Google Play，因此页面不使用 Google Play 下载徽章。

### iOS

- `iosAppStoreUrl`：填写 App Store 应用详情地址，例如 `https://apps.apple.com/cn/app/idXXXXXXXXXX`。
- 点击“iPhone / iPad 版”按钮后在新窗口打开 App Store 应用详情页。

## 下载区域与顶部导航

- 顶部不展示独立的“微信小程序”菜单。
- 顶部“下载”菜单直接定位到 App Store 与 Android APK 下载卡片。
- “从今天开始，把‘听过’变成‘真正听懂’”卡面不展示任何下载入口，仅保留原有“返回顶部”按钮。
- 移动端右上角菜单支持点击打开、菜单外关闭、选择菜单后关闭与 `Esc` 关闭。

## Android 图标

- Android 平台卡与 APK 下载按钮统一使用本地 `assets/android.svg`。
- 图标采用 Android 绿色渐变、机器人头部和高对比眼睛，在浅色与深色模式下均保持清晰。
- 不依赖在线图标库或第三方 CDN，单文件版本会自动内嵌该 SVG。

## 浅色与深色主题

- 页面使用 CSS `prefers-color-scheme` 自动跟随设备或浏览器的浅色/深色主题。
- 不使用 JavaScript、Cookie 或本地存储保存主题；系统主题变化时页面自动切换。
- 浅色主题下播放器预览、底部行动卡、微信小程序平台卡、Android/iPhone 下载卡及其他内容卡均使用浅色表面。
- 深色主题保留深海蓝、冰蓝与靛青层次。
- Android 与 iOS 下载按钮会跟随浅色、深色主题切换。

## 版权与备案

页脚将版权与备案信息放在同一合规区域，年份由浏览器自动获取：

```text
Copyright © {当前年份} 概念随听 All rights reserved.
```

备案展示顺序：

1. 公安备案：`苏公网安备32132202001554号`
2. ICP 备案：`苏ICP备2026049456号`

两个备案号均配置为可点击的新窗口查询链接，公安备案使用本地通用标准图标。
