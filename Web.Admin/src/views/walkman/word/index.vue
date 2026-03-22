<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_WORD"
			rowKey="wordId"
			:requestApi="wordApi.queryWordPaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'Word:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryWordPagedOutput }">
				<el-button v-auth="'Word:Detail'" size="small" plain @click="editFormRef.detail(row.wordId)">详情</el-button>
				<el-button v-auth="'Word:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.wordId)">编辑</el-button>
				<el-button v-auth="'Word:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<WordEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { wordApi } from "@/api/services/Admin/word";
import { QueryWordPagedOutput } from "@/api/services/Admin/word/models/QueryWordPagedOutput";
import WordEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanWord",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof WordEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryWordPagedOutput }) => {
	editFormRef.value.detail(row.wordId);
};

const handleDelete = (row: QueryWordPagedOutput) => {
	const { wordId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该单词？", {
		type: "warning",
		async beforeClose() {
			await wordApi.deleteWord({ wordId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
