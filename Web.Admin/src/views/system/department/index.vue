<template>
	<div>
		<div class="fa__display_lr-r">
			<FaTree
				ref="orgTreeRef"
				title="机构列表"
				width="240"
				:request-api="organizationApi.organizationSelector"
				@change="handleOrgChange"
				@node-contextmenu="(event, data) => handleOrgContextmenu(event as MouseEvent, data)"
			>
				<template #label="{ data }">
					<span>{{ data.label }}</span>
				</template>
				<template #default="{ data }">
					<el-text type="info">{{ data.data?.orgCode }}</el-text>
				</template>
			</FaTree>
			<FastTable
				ref="fastTableRef"
				table-key="1D1KGFUXKQ"
				row-key="departmentId"
				:request-api="departmentApi.queryDepartmentPaged"
				hide-search-time
				:pagination="false"
				default-expand-all
				@custom-cell-click="handleCustomCellClick"
			>
				<!-- 表格按钮操作区域 -->
				<template #header>
					<el-button v-auth="'Department:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
				</template>

				<!-- 表格操作 -->
				<template #operation="{ row }: { row: QueryDepartmentPagedOutput }">
					<el-button v-auth="'Department:Detail'" size="small" plain @click="editFormRef.detail(row.departmentId)">详情</el-button>
					<el-button v-auth="'Department:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.departmentId)">
						编辑
					</el-button>
					<el-button v-auth="'Department:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
				</template>
			</FastTable>
		</div>
		<FaContextMenu ref="faContextMenuRef" :data="state.contextMenuList" />
		<DepartmentEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
		<OrgEdit ref="orgEditFormRef" @ok="orgTreeRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { reactive, useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { departmentApi } from "@/api/services/Admin/department";
import { organizationApi } from "@/api/services/Admin/organization";
import DepartmentEdit from "./edit/index.vue";
import OrgEdit from "./edit/orgEdit.vue";
import type { ElTreeOutput, FaContextMenuData, FaContextMenuInstance, FaTreeInstance } from "fast-element-plus";
import type { QueryDepartmentPagedOutput } from "@/api/services/Admin/department/models/QueryDepartmentPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemDepartment",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const orgTreeRef = useTemplateRef<FaTreeInstance>("orgTreeRef");
const faContextMenuRef = useTemplateRef<FaContextMenuInstance>("faContextMenuRef");
const editFormRef = useTemplateRef<InstanceType<typeof DepartmentEdit>>("editFormRef");
const orgEditFormRef = useTemplateRef<InstanceType<typeof OrgEdit>>("orgEditFormRef");

const state = reactive({
	contextMenuList: withDefineType<FaContextMenuData[]>([
		{
			name: "add",
			label: "添加机构",
			icon: "el-icon-FolderAdd",
			click: () => {
				orgEditFormRef.value.add();
			},
		},
		{
			name: "edit",
			label: "编辑机构",
			icon: "el-icon-EditPen",
			click: (_, { data }: { data?: ElTreeOutput }) => {
				if (!data || typeof data.value !== "string") return;
				orgEditFormRef.value.edit(data.value);
			},
		},
		{
			name: "delete",
			label: "删除机构",
			icon: "el-icon-Delete",
			click: (_, { data }: { data?: ElTreeOutput }) => {
				if (!data || typeof data.value !== "string") return;
				const { value: orgId, data: orgData } = data;
				ElMessageBox.confirm("确定要删除机构？", {
					type: "warning",
				}).then(async () => {
					await organizationApi.deleteOrganization({ orgId, rowVersion: orgData?.rowVersion });
					ElMessage.success("删除成功！");
					await orgTreeRef.value?.refresh();
				});
			},
		},
	]),
});

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryDepartmentPagedOutput }) => {
	editFormRef.value.detail(row.departmentId);
};

/** 机构更改 */
const handleOrgChange = async (data: ElTreeOutput) => {
	fastTableRef.value.searchParam.orgId = data.value;
	await fastTableRef.value.refresh();
};

const handleOrgContextmenu = (event: MouseEvent, data: ElTreeOutput) => {
	if (data.all) {
		state.contextMenuList[0].hide = false;
		state.contextMenuList[1].hide = true;
		state.contextMenuList[2].hide = true;
	} else {
		state.contextMenuList[0].hide = true;
		state.contextMenuList[1].hide = false;
		state.contextMenuList[2].hide = false;
	}
	state.contextMenuList.forEach((item) => {
		item.data = data;
	});
	faContextMenuRef.value.open({ x: event.clientX, y: event.clientY });
};

/** 处理删除 */
const handleDelete = (row: QueryDepartmentPagedOutput) => {
	const { departmentId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除部门？", {
		type: "warning",
	}).then(async () => {
		await departmentApi.deleteDepartment({ departmentId, rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
