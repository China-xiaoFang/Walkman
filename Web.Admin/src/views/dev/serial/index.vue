<template>
	<div>
		<FastTable ref="fastTableRef" tableKey="1D11MFM59S" rowKey="serialRuleId" :requestApi="serialApi.querySerialRulePaged" hideSearchTime>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'SysSerial:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QuerySerialRulePagedOutput }">
				<el-button v-auth="'SysSerial:Detail'" size="small" plain @click="editFormRef.detail(row.serialRuleId)">详情</el-button>
				<el-button v-auth="'SysSerial:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.serialRuleId)">编辑</el-button>
			</template>
		</FastTable>
		<SerialEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { serialApi } from "@/api/services/Admin/serial";
import SerialEdit from "./edit/index.vue";
import type { QuerySerialRulePagedOutput } from "@/api/services/Admin/serial/models/QuerySerialRulePagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "DevSerial",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof SerialEdit>>();
</script>
