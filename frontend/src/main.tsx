import { ClerkLoaded, ClerkLoading, ClerkProvider } from "@clerk/react";
import ReactDOM from "react-dom/client";
import { LoadingScreen } from "./components/LoadingScreen";
import { AppQueryProvider } from "./providers/AppQueryProvider";
import { AppRouterProvider } from "./providers/AppRouterProvider";

const rootElement = document.getElementById("app") as HTMLElement;

if (!rootElement.innerHTML) {
	const root = ReactDOM.createRoot(rootElement);

	root.render(
		<ClerkProvider publishableKey={import.meta.env.VITE_CLERK_PUBLISHABLE_KEY ?? ""}>
			<ClerkLoading>
				<LoadingScreen />
			</ClerkLoading>
			<ClerkLoaded>
				<AppQueryProvider>
					<AppRouterProvider />
				</AppQueryProvider>
			</ClerkLoaded>
		</ClerkProvider>,
	);
}
