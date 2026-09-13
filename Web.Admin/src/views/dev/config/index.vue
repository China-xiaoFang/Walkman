<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="1D11JKRR5Q"
			row-key="configId"
			:request-api="configApi.queryConfigPaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'Config:Edit'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
				<el-button v-auth="'Config:Edit'" plain type="warning" :icon="Delete" @click="handleDeleteAll">删除全部缓存</el-button>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryConfigPagedOutput }">
				<el-button v-auth="'Config:Detail'" size="small" plain @click="editFormRef.detail(row.configId)">详情</el-button>
				<el-button v-auth="'Config:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.configId)">编辑</el-button>
				<el-button v-auth="'Config:Edit'" size="small" plain type="warning" @click="handleDelete(row)">删除缓存</el-button>
			</template>
		</FastTable>
		<ConfigEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Delete, Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { configApi } from "@/api/services/Center/config";
import ConfigEdit from "./edit/index.vue";
import type { QueryConfigPagedOutput } from "@/api/services/Center/config/models/QueryConfigPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "DevConfig",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof ConfigEdit>>("editFormRef");

const handleCustomCellClick = (_: string, { row }: { row: QueryConfigPagedOutput }) => {
	editFormRef.value.detail(row.configId);
};

/** 处理删除缓存 */
const handleDelete = (row: QueryConfigPagedOutput) => {
	const { configCode } = row;
	ElMessageBox.confirm("确定要删除缓存？", {
		type: "warning",
	}).then(async () => {
		await configApi.deleteConfigCache({ configCode });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};

/** 处理删除全部缓存 */
const handleDeleteAll = () => {
	ElMessageBox.confirm("确定要删除全部缓存？", {
		type: "warning",
	}).then(async () => {
		await configApi.deleteAllConfigCache();
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
