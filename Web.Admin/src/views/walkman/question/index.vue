<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_QUESTION"
			rowKey="questionId"
			:requestApi="questionApi.queryQuestionPaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'Question:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryQuestionPagedOutput }">
				<el-button v-auth="'Question:Detail'" size="small" plain @click="editFormRef.detail(row.questionId)">详情</el-button>
				<el-button v-auth="'Question:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.questionId)">编辑</el-button>
				<el-button v-auth="'Question:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<QuestionEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { questionApi } from "@/api/services/Admin/question";
import { QueryQuestionPagedOutput } from "@/api/services/Admin/question/models/QueryQuestionPagedOutput";
import QuestionEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanQuestion",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof QuestionEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryQuestionPagedOutput }) => {
	editFormRef.value.detail(row.questionId);
};

const handleDelete = (row: QueryQuestionPagedOutput) => {
	const { questionId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该题目？", {
		type: "warning",
		async beforeClose() {
			await questionApi.deleteQuestion({ questionId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
