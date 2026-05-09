import { defineConfig } from "orval";

const inputTarget = "../openapi/Autodor.API.json";

export default defineConfig({
	api: {
		input: {
			target: inputTarget,
		},
		output: {
			target: "./src/api/generated.ts",
			schemas: "./src/api/models",
			client: "react-query",
			mode: "tags-split",
			override: {
				mutator: {
					path: "./src/api/mutator.ts",
					name: "customFetch",
				},
				fetch: {
					includeHttpResponseReturnType: false,
				},
			},
		},
	},
	zod: {
		input: {
			target: inputTarget,
		},
		output: {
			target: "./src/api/generated.ts",
			client: "zod",
			mode: "tags-split",
			fileExtension: ".zod.ts",
		},
	},
});
