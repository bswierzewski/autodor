import { ClerkLoaded, ClerkLoading, ClerkProvider } from "@clerk/react";
import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "@tanstack/react-router";
import ReactDOM from "react-dom/client";
import { Empty, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "./components/ui/empty";
import { Spinner } from "./components/ui/spinner";
import { Toaster } from "./components/ui/sonner";

import { queryClient } from "./config/query-client";
import { router } from "./config/router";
import { useAuth } from "./hooks/use-auth";

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
				<Empty className="flex min-h-screen w-full items-center justify-center px-6">
					<EmptyHeader>
						<EmptyMedia variant="icon">
							<Spinner className="size-8" />
						</EmptyMedia>
						<EmptyTitle>Przygotowujemy logowanie</EmptyTitle>
						<EmptyDescription>
							Sprawdzamy Twoją sesję i inicjalizujemy bezpieczny dostęp do aplikacji. Ten ekran znika automatycznie,
							gdy Clerk zakończy ładowanie danych uwierzytelniania.
						</EmptyDescription>
					</EmptyHeader>
				</Empty>
			</ClerkLoading>
			<ClerkLoaded>
				<App />
			</ClerkLoaded>
		</ClerkProvider>,
	);
}
