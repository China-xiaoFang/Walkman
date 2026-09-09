<template>
	<FaDialog
		ref="faDialogRef"
		width="1200"
		:title="state.dialogTitle"
		:show-confirm-button="!state.formDisabled"
		:show-before-close="!state.formDisabled"
		confirm-button-text="保存"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" :disabled="state.formDisabled" cols="2">
			<FaFormItem prop="bookId" label="教材">
				<FaSelectPage
					:request-api="bookApi.bookSelector"
					v-model="state.formData.bookId"
					v-model:label="state.formData.bookName"
					placeholder="请选择教材"
					filterable
					clearable
				/>
			</FaFormItem>
			<FaFormItem prop="lessonId" label="课程">
				<FaSelectPage
					:key="state.formData.bookId"
					:request-api="lessonApi.lessonSelector"
					:init-param="{ bookId: state.formData.bookId }"
					:disabled="state.formDisabled || !state.formData.bookId"
					v-model="state.formData.lessonId"
					v-model:label="state.formData.lessonTitle"
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
			</FaFormItem>
			<FaFormItem prop="audioType" label="音频类型">
				<RadioGroup name="AudioTypeEnum" v-model="state.formData.audioType" />
			</FaFormItem>
			<FaFormItem prop="audioDuration" label="音频时长">
				<el-input v-model="state.formData.audioDuration" readonly placeholder="上传音频后自动识别" />
			</FaFormItem>
			<FaFormItem prop="audioUrl" label="音频文件" span="2">
				<FaUpload
					v-model="state.formData.audioUrl"
					:upload-api="fileApi.uploadAudioAsset"
					:before-upload="handleBeforeAudioUpload"
					accept="audio/*"
					:max-size="10240"
					@success="handleAudioUploadSuccess"
					@error="handleAudioUploadError"
					@remove="handleAudioRemove"
				/>
			</FaFormItem>
			<FaFormItem v-if="state.formData.audioTicketUrl" prop="audioTicketUrl" label="音频" span="2">
				<audio style="display: block; width: 100%; height: 48px" :src="state.formData.audioTicketUrl" controls preload="none" />
			</FaFormItem>
			<FaFormItem prop="lyricDocumentList" label="歌词文档" span="2">
				<LyricTable
					v-model="state.formData.lyricDocumentList"
					:audio-duration="state.formData.audioDuration"
					:disabled="state.formDisabled"
				/>
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, useTemplateRef } from "vue";
import { ElMessage } from "element-plus";
import { withDefineType } from "@fast-china/utils";
import { AudioTypeEnum } from "@/api/enums/AudioTypeEnum";
import { fileApi } from "@/api/services/File";
import { audioAssetApi } from "@/api/services/Walkman/audioAsset";
import { bookApi } from "@/api/services/Walkman/book";
import { lessonApi } from "@/api/services/Walkman/lesson";
import LyricTable from "./components/lyricTable.vue";
import type { FormRules, UploadRawFile } from "element-plus";
import type { FaDialogInstance, FaFormInstance } from "fast-element-plus";
import type { AddAudioAssetInput } from "@/api/services/Walkman/audioAsset/models/AddAudioAssetInput";
import type { EditAudioAssetInput } from "@/api/services/Walkman/audioAsset/models/EditAudioAssetInput";
import type { EditLyricDocumentInput } from "@/api/services/Walkman/audioAsset/models/EditLyricDocumentInput";
import type { QueryAudioAssetDetailOutput } from "@/api/services/Walkman/audioAsset/models/QueryAudioAssetDetailOutput";

defineOptions({ name: "WalkmanAudioAssetEdit" });

const emit = defineEmits(["ok"]);
const faDialogRef = useTemplateRef<FaDialogInstance>("faDialogRef");
const faFormRef = useTemplateRef<FaFormInstance>("faFormRef");
let pendingAudioDuration: string | undefined;

const timePattern = /^\d{2,}:[0-5]\d:[0-5]\d(?:\.\d{1,7})?$/u;
const parseDuration = (value: string) => {
	const [hours = "0", minutes = "0", secondPart = "0"] = value.split(":");
	return (Number(hours) * 3600 + Number(minutes) * 60 + Number(secondPart)) * 1000;
};
const validateLyricDocumentList = (_rule: unknown, value: EditLyricDocumentInput[] | undefined, callback: (error?: Error) => void) => {
	if (!value?.length) return callback(new Error("请至少添加一条歌词"));
	for (const [index, item] of value.entries()) {
		if (!item.english?.trim() || !item.chinese?.trim()) return callback(new Error(`第 ${index + 1} 条歌词的中英文不能为空`));
		if (!item.startTime || !item.endTime || !timePattern.test(item.startTime) || !timePattern.test(item.endTime)) {
			return callback(new Error(`第 ${index + 1} 条歌词时间格式应为 HH:mm:ss.fff`));
		}
		if (parseDuration(item.endTime) <= parseDuration(item.startTime)) {
			return callback(new Error(`第 ${index + 1} 条歌词的结束时间必须晚于开始时间`));
		}
		if (index > 0 && parseDuration(item.startTime) < parseDuration(value[index - 1].endTime ?? "")) {
			return callback(new Error(`第 ${index + 1} 条歌词与上一条歌词时间重叠`));
		}
	}
	callback();
};

const state = reactive({
	formData: withDefineType<AddAudioAssetInput & EditAudioAssetInput & QueryAudioAssetDetailOutput>({}),
	formRules: withDefineType<FormRules<AddAudioAssetInput & EditAudioAssetInput>>({
		bookId: [{ required: true, message: "请选择教材", trigger: "change" }],
		lessonId: [{ required: true, message: "请选择课程", trigger: "change" }],
		audioType: [{ required: true, message: "请选择音频类型", trigger: "change" }],
		audioUrl: [{ required: true, message: "请上传音频文件", trigger: "change" }],
		audioDuration: [{ required: true, message: "未识别到音频时长", trigger: "change" }],
		lyricDocumentList: [{ validator: validateLyricDocumentList, trigger: "change" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "音频资源",
});

const formatDuration = (duration: number) => {
	const totalMilliseconds = Math.round(duration * 1000);
	const milliseconds = totalMilliseconds % 1000;
	const totalSeconds = Math.floor(totalMilliseconds / 1000);
	const seconds = totalSeconds % 60;
	const totalMinutes = Math.floor(totalSeconds / 60);
	const minutes = totalMinutes % 60;
	const hours = Math.floor(totalMinutes / 60);
	return `${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}.${String(milliseconds).padStart(3, "0")}`;
};

const handleBeforeAudioUpload = async (file: UploadRawFile) => {
	if (file.type && !file.type.startsWith("audio/")) {
		ElMessage.error("请选择音频文件！");
		return false;
	}
	const objectUrl = URL.createObjectURL(file);
	try {
		const duration = await new Promise<number>((resolve, reject) => {
			const audio = new Audio();
			audio.preload = "metadata";
			audio.onloadedmetadata = () => resolve(audio.duration);
			audio.onerror = () => reject(new Error("无法读取音频信息"));
			audio.src = objectUrl;
		});
		if (!Number.isFinite(duration) || duration <= 0) throw new Error("音频时长无效");
		pendingAudioDuration = formatDuration(duration);
		return true;
	} catch {
		pendingAudioDuration = undefined;
		ElMessage.error("无法识别音频时长，请检查文件是否有效！");
		return false;
	} finally {
		URL.revokeObjectURL(objectUrl);
	}
};

const handleAudioUploadSuccess = () => {
	if (pendingAudioDuration) state.formData.audioDuration = pendingAudioDuration;
	if (state.formData.lyricDocumentList?.length) {
		state.formData.lyricDocumentList = [];
		ElMessage.warning("音频文件已变更，请重新选择对应的 LRC 文件！");
	}
	pendingAudioDuration = undefined;
};

const handleAudioUploadError = () => {
	pendingAudioDuration = undefined;
};

const handleAudioRemove = () => {
	pendingAudioDuration = undefined;
	state.formData.audioDuration = undefined;
	state.formData.audioTicketUrl = undefined;
	state.formData.lyricDocumentList = [];
};

const handleConfirm = () => {
	void faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		if (state.dialogState === "add") {
			await audioAssetApi.addAudioAsset(state.formData);
			ElMessage.success("新增成功！");
		} else if (state.dialogState === "edit") {
			await audioAssetApi.editAudioAsset(state.formData);
			ElMessage.success("保存成功！");
		}
		emit("ok");
	});
};

const detail = (audioAssetId: string) => {
	void faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await audioAssetApi.queryAudioAssetDetail(audioAssetId);
		state.formData = apiRes;
		state.dialogTitle = `音频资源详情 - ${apiRes.lessonTitle}`;
	});
};

const add = () => {
	void faDialogRef.value.open(() => {
		state.dialogState = "add";
		state.formDisabled = false;
		state.dialogTitle = "添加音频资源";
		state.formData = { audioType: AudioTypeEnum.American, lyricDocumentList: [] };
	});
};

const edit = (audioAssetId: string) => {
	void faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await audioAssetApi.queryAudioAssetDetail(audioAssetId);
		state.formData = apiRes;
		state.dialogTitle = `编辑音频资源 - ${apiRes.lessonTitle}`;
	});
};

defineExpose({ element: faDialogRef, detail, add, edit });
</script>
