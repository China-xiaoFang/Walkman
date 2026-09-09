<template>
	<div>
		<FastTable
			ref="fastTableRef"
			table-key="RRRVWF9K66R"
			row-key="audioAssetId"
			:request-api="audioAssetApi.queryAudioAssetPaged"
			hide-search-time
			@custom-cell-click="handleCustomCellClick"
		>
			<template #header>
				<el-button v-auth="'AudioAsset:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #bookId="{ searchParam }">
				<FaSelectPage :request-api="bookApi.bookSelector" v-model="searchParam.bookId" placeholder="请选择教材" filterable clearable />
			</template>

			<template #lessonId="{ searchParam }">
				<FaSelectPage
					:request-api="lessonApi.lessonSelector"
					:init-param="{ bookId: searchParam.bookId }"
					:disabled="!searchParam.bookId"
					v-model="searchParam.lessonId"
					placeholder="请选择课程"
					filterable
					clearable
					more-detail
				>
					<template #default="data">
						<span>{{ data.label }}</span>
						<span style="display: flex; justify-content: space-between; width: 100%">
							<span style="font-size: var(--el-font-size-extra-small); padding-right: 8px">第 {{ data.data?.lessonNumber }} 课</span>
							<span style="font-size: var(--el-font-size-extra-small)">{{ data.data?.bookName }}</span>
						</span>
					</template>
				</FaSelectPage>
			</template>

			<template #audioUrl="{ row }: { row?: QueryAudioAssetPagedOutput }">
				<audio v-if="row?.audioUrl" style="display: block; width: 240px; height: 32px" :src="row.audioUrl" controls preload="none" />
			</template>

			<template #operation="{ row }: { row: QueryAudioAssetPagedOutput }">
				<el-button v-auth="'AudioAsset:Detail'" size="small" plain @click="editFormRef.detail(row.audioAssetId!)">详情</el-button>
				<el-button v-auth="'AudioAsset:Edit'" size="small" plain type="primary" @click="editFormRef.edit(row.audioAssetId!)">编辑</el-button>
				<el-button v-auth="'AudioAsset:Delete'" size="small" plain type="danger" @click="handleDelete(row)">删除</el-button>
			</template>
		</FastTable>
		<AudioAssetEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { useTemplateRef } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { audioAssetApi } from "@/api/services/Walkman/audioAsset";
import { bookApi } from "@/api/services/Walkman/book";
import { lessonApi } from "@/api/services/Walkman/lesson";
import AudioAssetEdit from "./edit/index.vue";
import type { QueryAudioAssetPagedOutput } from "@/api/services/Walkman/audioAsset/models/QueryAudioAssetPagedOutput";
import type { FastTableInstance } from "@/components";

defineOptions({ name: "WalkmanAudioAsset" });

const fastTableRef = useTemplateRef<FastTableInstance>("fastTableRef");
const editFormRef = useTemplateRef<InstanceType<typeof AudioAssetEdit>>("editFormRef");

const handleCustomCellClick = (_emitName: string, { row }: { row: QueryAudioAssetPagedOutput }) => {
	if (row.audioAssetId) editFormRef.value.detail(row.audioAssetId);
};

const handleDelete = (row: QueryAudioAssetPagedOutput) => {
	void ElMessageBox.confirm("确定要删除音频资源？关联的歌词文档将同步删除。", { type: "warning" }).then(async () => {
		await audioAssetApi.deleteAudioAsset({ audioAssetId: row.audioAssetId, rowVersion: row.rowVersion });
		ElMessage.success("删除成功！");
		await fastTableRef.value?.refresh();
	});
};
</script>
