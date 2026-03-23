<template>
	<div>
		<FastTable tableKey="1DFGFQQD95" rowKey="recordId" :requestApi="passwordRecordApi.queryPasswordRecordPaged" />
	</div>
</template>

<script lang="ts" setup>
import { passwordRecordApi } from "@/api/services/Admin/passwordRecord";

defineOptions({
	name: "DevPasswordRecord",
});
</script>
