<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_PRONUNCIATION_TYPE"
			rowKey="pronunciationTypeId"
			:requestApi="audioApi.queryPronunciationTypePaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'PronunciationType:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryPronunciationTypePagedOutput }">
				<el-button v-auth="'PronunciationType:Detail'" size="small" plain @click="editFormRef.detail(row.pronunciationTypeId)">详情</el-button>
				<el-button v-auth="'PronunciationType:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.pronunciationTypeId)">编辑</el-button>
				<el-button v-auth="'PronunciationType:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<PronunciationTypeEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { audioApi } from "@/api/services/Admin/audio";
import { QueryPronunciationTypePagedOutput } from "@/api/services/Admin/audio/models/QueryPronunciationTypePagedOutput";
import PronunciationTypeEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanPronunciationType",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof PronunciationTypeEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryPronunciationTypePagedOutput }) => {
	editFormRef.value.detail(row.pronunciationTypeId);
};

const handleDelete = (row: QueryPronunciationTypePagedOutput) => {
	const { pronunciationTypeId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该发音类型？", {
		type: "warning",
		async beforeClose() {
			await audioApi.deletePronunciationType({ pronunciationTypeId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
