import { defineUniPages } from "@uni-helper/vite-plugin-uni-pages";

export default defineUniPages({
	pages: [],
	subPackages: [],
	globalStyle: {
		navigationBarTitleText: "Fast随身听",
		navigationBarBackgroundColor: "@navigationBarBackgroundColor",
		navigationBarTextStyle: "@navigationBarTextStyle",
		backgroundColor: "@backgroundColor",
		backgroundTextStyle: "@backgroundTextStyle",
	},
	tabBar: {
		custom: true,
		color: "#0a0a0a",
		selectedColor: "#4488fe",
		backgroundColor: "@backgroundColor",
		fontSize: "14px",
		list: [
			{
				pagePath: "pages/tabBar/home/index",
			},
			{
				pagePath: "pages/tabBar/my/index",
			},
		],
	},
});
