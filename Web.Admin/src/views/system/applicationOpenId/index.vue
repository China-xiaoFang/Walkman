<template>
	<div>
		<div class="fa__display_lr-r">
			<ApplicationTree @change="handleApplicationChange" />
			<FastTable
				ref="fastTableRef"
				table-key="1D1FCQZ5KT"
				row-key="recordId"
				:request-api="applicationOpenIdApi.queryApplicationOpenIdPaged"
				hide-search-time
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
		</div>
		<ApplicationOpenIdEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { applicationOpenIdApi } from "@/api/services/Center/applicationOpenId";
import ApplicationOpenIdEdit from "./edit/index.vue";
import type { ElSelectorOutput } from "fast-element-plus";
import type { QueryApplicationOpenIdPagedOutput } from "@/api/services/Center/applicationOpenId/models/QueryApplicationOpenIdPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemApplication",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof ApplicationOpenIdEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryApplicationOpenIdPagedOutput }) => {
	editFormRef.value.detail(row.recordId);
};

/** 应用更改 */
const handleApplicationChange = async (data: ElSelectorOutput) => {
	fastTableRef.value.searchParam.appId = data.value;
	await fastTableRef.value.refresh();
};

/** 处理删除 */
const handleDelete = (row: QueryApplicationOpenIdPagedOutput) => {
	const { recordId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除应用OpenId？", {
		type: "warning",
	}).then(async () => {
		await applicationOpenIdApi.deleteApplicationOpenId({ recordId, rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
