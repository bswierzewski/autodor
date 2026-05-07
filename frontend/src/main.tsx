import { ClerkProvider } from "@clerk/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createRouter, RouterProvider } from "@tanstack/react-router";
import ReactDOM from "react-dom/client";
import { routeTree } from "./routeTree.gen";

const queryClient = new QueryClient();

const clerkPublishableKey = import.meta.env.VITE_CLERK_PUBLISHABLE_KEY ?? "";

const router = createRouter({
	routeTree,
	defaultPreload: "intent",
	scrollRestoration: true,
});

declare module "@tanstack/react-router" {
	interface Register {
		router: typeof router;
	}
}

const rootElement = document.getElementById("app") as HTMLElement;

if (!rootElement.innerHTML) {
	const root = ReactDOM.createRoot(rootElement);

	root.render(
		<ClerkProvider publishableKey={clerkPublishableKey} afterSignOutUrl="/">
			<QueryClientProvider client={queryClient}>
				<RouterProvider router={router} />
			</QueryClientProvider>
		</ClerkProvider>,
	);
}
