<template>
	<view class="page">
		<wd-text
			customClass="pl20 pt20 mb20"
			customStyle="display: block;"
			text="本投诉为平台自有渠道，非官方监管投诉入口"
			size="var(--wot-font-size-small)"
		/>

		<wd-form ref="formRef" :model="state.formData" :rules="state.formRule">
			<wd-cell-group border>
				<wd-picker label="投诉类型" prop="complaintType" :columns="complaintTypeEnum" v-model="state.formData.complaintType" />

				<wd-input
					label="联系电话"
					prop="contactPhone"
					v-model="state.formData.contactPhone"
					placeholder="请输入联系电话"
					type="number"
					inputmode="numeric"
					:maxlength="11"
					clearable
				/>

				<wd-input
					label="联系邮箱"
					prop="contactEmail"
					v-model="state.formData.contactEmail"
					placeholder="请输入联系邮箱"
					type="text"
					inputmode="email"
					clearable
				/>

				<wd-textarea
					label="投诉描述"
					prop="description"
					v-model="state.formData.description"
					placeholder="请简单描述内容"
					:maxlength="200"
					clearable
				/>
			</wd-cell-group>

			<wd-cell title="附件图片" prop="patientMobile" titleWidth="33%" border valueAlign="left">
				<wd-upload
					prop="attachmentImages"
					v-model:fileList="state.fileList"
					multiple
					:limit="4"
					imageMode="aspectFill"
					:uploadMethod="handleUpload"
				/>
			</wd-cell>

			<wd-button customClass="btn__submit" type="primary" block :round="false" @tap.stop="handleSubmit">提交</wd-button>
		</wd-form>
	</view>
</template>

<script setup lang="ts">
import { reactive, ref } from "vue";
import { clickUtil, withDefineType } from "@fast-china/utils";
import { useRouter } from "uni-mini-router";
import { complaintApi } from "@/api/services/Admin/complaint";
import { fileApi } from "@/api/services/File";
import { useLoading, useToast } from "@/hooks";
import { useApp } from "@/stores";
import type { AddComplaintInput } from "@/api/services/Admin/complaint/models/AddComplaintInput";
import type { FormInstance, FormRules } from "wot-design-uni/components/wd-form/types";
import type { UploadFile, UploadMethod } from "wot-design-uni/components/wd-upload/types";

definePage({
	name: "ComplaintSubmit",
	layout: "layout",
	style: {
		navigationBarTitleText: "投诉",
	},
});

const appStore = useApp();
const router = useRouter();

const formRef = ref<FormInstance>();

const complaintTypeEnum = appStore.getDictionary("ComplaintTypeEnum");

const state = reactive({
	formRule: withDefineType<FormRules>({
		complaintType: [{ required: true, message: "请选择投诉类型" }],
		contactPhone: [{ required: true, message: "请输入联系电话" }],
		description: [{ required: true, message: "请输入投诉描述" }],
	}),
	formData: withDefineType<AddComplaintInput>({}),
	fileList: withDefineType<UploadFile[]>([]),
});

/** 处理上传 */
const handleUpload: UploadMethod = async (file, formData, options) => {
	fileApi
		.uploadFile(file.url)
		.then((res) => {
			options.onSuccess({ data: res, statusCode: 200 }, file, formData);
		})
		.catch((err) => {
			if (err?.message) {
				options.onError({ errMsg: err.message }, file, formData);
			} else {
				options.onError(err, file, formData);
			}
		});
};

/** 处理提交 */
const handleSubmit = async () => {
	await clickUtil.throttleAsync(async () => {
		const { valid } = await formRef.value.validate();
		if (valid) {
			const { complaintType, contactPhone, contactEmail, description } = state.formData;
			if (state.fileList?.length == 0) {
				useToast.warning("请上传附件图片");
				return;
			}
			useLoading.show("提交中...");
			await complaintApi
				.addComplaint({
					complaintType,
					contactPhone,
					contactEmail,
					description,
					attachmentImages: state.fileList.map((m) => m.response as string),
				})
				.finally(useLoading.hide);
			useToast.success("提交成功");
			setTimeout(router.back, 2000);
		}
	});
};
</script>

<style scoped lang="scss">
:deep(.wd-textarea__inner) {
	height: 150rpx;
}
.btn__submit {
	--wot-button-medium-fs: var(--wot-font-size-base);
	--wot-button-medium-height: 80rpx;
	margin: 30rpx 50rpx !important;
}
</style>
