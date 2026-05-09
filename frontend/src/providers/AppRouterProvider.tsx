import { createRouter, RouterProvider } from "@tanstack/react-router";
import { type AuthContext, useAuth } from "../hooks/useAuth";
import { routeTree } from "../routeTree.gen";

export type RouterContext = {
	auth: AuthContext;
};

const router = createRouter({
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

export function AppRouterProvider() {
	const auth = useAuth();

	return <RouterProvider router={router} context={{ auth }} />;
}
