<template>
	<div class="logo" :style="{ '--height': addUnit(configStore.layout.navBarHeight) }" title="首页" @click="router.push('/')">
		<img :src="LogoImg" alt="Logo" @error="setFallBack" />
		<span v-if="!configStore.layout.menuCollapse" title="Fast随身听">Fast随身听</span>
	</div>
</template>

<script setup lang="ts">
import { addUnit } from "@fast-china/utils";
import { useRouter } from "vue-router";
import LogoImg from "@/assets/logo.png";
import { useConfig } from "@/stores";

defineOptions({
	name: "Logo",
});

const router = useRouter();
const configStore = useConfig();

const setFallBack = (event: Event) => {
	const target = event.target as HTMLImageElement;
	target.src = LogoImg;
};
</script>

<style scoped lang="scss">
html.small {
	.logo {
		span {
			font-size: var(--el-font-size-medium);
		}
	}
}

.logo {
	width: var(--width);
	height: var(--height);
	display: flex;
	align-items: center;
	justify-content: center;
	gap: 10px;
	padding: 0 10px;
	// border-bottom: var(--el-border);
	box-sizing: border-box;
	cursor: pointer;
	img {
		height: 80%;
	}
	span {
		font-size: var(--el-font-size-large);
		font-weight: var(--el-font-weight-primary);
		max-width: 100%;
		display: block;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}
}
</style>
