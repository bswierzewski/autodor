import { createFileRoute, Outlet, useMatchRoute, useNavigate } from "@tanstack/react-router";
import { useState } from "react";
import { useGetContractors } from "#/api/contractors/contractors";
import type { GetContractorsResponse } from "#/api/models";
import { Drawer, DrawerContent, DrawerTitle } from "#/components/ui/drawer";
import { ContractorsCardList } from "#/features/contractors/components/ContractorsCardList";
import { ContractorsEmptyState } from "#/features/contractors/components/ContractorsEmptyState";
import { ContractorsFilters } from "#/features/contractors/components/ContractorsFilters";
import { ContractorsTable } from "#/features/contractors/components/ContractorsTable";
import { useMediaQuery } from "#/hooks/use-media-query";

export const Route = createFileRoute("/_app/contractors")({
	component: ContractorsRoute,
});

function ContractorsRoute() {
	const [query, setQuery] = useState("");
	const matchRoute = useMatchRoute();
	const navigate = useNavigate();
	const isDesktop = useMediaQuery("(min-width: 1024px)");
	const contractorsQuery = useGetContractors();
	const contractors: GetContractorsResponse[] = contractorsQuery.data ?? [];
	const normalizedQuery = query.trim().toLowerCase();
	const filteredContractors = contractors.filter((contractor) => {
		const searchableValue = [contractor.name, contractor.nip, contractor.city, contractor.street, contractor.email]
			.join(" ")
			.toLowerCase();

		return searchableValue.includes(normalizedQuery);
	});
	const isCreateDrawerRoute = Boolean(matchRoute({ to: "/contractors/create" }));
	const isEditDrawerRoute = Boolean(matchRoute({ to: "/contractors/$contractorId/edit" }));
	const isDrawerOpen = isCreateDrawerRoute || isEditDrawerRoute;
	const drawerTitle = isEditDrawerRoute ? "Edytuj kontrahenta" : "Dodaj kontrahenta";

	const handleCloseDrawer = () => {
		void navigate({ to: "/contractors" });
	};

	return (
		<div className="space-y-6">
			<section className="grid gap-6">
				<div className="space-y-4">
					<ContractorsFilters query={query} onClearQuery={() => setQuery("")} onQueryChange={setQuery} />

					{contractorsQuery.isLoading ? (
						<div className="rounded-3xl border border-dashed border-border bg-card p-6 text-sm text-muted-foreground shadow-sm">
							Ładowanie kontrahentów...
						</div>
					) : contractorsQuery.isError ? (
						<div className="rounded-3xl border border-dashed border-destructive/30 bg-card p-6 text-sm text-muted-foreground shadow-sm">
							Nie udało się pobrać listy kontrahentów.
						</div>
					) : filteredContractors.length === 0 ? (
						<ContractorsEmptyState hasContractors={contractors.length > 0} onClearFilters={() => setQuery("")} />
					) : isDesktop ? (
						<ContractorsTable contractors={filteredContractors} />
					) : (
						<ContractorsCardList contractors={filteredContractors} />
					)}
				</div>
			</section>

			<Drawer
				direction={isDesktop ? "right" : "bottom"}
				open={isDrawerOpen}
				onOpenChange={(open) => {
					if (!open) {
						handleCloseDrawer();
					}
				}}
			>
				<DrawerContent className={isDesktop ? "px-6 pb-6 [&>div:first-child]:hidden" : "px-4 pb-4"}>
					<DrawerTitle className="sr-only">{drawerTitle}</DrawerTitle>
					<div
						className={
							isDesktop
								? "h-full overflow-y-auto px-1 pb-2 pt-6"
								: "mx-auto w-full max-w-2xl overflow-y-auto px-1 pb-2 pt-4"
						}
					>
						<Outlet />
					</div>
				</DrawerContent>
			</Drawer>
		</div>
	);
}
