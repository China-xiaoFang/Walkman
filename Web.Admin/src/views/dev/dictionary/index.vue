<template>
	<div>
		<FaTable
			ref="faTableRef"
			rowKey="dictionaryId"
			:requestApi="dictionaryApi.queryDictionaryPaged"
			hideSearchTime
			@custom-cell-click="handleCustomCellClick"
		>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'Dictionary:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<FaTableColumn
				prop="dictionaryKey"
				label="字典Key"
				fixed="left"
				width="300"
				smallWidth="280"
				sortable
				copy
				link
				:click="({ row }) => editFormRef.detail(row.dictionaryId)"
			/>
			<FaTableColumn prop="dictionaryName" label="字典名称" width="300" smallWidth="280" sortable />
			<FaTableColumn
				prop="valueType"
				label="值类型"
				width="100"
				smallWidth="80"
				sortable
				tag
				:enum="[
					{ label: '字符串', value: 1, type: 'info' },
					{ label: 'Int', value: 2, type: 'success' },
					{ label: 'Long', value: 4, type: 'primary' },
					{ label: 'Boolean', value: 8, type: 'danger' },
				]"
			/>
			<FaTableColumn
				prop="hasFlags"
				label="Flags枚举"
				width="120"
				smallWidth="100"
				sortable
				tag
				:enum="[
					{ label: '是', value: true, type: 'success' },
					{ label: '否', value: false, type: 'danger' },
				]"
			/>
			<FaTableColumn
				prop="status"
				label="状态"
				width="100"
				smallWidth="80"
				sortable
				tag
				:enum="[
					{ label: '正常', value: 1, type: 'primary' },
					{ label: '禁用', value: 2, type: 'danger' },
				]"
			/>
			<FaTableColumn prop="remark" label="备注" width="200" smallWidth="180" sortable />
			<FaTableColumn prop="createdTime" label="创建时间" type="timeInfo" width="240" smallWidth="220" sortable />
			<FaTableColumn
				prop="updatedTime"
				label="更新时间"
				type="timeInfo"
				width="240"
				smallWidth="220"
				sortable
				:timeInfoField="{
					userName: 'updatedUserName',
					time: 'updatedTime',
				}"
			/>
			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryDictionaryPagedOutput }">
				<el-button v-auth="'Dictionary:Detail'" size="small" plain @click="editFormRef.detail(row.dictionaryId)">详情</el-button>
				<el-button v-auth="'Dictionary:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.dictionaryId)">编辑</el-button>
				<el-button v-auth="'Dictionary:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FaTable>
		<DictionaryEdit ref="editFormRef" @ok="faTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { dictionaryApi } from "@/api/services/Admin/dictionary";
import DictionaryEdit from "./edit/index.vue";
import type { QueryDictionaryPagedOutput } from "@/api/services/Admin/dictionary/models/QueryDictionaryPagedOutput";
import type { FaTableInstance } from "fast-element-plus";

defineOptions({
	name: "DevDictionary",
});

const faTableRef = ref<FaTableInstance>();
const editFormRef = ref<InstanceType<typeof DictionaryEdit>>();

const handleCustomCellClick = (_, { row }: { row: QueryDictionaryPagedOutput }) => {
	editFormRef.value.detail(row.dictionaryId);
};

/** 处理删除 */
const handleDelete = (row: QueryDictionaryPagedOutput) => {
	const { dictionaryId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除数据字典？", {
		type: "warning",
		async beforeClose() {
			await dictionaryApi.deleteDictionary({ dictionaryId, rowVersion });
			ElMessage.success("删除成功！");
			faTableRef.value?.refresh();
		},
	});
};
</script>
