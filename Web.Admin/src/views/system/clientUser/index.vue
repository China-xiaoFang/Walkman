<template>
	<div>
		<FastTable table-key="1D1KL4GV24" row-key="clientUserId" :request-api="clientUserApi.queryClientUserPaged">
			<template #lastLoginOS="{ row }: { row?: QueryClientUserPagedOutput }">
				<span>设备：{{ row.lastLoginDevice }}</span>
				<br />
				<span>操作系统：{{ row.lastLoginOS }}</span>
				<br />
				<span>浏览器：{{ row.lastLoginBrowser }}</span>
			</template>

			<template #lastLoginTime="{ row }: { row?: QueryClientUserPagedOutput }">
				<span>地区：{{ row.lastLoginProvince }} - {{ row.lastLoginCity }}</span>
				<br />
				<span>Ip：{{ row.lastLoginIp }}</span>
				<br />
				<span>时间：{{ dayjs(row.lastLoginTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.lastLoginTime" type="info" round effect="light" size="small" class="ml5">
					{{ formatChineseRelativeTime(row.createdTime) }}
				</el-tag>
			</template>
		</FastTable>
	</div>
</template>

<script lang="ts" setup>
import { dayjs } from "element-plus";
import { formatChineseRelativeTime } from "@fast-china/utils";
import { clientUserApi } from "@/api/services/Center/clientUser";
import type { QueryClientUserPagedOutput } from "@/api/services/Center/clientUser/models/QueryClientUserPagedOutput";

defineOptions({
	name: "SystemClientUser",
});
</script>
