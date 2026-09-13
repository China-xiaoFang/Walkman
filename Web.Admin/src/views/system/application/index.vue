<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="1D1F9HNVPQ"
			row-key="appId"
			:request-api="applicationApi.queryApplicationPaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'App:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #themeColor="{ row }: { row?: QueryApplicationPagedOutput }">
				<el-tag v-if="row?.themeColor" effect="dark" :color="row.themeColor" :style="{ borderColor: row.themeColor }">
					{{ row.themeColor }}
				</el-tag>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryApplicationPagedOutput }">
				<el-button v-auth="'App:Detail'" size="small" plain @click="editFormRef.detail(row.appId)">详情</el-button>
				<el-button v-auth="'App:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.appId)">编辑</el-button>
				<el-button v-auth="'App:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<ApplicationEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { applicationApi } from "@/api/services/Center/application";
import ApplicationEdit from "./edit/index.vue";
import type { QueryApplicationPagedOutput } from "@/api/services/Center/application/models/QueryApplicationPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemApplication",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof ApplicationEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryApplicationPagedOutput }) => {
	editFormRef.value.detail(row.appId);
};

/** 处理删除 */
const handleDelete = (row: QueryApplicationPagedOutput) => {
	const { appId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除应用？", {
		type: "warning",
	}).then(async () => {
		await applicationApi.deleteApplication({ appId, rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
