import { DotsThreeOutlineVerticalIcon, MagnifyingGlassIcon, PlusIcon, XIcon } from "@phosphor-icons/react";
import { useQueryClient } from "@tanstack/react-query";
import dayjs from "dayjs";
import { useState } from "react";
import { toast } from "sonner";
import { getGetOrdersQueryKey, useGetOrders, useUpdateOrderExclusion } from "#/api/orders/orders";
import { DatePickerField } from "#/components/common/DatePickerField";
import { Button } from "#/components/ui/button";
import { Checkbox } from "#/components/ui/checkbox";
import { Drawer, DrawerContent, DrawerTitle } from "#/components/ui/drawer";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "#/components/ui/dropdown-menu";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyTitle } from "#/components/ui/empty";
import { Label } from "#/components/ui/label";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
import { OrderInvoiceForm } from "#/features/orders/components/OrderInvoiceForm";
import { useMediaQuery } from "#/hooks/use-media-query";

function formatDate(date: Date): string {
	return dayjs(date).format("YYYY-MM-DD");
}

function normalizeDate(date: Date): Date {
	return dayjs(date).startOf("day").toDate();
}

function formatOrderDate(value: string): string {
	return dayjs(value).format("DD.MM.YYYY");
}

const currencyFormatter = new Intl.NumberFormat("pl-PL", {
	style: "currency",
	currency: "PLN",
	minimumFractionDigits: 2,
	maximumFractionDigits: 2,
});

export function OrdersPage() {
	const today = normalizeDate(new Date());
	const weekAgo = dayjs(today).subtract(7, "day").toDate();

	const [query, setQuery] = useState("");
	const [fromDate, setFromDate] = useState(weekAgo);
	const [toDate, setToDate] = useState(today);
	const [selectedOrderIds, setSelectedOrderIds] = useState<string[]>([]);
	const [isFormOpen, setIsFormOpen] = useState(false);
	const isDesktop = useMediaQuery("(min-width: 1024px)");
	const from = formatDate(fromDate);
	const to = formatDate(toDate);
	const queryClient = useQueryClient();

	const { data, isLoading, isError } = useGetOrders({ from, to });
	const updateOrderExclusionMutation = useUpdateOrderExclusion({
		mutation: {
			onSuccess: async (_, variables) => {
				await queryClient.invalidateQueries({
					queryKey: getGetOrdersQueryKey({ from, to }),
				});
				toast.success(
					variables.data.excluded
						? "Zamówienie zostało wyłączone z fakturowania."
						: "Zamówienie zostało przywrócone do fakturowania.",
				);
			},
			onError: () => {
				toast.error("Nie udało się zmienić statusu zamówienia.");
			},
		},
	});
	const orders = data?.orders ?? [];
	const selectedOrders = orders.filter((order) => selectedOrderIds.includes(order.id));
	const hasSelectedOrders = selectedOrders.length > 0;
	const selectedOrderDates = [...new Set(selectedOrders.map((order) => order.date))];
	const selectedContractorNumbers = [
		...new Set(selectedOrders.map((order) => order.customerNumber?.trim()).filter(Boolean)),
	] as string[];
	const defaultContractorNip = selectedContractorNumbers.length === 1 ? (selectedContractorNumbers[0] ?? "") : "";
	const selectedOrdersKey = selectedOrders
		.map((order) => order.id)
		.sort()
		.join(",");
	const normalizedQuery = query.trim().toLowerCase();
	const filteredOrders = orders.filter((order) => {
		const searchableValue = [order.number, order.customerNumber, order.person].join(" ").toLowerCase();

		return searchableValue.includes(normalizedQuery);
	});
	const visibleOrderIds = filteredOrders.map((order) => order.id);
	const allVisibleSelected = visibleOrderIds.length > 0 && visibleOrderIds.every((id) => selectedOrderIds.includes(id));
	const someVisibleSelected = visibleOrderIds.some((id) => selectedOrderIds.includes(id));

	const toggleAllVisible = (checked: boolean | "indeterminate") => {
		setSelectedOrderIds((current) => {
			if (checked === true) {
				return [...new Set([...current, ...visibleOrderIds])];
			}

			return current.filter((id) => !visibleOrderIds.includes(id));
		});
	};

	const toggleOrderSelection = (orderId: string, checked: boolean | "indeterminate") => {
		setSelectedOrderIds((current) => {
			if (checked === true) {
				return current.includes(orderId) ? current : [...current, orderId];
			}

			return current.filter((id) => id !== orderId);
		});
	};

	const toggleOrderExclusion = (orderId: string, excluded: boolean) => {
		updateOrderExclusionMutation.mutate({
			id: orderId,
			data: { excluded },
		});
	};

	const toggleOrderCardSelection = (orderId: string) => {
		const isSelected = selectedOrderIds.includes(orderId);
		toggleOrderSelection(orderId, !isSelected);
	};

	const closeForm = () => setIsFormOpen(false);

	return (
		<div className="space-y-6">
			<section className="grid gap-6">
				<div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
					<label className="relative block w-full">
						<MagnifyingGlassIcon
							className="pointer-events-none absolute top-1/2 left-4 -translate-y-1/2 text-muted-foreground"
							size={18}
						/>
						<input
							className="h-11 w-full rounded-2xl border border-border bg-background pl-11 pr-12 text-sm outline-none transition focus:border-foreground/30"
							placeholder="Szukaj po numerze, nr klienta lub osobie"
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
								<span className="sr-only">Wyczyść wyszukiwanie zamówień</span>
							</button>
						) : null}
					</label>
					<Button
						className="h-11 w-full rounded-2xl px-4 lg:w-auto"
						disabled={!hasSelectedOrders}
						onClick={() => setIsFormOpen(true)}
						type="button"
					>
						<PlusIcon size={16} />
						Wystaw
					</Button>
				</div>

				<div className="grid gap-4 sm:grid-cols-2">
					<div className="grid gap-2">
						<Label htmlFor="orders-date-from">Od</Label>
						<DatePickerField
							id="orders-date-from"
							value={fromDate}
							onChange={(date) => {
								if (!date) {
									return;
								}

								setFromDate(date);
							}}
							disabled={(date) => dayjs(date).isAfter(toDate, "day")}
						/>
					</div>
					<div className="grid gap-2">
						<Label htmlFor="orders-date-to">Do</Label>
						<DatePickerField
							id="orders-date-to"
							value={toDate}
							onChange={(date) => {
								if (!date) {
									return;
								}

								setToDate(date);
							}}
							disabled={(date) => dayjs(date).isBefore(fromDate, "day")}
						/>
					</div>
				</div>

				{isLoading ? (
					<div className="rounded-3xl border border-dashed border-border bg-card p-6 text-sm text-muted-foreground shadow-sm">
						Ładowanie zamówień...
					</div>
				) : isError ? (
					<div className="rounded-3xl border border-dashed border-destructive/30 bg-card p-6 text-sm text-muted-foreground shadow-sm">
						Nie udało się pobrać zamówień dla wybranego zakresu.
					</div>
				) : orders.length === 0 ? (
					<div className="rounded-3xl border border-dashed border-border bg-card p-6 shadow-sm">
						<Empty>
							<EmptyHeader>
								<EmptyTitle>Brak zamówień w wybranym zakresie</EmptyTitle>
								<EmptyDescription>Wybierz inny przedział dat, aby zobaczyć wyniki.</EmptyDescription>
							</EmptyHeader>
						</Empty>
					</div>
				) : filteredOrders.length === 0 ? (
					<div className="rounded-3xl border border-dashed border-border bg-card p-6 shadow-sm">
						<Empty>
							<EmptyHeader>
								<EmptyTitle>Brak zamówień dla podanych filtrów</EmptyTitle>
								<EmptyDescription>Zmień frazę wyszukiwania aby zobaczyć wyniki.</EmptyDescription>
							</EmptyHeader>
							<EmptyContent>
								<Button type="button" variant="outline" onClick={() => setQuery("")}>
									Wyczyść filtry
								</Button>
							</EmptyContent>
						</Empty>
					</div>
				) : (
					<div className="space-y-3">
						<div className="grid gap-4 lg:hidden">
							{filteredOrders.map((order) => (
								<div
									className={[
										"relative rounded-3xl border bg-card p-5 shadow-sm transition focus-within:ring-2 focus-within:ring-primary/20",
										selectedOrderIds.includes(order.id) ? "ring-2 ring-primary/20" : "",
										order.isExcluded ? "border-destructive/30 bg-destructive/5" : "",
									]
										.filter(Boolean)
										.join(" ")}
									key={order.id}
								>
									<button
										aria-label={`Zaznacz zamówienie ${order.number ?? order.id}`}
										aria-pressed={selectedOrderIds.includes(order.id)}
										className="absolute inset-0 rounded-3xl"
										onClick={() => toggleOrderCardSelection(order.id)}
										type="button"
									>
										<span className="sr-only">Zaznacz zamówienie {order.number ?? order.id}</span>
									</button>

									<div className="relative z-10 flex items-start justify-between gap-3 pointer-events-none">
										<div className="flex min-w-0 items-start gap-3 pointer-events-none">
											<div className="pointer-events-none pt-0.5">
												<Checkbox aria-hidden="true" checked={selectedOrderIds.includes(order.id)} tabIndex={-1} />
											</div>
											<div className="min-w-0">
												<p className="truncate text-base font-semibold tracking-tight">{order.number ?? order.id}</p>
												<p className="text-sm text-muted-foreground">{formatOrderDate(order.date)}</p>
											</div>
										</div>
										<div className="pointer-events-auto">
											<DropdownMenu>
												<DropdownMenuTrigger asChild>
													<Button size="icon-sm" type="button" variant="ghost">
														<DotsThreeOutlineVerticalIcon size={16} />
														<span className="sr-only">Otwórz menu akcji dla zamówienia {order.number ?? order.id}</span>
													</Button>
												</DropdownMenuTrigger>
												<DropdownMenuContent align="end">
													<DropdownMenuItem
														disabled={updateOrderExclusionMutation.isPending}
														onSelect={() => toggleOrderExclusion(order.id, !order.isExcluded)}
														variant={order.isExcluded ? "default" : "destructive"}
													>
														{order.isExcluded ? "Przywróć do fakturowania" : "Wyłącz z fakturowania"}
													</DropdownMenuItem>
												</DropdownMenuContent>
											</DropdownMenu>
										</div>
									</div>

									<div className="relative z-10 mt-4 grid gap-3 text-sm sm:grid-cols-2 pointer-events-none">
										<div>
											<p className="text-muted-foreground">Osoba</p>
											<p className="font-medium">{order.person ?? "-"}</p>
										</div>
										<div>
											<p className="text-muted-foreground">Nr klienta</p>
											<p className="font-medium">{order.customerNumber ?? "-"}</p>
										</div>
										<div>
											<p className="text-muted-foreground">Pozycje</p>
											<p className="font-medium">{order.itemsCount}</p>
										</div>
										<div>
											<p className="text-muted-foreground">Kwota</p>
											<p className="font-medium">{currencyFormatter.format(Number(order.totalAmount))}</p>
										</div>
									</div>
								</div>
							))}
						</div>

						<p className="hidden text-sm text-muted-foreground lg:block">
							Zaznacz zamówienia w tabeli, a następnie użyj przycisku Wystaw, aby otworzyć formularz z prawej strony.
						</p>
						<div className="hidden overflow-hidden rounded-3xl border bg-card shadow-sm lg:block">
							<Table>
								<TableHeader>
									<TableRow>
										<TableHead className="w-12 pl-5">
											<Checkbox
												aria-label="Zaznacz wszystkie zamówienia"
												checked={allVisibleSelected ? true : someVisibleSelected ? "indeterminate" : false}
												onCheckedChange={toggleAllVisible}
											/>
										</TableHead>
										<TableHead>Numer</TableHead>
										<TableHead>Data</TableHead>
										<TableHead>Osoba</TableHead>
										<TableHead>Nr klienta</TableHead>
										<TableHead className="text-right">Pozycje</TableHead>
										<TableHead className="text-right">Kwota</TableHead>
										<TableHead className="w-14 pr-5 text-right">Akcje</TableHead>
									</TableRow>
								</TableHeader>
								<TableBody>
									{filteredOrders.map((order) => (
										<TableRow
											className={["cursor-pointer", order.isExcluded ? "bg-destructive/5 hover:bg-destructive/10" : ""]
												.filter(Boolean)
												.join(" ")}
											data-state={selectedOrderIds.includes(order.id) ? "selected" : undefined}
											key={order.id}
											onClick={() => toggleOrderCardSelection(order.id)}
										>
											<TableCell className="pl-5" onClick={(event) => event.stopPropagation()}>
												<Checkbox
													aria-label={`Zaznacz zamówienie ${order.number ?? order.id}`}
													checked={selectedOrderIds.includes(order.id)}
													onCheckedChange={(checked) => toggleOrderSelection(order.id, checked)}
												/>
											</TableCell>
											<TableCell className="font-medium">{order.number ?? "-"}</TableCell>
											<TableCell className="text-muted-foreground">{formatOrderDate(order.date)}</TableCell>
											<TableCell className="text-muted-foreground">{order.person ?? "-"}</TableCell>
											<TableCell className="text-muted-foreground">{order.customerNumber ?? "-"}</TableCell>
											<TableCell className="text-right">{order.itemsCount}</TableCell>
											<TableCell className="text-right font-medium">
												{currencyFormatter.format(Number(order.totalAmount))}
											</TableCell>
											<TableCell className="pr-5 text-right" onClick={(event) => event.stopPropagation()}>
												<DropdownMenu>
													<DropdownMenuTrigger asChild>
														<Button className="ml-auto" size="icon-sm" type="button" variant="ghost">
															<DotsThreeOutlineVerticalIcon size={16} />
															<span className="sr-only">
																Otwórz menu akcji dla zamówienia {order.number ?? order.id}
															</span>
														</Button>
													</DropdownMenuTrigger>
													<DropdownMenuContent align="end">
														<DropdownMenuItem
															disabled={updateOrderExclusionMutation.isPending}
															onSelect={() => toggleOrderExclusion(order.id, !order.isExcluded)}
															variant={order.isExcluded ? "default" : "destructive"}
														>
															{order.isExcluded ? "Przywróć do fakturowania" : "Wyłącz z fakturowania"}
														</DropdownMenuItem>
													</DropdownMenuContent>
												</DropdownMenu>
											</TableCell>
										</TableRow>
									))}
								</TableBody>
							</Table>
						</div>
					</div>
				)}
			</section>

			<Drawer
				direction={isDesktop ? "right" : "bottom"}
				open={isFormOpen}
				onOpenChange={(open) => {
					if (!open) {
						closeForm();
					}
				}}
			>
				<DrawerContent className={isDesktop ? "px-6 pb-6 [&>div:first-child]:hidden" : "px-4 pb-4"}>
					<DrawerTitle className="sr-only">Wystaw fakturę</DrawerTitle>
					<div
						className={
							isDesktop
								? "h-full overflow-y-auto px-1 pb-2 pt-6"
								: "mx-auto w-full max-w-2xl overflow-y-auto px-1 pb-2 pt-4"
						}
					>
						{isFormOpen ? (
							<OrderInvoiceForm
								key={selectedOrdersKey || "empty"}
								contractorNip={defaultContractorNip}
								onClose={closeForm}
								orderDates={selectedOrderDates}
								orderIds={selectedOrderIds}
								onSuccess={() => {
									setSelectedOrderIds([]);
									closeForm();
								}}
							/>
						) : null}
					</div>
				</DrawerContent>
			</Drawer>
		</div>
	);
}
