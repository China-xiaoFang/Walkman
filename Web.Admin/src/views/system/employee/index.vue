<template>
	<div>
		<FastTable ref="fastTableRef" tableKey="1D1KVW17SY" rowKey="employeeId" :requestApi="employeeApi.queryEmployeePaged" hideSearchTime>
			<!-- 表格按钮操作区域 -->
			<template #header>
				<el-button v-auth="'Employee:Add'" type="primary" :icon="Plus" @click="editFormRef.add()">新增</el-button>
			</template>

			<template #employeeName="{ row }: { row?: QueryEmployeePagedOutput }">
				<el-button link type="primary" @click="editFormRef.detail(row.employeeId)">{{ row.employeeName }}</el-button>
				<br />
				工号：<span v-iconCopy="row.employeeNo">{{ row.employeeNo }}</span>
				<br />
				手机：<span v-iconCopy="row.mobile">{{ row.mobile }}</span>
				<br />
				邮箱：<span v-iconCopy="row.email">{{ row.email }}</span>
			</template>

			<template #account="{ row }: { row?: QueryEmployeePagedOutput }">
				<template v-if="row.accountMobile">
					<span>昵称：{{ row.accountNickName }}</span>
					<br />
					手机：<span v-iconCopy="row.accountMobile">{{ row.accountMobile }}</span>
					<br />
					<span>
						登陆时间：{{ row.lastLoginTime ?? "" }}
						<el-tag v-if="row.lastLoginTime" type="info" round effect="light" class="ml5">
							{{ dateUtil.dateTimeFix(row.lastLoginTime) }}
						</el-tag>
					</span>
				</template>
				<template v-else>
					<el-text type="info">未绑定登录账号</el-text>
				</template>
			</template>

			<template #departmentName="{ row }: { row?: QueryEmployeePagedOutput }">
				机构：<span>{{ row.orgName }}</span>
				<br />
				<span>{{ row.departmentNames?.join(" > ") }}</span>
				<template v-if="row.isPrincipal">
					<br />
					<el-tag type="primary">负责人</el-tag>
				</template>
			</template>

			<template #positionName="{ row }: { row?: QueryEmployeePagedOutput }">
				职位：<span>{{ row.positionName }}</span>
				<br />
				职级：<span>{{ row.jobLevelName }}</span>
				<br />
				角色：<span>{{ row.roleNames }}</span>
			</template>

			<!-- 表格操作 -->
			<template #operation="{ row }: { row: QueryEmployeePagedOutput }">
				<div class="mb5">
					<el-button v-auth="'Employee:Detail'" size="small" plain @click="editFormRef.detail(row.employeeId)">详情</el-button>
					<el-button
						v-if="row.userType === UserTypeEnum.None"
						v-auth="'Employee:Edit'"
						size="small"
						plain
						type="primary"
						@click="editFormRef.edit(row.employeeId)"
						>编辑</el-button
					>
					<el-dropdown v-if="row.userType === UserTypeEnum.None" v-auth="'Employee:Status'" class="pl12" trigger="click">
						<el-button size="small" plain type="primary">
							更多
							<el-icon class="el-icon--right"><ArrowDown /></el-icon>
						</el-button>
						<template #dropdown>
							<el-dropdown-menu>
								<el-dropdown-item
									v-if="row.status !== EmployeeStatusEnum.Temporary"
									@click="handleChangeStatus(row, EmployeeStatusEnum.Temporary)"
								>
									设为临时
								</el-dropdown-item>
								<el-dropdown-item
									v-if="row.status !== EmployeeStatusEnum.Probation"
									@click="handleChangeStatus(row, EmployeeStatusEnum.Probation)"
								>
									设为试用
								</el-dropdown-item>
								<el-dropdown-item
									v-if="row.status !== EmployeeStatusEnum.Intern"
									@click="handleChangeStatus(row, EmployeeStatusEnum.Intern)"
								>
									设为实习生
								</el-dropdown-item>
								<el-dropdown-item
									v-if="row.status !== EmployeeStatusEnum.Outsourcing"
									@click="handleChangeStatus(row, EmployeeStatusEnum.Outsourcing)"
								>
									设为外包
								</el-dropdown-item>
								<el-dropdown-item
									v-if="row.status !== EmployeeStatusEnum.Secondment"
									@click="handleChangeStatus(row, EmployeeStatusEnum.Secondment)"
								>
									设为挂职
								</el-dropdown-item>
								<el-dropdown-item
									v-if="row.status !== EmployeeStatusEnum.Formal"
									@click="handleChangeStatus(row, EmployeeStatusEnum.Formal)"
								>
									设为正式
								</el-dropdown-item>
								<el-dropdown-item v-if="row.status !== EmployeeStatusEnum.Resigned" @click="resignedEditFormRef.open(row.employeeId)">
									职员离职
								</el-dropdown-item>
							</el-dropdown-menu>
						</template>
					</el-dropdown>
				</div>
			</template>
		</FastTable>
		<EmployeeEdit ref="editFormRef" @ok="fastTableRef.refresh()" />
		<ResignedEdit ref="resignedEditFormRef" @ok="fastTableRef.refresh()" />
	</div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { ArrowDown, Plus } from "@element-plus/icons-vue";
import { dateUtil } from "@fast-china/utils";
import { EmployeeStatusEnum } from "@/api/enums/EmployeeStatusEnum";
import { UserTypeEnum } from "@/api/enums/UserTypeEnum";
import { employeeApi } from "@/api/services/Admin/employee";
import { QueryEmployeePagedOutput } from "@/api/services/Admin/employee/models/QueryEmployeePagedOutput";
import EmployeeEdit from "./edit/index.vue";
import ResignedEdit from "./edit/resignedEdit.vue";
import type { FastTableInstance } from "@/components";

defineOptions({
	name: "SystemEmployee",
});

const fastTableRef = ref<FastTableInstance>();
const editFormRef = ref<InstanceType<typeof EmployeeEdit>>();
const resignedEditFormRef = ref<InstanceType<typeof ResignedEdit>>();

/** 处理状态变更 */
const handleChangeStatus = (row: QueryEmployeePagedOutput, status: EmployeeStatusEnum) => {
	const { employeeId, rowVersion } = row;
	ElMessageBox.confirm(`确定修改职员状态？`, {
		type: "warning",
		async beforeClose() {
			await employeeApi.changeStatus({
				employeeId,
				status,
				rowVersion,
			});
			ElMessage.success("操作成功！");
			fastTableRef.value?.refresh();
		},
	});
};
</script>
