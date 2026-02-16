import { defineConfig } from "orval";

export default defineConfig({
	api: {
		input: {
			// Update this path to your OpenAPI/Swagger spec
			target: "../backend/swagger.json",
		},
		output: {
			mode: "tags-split",
			target: "src/api/generated.ts",
			schemas: "src/api/models",
			client: "react-query",
			httpClient: "axios",
			mock: false,
			override: {
				mutator: {
					path: "src/api/axios-instance.ts",
					name: "customInstance",
				},
			},
		},
	},
});
