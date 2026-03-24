<template>
	<view class="page">
		<rich-text :nodes="privacyAgreement" />
	</view>
</template>

<script setup lang="ts">
definePage({
	name: "PrivacyAgreement",
	layout: "layout",
	footer: false,
	watermark: false,
	style: {
		navigationBarTitleText: "隐私协议",
	},
});

const privacyAgreement = "";
</script>

<style scoped lang="scss">
.page {
	background-color: var(--wot-bg-color);
}
</style>
