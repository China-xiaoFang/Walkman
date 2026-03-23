<template>
	<div>
		<FastTable ref="fastTableRef" tableKey="1D1K7QDL5Y" rowKey="complaintId" :requestApi="complaintApi.queryComplaintPaged" hideSearchTime>
			<template #attachmentImages="{ row }: { row?: QueryComplaintPagedOutput }">
				<el-button size="small" plain @click="state.previewSrcList = row.attachmentImages"> 查看 </el-button>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryComplaintPagedOutput }">
				<el-button
					v-if="!row.handleTime"
					v-auth="'Complaint:Handle'"
					size="small"
					plain
					type="primary"
					@click="editFormRef.open(row.complaintId)"
				>
					处理
				</el-button>
			</template>
		</FastTable>
		<ComplaintEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
		<el-image-viewer
			v-if="state.previewSrcList.length > 0"
			:urlList="state.previewSrcList"
			hideOnClickModal
			teleported
			showProgress
			@close="state.previewSrcList = []"
		/>
	</div>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { complaintApi } from "@/api/services/Admin/complaint";
import { QueryComplaintPagedOutput } from "@/api/services/Admin/complaint/models/QueryComplaintPagedOutput";
import ComplaintEdit from "./edit/index.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemComplaint",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof ComplaintEdit>>();

const state = reactive({
	previewSrcList: [],
});
</script>
