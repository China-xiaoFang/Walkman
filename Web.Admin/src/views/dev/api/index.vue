<template>
	<div>
		<FastTable table-key="1D11YYQH9M" row-key="apiId" :request-api="apiApi.queryApiPaged" hide-search-time>
			<template #tags="{ row }: { row?: ApiInfoModel }">
				<el-tag v-for="(item, index) in row.tags" :key="index" type="info" effect="dark">{{ item }}</el-tag>
			</template>
		</FastTable>
	</div>
</template>

<script lang="ts" setup>
import { apiApi } from "@/api/services/Center/api";
import type { ApiInfoModel } from "@/api/services/Center/api/models/ApiInfoModel";

defineOptions({
	name: "DevApi",
});
</script>
