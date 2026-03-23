<template>
	<el-divider contentPosition="left">按钮信息</el-divider>
	<div style="height: 500px">
		<FaTable rowKey="dictionaryItemId" :data="modelValue">
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button :disabled="props.disabled" type="primary" :icon="Plus" @click="editFormRef.add()">新增按钮</el-button>
			</template>
			<FaTableColumn prop="buttonName" label="按钮名称" width="120" smallWidth="100" />
			<FaTableColumn prop="buttonCode" label="按钮编码" width="200" smallWidth="200" />
			<FaTableColumn prop="hasWeb" label="Web端" width="80" smallWidth="80" tag :enum="booleanEnum" />
			<FaTableColumn prop="hasMobile" label="移动端" width="80" smallWidth="80" tag :enum="booleanEnum" />
			<FaTableColumn prop="hasDesktop" label="桌面端" width="80" smallWidth="80" tag :enum="booleanEnum" />
			<FaTableColumn prop="sort" label="排序" width="100" smallWidth="100" />
			<FaTableColumn prop="status" label="状态" width="100" smallWidth="80" tag :enum="commonStatusEnum" />
			<!-- 表格操作 -->
			<template #operation="{ row, $index }: { row: EditDictionaryItemInput; $index: number }">
				<el-button size="small" plain @click="editFormRef.detail(row)">详情</el-button>
				<el-button :disabled="props.disabled" size="small" plain type="primary" @click="editFormRef.edit(row, $index)">编辑</el-button>
				<el-button :disabled="props.disabled" size="small" plain type="danger" @click="handleDelete(row, $index)">删除</el-button>
			</template>
		</FaTable>
	</div>
	<DevMenuEditButtonEdit ref="editFormRef" v-model="modelValue" />
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessageBox } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import { definePropType } from "@fast-china/utils";
import { useVModel } from "@vueuse/core";
import { useApp } from "@/stores";
import DevMenuEditButtonEdit from "./buttonEdit.vue";
import type { EditDictionaryItemInput } from "@/api/services/Admin/dictionary/models/EditDictionaryItemInput";
import { EditMenuButtonInput } from "@/api/services/Admin/menu/models/EditMenuButtonInput";

defineOptions({
	name: "DevMenuEditButtonTable",
});

const props = defineProps({
	/** @description 是否禁用 */
	disabled: Boolean,
	/** @description v-model绑定值 */
	modelValue: definePropType<EditMenuButtonInput[]>([Array]),
});

const emit = defineEmits(["update:modelValue"]);

const appStore = useApp();
const booleanEnum = appStore.getDictionary("BooleanEnum");
const commonStatusEnum = appStore.getDictionary("CommonStatusEnum");

const modelValue = useVModel(props, "modelValue", emit, { passive: false });

const editFormRef = ref<InstanceType<typeof DevMenuEditButtonEdit>>();

const handleDelete = (row: EditMenuButtonInput, index: number) => {
	ElMessageBox.confirm("确定要删除按钮？", {
		type: "warning",
		async beforeClose() {
			modelValue.value.splice(index, 1);
		},
	});
};
</script>
