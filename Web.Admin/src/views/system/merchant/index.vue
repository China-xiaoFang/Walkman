<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="1D1KYCMJCP"
			rowKey="merchantId"
			:requestApi="merchantApi.queryMerchantPaged"
			hideSearchTime
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
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { merchantApi } from "@/api/services/Admin/merchant";
import ConfigEdit from "./edit/index.vue";
import type { QueryMerchantPagedOutput } from "@/api/services/Admin/merchant/models/QueryMerchantPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemMerchant",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof ConfigEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryMerchantPagedOutput }) => {
	editFormRef.value.detail(row.merchantId);
};

/** 处理删除缓存 */
const handleDelete = (row: QueryMerchantPagedOutput) => {
	const { merchantId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除商户号？", {
		type: "warning",
		async beforeClose() {
			await merchantApi.deleteMerchant({ merchantId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
