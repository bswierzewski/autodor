import { z } from "zod";

const envSchema = z.object({
	// @env: VITE_CLERK_PUBLISHABLE_KEY=
	VITE_CLERK_PUBLISHABLE_KEY: z.string().default(""),
	// @env: VITE_GIT_SHA=local
	VITE_GIT_SHA: z.string().default("local"),
});

export const ENV = envSchema.parse(import.meta.env);

export type Env = typeof ENV;
