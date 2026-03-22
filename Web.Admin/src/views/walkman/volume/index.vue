<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_VOLUME"
			rowKey="volumeId"
			:requestApi="textbookApi.queryVolumePaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'Volume:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryVolumePagedOutput }">
				<el-button v-auth="'Volume:Detail'" size="small" plain @click="editFormRef.detail(row.volumeId)">详情</el-button>
				<el-button v-auth="'Volume:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.volumeId)">编辑</el-button>
				<el-button v-auth="'Volume:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<VolumeEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { textbookApi } from "@/api/services/Admin/textbook";
import { QueryVolumePagedOutput } from "@/api/services/Admin/textbook/models/QueryVolumePagedOutput";
import VolumeEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanVolume",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof VolumeEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryVolumePagedOutput }) => {
	editFormRef.value.detail(row.volumeId);
};

const handleDelete = (row: QueryVolumePagedOutput) => {
	const { volumeId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该册？", {
		type: "warning",
		async beforeClose() {
			await textbookApi.deleteVolume({ volumeId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
