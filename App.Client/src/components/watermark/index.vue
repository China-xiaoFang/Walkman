<template>
	<wd-watermark v-bind="watermarkProps" />
</template>

<script setup lang="ts">
import { computed, reactive } from "vue";
import { useConfig, useUserInfo } from "@/stores";

defineOptions({
	name: "Watermark",
	options: {
		virtualHost: true,
		addGlobalClass: true,
		styleIsolation: "shared",
	},
});

const configStore = useConfig();
const userInfoStore = useUserInfo();

const watermarkProps = reactive({
	rotate: -38,
	opacity: 0.5,
	width: 150,
	height: 250,
	size: 14,
	color: computed(() => (configStore.layout.isDark ? "rgba(255,255,255,0.2)" : "rgba(0,0,0,0.2)")),
	content: computed(() => {
		let watermarkContent = "Fast随身听";
		if (userInfoStore.hasUserInfo) {
			watermarkContent += ` - ${userInfoStore.nickName}`;
		}

		return watermarkContent;
	}),
});
</script>
