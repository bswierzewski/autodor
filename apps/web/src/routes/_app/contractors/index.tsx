import {
	EnvelopeSimpleIcon,
	IdentificationCardIcon,
	MagnifyingGlassIcon,
	MapPinLineIcon,
	PencilSimpleLineIcon,
	PlusIcon,
	TrashIcon,
	XIcon,
} from "@phosphor-icons/react";
import { createFileRoute, Link, Outlet, useMatchRoute, useNavigate } from "@tanstack/react-router";
import { useState } from "react";
import { type GetContractorsQueryResult, useGetContractors } from "#/api/contractors/contractors";
import { Button } from "#/components/ui/button";
import { Drawer, DrawerContent, DrawerTitle } from "#/components/ui/drawer";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyTitle } from "#/components/ui/empty";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
import { DeleteContractorDialog } from "#/features/contractors/components/DeleteContractorDialog";
import { useMediaQuery } from "#/hooks/use-media-query";

export const Route = createFileRoute("/_app/contractors/")({
	component: ContractorsRoute,
});

function ContractorsRoute() {
	const [query, setQuery] = useState("");
	const isDesktop = useMediaQuery("(min-width: 1024px)");
	const matchRoute = useMatchRoute();
	const navigate = useNavigate();
	const contractorsQuery = useGetContractors();
	const contractors: GetContractorsQueryResult = contractorsQuery.data ?? [];
	const isCreateRoute = Boolean(matchRoute({ to: "/contractors/create" }));
	const editRouteMatch = matchRoute({ to: "/contractors/$contractorId/edit" });
	const selectedContractorId = editRouteMatch ? editRouteMatch.contractorId : undefined;
	const isFormOpen = isCreateRoute || Boolean(editRouteMatch);
	const drawerTitle = isCreateRoute ? "Dodaj kontrahenta" : "Edytuj kontrahenta";

	const closeDialog = () => {
		void navigate({ to: "/contractors" });
	};

	const normalizedQuery = query.trim().toLowerCase();
	const filteredContractors = contractors.filter((contractor) => {
		const searchableValue = [contractor.name, contractor.nip, contractor.city, contractor.street, contractor.email]
			.join(" ")
			.toLowerCase();

		return searchableValue.includes(normalizedQuery);
	});

	return (
		<div className="space-y-6">
			<section className="grid gap-6">
				<div className="space-y-4">
					<div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
						<label className="relative block w-full">
							<MagnifyingGlassIcon
								className="pointer-events-none absolute top-1/2 left-4 -translate-y-1/2 text-muted-foreground"
								size={18}
							/>
							<input
								className="h-11 w-full rounded-2xl border border-border bg-background pl-11 pr-12 text-sm outline-none transition focus:border-foreground/30"
								placeholder="Szukaj po nazwie, NIP, mieście lub emailu"
								type="text"
								value={query}
								onChange={(event) => setQuery(event.target.value)}
							/>
							{query ? (
								<button
									className="absolute top-1/2 right-3 flex size-7 -translate-y-1/2 cursor-pointer items-center justify-center rounded-full text-muted-foreground transition hover:bg-muted hover:text-foreground"
									onClick={() => setQuery("")}
									type="button"
								>
									<XIcon size={14} />
									<span className="sr-only">Wyczyść wyszukiwanie</span>
								</button>
							) : null}
						</label>
						<Button asChild className="h-11 w-full rounded-2xl px-4 lg:w-auto" type="button">
							<Link to="/contractors/create">
								<PlusIcon size={16} />
								Dodaj kontrahenta
							</Link>
						</Button>
					</div>

					{contractorsQuery.isLoading ? (
						<div className="rounded-3xl border border-dashed border-border bg-card p-6 text-sm text-muted-foreground shadow-sm">
							Ładowanie kontrahentów...
						</div>
					) : contractorsQuery.isError ? (
						<div className="rounded-3xl border border-dashed border-destructive/30 bg-card p-6 text-sm text-muted-foreground shadow-sm">
							Nie udało się pobrać listy kontrahentów.
						</div>
					) : filteredContractors.length === 0 ? (
						<div className="rounded-3xl border border-dashed border-border bg-card p-6 shadow-sm">
							<Empty>
								<EmptyHeader>
									<EmptyTitle>
										{contractors.length === 0 ? "Brak kontrahentów" : "Brak kontrahentów dla podanych filtrów"}
									</EmptyTitle>
									<EmptyDescription>
										{contractors.length === 0
											? "Dodaj pierwszego kontrahenta, aby rozpocząć pracę z bazą klientów."
											: "Zmień frazę wyszukiwania aby zobaczyć wyniki."}
									</EmptyDescription>
								</EmptyHeader>
								<EmptyContent>
									{contractors.length === 0 ? (
										<Button asChild type="button">
											<Link to="/contractors/create">
												<PlusIcon size={16} />
												Dodaj kontrahenta
											</Link>
										</Button>
									) : (
										<Button
											type="button"
											variant="outline"
											onClick={() => {
												setQuery("");
											}}
										>
											Wyczyść filtry
										</Button>
									)}
								</EmptyContent>
							</Empty>
						</div>
					) : (
						<>
							<div className="grid gap-4 lg:hidden">
								{filteredContractors.map((contractor) => (
									<article
										className="rounded-3xl border bg-card p-5 text-left shadow-sm transition hover:-translate-y-0.5 hover:shadow-md"
										key={contractor.id}
									>
										<div className="flex items-start justify-between gap-4">
											<p className="text-lg font-semibold tracking-tight">{contractor.name}</p>
											<div className="flex items-center gap-2">
												<Button asChild size="icon-sm" type="button" variant="outline">
													<Link params={{ contractorId: contractor.id }} to="/contractors/$contractorId/edit">
														<PencilSimpleLineIcon size={16} />
														<span className="sr-only">Edytuj kontrahenta {contractor.name}</span>
													</Link>
												</Button>
												<DeleteContractorDialog contractor={contractor}>
													<Button size="icon-sm" type="button" variant="destructive">
														<TrashIcon size={16} />
														<span className="sr-only">Usuń kontrahenta {contractor.name}</span>
													</Button>
												</DeleteContractorDialog>
											</div>
										</div>
										<div className="mt-5 space-y-2 text-sm text-muted-foreground">
											<div className="flex items-center gap-2">
												<IdentificationCardIcon size={16} />
												<span>NIP {contractor.nip}</span>
											</div>
											<div className="flex items-center gap-2">
												<MapPinLineIcon size={16} />
												<span>
													{contractor.street}, {contractor.zipCode} {contractor.city}
												</span>
											</div>
											<div className="flex items-center gap-2">
												<EnvelopeSimpleIcon size={16} />
												<span>{contractor.email}</span>
											</div>
										</div>
									</article>
								))}
							</div>

							<div className="hidden space-y-3 lg:block">
								<p className="text-sm text-muted-foreground">
									Użyj akcji w ostatniej kolumnie, aby edytować lub usunąć kontrahenta.
								</p>
								<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
									<Table>
										<TableHeader>
											<TableRow>
												<TableHead className="pl-5">Nazwa</TableHead>
												<TableHead>NIP</TableHead>
												<TableHead>Adres</TableHead>
												<TableHead>Email</TableHead>
												<TableHead className="w-28 pr-5" />
											</TableRow>
										</TableHeader>
										<TableBody>
											{filteredContractors.map((contractor) => (
												<TableRow
													className="transition-colors hover:bg-muted/40"
													data-state={selectedContractorId === contractor.id ? "selected" : undefined}
													key={contractor.id}
												>
													<TableCell className="pl-5 font-medium">{contractor.name}</TableCell>
													<TableCell className="text-muted-foreground">{contractor.nip}</TableCell>
													<TableCell className="text-muted-foreground">
														{contractor.street}, {contractor.zipCode} {contractor.city}
													</TableCell>
													<TableCell className="text-muted-foreground">{contractor.email}</TableCell>
													<TableCell className="pr-5">
														<div className="flex items-center justify-end gap-2">
															<Button asChild size="icon-sm" type="button" variant="outline">
																<Link params={{ contractorId: contractor.id }} to="/contractors/$contractorId/edit">
																	<PencilSimpleLineIcon size={16} />
																	<span className="sr-only">Edytuj kontrahenta {contractor.name}</span>
																</Link>
															</Button>
															<DeleteContractorDialog contractor={contractor}>
																<Button size="icon-sm" type="button" variant="destructive">
																	<TrashIcon size={16} />
																	<span className="sr-only">Usuń kontrahenta {contractor.name}</span>
																</Button>
															</DeleteContractorDialog>
														</div>
													</TableCell>
												</TableRow>
											))}
										</TableBody>
									</Table>
								</div>
							</div>
						</>
					)}
				</div>
			</section>

			<Drawer
				direction={isDesktop ? "right" : "bottom"}
				open={isFormOpen}
				onOpenChange={(open) => {
					if (!open) closeDialog();
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
						{isFormOpen ? <Outlet /> : null}
					</div>
				</DrawerContent>
			</Drawer>
		</div>
	);
}
