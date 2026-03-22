<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_AUDIO"
			rowKey="audioId"
			:requestApi="audioApi.queryAudioPaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'Audio:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryAudioPagedOutput }">
				<el-button v-auth="'Audio:Detail'" size="small" plain @click="editFormRef.detail(row.audioId)">详情</el-button>
				<el-button v-auth="'Audio:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.audioId)">编辑</el-button>
				<el-button v-auth="'Audio:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<AudioEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { audioApi } from "@/api/services/Admin/audio";
import { QueryAudioPagedOutput } from "@/api/services/Admin/audio/models/QueryAudioPagedOutput";
import AudioEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanAudio",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof AudioEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryAudioPagedOutput }) => {
	editFormRef.value.detail(row.audioId);
};

const handleDelete = (row: QueryAudioPagedOutput) => {
	const { audioId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该音频？", {
		type: "warning",
		async beforeClose() {
			await audioApi.deleteAudio({ audioId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
