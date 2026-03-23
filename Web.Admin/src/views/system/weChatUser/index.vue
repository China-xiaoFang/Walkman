<template>
	<div>
		<FastTable tableKey="1D1KL4GV24" rowKey="weChatId" :requestApi="weChatApi.queryWeChatUserPaged" stripe>
			<template #lastLoginOS="{ row }: { row?: QueryWeChatUserPagedOutput }">
				<span>设备：{{ row.lastLoginDevice }}</span>
				<br />
				<span>操作系统：{{ row.lastLoginOS }}</span>
				<br />
				<span>浏览器：{{ row.lastLoginBrowser }}</span>
			</template>

			<template #lastLoginTime="{ row }: { row?: QueryWeChatUserPagedOutput }">
				<span>地区：{{ row.lastLoginProvince }} - {{ row.lastLoginCity }}</span>
				<br />
				<span>Ip：{{ row.lastLoginIp }}</span>
				<br />
				<span>时间：{{ dayjs(row.lastLoginTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.lastLoginTime" type="info" round effect="light" size="small" class="ml5">
					{{ dateUtil.dateTimeFix(String(row.createdTime)) }}
				</el-tag>
			</template>
		</FastTable>
	</div>
</template>

<script lang="ts" setup>
import { dayjs } from "element-plus";
import { dateUtil } from "@fast-china/utils";
import { weChatApi } from "@/api/services/Admin/weChat";
import { QueryWeChatUserPagedOutput } from "@/api/services/Admin/weChat/models/QueryWeChatUserPagedOutput";

defineOptions({
	name: "SystemWeChatUser",
});
</script>
