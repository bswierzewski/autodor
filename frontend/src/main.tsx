import { ClerkLoaded, ClerkLoading, ClerkProvider } from "@clerk/react";
import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "@tanstack/react-router";
import ReactDOM from "react-dom/client";
import { LoadingScreen } from "./components/screens/LoadingScreen";
import { Toaster } from "./components/ui/sonner";

import { queryClient } from "./config/query-client";
import { router } from "./config/router";
import { useAuth } from "./hooks/useAuth";

import "./styles.css";

const rootElement = document.getElementById("app") as HTMLElement;

function App() {
	const auth = useAuth();

	return (
		<QueryClientProvider client={queryClient}>
			<RouterProvider router={router} context={{ auth }} />
			<Toaster />
		</QueryClientProvider>
	);
}

if (!rootElement.innerHTML) {
	const root = ReactDOM.createRoot(rootElement);

	root.render(
		<ClerkProvider publishableKey={import.meta.env.VITE_CLERK_PUBLISHABLE_KEY ?? ""}>
			<ClerkLoading>
				<LoadingScreen />
			</ClerkLoading>
			<ClerkLoaded>
				<App />
			</ClerkLoaded>
		</ClerkProvider>,
	);
}
