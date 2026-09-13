<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="1D1F3QYJST"
			row-key="mainId"
			:request-api="databaseApi.queryDatabasePaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'Database:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryDatabasePagedOutput }">
				<el-button v-auth="'Database:Detail'" size="small" plain @click="editFormRef.detail(row.mainId)">详情</el-button>
				<el-button v-auth="'Database:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.mainId)">编辑</el-button>
				<el-button v-auth="'Database:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
				<el-button v-if="!row.isInitialized" v-auth="'Database:Edit'" size="small" plain type="warning" @click="handleInitDatabase(row)">
					初始化
				</el-button>
			</template>
		</FastTable>
		<DatabaseEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { tenantDatabaseApi } from "@/api/services/Admin/tenantDatabase";
import { databaseApi } from "@/api/services/Center/database";
import DatabaseEdit from "./edit/index.vue";
import type { QueryDatabasePagedOutput } from "@/api/services/Center/database/models/QueryDatabasePagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemDatabase",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof DatabaseEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryDatabasePagedOutput }) => {
	editFormRef.value.detail(row.mainId);
};

/** 处理删除 */
const handleDelete = (row: QueryDatabasePagedOutput) => {
	const { mainId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除数据库？", {
		type: "warning",
	}).then(async () => {
		await databaseApi.deleteDatabase({ mainId, rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};

/** 处理初始化 */
const handleInitDatabase = (row: QueryDatabasePagedOutput) => {
	const { tenantId, databaseType, isInitialized } = row;
	if (isInitialized) return;
	ElMessageBox.confirm("确定要初始化数据库？", {
		type: "warning",
	}).then(async () => {
		await tenantDatabaseApi.initDatabase({ tenantId, databaseType });
		ElMessage.success("初始化成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
