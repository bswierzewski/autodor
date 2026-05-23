import { DotsThreeOutlineVerticalIcon, MagnifyingGlassIcon, PlusIcon, XIcon } from "@phosphor-icons/react";
import { useQueryClient } from "@tanstack/react-query";
import dayjs from "dayjs";
import { useState } from "react";
import { toast } from "sonner";
import { getGetOrdersQueryKey, useGetOrders, useUpdateOrderExclusion } from "#/api/orders/orders";
import { DatePickerField } from "#/components/common/DatePickerField";
import { Button } from "#/components/ui/button";
import { Checkbox } from "#/components/ui/checkbox";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "#/components/ui/dropdown-menu";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyTitle } from "#/components/ui/empty";
import { Label } from "#/components/ui/label";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";

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

	return (
		<div className="space-y-6">
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
					disabled={selectedOrderIds.length === 0}
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
				<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
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
									className={order.isExcluded ? "bg-destructive/5 hover:bg-destructive/10" : undefined}
									data-state={selectedOrderIds.includes(order.id) ? "selected" : undefined}
									key={order.id}
								>
									<TableCell className="pl-5">
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
									<TableCell className="pr-5 text-right">
										<DropdownMenu>
											<DropdownMenuTrigger asChild>
												<Button className="ml-auto" size="icon-sm" type="button" variant="ghost">
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
									</TableCell>
								</TableRow>
							))}
						</TableBody>
					</Table>
				</div>
			)}
		</div>
	);
}
