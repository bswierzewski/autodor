import { ClerkProvider } from "@clerk/react";
import ReactDOM from "react-dom/client";
import { AppQueryProvider } from "./providers/AppQueryProvider";
import { AppRouterProvider } from "./providers/AppRouterProvider";

const rootElement = document.getElementById("app") as HTMLElement;

if (!rootElement.innerHTML) {
	const root = ReactDOM.createRoot(rootElement);

	root.render(
		<ClerkProvider
			publishableKey={import.meta.env.VITE_CLERK_PUBLISHABLE_KEY ?? ""}
		>
			<AppQueryProvider>
				<AppRouterProvider />
			</AppQueryProvider>
		</ClerkProvider>,
	);
}
