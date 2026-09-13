<template>
	<div />
</template>

<script lang="ts" setup>
import { useRoute, useRouter } from "vue-router";

defineOptions({
	name: "Redirect",
});

const route = useRoute();
const router = useRouter();

const redirectPath = Array.isArray(route.params.path) ? route.params.path.join("/") : route.params.path;
router.replace({ path: `/${redirectPath}`, query: route.query });
</script>
