<template>
	<div>
		<FastTable table-key="1D1K9GNFPT" row-key="recordId" :request-api="payRecordApi.queryPayRecordPaged">
			<template #mobile="{ row }: { row?: PayRecordModel }">
				<span v-iconCopy="row.mobile">{{ row.mobile }}</span>
				<br />
				OpenId：<span v-iconCopy="row.openId">{{ row.openId }}</span>
				<br />
				UnionId：<span v-iconCopy="row.unionId">{{ row.unionId }}</span>
			</template>

			<template #os="{ row }: { row?: PayRecordModel }">
				<span>设备：{{ row.device }}</span>
				<br />
				<span>操作系统：{{ row.os }}</span>
				<br />
				<span>浏览器：{{ row.browser }}</span>
			</template>

			<template #createdTime="{ row }: { row?: PayRecordModel }">
				<span>地区：{{ row.province }} - {{ row.city }}</span>
				<br />
				<span>Ip：{{ row.ip }}</span>
				<br />
				<span>时间：{{ dayjs(row.createdTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.createdTime" type="info" round effect="light" size="small" class="ml5">
					{{ formatChineseRelativeTime(row.createdTime) }}
				</el-tag>
			</template>

			<template #paymentTime="{ row }: { row?: PayRecordModel }">
				<span>状态：<Tag name="BooleanEnum" :value="row.isPaid" /></span>
				<br />
				<span>金额：￥{{ row.paymentAmount?.toFixed(2) || 0.0 }}</span>
				<br />
				<template v-if="row.paymentTime">
					<span>时间：{{ dayjs(row.paymentTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
					<el-tag v-if="row.paymentTime" type="info" round effect="light" size="small" class="ml5">
						{{ formatChineseRelativeTime(row.paymentTime) }}
					</el-tag>
				</template>
				<span v-else>--</span>
			</template>

			<template #closeTime="{ row }: { row?: PayRecordModel }">
				<template v-if="row.isPaid">
					<span>--</span>
				</template>
				<template v-else>
					<span>状态：<Tag name="BooleanEnum" :value="row.isClosed" /></span>
					<br />
					<template v-if="row.closeTime">
						<span>时间：{{ dayjs(row.closeTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
						<el-tag v-if="row.closeTime" type="info" round effect="light" size="small" class="ml5">
							{{ formatChineseRelativeTime(row.closeTime) }}
						</el-tag>
					</template>
					<span v-else>--</span>
				</template>
			</template>
		</FastTable>
	</div>
</template>

<script lang="ts" setup>
import { dayjs } from "element-plus";
import { formatChineseRelativeTime } from "@fast-china/utils";
import { payRecordApi } from "@/api/services/Center/payRecord";
import type { PayRecordModel } from "@/api/services/Center/payRecord/models/PayRecordModel";

defineOptions({
	name: "SystemPayRecord",
});
</script>
