<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="RRRG6LNFYXR"
			row-key="activationCodeId"
			hide-search-time
			:request-api="activationCodeApi.queryActivationCodePaged"
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'ActivationCode:Generate'" type="primary" :icon="Plus" @click="generateFormRef.open()"> 生成激活码 </el-button>
			</template>

			<template #userId="{ row }: { row?: QueryActivationCodePagedOutput }">
				<el-tag :type="row?.userId ? 'success' : 'info'">{{ row?.userId ? "已使用" : "未使用" }}</el-tag>
			</template>

			<template #operation="{ row }: { row: QueryActivationCodePagedOutput }">
				<el-button v-auth="'ActivationCode:Detail'" size="small" plain @click="detailFormRef.open(row.activationCodeId!)"> 详情 </el-button>
			</template>
		</FastTable>
		<ActivationCodeGenerate ref="generateFormRef" @ok="fastTableRef.refresh()" />
		<ActivationCodeDetail ref="detailFormRef" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { activationCodeApi } from "@/api/services/Walkman/activationCode";
import ActivationCodeDetail from "./detail/index.vue";
import ActivationCodeGenerate from "./generate/index.vue";
import type { QueryActivationCodePagedOutput } from "@/api/services/Walkman/activationCode/models/QueryActivationCodePagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({ name: "WalkmanActivationCode" });

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const generateFormRef = useTemplateRef<InstanceType<typeof ActivationCodeGenerate>>("generateFormRef");
const detailFormRef = useTemplateRef<InstanceType<typeof ActivationCodeDetail>>("detailFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryActivationCodePagedOutput }) => {
	if (row.activationCodeId) detailFormRef.value.open(row.activationCodeId);
};
</script>
