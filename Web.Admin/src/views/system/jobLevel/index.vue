<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="1D1KRB5KK9"
			row-key="jobLevelId"
			:request-api="jobLevelApi.queryJobLevelPaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'JobLevel:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryJobLevelPagedOutput }">
				<el-button v-auth="'JobLevel:Detail'" size="small" plain @click="editFormRef.detail(row.jobLevelId)">详情</el-button>
				<el-button v-auth="'JobLevel:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.jobLevelId)">编辑</el-button>
				<el-button v-auth="'JobLevel:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<JobLevelEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { jobLevelApi } from "@/api/services/Admin/jobLevel";
import JobLevelEdit from "./edit/index.vue";
import type { QueryJobLevelPagedOutput } from "@/api/services/Admin/jobLevel/models/QueryJobLevelPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemJobLevel",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof JobLevelEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryJobLevelPagedOutput }) => {
	editFormRef.value.detail(row.jobLevelId);
};

/** 处理删除 */
const handleDelete = (row: QueryJobLevelPagedOutput) => {
	const { jobLevelId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除职级？", {
		type: "warning",
	}).then(async () => {
		await jobLevelApi.deleteJobLevel({ jobLevelId, rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
