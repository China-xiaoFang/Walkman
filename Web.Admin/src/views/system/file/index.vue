<template>
	<div>
		<FastTable tableKey="1D11KCYJJ9" rowKey="fileId" :requestApi="fileApi.queryFilePaged">
			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryFilePagedOutput }">
				<el-button v-if="state.imageMimeType.includes(row.fileMimeType)" size="small" plain @click="state.previewSrc = row.fileLocation">
					预览
				</el-button>
			</template>
		</FastTable>
		<el-image-viewer
			v-if="state.previewSrc"
			:urlList="[state.previewSrc]"
			hideOnClickModal
			teleported
			showProgress
			@close="state.previewSrc = ''"
		/>
	</div>
</template>

<script lang="ts" setup>
import { reactive } from "vue";
import { fileApi } from "@/api/services/File";
import { QueryFilePagedOutput } from "@/api/services/File/models/QueryFilePagedOutput";

defineOptions({
	name: "SystemFile",
});

const state = reactive({
	imageMimeType: ["image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp"],
	previewSrc: "",
});
</script>
