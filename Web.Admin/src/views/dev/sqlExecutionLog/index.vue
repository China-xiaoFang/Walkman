<template>
	<div>
		<FastTable ref="fastTableRef" tableKey="1D11PB3CXH" rowKey="recordId" :requestApi="sqlExecutionLogApi.querySqlExecutionLogPaged" stripe>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-if="userInfoStore.isSuperAdmin" plain type="danger" :icon="Delete" @click="handleDeleteLog">删除日志</el-button>
			</template>

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
					{{ dateUtil.dateTimeFix(String(row.createdTime)) }}
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
import { computed, reactive, ref } from "vue";
import { ElMessage, ElMessageBox, dayjs } from "element-plus";
import { Delete } from "@element-plus/icons-vue";
import { dateUtil } from "@fast-china/utils";
import VueJsonPretty from "vue-json-pretty";
import { sqlExecutionLogApi } from "@/api/services/Admin/sqlExecutionLog";
import { SqlExecutionLogModel } from "@/api/services/Admin/sqlExecutionLog/models/SqlExecutionLogModel";
import { FastTableInstance } from "@/components";
import { useConfig, useUserInfo } from "@/stores";

defineOptions({
	name: "DevSqlExecutionLog",
});

const configStore = useConfig();
const userInfoStore = useUserInfo();

const fastTableRef = ref<FastTableInstance>();

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

/** 处理删除日志 */
const handleDeleteLog = () => {
	ElMessageBox.confirm("确定要删除90天前的执行日志？", {
		type: "warning",
		async beforeClose() {
			await sqlExecutionLogApi.deleteSqlExecutionLog();
			ElMessage.success("删除成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
