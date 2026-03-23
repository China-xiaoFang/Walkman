<template>
	<FaDialog
		ref="faDialogRef"
		width="1200"
		:title="state.dialogTitle"
		:showConfirmButton="!state.formDisabled"
		:showBeforeClose="!state.formDisabled"
		confirmButtonText="保存"
		@confirm-click="handleConfirm"
		@close="faFormRef.resetFields()"
	>
		<FaForm ref="faFormRef" :model="state.formData" :rules="state.formRules" :disabled="state.formDisabled" cols="3">
			<FaLayoutGridItem span="3">
				<el-divider contentPosition="left">组织架构</el-divider>
			</FaLayoutGridItem>
			<FaFormItem prop="orgId" label="机构">
				<FaTreeSelect
					:requestApi="organizationApi.organizationSelector"
					v-model="state.formData.orgId"
					v-model:label="state.formData.orgName"
					placeholder="请选择机构"
					checkStrictly
					filterable
					clearable
				/>
			</FaFormItem>
			<FaFormItem prop="departmentId" label="部门">
				<FaTreeSelect
					:requestApi="departmentApi.departmentSelector"
					:disabled="state.formDisabled || !state.formData?.orgId"
					:initParam="state.formData.orgId"
					v-model="state.formData.departmentId"
					v-model:label="state.formData.departmentName"
					placeholder="请选择部门"
					checkStrictly
					filterable
					clearable
				/>
			</FaFormItem>
			<FaFormItem prop="sex" label="部门负责人">
				<RadioGroup name="BooleanEnum" v-model="state.formData.isPrincipal" />
			</FaFormItem>
			<FaFormItem prop="positionId" label="职位">
				<FaSelect
					:requestApi="positionApi.positionSelector"
					v-model="state.formData.positionId"
					v-model:label="state.formData.positionName"
					placeholder="请选择职位"
					clearable
				/>
			</FaFormItem>
			<FaFormItem prop="jobLevelId" label="职级">
				<FaSelect
					:requestApi="jobLevelApi.jobLevelSelector"
					v-model="state.formData.jobLevelId"
					v-model:label="state.formData.jobLevelName"
					placeholder="请选择职级"
					clearable
				/>
			</FaFormItem>

			<FaLayoutGridItem span="3">
				<el-divider contentPosition="left">基础档案</el-divider>
			</FaLayoutGridItem>
			<FaFormItem prop="employeeName" label="职员名称">
				<el-input v-model="state.formData.employeeName" maxlength="20" placeholder="请输入职员名称" />
			</FaFormItem>
			<FaFormItem prop="mobile" label="手机">
				<el-input v-model="state.formData.mobile" maxlength="11" placeholder="请输入手机" />
			</FaFormItem>
			<FaFormItem prop="email" label="邮箱">
				<el-input v-model="state.formData.email" maxlength="50" placeholder="请输入邮箱" />
			</FaFormItem>
			<FaFormItem prop="sex" label="性别">
				<RadioGroup name="GenderEnum" v-model="state.formData.sex" />
			</FaFormItem>
			<FaFormItem prop="entryDate" label="入职日期">
				<el-date-picker
					type="date"
					v-model="state.formData.entryDate"
					:disabledDate="dateUtil.getDisabledDate"
					valueFormat="YYYY-MM-DD"
					placeholder="请选择入职日期"
				/>
			</FaFormItem>
			<FaFormItem prop="birthday" label="生日">
				<el-date-picker
					type="date"
					v-model="state.formData.birthday"
					:disabledDate="dateUtil.getDisabledDate"
					valueFormat="YYYY-MM-DD"
					placeholder="请选择生日"
				/>
			</FaFormItem>
			<FaFormItem prop="idType" label="证件类型">
				<FaSelect :data="appStore.getDictionary('IdTypeEnum')" v-model="state.formData.idType" placeholder="请选择证件类型" clearable />
			</FaFormItem>
			<FaFormItem prop="idNumber" label="证件号码">
				<el-input v-model="state.formData.idNumber" maxlength="50" placeholder="请输入证件号码" />
			</FaFormItem>
			<FaFormItem prop="remark" label="备注">
				<el-input type="textarea" v-model="state.formData.remark" :rows="2" maxlength="200" placeholder="请输入备注" />
			</FaFormItem>
			<FaFormItem prop="idPhoto" label="证件照">
				<FaUploadImage v-model="state.formData.idPhoto" :uploadApi="fileApi.uploadIdPhoto" />
			</FaFormItem>

			<FaLayoutGridItem span="3">
				<el-divider contentPosition="left">角色信息</el-divider>
			</FaLayoutGridItem>
			<FaFormItem prop="roleList" label="角色" span="3">
				<el-checkbox-group v-model="state.formData.roleIds" @change="handleRoleChange">
					<el-checkbox v-for="(item, index) of state.roleList" :key="index" :value="item.value">
						{{ item.label }}
					</el-checkbox>
				</el-checkbox-group>
			</FaFormItem>

			<FaLayoutGridItem span="3">
				<el-divider contentPosition="left">学籍档案</el-divider>
			</FaLayoutGridItem>
			<FaFormItem prop="graduationCollege" label="毕业学院">
				<el-input v-model="state.formData.graduationCollege" maxlength="50" placeholder="请输入毕业学院" />
			</FaFormItem>
			<FaFormItem prop="academicQualifications" label="学历">
				<FaSelect
					:data="appStore.getDictionary('AcademicQualificationsEnum')"
					v-model="state.formData.academicQualifications"
					placeholder="请选择学历"
					clearable
				/>
			</FaFormItem>
			<FaFormItem prop="academicSystem" label="学制">
				<FaSelect
					:data="appStore.getDictionary('AcademicSystemEnum')"
					v-model="state.formData.academicSystem"
					placeholder="请选择学制"
					clearable
				/>
			</FaFormItem>
			<FaFormItem prop="degree" label="学位">
				<FaSelect :data="appStore.getDictionary('DegreeEnum')" v-model="state.formData.degree" placeholder="请选择学位" clearable />
			</FaFormItem>
			<FaFormItem prop="educationLevel" label="文化程度">
				<FaSelect :data="appStore.getDictionary('EducationLevelEnum')" v-model="state.formData.educationLevel" clearable />
			</FaFormItem>
			<FaFormItem prop="politicalStatus" label="政治面貌">
				<FaSelect :data="appStore.getDictionary('PoliticalStatusEnum')" v-model="state.formData.politicalStatus" clearable />
			</FaFormItem>

			<FaLayoutGridItem span="3">
				<el-divider contentPosition="left">人事档案</el-divider>
			</FaLayoutGridItem>

			<FaFormItem prop="nation" label="民族">
				<FaSelect :data="appStore.getDictionary('NationEnum')" v-model="state.formData.nation" placeholder="请选择民族" clearable />
			</FaFormItem>
			<FaFormItem prop="nativePlace" label="籍贯">
				<el-input v-model="state.formData.nativePlace" maxlength="50" placeholder="请输入籍贯" />
			</FaFormItem>
			<FaFormItem prop="familyPhone" label="家庭电话">
				<el-input v-model="state.formData.familyPhone" maxlength="20" placeholder="请输入家庭电话" />
			</FaFormItem>
			<FaFormItem prop="officePhone" label="办公电话">
				<el-input v-model="state.formData.officePhone" maxlength="20" placeholder="请输入办公电话" />
			</FaFormItem>
			<FaFormItem prop="familyAddress" label="家庭地址">
				<el-input type="textarea" v-model="state.formData.familyAddress" :rows="2" maxlength="200" placeholder="请输入家庭地址" />
			</FaFormItem>
			<FaFormItem prop="mailingAddress" label="通信地址">
				<el-input type="textarea" v-model="state.formData.mailingAddress" :rows="2" maxlength="200" placeholder="请输入通信地址" />
			</FaFormItem>
			<FaFormItem prop="emergencyContact" label="紧急联系人">
				<el-input v-model="state.formData.emergencyContact" maxlength="20" placeholder="请输入紧急联系人" />
			</FaFormItem>
			<FaFormItem prop="emergencyPhone" label="紧急联系电话">
				<el-input v-model="state.formData.emergencyPhone" maxlength="20" placeholder="请输入紧急联系电话" />
			</FaFormItem>
			<FaFormItem prop="emergencyAddress" label="紧急联系地址">
				<el-input type="textarea" v-model="state.formData.emergencyAddress" :rows="2" maxlength="200" placeholder="请输入紧急联系地址" />
			</FaFormItem>
		</FaForm>
	</FaDialog>
</template>

<script lang="ts" setup>
import { reactive, ref } from "vue";
import { CheckboxValueType, ElMessage, type FormRules, dayjs } from "element-plus";
import { dateUtil, withDefineType } from "@fast-china/utils";
import { GenderEnum } from "@/api/enums/GenderEnum";
import { departmentApi } from "@/api/services/Admin/department";
import { employeeApi } from "@/api/services/Admin/employee";
import { AddEmployeeInput } from "@/api/services/Admin/employee/models/AddEmployeeInput";
import { EditEmployeeInput } from "@/api/services/Admin/employee/models/EditEmployeeInput";
import { EmployeeOrgModel } from "@/api/services/Admin/employee/models/EmployeeOrgModel";
import { jobLevelApi } from "@/api/services/Admin/jobLevel";
import { organizationApi } from "@/api/services/Admin/organization";
import { positionApi } from "@/api/services/Admin/position";
import { roleApi } from "@/api/services/Admin/role";
import { fileApi } from "@/api/services/File";
import { useApp } from "@/stores";
import type { ElSelectorOutput, FaDialogInstance, FaFormInstance } from "fast-element-plus";

defineOptions({
	name: "SystemEmployeeEdit",
});

const emit = defineEmits(["ok"]);

const appStore = useApp();

const faDialogRef = ref<FaDialogInstance>();
const faFormRef = ref<FaFormInstance>();

const state = reactive({
	formData: withDefineType<EditEmployeeInput & AddEmployeeInput & { roleIds?: number[] }>({
		roleIds: [],
	}),
	formRules: withDefineType<FormRules>({
		orgId: [{ required: true, message: "请选择机构", trigger: "change" }],
		departmentId: [{ required: true, message: "请选择部门", trigger: "change" }],
		positionId: [{ required: true, message: "请选择职位", trigger: "change" }],
		jobLevelId: [{ required: true, message: "请选择职级", trigger: "change" }],
		employeeName: [{ required: true, message: "请输入职员名称", trigger: "blur" }],
		mobile: [{ required: true, message: "请输入手机", trigger: "blur" }],
		email: [{ required: true, message: "请输入邮箱", trigger: "blur" }],
		idPhoto: [{ required: true, message: "请上传证件照", trigger: "change" }],
		firstWorkDate: [{ required: true, message: "请选择初次工作日期", trigger: "change" }],
		entryDate: [{ required: true, message: "请选择入职日期", trigger: "change" }],
	}),
	formDisabled: false,
	dialogState: withDefineType<IPageStateType>("detail"),
	dialogTitle: "职员",
	roleList: withDefineType<ElSelectorOutput<number>[]>([]),
});

/** 从 API 详情响应中提取主部门信息，展平到 formData */
const flattenPrimaryOrg = (apiRes: any) => {
	const primaryOrg = apiRes.orgList?.find((f: any) => f.isPrimary) ?? apiRes.orgList?.[0];
	if (!primaryOrg) return;

	state.formData.orgId = primaryOrg.orgId;
	state.formData.orgName = primaryOrg.orgName;
	state.formData.departmentId = primaryOrg.departmentId;
	state.formData.departmentName = primaryOrg.departmentName;
	state.formData.positionId = primaryOrg.positionId;
	state.formData.positionName = primaryOrg.positionName;
	state.formData.jobLevelId = primaryOrg.jobLevelId;
	state.formData.jobLevelName = primaryOrg.jobLevelName;
	state.formData.isPrincipal = primaryOrg.isPrincipal;
};

/** 将 UI 展平的单部门字段组装回 orgList 数组，用于提交 API */
const buildOrgList = (): EmployeeOrgModel[] => {
	return [
		{
			orgId: state.formData.orgId!,
			departmentId: state.formData.departmentId!,
			isPrimary: true,
			positionId: state.formData.positionId!,
			jobLevelId: state.formData.jobLevelId!,
			isPrincipal: state.formData.isPrincipal ?? false,
		},
	];
};

const handleRoleChange = (val: CheckboxValueType[]) => {
	state.formData.roleList = val
		.map((m) => {
			const roleInfo = state.roleList.find((f) => f.value === m);
			if (!roleInfo) return null;

			return {
				employeeId: state.formData.employeeId,
				roleId: roleInfo.value,
				roleName: roleInfo.label,
			};
		})
		.filter(Boolean);
};

const handleConfirm = () => {
	faDialogRef.value.close(async () => {
		await faFormRef.value.validateScrollToField();
		switch (state.dialogState) {
			case "add":
				await employeeApi.addEmployee(state.formData);
				ElMessage.success("新增成功！");
				break;
			case "edit":
				await employeeApi.editEmployee({
					...state.formData,
					orgList: buildOrgList(),
				});
				ElMessage.success("保存成功！");
				break;
		}
		emit("ok");
	});
};

const detail = (employeeId: number) => {
	faDialogRef.value.open(async () => {
		state.formDisabled = true;
		const apiRes = await employeeApi.queryEmployeeDetail(employeeId);
		state.formData = apiRes;
		state.formData.roleIds = apiRes.roleList.map((m) => m.roleId);
		flattenPrimaryOrg(apiRes);
		state.dialogTitle = `职员详情 - ${apiRes.employeeName}`;
		state.roleList = await roleApi.roleSelector();
	});
};

const add = () => {
	faDialogRef.value.open(async () => {
		state.dialogState = "add";
		state.dialogTitle = "添加职员";
		state.formDisabled = false;
		state.formData = {
			isPrincipal: false,
			sex: GenderEnum.Unknown,
			entryDate: dayjs(new Date()).format("YYYY-MM-DD 00:00:00") as unknown as Date,
			roleList: [],
		};
		state.roleList = await roleApi.roleSelector();
	});
};

const edit = (employeeId: number) => {
	faDialogRef.value.open(async () => {
		state.dialogState = "edit";
		state.formDisabled = false;
		const apiRes = await employeeApi.queryEmployeeDetail(employeeId);
		state.formData = apiRes;
		state.formData.roleIds = apiRes.roleList.map((m) => m.roleId);
		flattenPrimaryOrg(apiRes);
		state.dialogTitle = `编辑职员 - ${apiRes.employeeName}`;
		state.roleList = await roleApi.roleSelector();
	});
};

// 暴露给父组件的参数和方法(外部需要什么，都可以从这里暴露出去)
defineExpose({
	element: faDialogRef,
	detail,
	add,
	edit,
});
</script>

<style scoped lang="scss">
.el-table__cell {
	.el-form-item {
		margin-bottom: 0;
	}
}
</style>
