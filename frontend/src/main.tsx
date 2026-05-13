import { ClerkLoaded, ClerkLoading, ClerkProvider } from "@clerk/react";
import { MantineProvider } from "@mantine/core";
import { ModalsProvider } from "@mantine/modals";
import { Notifications } from "@mantine/notifications";
import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "@tanstack/react-router";
import ReactDOM from "react-dom/client";
import { LoadingScreen } from "./components/layouts/LoadingScreen";

import { queryClient } from "./config/query-client";
import { router } from "./config/router";
import { theme } from "./config/theme";
import { useAuth } from "./hooks/useAuth";

import "./styles.css";

const rootElement = document.getElementById("app") as HTMLElement;

function App() {
	const auth = useAuth();

	return (
		<MantineProvider theme={theme}>
			<ModalsProvider>
				<Notifications />
				<QueryClientProvider client={queryClient}>
					<RouterProvider router={router} context={{ auth }} />
				</QueryClientProvider>
			</ModalsProvider>
		</MantineProvider>
	);
}

if (!rootElement.innerHTML) {
	const root = ReactDOM.createRoot(rootElement);

	root.render(
		<ClerkProvider
			publishableKey={import.meta.env.VITE_CLERK_PUBLISHABLE_KEY ?? ""}
		>
			<ClerkLoading>
				<LoadingScreen />
			</ClerkLoading>
			<ClerkLoaded>
				<App />
			</ClerkLoaded>
		</ClerkProvider>,
	);
}
