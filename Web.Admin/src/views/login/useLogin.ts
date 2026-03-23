import { inject } from "vue";
import { type FormInstance } from "element-plus";
import { type FaButtonInstance, formUtil } from "fast-element-plus";
import { Local, cryptoUtil } from "@fast-china/utils";
import { LoginStatusEnum } from "@/api/enums/LoginStatusEnum";
import { loginApi } from "@/api/services/Auth/login";
import { useUserInfo } from "@/stores";
import type { IFormData } from "./index.vue";
import type { Ref } from "vue";

/** 登录服务 */
// eslint-disable-next-line @typescript-eslint/explicit-function-return-type, @typescript-eslint/explicit-module-boundary-types
export const useLogin = (elFormRef: Ref<FormInstance>, faButtonRef: Ref<FaButtonInstance>) => {
	const userInfoStore = useUserInfo();

	/** 表单数据 */
	const formData = inject<Ref<IFormData>>("formData");
	/** 缓存Key */
	const cFormKey = inject<string>("cFormKey");

	/** 账号改变 */
	const handleAccountChange = (): void => {
		formData.value.password = undefined;
		formData.value.encryptPassword = false;
	};

	/** 密码输入 */
	const handlePasswordInput = (value: string): void => {
		if (formData.value.encryptPassword) {
			const newValue = value.substring(value.length - 1);
			formData.value.password = newValue;
			formData.value.encryptPassword = false;
		}
	};

	/** 登录 */
	const handleLogin = async (_, done?: () => void): Promise<void> => {
		try {
			const { account, password, rememberMe, encryptPassword } = formData.value;
			if (!encryptPassword) {
				formData.value.password = cryptoUtil.sha1.encrypt(password);
				formData.value.encryptPassword = true;
			}
			const apiRes = await loginApi.login({
				account,
				password: formData.value.password,
			});
			// 登录成功
			if (apiRes.status === LoginStatusEnum.Success) {
				Local.set(
					cFormKey,
					rememberMe
						? formData.value
						: {
								...formData.value,
								rememberMe: false,
								password: undefined,
							}
				);
				userInfoStore.login();
			}
		} finally {
			done && done();
		}
	};

	/** 表单登录 */
	const handleFormLogin = (_, done?: () => void): void => {
		formUtil
			.validate(elFormRef)
			.then(() => handleLogin(_, done))
			.finally(() => done && done());
	};

	/** 回车键摁下 */
	const handleKeyupEnter = (): void => {
		faButtonRef.value.doLoading(() => handleFormLogin(null));
	};

	return {
		formData,
		handleAccountChange,
		handlePasswordInput,
		handleFormLogin,
		handleKeyupEnter,
	};
};
