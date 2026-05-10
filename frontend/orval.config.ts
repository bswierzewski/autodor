import { defineConfig } from "orval";

export default defineConfig({
	api: {
		input: {
			target: "../openapi/Autodor.API.json",
		},
		output: {
			target: "./src/api/generated.ts",
			schemas: "./src/api/models",
			client: "react-query",
			mode: "tags-split",
			clean: ["!mutator.ts"],
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
			target: "../openapi/Autodor.API.json",
		},
		output: {
			target: "./src/api/generated.ts",
			client: "zod",
			mode: "tags-split",
			fileExtension: ".zod.ts",
		},
	},
});
