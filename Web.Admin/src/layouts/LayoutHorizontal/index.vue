<template>
	<el-container
		:class="[
			{
				contentFull: navTabsStore.contentFull,
				contentLarge: navTabsStore.contentLarge,
			},
		]"
	>
		<el-header>
			<div class="nav-bar" :style="{ '--height': addUnit(configStore.layout.navBarHeight) }">
				<div class="left">
					<LayoutLogo :style="{ '--width': addUnit(configStore.layout.menuWidth) }" />
					<LayoutMenu />
				</div>
				<div class="right">
					<el-icon class="menu-search fa__hover__twinkle" title="搜索菜单" @click="menuSearchRef.open()">
						<Search />
					</el-icon>
					<LayoutScreenFull />
					<el-dropdown
						class="avatar"
						placement="bottom"
						trigger="click"
						hideOnClick
						:title="userInfoStore.employeeName || userInfoStore.nickName"
					>
						<div class="user-info">
							<FaAvatar original :size="24" :src="userInfoStore.avatar" :icon="UserFilled" />
							<span class="nick-name">{{ userInfoStore.nickName }}（{{ userInfoStore.employeeName }}）</span>
						</div>
						<template #dropdown>
							<el-dropdown-menu>
								<el-dropdown-item :icon="User" @click="routerUtil.routePushSafe(router, { path: '/settings/account' })">
									个人信息
								</el-dropdown-item>
								<el-dropdown-item :icon="Key" @click="changePasswordRef.open()">修改密码</el-dropdown-item>
								<el-dropdown-item :icon="Refresh" @click="handleRefreshSystem">刷新系统</el-dropdown-item>
								<el-dropdown-item divided :icon="Lock" @click="handleScreenLock">锁定屏幕</el-dropdown-item>
								<el-dropdown-item :icon="SwitchButton" @click="handleLogout">退出系统</el-dropdown-item>
							</el-dropdown-menu>
						</template>
					</el-dropdown>
					<el-icon class="setting fa__hover__twinkle" title="高级配置" @click="layoutConfigRef.open()"><Setting /></el-icon>
				</div>
			</div>
			<LayoutNavTab />
		</el-header>
		<el-main :style="{ '--el-main-padding': addUnit(configStore.layout.mainPadding) }">
			<el-scrollbar>
				<RouterView v-slot="{ Component, route }">
					<transition mode="out-in" :name="configStore.layout.mainAnimation">
						<KeepAlive :include="navTabsStore.keepAliveComponentNameList">
							<component :is="Component" :key="route.path" class="layout-main" />
						</KeepAlive>
					</transition>
				</RouterView>
			</el-scrollbar>
		</el-main>
		<el-footer :style="{ '--el-footer-height': configStore.layout.footer ? addUnit(configStore.layout.footerHeight) : 0 }">
			<Footer />
		</el-footer>
		<teleport to="body">
			<transition name="slide-bottom" mode="out-in">
				<LayoutScreenLock v-if="configStore.screen.screenLock" />
			</transition>
		</teleport>
	</el-container>
</template>

<script setup lang="ts">
import { inject } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Key, Lock, Refresh, Search, Setting, SwitchButton, User, UserFilled } from "@element-plus/icons-vue";
import { Local, addUnit } from "@fast-china/utils";
import { RouterView, useRouter } from "vue-router";
import { changePasswordKey, layoutConfigKey, menuSearchKey } from "@/layouts";
import LayoutLogo from "@/layouts/components/Logo/index.vue";
import LayoutNavTab from "@/layouts/components/NavTab/index.vue";
import LayoutScreenFull from "@/layouts/components/ScreenFull/index.vue";
import LayoutScreenLock from "@/layouts/components/ScreenLock/index.vue";
import { routerUtil } from "@/router";
import { useConfig, useNavTabs, useUserInfo } from "@/stores";
import LayoutMenu from "./components/menu.vue";

defineOptions({
	name: "LayoutHorizontal",
});

const router = useRouter();
const configStore = useConfig();
const navTabsStore = useNavTabs();
const userInfoStore = useUserInfo();

const layoutConfigRef = inject(layoutConfigKey);
const menuSearchRef = inject(menuSearchKey);
const changePasswordRef = inject(changePasswordKey);

const handleRefreshSystem = () => {
	ElMessageBox.confirm("此操作会强制刷新当前页面，是否继续操作？", {
		type: "warning",
	}).then(() => {
		Local.removeByPrefix("HTTP_CACHE_");
		window.location.reload();
	});
};

const handleScreenLock = () => {
	ElMessageBox.prompt("请输入锁屏密码", {
		showClose: false,
		confirmButtonText: "锁定",
		closeOnPressEscape: true,
		inputType: "password",
		inputPlaceholder: "请输入锁屏密码",
		inputValidator(value) {
			if (!value || !value.trim()) {
				return "锁屏密码不能为空";
			}
			return true;
		},
	}).then(({ value }) => {
		configStore.screen.password = value;
		configStore.screen.screenLock = true;
	});
};

const handleLogout = async () => {
	ElMessageBox.confirm(`确定要退出登录？`, {
		type: "warning",
		async beforeClose() {
			await userInfoStore.logout();
			ElMessage.success(`退出登录成功`);
		},
	});
};
</script>

<style scoped lang="scss">
@use "./index.scss";
</style>
