<template>
	<FaTable :data="modelValue" :pagination="false" style="min-height: 300px; max-height: 500px">
		<template #header>
			<div style="display: flex; align-items: center; gap: 12px">
				<el-button v-if="!props.disabled" type="primary" :icon="Upload" @click="fileInputRef?.click()">
					{{ modelValue.length ? "重新选择 LRC" : "选择 LRC 文件" }}
				</el-button>
				<span>共 {{ modelValue.length }} 条歌词，解析后仅供确认，不允许手动修改</span>
			</div>
			<input ref="fileInputRef" style="display: none" type="file" accept=".lrc,text/plain" @change="handleFileChange" />
		</template>
		<FaTableColumn prop="english" label="英文" width="300" />
		<FaTableColumn prop="chinese" label="中文" width="300" />
		<FaTableColumn prop="startTime" label="开始时间" width="130" />
		<FaTableColumn prop="endTime" label="结束时间" width="130" />
	</FaTable>
</template>

<script lang="ts" setup>
import { useVModel } from "@vueuse/core";
import { useTemplateRef } from "vue";
import { Upload } from "@element-plus/icons-vue";
import { ElMessage } from "element-plus";
import { definePropType } from "@fast-china/utils";
import type { EditLyricDocumentInput } from "@/api/services/Walkman/audioAsset/models/EditLyricDocumentInput";

defineOptions({ name: "WalkmanAudioAssetLyricTable" });

interface ParsedLyricLine {
	lineNumber: number;
	startMilliseconds: number;
	english: string;
	chinese: string;
}

const props = defineProps({
	disabled: Boolean,
	audioDuration: String,
	modelValue: definePropType<EditLyricDocumentInput[]>([Array]),
});
const emit = defineEmits(["update:modelValue"]);
const modelValue = useVModel(props, "modelValue", emit, { passive: false, defaultValue: [] });
const fileInputRef = useTemplateRef<HTMLInputElement>("fileInputRef");

const maxFileSize = 2 * 1024 * 1024;
const metadataPattern = /^\[(?:al|ar|ti|by|re|ve|length):.*\]$/iu;

const parseDuration = (value: string) => {
	const [hours = "0", minutes = "0", secondPart = "0"] = value.split(":");
	return (Number(hours) * 3600 + Number(minutes) * 60 + Number(secondPart)) * 1000;
};

const formatDuration = (milliseconds: number) => {
	const totalMilliseconds = Math.round(milliseconds);
	const millisecondPart = totalMilliseconds % 1000;
	const totalSeconds = Math.floor(totalMilliseconds / 1000);
	const seconds = totalSeconds % 60;
	const totalMinutes = Math.floor(totalSeconds / 60);
	const minutes = totalMinutes % 60;
	const hours = Math.floor(totalMinutes / 60);
	return `${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}.${String(millisecondPart).padStart(3, "0")}`;
};

const parseTimestamp = (minutes: string, seconds: string, fraction = "0") => {
	return (Number(minutes) * 60 + Number(seconds)) * 1000 + Number(fraction.padEnd(3, "0"));
};

const parseLrc = (content: string, audioDuration: string) => {
	const durationMilliseconds = parseDuration(audioDuration);
	if (!Number.isFinite(durationMilliseconds) || durationMilliseconds <= 0) throw new Error("请先上传有效的音频文件，再选择 LRC 文件！");

	const lyricLines: ParsedLyricLine[] = [];
	let offsetMilliseconds = 0;
	for (const [index, rawLine] of content
		.replace(/^\uFEFF/u, "")
		.split(/\r?\n/u)
		.entries()) {
		const line = rawLine.trim();
		if (!line) continue;

		const offsetMatch = /^\[offset:([+-]?\d+)\]$/iu.exec(line);
		if (offsetMatch) {
			offsetMilliseconds = Number(offsetMatch[1]);
			continue;
		}
		if (metadataPattern.test(line)) continue;

		const timestampPattern = /\[(\d+):([0-5]\d)(?:[.:](\d{1,3}))?\]/gu;
		const timestampMatches = [...line.matchAll(timestampPattern)];
		if (!timestampMatches.length) throw new Error(`LRC 第 ${index + 1} 行缺少有效时间标签！`);

		const lyric = line.replace(timestampPattern, "").trim();
		const separatorIndex = lyric.indexOf("|");
		if (separatorIndex <= 0 || separatorIndex === lyric.length - 1) {
			throw new Error(`LRC 第 ${index + 1} 行必须使用“英文 | 中文”格式！`);
		}
		const english = lyric.slice(0, separatorIndex).trim();
		const chinese = lyric.slice(separatorIndex + 1).trim();
		if (!english || !chinese) throw new Error(`LRC 第 ${index + 1} 行的中英文不能为空！`);
		if (english.length > 1000 || chinese.length > 1000) throw new Error(`LRC 第 ${index + 1} 行的歌词不能超过 1000 个字符！`);

		for (const timestampMatch of timestampMatches) {
			lyricLines.push({
				lineNumber: index + 1,
				startMilliseconds: parseTimestamp(timestampMatch[1], timestampMatch[2], timestampMatch[3]) + offsetMilliseconds,
				english,
				chinese,
			});
		}
	}

	if (!lyricLines.length) throw new Error("LRC 文件中没有可解析的歌词！");
	for (const [index, lyricLine] of lyricLines.entries()) {
		if (lyricLine.startMilliseconds < 0) throw new Error(`LRC 第 ${lyricLine.lineNumber} 行应用偏移量后的时间不能小于 0！`);
		if (index > 0 && lyricLine.startMilliseconds <= lyricLines[index - 1].startMilliseconds) {
			throw new Error(`LRC 第 ${lyricLine.lineNumber} 行时间必须晚于上一条歌词！`);
		}
		if (lyricLine.startMilliseconds >= durationMilliseconds) {
			throw new Error(`LRC 第 ${lyricLine.lineNumber} 行时间不能大于或等于音频时长！`);
		}
	}

	return lyricLines.map((item, index) => ({
		english: item.english,
		chinese: item.chinese,
		startTime: formatDuration(item.startMilliseconds),
		endTime: formatDuration(lyricLines[index + 1]?.startMilliseconds ?? durationMilliseconds),
	}));
};

const handleFileChange = async (event: Event) => {
	const input = event.target as HTMLInputElement;
	const file = input.files?.[0];
	input.value = "";
	if (!file) return;

	try {
		if (!file.name.toLowerCase().endsWith(".lrc")) throw new Error("请选择 LRC 歌词文件！");
		if (file.size > maxFileSize) throw new Error("LRC 文件大小不能超过 2 MB！");
		const content = new TextDecoder("utf-8", { fatal: true }).decode(await file.arrayBuffer());
		const lyricDocumentList = parseLrc(content, props.audioDuration ?? "");
		modelValue.value = lyricDocumentList;
		ElMessage.success(`已解析 ${lyricDocumentList.length} 条歌词，请确认后保存！`);
	} catch (error) {
		ElMessage.error(error instanceof Error ? error.message : "LRC 文件解析失败！");
	}
};
</script>
