import { ClerkLoaded, ClerkLoading, Show, SignIn } from "@clerk/react";
import { Outlet, createRootRoute } from "@tanstack/react-router";
import { TanStackRouterDevtoolsPanel } from "@tanstack/react-router-devtools";
import { TanStackDevtools } from "@tanstack/react-devtools";

import "../styles.css";

export const Route = createRootRoute({
	component: RootComponent,
});

function RootComponent() {
	return (
		<>
			<ClerkLoading>
				<div className="flex min-h-screen items-center justify-center bg-background px-6 text-foreground">
					<div className="rounded-full border border-border px-4 py-2 text-sm text-muted-foreground">
						Loading authentication...
					</div>
				</div>
			</ClerkLoading>

			<ClerkLoaded>
				<Show when="signed-out">
					<div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-[radial-gradient(circle_at_top,rgba(24,24,27,0.08),transparent_45%),linear-gradient(180deg,#f8fafc_0%,#eef2f7_100%)] px-6 py-10">
						<div className="absolute inset-x-0 top-0 h-56 bg-[linear-gradient(135deg,rgba(15,23,42,0.08),transparent)]" />
						<div className="relative grid w-full max-w-6xl gap-10 lg:grid-cols-[1.15fr_0.85fr] lg:items-center">
							<section className="space-y-6 text-center lg:text-left">
								<p className="text-sm font-medium uppercase tracking-[0.28em] text-muted-foreground">
									Autodor
								</p>
								<div className="space-y-4">
									<h1 className="text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
										Zaloguj się, aby wejść do panelu operacyjnego.
									</h1>
									<p className="max-w-2xl text-base leading-7 text-muted-foreground sm:text-lg">
										Cała aplikacja jest chroniona przez Clerk. Po poprawnym
										logowaniu wrócisz bezpośrednio na stronę główną.
									</p>
								</div>
							</section>

							<div className="flex justify-center lg:justify-end">
								<SignIn
									fallbackRedirectUrl="/"
									forceRedirectUrl="/"
									appearance={{
										elements: {
											card: "shadow-2xl border border-white/70",
											rootBox: "w-full",
										},
									}}
								/>
							</div>
						</div>
					</div>
				</Show>

				<Show when="signed-in">
					<Outlet />
					<TanStackDevtools
						config={{
							position: "bottom-right",
						}}
						plugins={[
							{
								name: "TanStack Router",
								render: <TanStackRouterDevtoolsPanel />,
							},
						]}
					/>
				</Show>
			</ClerkLoaded>
		</>
	);
}
