<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="1D1KYCMJCP"
			row-key="merchantId"
			:request-api="merchantApi.queryMerchantPaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'Merchant:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryMerchantPagedOutput }">
				<el-button v-auth="'Merchant:Detail'" size="small" plain @click="editFormRef.detail(row.merchantId)">详情</el-button>
				<el-button v-auth="'Merchant:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.merchantId)">编辑</el-button>
				<el-button v-auth="'Merchant:Delete'" size="small" plain type="warning" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<ConfigEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { merchantApi } from "@/api/services/Center/merchant";
import ConfigEdit from "./edit/index.vue";
import type { QueryMerchantPagedOutput } from "@/api/services/Center/merchant/models/QueryMerchantPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemMerchant",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof ConfigEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryMerchantPagedOutput }) => {
	editFormRef.value.detail(row.merchantId);
};

/** 处理删除缓存 */
const handleDelete = (row: QueryMerchantPagedOutput) => {
	const { merchantId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除商户号？", {
		type: "warning",
	}).then(async () => {
		await merchantApi.deleteMerchant({ merchantId, rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
