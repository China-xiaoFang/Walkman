<template>
	<div>
		<FastTable table-key="FRMY6974MU" row-key="recordId" :request-api="refundRecordApi.queryRefundRecordPaged">
			<template #mobile="{ row }: { row?: RefundRecordModel }">
				<span v-iconCopy="row.mobile">{{ row.mobile }}</span>
				<br />
				OpenId：<span v-iconCopy="row.openId">{{ row.openId }}</span>
				<br />
				UnionId：<span v-iconCopy="row.unionId">{{ row.unionId }}</span>
			</template>

			<template #os="{ row }: { row?: RefundRecordModel }">
				<span>设备：{{ row.device }}</span>
				<br />
				<span>操作系统：{{ row.os }}</span>
				<br />
				<span>浏览器：{{ row.browser }}</span>
			</template>

			<template #createdTime="{ row }: { row?: RefundRecordModel }">
				<span>地区：{{ row.province }} - {{ row.city }}</span>
				<br />
				<span>Ip：{{ row.ip }}</span>
				<br />
				<span>时间：{{ dayjs(row.createdTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.createdTime" type="info" round effect="light" size="small" class="ml5">
					{{ formatChineseRelativeTime(row.createdTime) }}
				</el-tag>
			</template>

			<template #refundTime="{ row }: { row?: RefundRecordModel }">
				<span>状态：<Tag name="BooleanEnum" :value="row.isRefunded" /></span>
				<br />
				<span>金额：￥{{ row.refundAmount?.toFixed(2) || 0.0 }}</span>
				<br />
				<template v-if="row.refundTime">
					<span>时间：{{ dayjs(row.refundTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
					<el-tag v-if="row.refundTime" type="info" round effect="light" size="small" class="ml5">
						{{ formatChineseRelativeTime(row.refundTime) }}
					</el-tag>
				</template>
				<span v-else>--</span>
			</template>
		</FastTable>
	</div>
</template>

<script lang="ts" setup>
import { dayjs } from "element-plus";
import { formatChineseRelativeTime } from "@fast-china/utils";
import { refundRecordApi } from "@/api/services/Center/refundRecord";
import type { RefundRecordModel } from "@/api/services/Center/refundRecord/models/RefundRecordModel";

defineOptions({
	name: "SystemRefundRecord",
});
</script>
