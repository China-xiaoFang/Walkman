<template>
	<div>
		<FastTable
			ref="fastTableRef"
			tableKey="WK_TEXTBOOK"
			rowKey="textbookId"
			:requestApi="textbookApi.queryTextbookPaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'Textbook:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #operation="{ row }: { row: QueryTextbookPagedOutput }">
				<el-button v-auth="'Textbook:Detail'" size="small" plain @click="editFormRef.detail(row.textbookId)">详情</el-button>
				<el-button v-auth="'Textbook:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.textbookId)">编辑</el-button>
				<el-button v-auth="'Textbook:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<TextbookEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { textbookApi } from "@/api/services/Admin/textbook";
import { QueryTextbookPagedOutput } from "@/api/services/Admin/textbook/models/QueryTextbookPagedOutput";
import TextbookEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "WalkmanTextbook",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof TextbookEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryTextbookPagedOutput }) => {
	editFormRef.value.detail(row.textbookId);
};

const handleDelete = (row: QueryTextbookPagedOutput) => {
	const { textbookId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除该教材？", {
		type: "warning",
		async beforeClose() {
			await textbookApi.deleteTextbook({ textbookId, rowVersion });
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
