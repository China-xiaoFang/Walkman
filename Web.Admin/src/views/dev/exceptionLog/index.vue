<template>
	<div>
		<FastTable tableKey="1D11ZDNHT5" rowKey="recordId" :requestApi="exceptionLogApi.queryExceptionLogPaged" stripe>
			<template #mobile="{ row }: { row?: ExceptionLogModel }">
				{{ row.nickName }}
				<br />
				手机：<span v-iconCopy="row.mobile">{{ row.mobile }}</span>
			</template>

			<template #os="{ row }: { row?: ExceptionLogModel }">
				<span>设备：{{ row.device }}</span>
				<br />
				<span>操作系统：{{ row.os }}</span>
				<br />
				<span>浏览器：{{ row.browser }}</span>
			</template>

			<template #createdTime="{ row }: { row?: ExceptionLogModel }">
				<span>地区：{{ row.province }} - {{ row.city }}</span>
				<br />
				<span>Ip：{{ row.ip }}</span>
				<br />
				<span>时间：{{ dayjs(row.createdTime).format("YYYY-MM-DD HH:mm:ss") }}</span>
				<el-tag v-if="row.createdTime" type="info" round effect="light" size="small" class="ml5">
					{{ dateUtil.dateTimeFix(String(row.createdTime)) }}
				</el-tag>
			</template>

			<template #stackTrace="{ row }: { row?: ExceptionLogModel }">
				<el-tag
					v-if="row.stackTrace"
					type="danger"
					style="cursor: pointer"
					@click="
						() => {
							state.title = '堆栈信息';
							state.content = row.stackTrace || '';
							state.visible = true;
						}
					"
				>
					查看
				</el-tag>
				<span v-else>--</span>
			</template>

			<template #paramsObj="{ row }: { row?: ExceptionLogModel }">
				<el-tag
					v-if="row.paramsObj"
					type="danger"
					style="cursor: pointer"
					@click="
						() => {
							state.title = '参数对象';
							state.content = row.paramsObj || '';
							state.visible = true;
						}
					"
				>
					查看
				</el-tag>
				<span v-else>--</span>
			</template>
		</FastTable>
		<el-dialog v-model="state.visible" :title="state.title" width="1000px" alignCenter draggable destroyOnClose>
			<el-scrollbar>
				<div style="max-height: 500px; padding-bottom: 20px; padding-right: 10px">
					<VueJsonPretty
						:data="jsonContent"
						:deep="3"
						showLength
						showLineNumber
						showIcon
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
import { dateUtil } from "@fast-china/utils";
import VueJsonPretty from "vue-json-pretty";
import { exceptionLogApi } from "@/api/services/Admin/exceptionLog";
import { ExceptionLogModel } from "@/api/services/Admin/exceptionLog/models/ExceptionLogModel";
import { useConfig } from "@/stores";

defineOptions({
	name: "DevExceptionLog",
});

const configStore = useConfig();

const state = reactive({
	visible: false,
	title: "日志",
	content: "",
});

const jsonContent = computed(() => {
	try {
		return JSON.parse(state.content);
	} catch {
		return state.content;
	}
});
</script>
