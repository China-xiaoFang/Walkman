<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="1D1KR8WZ6U"
			row-key="positionId"
			:request-api="positionApi.queryPositionPaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'Position:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryPositionPagedOutput }">
				<el-button v-auth="'Position:Detail'" size="small" plain @click="editFormRef.detail(row.positionId)">详情</el-button>
				<el-button v-auth="'Position:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.positionId)">编辑</el-button>
				<el-button v-auth="'Position:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<PositionEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { positionApi } from "@/api/services/Admin/position";
import PositionEdit from "./edit/index.vue";
import type { QueryPositionPagedOutput } from "@/api/services/Admin/position/models/QueryPositionPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemPosition",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof PositionEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryPositionPagedOutput }) => {
	editFormRef.value.detail(row.positionId);
};

/** 处理删除 */
const handleDelete = (row: QueryPositionPagedOutput) => {
	const { positionId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除职位？", {
		type: "warning",
	}).then(async () => {
		await positionApi.deletePosition({ positionId, rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
