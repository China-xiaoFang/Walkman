<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_LESSON"
			rowKey="lessonId"
			:requestApi="textbookApi.queryLessonPaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'Lesson:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryLessonPagedOutput }">
				<el-button v-auth="'Lesson:Detail'" size="small" plain @click="editFormRef.detail(row.lessonId)">详情</el-button>
				<el-button v-auth="'Lesson:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.lessonId)">编辑</el-button>
				<el-button v-auth="'Lesson:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<LessonEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { textbookApi } from "@/api/services/Admin/textbook";
import { QueryLessonPagedOutput } from "@/api/services/Admin/textbook/models/QueryLessonPagedOutput";
import LessonEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanLesson",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof LessonEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryLessonPagedOutput }) => {
	editFormRef.value.detail(row.lessonId);
};

const handleDelete = (row: QueryLessonPagedOutput) => {
	const { lessonId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该课程？", {
		type: "warning",
		async beforeClose() {
			await textbookApi.deleteLesson({ lessonId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
