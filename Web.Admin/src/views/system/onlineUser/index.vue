<template>
	<div>
		<FastTable ref="fastTableRef" tableKey="1D1K3NW4XY" rowKey="connectionId" :requestApi="onlineUserApi.queryOnlineUserPaged" hideSearchTime>
			<template #mobile="{ row }: { row?: OnlineUserModel }">
				<span>{{ row.mobile }}</span>
				<br />
				手机：<span v-iconCopy="row.mobile">{{ row.mobile }}</span>
			</template>

			<template #employeeNo="{ row }: { row?: OnlineUserModel }">
				<span>{{ row.employeeName }}</span>
				<br />
				工号：<span v-iconCopy="row.employeeNo">{{ row.employeeNo }}</span>
				<br />
				部门：<span>{{ row.departmentName }}</span>
			</template>

			<template #lastLoginTime="{ row }: { row?: OnlineUserModel }">
				<span>地区：{{ row.lastLoginProvince }} - {{ row.lastLoginCity }}</span>
				<br />
				<span>Ip：{{ row.lastLoginIp }}</span>
				<br />
				<span>时间：{{ dayjs(row.lastLoginTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.lastLoginTime" type="info" round effect="light" size="small" class="ml5">
					{{ dateUtil.dateTimeFix(String(row.lastLoginTime)) }}
				</el-tag>
			</template>

			<template #lastLoginOS="{ row }: { row?: OnlineUserModel }">
				<span>设备：{{ row.lastLoginDevice }}</span>
				<br />
				<span>操作系统：{{ row.lastLoginOS }}</span>
				<br />
				<span>浏览器：{{ row.lastLoginTime }}</span>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: OnlineUserModel }">
				<el-button v-auth="'OnlineUser:ForceOffline'" size="small" plain type="warning" @click="handleForceOffline(row)">
					强制下线
				</el-button>
			</template>
		</FastTable>
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox, dayjs } from "element-plus";
import { dateUtil } from "@fast-china/utils";
import { onlineUserApi } from "@/api/services/Admin/onlineUser";
import { OnlineUserModel } from "@/api/services/Admin/onlineUser/models/OnlineUserModel";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemOnlineUser",
});

const fastTableRef = ref<FastTableInstance>();

/** 处理重置密码 */
const handleForceOffline = (row: OnlineUserModel) => {
	const { connectionId, mobile } = row;
	ElMessageBox.confirm(`确定踢掉账号：【${mobile}】`, {
		type: "warning",
		async beforeClose() {
			await onlineUserApi.forceOffline({
				connectionId,
			});
			ElMessage.success("强制下线成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
