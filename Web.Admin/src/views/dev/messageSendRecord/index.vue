<template>
	<div>
		<FastTable ref="fastTableRef" table-key="R8FWK6WH52Q" row-key="recordId" :request-api="messageSendRecordApi.queryMessageSendRecordPaged">
			<template #os="{ row }: { row?: QueryMessageSendRecordPagedOutput }">
				<span>设备：{{ row.device }}</span>
				<br />
				<span>操作系统：{{ row.os }}</span>
				<br />
				<span>浏览器：{{ row.browser }}</span>
			</template>

			<template #createdTime="{ row }: { row?: QueryMessageSendRecordPagedOutput }">
				<span>地区：{{ row.province }} - {{ row.city }}</span>
				<br />
				<span>Ip：{{ row.ip }}</span>
				<br />
				<span>时间：{{ dayjs(row.createdTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.createdTime" type="info" round effect="light" size="small" class="ml5">
					{{ formatChineseRelativeTime(row.createdTime) }}
				</el-tag>
			</template>

			<template #operation="{ row }: { row?: QueryMessageSendRecordPagedOutput }">
				<el-button v-auth="'MessageSendRecord:Detail'" size="small" plain @click="editFormRef.detail(row.recordId)">详情</el-button>
			</template>
		</FastTable>
		<MessageSendRecordEdit ref="editFormRef" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { dayjs } from "element-plus";
import { formatChineseRelativeTime } from "@fast-china/utils";
import { messageSendRecordApi } from "@/api/services/Center/messageSendRecord";
import MessageSendRecordEdit from "./edit/index.vue";
import type { QueryMessageSendRecordPagedOutput } from "@/api/services/Center/messageSendRecord/models/QueryMessageSendRecordPagedOutput";

defineOptions({
	name: "DevMessageSendRecord",
});

const editFormRef = useTemplateRef<InstanceType<typeof MessageSendRecordEdit>>("editFormRef");
</script>
