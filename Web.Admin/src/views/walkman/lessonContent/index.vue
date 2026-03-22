<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_LESSONCONTENT"
			rowKey="lessonContentId"
			:requestApi="lessonContentApi.queryLessonContentPaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'LessonContent:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryLessonContentPagedOutput }">
				<el-button v-auth="'LessonContent:Detail'" size="small" plain @click="editFormRef.detail(row.lessonContentId)">详情</el-button>
				<el-button v-auth="'LessonContent:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.lessonContentId)">编辑</el-button>
				<el-button v-auth="'LessonContent:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<LessonContentEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { lessonContentApi } from "@/api/services/Admin/lessonContent";
import { QueryLessonContentPagedOutput } from "@/api/services/Admin/lessonContent/models/QueryLessonContentPagedOutput";
import LessonContentEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanLessonContent",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof LessonContentEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryLessonContentPagedOutput }) => {
	editFormRef.value.detail(row.lessonContentId);
};

const handleDelete = (row: QueryLessonContentPagedOutput) => {
	const { lessonContentId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该课文内容？", {
		type: "warning",
		async beforeClose() {
			await lessonContentApi.deleteLessonContent({ lessonContentId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
