<template>
	<div class="mobile-blocked">
		<div class="mobile-blocked__card">
			<div class="mobile-blocked__icon">
				<el-icon :size="64" color="var(--el-color-warning)">
					<Monitor />
				</el-icon>
			</div>
			<h2 class="mobile-blocked__title">请使用电脑访问</h2>
			<p class="mobile-blocked__desc">当前系统暂不支持移动端访问，请使用电脑浏览器打开以获得最佳体验。</p>
		</div>
	</div>
</template>

<script lang="ts" setup>
import { Monitor } from "@element-plus/icons-vue";

defineOptions({
	name: "MobileBlocked",
});
</script>

<style scoped lang="scss">
.mobile-blocked {
	width: 100%;
	height: 100%;
	display: flex;
	align-items: center;
	justify-content: center;
	background: var(--el-bg-color-page);
	padding: 20px;
	box-sizing: border-box;
	.mobile-blocked__card {
		text-align: center;
		max-width: 360px;
		padding: 40px 30px;
		background: var(--el-bg-color);
		border-radius: 16px;
		box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
	}
	.mobile-blocked__icon {
		margin-bottom: 20px;
	}
	.mobile-blocked__title {
		font-size: 20px;
		font-weight: 600;
		color: var(--el-text-color-primary);
		margin: 0 0 12px;
	}
	.mobile-blocked__desc {
		font-size: 14px;
		color: var(--el-text-color-secondary);
		line-height: 1.6;
		margin: 0;
	}
}
</style>
