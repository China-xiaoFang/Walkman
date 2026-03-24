<template>
	<view class="page">
		<rich-text :nodes="serviceAgreement" />
	</view>
</template>

<script setup lang="ts">
definePage({
	name: "ServiceAgreement",
	layout: "layout",
	footer: false,
	watermark: false,
	style: {
		navigationBarTitleText: "服务协议",
	},
});

const serviceAgreement = "";
</script>

<style scoped lang="scss">
.page {
	background-color: var(--wot-bg-color);
}
</style>
