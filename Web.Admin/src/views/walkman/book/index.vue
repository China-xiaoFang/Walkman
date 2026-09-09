<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="RRRVM3CJBHB"
			row-key="bookId"
			:request-api="bookApi.queryBookPaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'Book:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>
			<template #coverUrl="{ row }: { row?: QueryBookPagedOutput }">
				<FaAvatar :src="row?.coverUrl" thumb shape="square" />
			</template>
			<template #operation="{ row }: { row: QueryBookPagedOutput }">
				<el-button v-auth="'Book:Detail'" size="small" plain @click="editFormRef.detail(row.bookId!)">详情</el-button>
				<el-button v-auth="'Book:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.bookId!)">编辑</el-button>
				<el-button v-auth="'Book:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<BookEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { bookApi } from "@/api/services/Walkman/book";
import BookEdit from "./edit/index.vue";
import type { QueryBookPagedOutput } from "@/api/services/Walkman/book/models/QueryBookPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({ name: "WalkmanBook" });

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof BookEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryBookPagedOutput }) => {
	if (row.bookId) editFormRef.value.detail(row.bookId);
};

const handleDelete = (row: QueryBookPagedOutput) => {
	void ElMessageBox.confirm("确定要删除教材？", { type: "warning" }).then(async () => {
		await bookApi.deleteBook({ bookId: row.bookId, rowVersion: row.rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
