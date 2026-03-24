<template>
	<view class="page">
		<rich-text :nodes="userAgreement" />
	</view>
</template>

<script setup lang="ts">
definePage({
	name: "UserAgreement",
	layout: "layout",
	footer: false,
	watermark: false,
	style: {
		navigationBarTitleText: "用户协议",
	},
});

const userAgreement = "";
</script>

<style scoped lang="scss">
.page {
	background-color: var(--wot-bg-color);
}
</style>
