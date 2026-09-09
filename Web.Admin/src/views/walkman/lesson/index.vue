<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="RRRVPM1U44M"
			row-key="lessonId"
			:request-api="lessonApi.queryLessonPaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'Lesson:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #bookId="{ searchParam }">
				<FaSelectPage :request-api="bookApi.bookSelector" v-model="searchParam.bookId" placeholder="请选择教材" filterable clearable />
			</template>

			<template #operation="{ row }: { row: QueryLessonPagedOutput }">
				<el-button v-auth="'Lesson:Detail'" size="small" plain @click="editFormRef.detail(row.lessonId!)">详情</el-button>
				<el-button v-auth="'Lesson:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.lessonId!)">编辑</el-button>
				<el-button v-auth="'Lesson:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<LessonEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { bookApi } from "@/api/services/Walkman/book";
import { lessonApi } from "@/api/services/Walkman/lesson";
import LessonEdit from "./edit/index.vue";
import type { QueryLessonPagedOutput } from "@/api/services/Walkman/lesson/models/QueryLessonPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({ name: "WalkmanLesson" });

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof LessonEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryLessonPagedOutput }) => {
	if (row.lessonId) editFormRef.value.detail(row.lessonId);
};

const handleDelete = (row: QueryLessonPagedOutput) => {
	void ElMessageBox.confirm("确定要删除课程？", { type: "warning" }).then(async () => {
		await lessonApi.deleteLesson({ lessonId: row.lessonId, rowVersion: row.rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
