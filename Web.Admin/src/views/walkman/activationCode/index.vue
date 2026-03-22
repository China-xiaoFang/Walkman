<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_ACTIVATION_CODE"
			rowKey="activationCodeId"
			:requestApi="activationCodeApi.queryActivationCodePaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'ActivationCode:Generate'" type="primary" :icon="Plus" @click="generateFormRef.open()">批量生成</el-button>
			</template>

			<template #code="{ row }: { row: QueryActivationCodePagedOutput }">
				<span v-iconCopy="row.code">{{ row.code }}</span>
			</template>

			<template #status="{ row }: { row: QueryActivationCodePagedOutput }">
				<el-tag v-if="row.status === ActivationCodeStatusEnum.Unused" type="success">未使用</el-tag>
				<el-tag v-else-if="row.status === ActivationCodeStatusEnum.Used" type="info">已使用</el-tag>
			</template>

			<template #operation="{ row }: { row: QueryActivationCodePagedOutput }">
				<el-button v-auth="'ActivationCode:Detail'" size="small" plain @click="handleDetail(row.activationCodeId)">详情</el-button>
			</template>
		</FastTable>
		<GenerateForm ref="generateFormRef" @ok="fastTableRef.refresh()" />
		<FaDialog ref="detailDialogRef" width="500" :title="detailState.dialogTitle" :showConfirmButton="false">
			<el-descriptions :column="1" border>
				<el-descriptions-item label="激活码">{{ detailState.data.code }}</el-descriptions-item>
				<el-descriptions-item label="教材">{{ detailState.data.textbookName }}</el-descriptions-item>
				<el-descriptions-item label="状态">
					<el-tag v-if="detailState.data.status === ActivationCodeStatusEnum.Unused" type="success">未使用</el-tag>
					<el-tag v-else type="info">已使用</el-tag>
				</el-descriptions-item>
				<el-descriptions-item v-if="detailState.data.accountId" label="绑定用户">{{ detailState.data.accountId }}</el-descriptions-item>
				<el-descriptions-item v-if="detailState.data.usedTime" label="使用时间">{{ detailState.data.usedTime }}</el-descriptions-item>
				<el-descriptions-item v-if="detailState.data.remark" label="备注">{{ detailState.data.remark }}</el-descriptions-item>
			</el-descriptions>
		</FaDialog>
	</div>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ActivationCodeStatusEnum } from "@/api/enums/ActivationCodeStatusEnum";
import { activationCodeApi } from "@/api/services/Admin/activationCode";
import { QueryActivationCodePagedOutput } from "@/api/services/Admin/activationCode/models/QueryActivationCodePagedOutput";
import { QueryActivationCodeDetailOutput } from "@/api/services/Admin/activationCode/models/QueryActivationCodeDetailOutput";
import GenerateForm from "./edit/generateForm.vue";
import type { FastTableInstance } from "@/components";
import type { FaDialogInstance } from "fast-element-plus";

defineOptions({
	name: "WalkmanActivationCode",
});

const fastTableRef = ref<FastTableInstance>();
const generateFormRef = ref<InstanceType<typeof GenerateForm>>();
const detailDialogRef = ref<FaDialogInstance>();

const detailState = reactive({
	dialogTitle: "激活码详情",
	data: {} as QueryActivationCodeDetailOutput,
});

const handleCustomCellClick = (_, { row }: { row: QueryActivationCodePagedOutput }) => {
	handleDetail(row.activationCodeId);
};

const handleDetail = (activationCodeId: number) => {
	detailDialogRef.value.open(async () => {
		const apiRes = await activationCodeApi.queryActivationCodeDetail(activationCodeId);
		detailState.data = apiRes;
		detailState.dialogTitle = `激活码详情 - ${apiRes.code}`;
	});
};
</script>
