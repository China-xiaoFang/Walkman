<template>
	<view class="page">
		<view class="volume-list">
			<view
				v-for="item in state.volumeList"
				:key="item.volumeId"
				class="volume-card"
				@click="handleVolumeClick(item)"
			>
				<FaImage
					v-if="item.coverUrl"
					width="160rpx"
					height="200rpx"
					:src="item.coverUrl"
					:hideImage="false"
				/>
				<view v-else class="volume-cover-placeholder">
					<text>{{ item.volumeName?.substring(0, 2) }}</text>
				</view>
				<view class="volume-info">
					<text class="volume-name">{{ item.volumeName }}</text>
				</view>
			</view>
		</view>

		<view v-if="state.volumeList.length === 0 && !state.loading" class="empty-tip">
			<text>暂无册数据</text>
		</view>
	</view>
</template>

<script setup lang="ts">
import { onLoad, onPullDownRefresh } from "@dcloudio/uni-app";
import { reactive } from "vue";
import { useRouter } from "uni-mini-router";
import { playApi } from "@/api/services/Play";
import type { VolumeListOutput } from "@/api/services/Play/models/VolumeListOutput";

definePage({
	name: "WalkmanVolume",
	layout: "layout",
	style: {
		navigationBarTitleText: "选择册",
		enablePullDownRefresh: true,
	},
});

const router = useRouter();

const state = reactive({
	textbookId: 0,
	textbookName: "",
	volumeList: [] as VolumeListOutput[],
	loading: false,
});

const handleVolumeClick = (item: VolumeListOutput) => {
	router.push({
		path: "/pages/walkman/lesson/index",
		query: {
			volumeId: item.volumeId,
			volumeName: item.volumeName,
		},
	});
};

const loadVolumes = async () => {
	if (!state.textbookId) return;
	try {
		state.loading = true;
		state.volumeList = await playApi.getVolumeList(state.textbookId);
	} catch {
		state.volumeList = [];
	} finally {
		state.loading = false;
	}
};

onLoad(async (options: any) => {
	state.textbookId = Number(options?.textbookId || 0);
	state.textbookName = options?.textbookName || "";
	if (state.textbookName) {
		uni.setNavigationBarTitle({ title: state.textbookName });
	}
	await loadVolumes();
});

onPullDownRefresh(async () => {
	await loadVolumes();
	uni.stopPullDownRefresh();
});
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
