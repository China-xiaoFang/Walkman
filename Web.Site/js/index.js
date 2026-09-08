"use strict";

const siteConfig = Object.freeze({
    miniProgramUrlScheme: "",
    androidApkUrl: "",
    iosAppStoreUrl: "",
});

const header = document.getElementById("siteHeader");
const navLinks = document.getElementById("navLinks");
const mobileMenuButton = document.getElementById("mobileMenuButton");
const toast = document.getElementById("toast");
const toastText = document.getElementById("toastText");
let toastTimer = 0;

document.getElementById("currentYear").textContent = String(
    new Date().getFullYear(),
);

const updateHeader = () => {
    header?.classList.toggle("is-scrolled", window.scrollY > 12);
};

updateHeader();
window.addEventListener("scroll", updateHeader, { passive: true });

const setMobileMenuState = (isOpen) => {
    if (!navLinks || !mobileMenuButton) return;

    navLinks.classList.toggle("is-open", isOpen);
    header?.classList.toggle("is-menu-open", isOpen);
    mobileMenuButton.setAttribute("aria-expanded", String(isOpen));
    mobileMenuButton.setAttribute(
        "aria-label",
        isOpen ? "关闭导航菜单" : "打开导航菜单",
    );
};

mobileMenuButton?.addEventListener("click", (event) => {
    event.preventDefault();
    event.stopPropagation();
    const shouldOpen = mobileMenuButton.getAttribute("aria-expanded") !== "true";
    setMobileMenuState(shouldOpen);
});

navLinks?.querySelectorAll("a, button").forEach((item) => {
    item.addEventListener("click", () => setMobileMenuState(false));
});

document.addEventListener("click", (event) => {
    if (
        navLinks?.classList.contains("is-open") &&
        event.target instanceof Node &&
        !navLinks.contains(event.target) &&
        !mobileMenuButton?.contains(event.target)
    ) {
        setMobileMenuState(false);
    }
});

document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") setMobileMenuState(false);
});

const desktopViewport = window.matchMedia("(min-width: 921px)");
const syncMenuWithViewport = (event) => {
    if (event.matches) setMobileMenuState(false);
};

if (typeof desktopViewport.addEventListener === "function") {
    desktopViewport.addEventListener("change", syncMenuWithViewport);
} else {
    desktopViewport.addListener(syncMenuWithViewport);
}

const showToast = (message) => {
    if (!toast || !toastText) return;

    window.clearTimeout(toastTimer);
    toastText.textContent = message;
    toast.classList.add("is-visible");
    toastTimer = window.setTimeout(
        () => toast.classList.remove("is-visible"),
        2200,
    );
};

const readConfiguredUrl = (value) => String(value ?? "").trim();

const openMiniProgram = () => {
    const targetUrl = readConfiguredUrl(siteConfig.miniProgramUrlScheme);
    if (!targetUrl) return;
    window.location.assign(targetUrl);
};

const downloadAndroidApk = () => {
    const targetUrl = readConfiguredUrl(siteConfig.androidApkUrl);
    if (!targetUrl) return;

    const downloadLink = document.createElement("a");
    downloadLink.href = targetUrl;
    downloadLink.download = "";
    downloadLink.rel = "noopener";
    document.body.appendChild(downloadLink);
    downloadLink.click();
    downloadLink.remove();
};

const openIosAppStore = () => {
    const targetUrl = readConfiguredUrl(siteConfig.iosAppStoreUrl);
    if (!targetUrl) return;

    const appStoreLink = document.createElement("a");
    appStoreLink.href = targetUrl;
    appStoreLink.target = "_blank";
    appStoreLink.rel = "noopener noreferrer";
    document.body.appendChild(appStoreLink);
    appStoreLink.click();
    appStoreLink.remove();
};

document.querySelectorAll("[data-open-mini-program]").forEach((button) => {
    button.addEventListener("click", openMiniProgram);
});

document.querySelectorAll("[data-download-android]").forEach((button) => {
    button.addEventListener("click", downloadAndroidApk);
});

document.querySelectorAll("[data-download-ios]").forEach((button) => {
    button.addEventListener("click", openIosAppStore);
});

document.querySelectorAll("[data-demo-control]").forEach((button) => {
    button.addEventListener("click", () =>
        showToast("当前为首页界面预览，播放器功能将在客户端开放"),
    );
});

const revealElements = document.querySelectorAll(".reveal");
if ("IntersectionObserver" in window) {
    const revealObserver = new IntersectionObserver(
        (entries, observer) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    observer.unobserve(entry.target);
                }
            });
        },
        { threshold: 0.12, rootMargin: "0px 0px -50px" },
    );

    revealElements.forEach((element) => revealObserver.observe(element));
} else {
    revealElements.forEach((element) => element.classList.add("is-visible"));
}
