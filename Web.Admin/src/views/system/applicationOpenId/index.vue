<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="1D1FCQZ5KT"
			rowKey="recordId"
			:requestApi="applicationOpenIdApi.queryApplicationOpenIdPaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'App:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryApplicationOpenIdPagedOutput }">
				<el-button v-auth="'App:Detail'" size="small" plain @click="editFormRef.detail(row.recordId)">详情</el-button>
				<el-button v-auth="'App:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.recordId)"> 编辑 </el-button>
				<el-button v-auth="'App:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<ApplicationOpenIdEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { applicationOpenIdApi } from "@/api/services/Admin/applicationOpenId";
import { QueryApplicationOpenIdPagedOutput } from "@/api/services/Admin/applicationOpenId/models/QueryApplicationOpenIdPagedOutput";
import ApplicationOpenIdEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemApplication",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof ApplicationOpenIdEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryApplicationOpenIdPagedOutput }) => {
	editFormRef.value.detail(row.recordId);
};

/** 处理删除 */
const handleDelete = (row: QueryApplicationOpenIdPagedOutput) => {
	const { recordId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除应用OpenId？", {
		type: "warning",
		async beforeClose() {
			await applicationOpenIdApi.deleteApplicationOpenId({ recordId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
