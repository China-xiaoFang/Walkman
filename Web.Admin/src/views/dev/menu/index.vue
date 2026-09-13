<template>
	<div>
		<div class="fa__display_lr-r">
			<ApplicationTree @change="handleApplicationChange" />
			<FastTable
				ref="fastTableRef"
				table-key="1D11Q5S4P2"
				row-key="menuId"
				:request-api="menuApi.queryMenuPaged"
				hide-search-time
				:pagination="false"
				default-expand-all
			>
				<!-- 表格按钮操作区域 -->
				<template #header>
					<el-button v-auth="'Menu:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
				</template>

				<template #menuName="{ row }: { row?: QueryMenuPagedOutput }">
					<FaIcon v-if="row.webIcon" style="margin-right: 5px" size="16" :name="row.webIcon" />
					<span>{{ row.menuName }}</span>
				</template>

				<template #web="{ row }: { row?: QueryMenuPagedOutput }">
					<div style="display: flex; align-items: center; gap: 5px">
						<Tag size="small" name="CommonStatusEnum" :value="row.hasWeb ? CommonStatusEnum.Enable : CommonStatusEnum.Disable" />
						<FaIcon v-if="row.webIcon" size="16" :name="row.webIcon" />
						<el-tag v-if="row.webRouter" type="primary" effect="plain">{{ row.webRouter }}</el-tag>
					</div>
					<el-tag v-if="row.webComponent" type="info" effect="plain">{{ row.webComponent }}</el-tag>
				</template>

				<template #mobile="{ row }: { row?: QueryMenuPagedOutput }">
					<div style="display: flex; align-items: center; gap: 5px">
						<Tag size="small" name="CommonStatusEnum" :value="row.hasMobile ? CommonStatusEnum.Enable : CommonStatusEnum.Disable" />
						<FaImage
							v-if="row.mobileIcon"
							src="https://gitee.com/FastDotnet/Fast.Admin/raw/master/Fast.png@!thumb"
							original
							:preview="false"
						/>
					</div>
					<el-tag v-if="row.mobileRouter" type="info" effect="plain">{{ row.mobileRouter }}</el-tag>
				</template>

				<template #desktop="{ row }: { row?: QueryMenuPagedOutput }">
					<div style="display: flex; align-items: center; gap: 5px">
						<Tag size="small" name="CommonStatusEnum" :value="row.hasDesktop ? CommonStatusEnum.Enable : CommonStatusEnum.Disable" />
						<el-tag v-if="row.desktopIcon" type="primary" effect="plain">{{ row.desktopIcon }}</el-tag>
					</div>
					<el-tag v-if="row.desktopRouter" type="info" effect="plain">{{ row.desktopRouter }}</el-tag>
				</template>

				<template #link="{ row }: { row?: QueryMenuPagedOutput }">
					<el-link type="info" target="_blank" rel="noopener noreferrer" :href="row.link">{{ row.link }}</el-link>
				</template>

				<!-- 表格操作 -->
				<template #operation="{ row }: { row: QueryMenuPagedOutput }">
					<el-button v-auth="'Menu:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.menuId)">编辑</el-button>
					<el-button v-auth="'Menu:Delete'" size="small" plain type="warning" @click="handleDelete(row)">删除</el-button>
					<el-button
						v-auth="'Menu:Status'"
						v-if="row.status == CommonStatusEnum.Enable"
						size="small"
						plain
						type="danger"
						@click="handleChangeStatus(row)"
					>
						禁用
					</el-button>
					<el-button v-auth="'Menu:Status'" v-else size="small" plain type="warning" @click="handleChangeStatus(row)">启用</el-button>
				</template>
			</FastTable>
		</div>
		<MenuEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";
import { menuApi } from "@/api/services/Center/menu";
import MenuEdit from "./edit/index.vue";
import type { ElSelectorOutput } from "fast-element-plus";
import type { QueryMenuPagedOutput } from "@/api/services/Center/menu/models/QueryMenuPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "DevMenu",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof MenuEdit>>("editFormRef");

/** 应用更改 */
const handleApplicationChange = (data: ElSelectorOutput) => {
	fastTableRef.value.searchParam.appId = data.value;
	fastTableRef.value.refresh();
};

/** 处理删除 */
const handleDelete = (row: QueryMenuPagedOutput) => {
	const { menuId, rowVersion } = row;
	ElMessageBox.confirm("确定要删除菜单？", {
		type: "warning",
	}).then(async () => {
		await menuApi.deleteMenu({ menuId, rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};

/** 处理状态变更 */
const handleChangeStatus = (row: QueryMenuPagedOutput) => {
	const { menuId, status, rowVersion } = row;
	ElMessageBox.confirm(`确定${status === CommonStatusEnum.Enable ? "禁用" : "启用"}菜单？`, {
		type: "warning",
	}).then(async () => {
		await menuApi.changeStatus({
			menuId,
			rowVersion,
		});
		ElMessage.success("操作成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
