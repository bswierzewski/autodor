import { HouseIcon, MagnifyingGlassIcon } from "@phosphor-icons/react";
import { TanStackDevtools } from "@tanstack/react-devtools";
import { createRootRouteWithContext, Link, Outlet } from "@tanstack/react-router";
import { TanStackRouterDevtoolsPanel } from "@tanstack/react-router-devtools";

import { Button } from "../components/ui/button";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "../components/ui/empty";
import type { RouterContext } from "../config/router";

export const Route = createRootRouteWithContext<RouterContext>()({
	component: RootComponent,
	notFoundComponent: NotFoundComponent,
});

function RootComponent() {
	return (
		<>
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
		</>
	);
}

function NotFoundComponent() {
	return (
		<Empty className="flex min-h-screen w-full items-center justify-center px-6">
			<EmptyHeader>
				<EmptyMedia variant="icon">
					<MagnifyingGlassIcon size={20} weight="duotone" />
				</EmptyMedia>
				<EmptyTitle>404 - Nie znaleziono strony</EmptyTitle>
				<EmptyDescription>
					Nie udało się odnaleźć strony pod tym adresem. Sprawdź, czy link jest poprawny, albo wróć na stronę główną
					aplikacji.
				</EmptyDescription>
			</EmptyHeader>
			<EmptyContent>
				<Button asChild size="lg">
					<Link to="/">
						<HouseIcon size={18} weight="duotone" />
						Strona główna
					</Link>
				</Button>
			</EmptyContent>
		</Empty>
	);
}
