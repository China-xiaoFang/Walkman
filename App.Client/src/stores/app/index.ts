import { reactive, ref, toRefs } from "vue";
import { Local, consoleError, useIdentity } from "@fast-china/utils";
import { defineStore } from "pinia";
import { AppEnvironmentEnum } from "@/api/enums/AppEnvironmentEnum";
import { dictionaryApi } from "@/api/services/Admin/dictionary";

export type ILoginComponent = "ClassicLogin";

type IState = {
	/** 环境 */
	env: ViteEnv;
	/** 设备类型 */
	deviceType: AppEnvironmentEnum;
	/** 当前应用的AppId，获取到的 */
	appId: string;
	/**
	 * - manifest.json 中应用版本名称
	 * - 如果存在热更新则为 应用资源（wgt）的版本名称
	 */
	appVersion: string;
	/** 是否为 IPhone 设备 */
	isIphone: boolean;
	/** 是否为客户端（Pc） */
	isClient: boolean;
	/** 设备基础信息 */
	appBaseInfo: UniNamespace.GetAppBaseInfoResult;
	/** 设备信息 */
	deviceInfo: UniNamespace.GetDeviceInfoResult;
	/** 窗口信息 */
	windowInfo: UniNamespace.GetWindowInfoResult;
	/**
	 * 小程序胶囊按钮信息
	 * @description 非小程序平台下，只有默认高度
	 */
	menuButton: UniNamespace.GetMenuButtonBoundingClientRectRes;
	/** 网络 */
	network: {
		/** 是否在线 */
		onLine: boolean;
		/** 网络类型 */
		networkType: INetworkType;
	};
};

export const useApp = defineStore(
	"app",
	() => {
		const state = reactive<IState>({
			env: "production",
			deviceType: AppEnvironmentEnum.MobileThree,
			appId: "",
			appVersion: "",
			isIphone: false,
			isClient: false,
			appBaseInfo: null,
			deviceInfo: null,
			windowInfo: null,
			menuButton: null,
			network: {
				onLine: false,
				networkType: "none",
			},
		});

		/** 字典 */
		const dictionary = ref<Map<string, FaTableEnumColumnCtx[]>>(new Map());

		/** 设置字典 */
		const setDictionary = async (): Promise<void> => {
			try {
				dictionary.value.clear();
				// 处理数据字典
				const _dictionary = await dictionaryApi.queryDictionary();
				Object.entries(_dictionary).forEach(([key, value]) => {
					dictionary.value.set(key, value);
				});
			} catch {
				consoleError("App", "字典加载失败");
			}
		};

		/** 获取字典 */
		const getDictionary = (key: string, isAll = false): FaTableEnumColumnCtx[] => {
			if (!dictionary.value.has(key)) {
				consoleError("app", `字典 [${key}] 不存在`);
				return;
			}
			let result = dictionary.value.get(key);
			if (isAll) {
				result = [
					{
						label: "全部",
						value: null,
					},
					...result.slice(),
				];
			}
			return result;
		};

		/** 清除 App 缓存 */
		const clearAppCache = (): void => {
			// 获取设备Id，这里按理来说不应该不存在的
			const uIdentity = useIdentity();
			// 清空 Local 缓存
			Local.clear();
			// 重新设置设备Id
			uIdentity.makeIdentity(uIdentity.deviceId);
		};

		return {
			...toRefs(state),
			setDictionary,
			getDictionary,
			clearAppCache,
		};
	},
	{
		persist: {
			key: "store-app",
		},
	}
);
