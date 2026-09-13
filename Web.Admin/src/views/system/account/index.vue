<template>
	<div>
		<FastTable ref="fastTableRef" table-key="1D1FXFM1GH" row-key="accountId" :request-api="accountApi.queryAccountPaged" hide-search-time>
			<template #mobile="{ row }: { row?: QueryAccountPagedOutput }">
				<el-button link type="primary" @click="editFormRef.detail(row.accountId)">{{ row.nickName }}</el-button>
				<br />
				手机：<span v-iconCopy="row.mobile">{{ row.mobile }}</span>
				<br />
				邮箱：<span v-iconCopy="row.email">{{ row.email }}</span>
			</template>

			<template #firstLoginTime="{ row }: { row?: QueryAccountPagedOutput }">
				<span>地区：{{ row.firstLoginProvince }} - {{ row.firstLoginCity }}</span>
				<br />
				<span>Ip：{{ row.firstLoginIp }}</span>
				<br />
				<span>时间：{{ dayjs(row.firstLoginTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.firstLoginTime" type="info" round effect="light" size="small" class="ml5">
					{{ formatChineseRelativeTime(row.firstLoginTime) }}
				</el-tag>
			</template>

			<template #firstLoginOS="{ row }: { row?: QueryAccountPagedOutput }">
				<span>设备：{{ row.firstLoginDevice }}</span>
				<br />
				<span>操作系统：{{ row.firstLoginOS }}</span>
				<br />
				<span>浏览器：{{ row.firstLoginBrowser }}</span>
			</template>

			<template #lastLoginTime="{ row }: { row?: QueryAccountPagedOutput }">
				<span>地区：{{ row.lastLoginProvince }} - {{ row.lastLoginCity }}</span>
				<br />
				<span>Ip：{{ row.lastLoginIp }}</span>
				<br />
				<span>时间：{{ dayjs(row.lastLoginTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.lastLoginTime" type="info" round effect="light" size="small" class="ml5">
					{{ formatChineseRelativeTime(row.lastLoginTime) }}
				</el-tag>
			</template>

			<template #lastLoginOS="{ row }: { row?: QueryAccountPagedOutput }">
				<span>设备：{{ row.lastLoginDevice }}</span>
				<br />
				<span>操作系统：{{ row.lastLoginOS }}</span>
				<br />
				<span>浏览器：{{ row.lastLoginTime }}</span>
			</template>

			<template #lockStartTime="{ row }: { row?: QueryAccountPagedOutput }">
				<template v-if="row.isLock">
					<el-tag type="warning">已锁定</el-tag>
					<br />
					<span>错误次数：{{ row.passwordErrorTime }}次</span>
					<br />
					<span>开始时间：{{ dayjs(row.lockStartTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
					<br />
					<span>结束时间：{{ dayjs(row.lockEndTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				</template>
				<template v-else>
					<el-tag type="info">未锁定</el-tag>
				</template>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryAccountPagedOutput }">
				<el-button v-auth="'Account:Detail'" size="small" plain @click="editFormRef.detail(row.accountId)">详情</el-button>
				<el-button v-auth="'Account:ResetPassword'" size="small" plain type="warning" @click="handleResetPassword(row)">重置密码</el-button>
				<el-button v-if="row.isLock" v-auth="'Account:Unlock'" size="small" plain type="warning" @click="handleUnlock(row)">解锁</el-button>
				<el-button
					v-if="row.status == CommonStatusEnum.Enable"
					v-auth="'Account:Status'"
					size="small"
					plain
					type="danger"
					@click="handleChangeStatus(row)"
				>
					禁用
				</el-button>
				<el-button v-else v-auth="'Account:Status'" size="small" plain type="warning" @click="handleChangeStatus(row)">启用</el-button>
			</template>
		</FastTable>
		<AccountEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { ElMessage, ElMessageBox, dayjs } from "element-plus";
import { formatChineseRelativeTime } from "@fast-china/utils";
import { CommonStatusEnum } from "@/api/enums/CommonStatusEnum";
import { accountApi } from "@/api/services/Center/account";
import AccountEdit from "./edit/index.vue";
import type { QueryAccountPagedOutput } from "@/api/services/Center/account/models/QueryAccountPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemAccount",
});

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof AccountEdit>>("editFormRef");

/** 处理重置密码 */
const handleResetPassword = (row: QueryAccountPagedOutput) => {
	const { accountId, rowVersion } = row;
	ElMessageBox.confirm("确定重置密码？", {
		type: "warning",
	}).then(async () => {
		await accountApi.resetPassword({
			accountId,
			rowVersion,
		});
		ElMessage.success("操作成功！");
		await fastTableRef.value?.refresh();
	});
};

/** 处理解锁 */
const handleUnlock = (row: QueryAccountPagedOutput) => {
	const { accountId, rowVersion } = row;
	ElMessageBox.confirm("确定解锁账号？", {
		type: "warning",
	}).then(async () => {
		await accountApi.unlock({
			accountId,
			rowVersion,
		});
		ElMessage.success("操作成功！");
		await fastTableRef.value?.refresh();
	});
};

/** 处理状态变更 */
const handleChangeStatus = (row: QueryAccountPagedOutput) => {
	const { accountId, status, rowVersion } = row;
	ElMessageBox.confirm(`确定${status === CommonStatusEnum.Enable ? "禁用" : "启用"}账号？`, {
		type: "warning",
	}).then(async () => {
		await accountApi.changeStatus({
			accountId,
			rowVersion,
		});
		ElMessage.success("操作成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
