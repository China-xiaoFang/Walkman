<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_AUDIO_TYPE"
			rowKey="audioTypeId"
			:requestApi="audioApi.queryAudioTypePaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'AudioType:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryAudioTypePagedOutput }">
				<el-button v-auth="'AudioType:Detail'" size="small" plain @click="editFormRef.detail(row.audioTypeId)">详情</el-button>
				<el-button v-auth="'AudioType:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.audioTypeId)">编辑</el-button>
				<el-button v-auth="'AudioType:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<AudioTypeEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { audioApi } from "@/api/services/Admin/audio";
import { QueryAudioTypePagedOutput } from "@/api/services/Admin/audio/models/QueryAudioTypePagedOutput";
import AudioTypeEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanAudioType",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof AudioTypeEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryAudioTypePagedOutput }) => {
	editFormRef.value.detail(row.audioTypeId);
};

const handleDelete = (row: QueryAudioTypePagedOutput) => {
	const { audioTypeId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该音频类型？", {
		type: "warning",
		async beforeClose() {
			await audioApi.deleteAudioType({ audioTypeId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
