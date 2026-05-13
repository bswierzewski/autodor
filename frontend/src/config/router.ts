import { createRouter } from "@tanstack/react-router";
import type { AuthContext } from "../hooks/useAuth";
import { routeTree } from "../routeTree.gen";

export type RouterContext = {
	auth: AuthContext;
};

export const router = createRouter({
	routeTree,
	defaultPreload: "intent",
	scrollRestoration: true,
	context: {
		auth: {} as AuthContext,
	},
});

declare module "@tanstack/react-router" {
	interface Register {
		router: typeof router;
	}
}