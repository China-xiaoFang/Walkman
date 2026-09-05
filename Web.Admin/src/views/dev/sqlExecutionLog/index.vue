<template>
	<div>
		<FastTable table-key="1D11PB3CXH" row-key="recordId" :request-api="sqlExecutionLogApi.querySqlExecutionLogPaged">
			<template #mobile="{ row }: { row?: SqlExecutionLogModel }">
				{{ row.nickName }}
				<br />
				手机：<span v-iconCopy="row.mobile">{{ row.mobile }}</span>
			</template>

			<template #os="{ row }: { row?: SqlExecutionLogModel }">
				<span>设备：{{ row.device }}</span>
				<br />
				<span>操作系统：{{ row.os }}</span>
				<br />
				<span>浏览器：{{ row.browser }}</span>
			</template>

			<template #createdTime="{ row }: { row?: SqlExecutionLogModel }">
				<span>地区：{{ row.province }} - {{ row.city }}</span>
				<br />
				<span>Ip：{{ row.ip }}</span>
				<br />
				<span>时间：{{ dayjs(row.createdTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.createdTime" type="info" round effect="light" size="small" class="ml5">
					{{ formatChineseRelativeTime(row.createdTime) }}
				</el-tag>
			</template>

			<template #pureSql="{ row }: { row?: SqlExecutionLogModel }">
				<el-tag
					v-if="row.pureSql"
					type="info"
					style="cursor: pointer"
					@click="
						() => {
							state.title = '纯Sql';
							state.content = row.pureSql || '';
							state.visible = true;
						}
					"
				>
					查看
				</el-tag>
				<span v-else>--</span>
			</template>
		</FastTable>
		<el-dialog v-model="state.visible" :title="state.title" width="1000px" align-center draggable destroy-on-close>
			<el-scrollbar>
				<div style="max-height: 500px; padding-bottom: 20px; padding-right: 10px">
					<VueJsonPretty
						:data="jsonContent"
						:deep="3"
						show-length
						show-line-number
						show-icon
						virtual
						:height="500"
						:theme="configStore.layout.isDark ? 'dark' : 'light'"
					/>
				</div>
			</el-scrollbar>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup>
import { computed, reactive } from "vue";
import { dayjs } from "element-plus";
import { formatChineseRelativeTime } from "@fast-china/utils";
import VueJsonPretty from "vue-json-pretty";
import { sqlExecutionLogApi } from "@/api/services/Center/sqlExecutionLog";
import { useConfig } from "@/stores";
import type { JSONDataType } from "vue-json-pretty/types/utils";
import type { SqlExecutionLogModel } from "@/api/services/Center/sqlExecutionLog/models/SqlExecutionLogModel";

defineOptions({
	name: "DevSqlExecutionLog",
});

const configStore = useConfig();

const state = reactive({
	visible: false,
	title: "日志",
	content: "",
});

const jsonContent = computed<JSONDataType>(() => {
	try {
		return JSON.parse(state.content) as JSONDataType;
	} catch {
		return state.content;
	}
});
</script>
