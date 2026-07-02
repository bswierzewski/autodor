import { ReceiptIcon } from "@phosphor-icons/react";
import { createFileRoute } from "@tanstack/react-router";
import dayjs from "dayjs";
import { useState } from "react";
import { useGetOrders } from "#/api/orders/orders";
import { Button } from "#/components/ui/button";
import { Drawer, DrawerContent, DrawerDescription, DrawerTitle } from "#/components/ui/drawer";
import { CreateInvoiceForm } from "#/features/orders/components/invoice/CreateInvoiceForm";
import { OrderCardList } from "#/features/orders/components/list/OrderCardList";
import { OrdersEmptyState } from "#/features/orders/components/list/OrdersEmptyState";
import { OrdersFilteredEmptyState } from "#/features/orders/components/list/OrdersFilteredEmptyState";
import { OrdersFilters } from "#/features/orders/components/list/OrdersFilters";
import { OrdersTable } from "#/features/orders/components/list/OrdersTable";
import { ordersSearchSchema, useOrdersSearch } from "#/features/orders/hooks/useOrdersSearch";
import { useMediaQuery } from "#/hooks/use-media-query";
import { formatDate } from "#/lib/formatters";

export const Route = createFileRoute("/_app/")({
	validateSearch: ordersSearchSchema,
	component: OrdersRoute,
});

function OrdersRoute() {
	const isDesktop = useMediaQuery("(min-width: 1024px)");
	const { from, to, query, updateSearch } = useOrdersSearch();
	const fromDate = dayjs(from).toDate();
	const toDate = dayjs(to).toDate();

	const [selectedOrderIds, setSelectedOrderIds] = useState<Set<string>>(new Set());
	const [isInvoiceDrawerOpen, setIsInvoiceDrawerOpen] = useState(false);

	const toggleOrderSelection = (id: string) => {
		setSelectedOrderIds((prev) => {
			const next = new Set(prev);
			if (next.has(id)) {
				next.delete(id);
			} else {
				next.add(id);
			}
			return next;
		});
	};

	const toggleSelectAll = (checked: boolean) => {
		if (checked) {
			setSelectedOrderIds(new Set(filteredOrders.map((o) => o.id)));
		} else {
			setSelectedOrderIds(new Set());
		}
	};

	const { data, isLoading, isError } = useGetOrders({ from, to });
	const orders = data?.orders ?? [];
	const filteredOrders = orders.filter((order) => {
		const searchableValue = [order.number, order.customerNumber, order.person].join(" ").toLowerCase();

		return searchableValue.includes(query.trim().toLowerCase());
	});

	const selectedOrders = orders.filter((o) => selectedOrderIds.has(o.id));

	return (
		<div className="space-y-4">
			<section className="grid gap-4">
				<OrdersFilters
					actions={
						<Button
							className="h-11 w-full rounded-2xl px-4 lg:w-56"
							disabled={selectedOrders.length === 0}
							onClick={(event) => {
								event.currentTarget.blur();
								setIsInvoiceDrawerOpen(true);
							}}
						>
							<ReceiptIcon size={16} />
							Wystaw fakturę
						</Button>
					}
					fromDate={fromDate}
					onClearQuery={() => updateSearch({ query: undefined })}
					onFromDateChange={(date) => updateSearch({ from: formatDate(date) })}
					onQueryChange={(value) => updateSearch({ query: value || undefined })}
					onToDateChange={(date) => updateSearch({ to: formatDate(date) })}
					query={query}
					toDate={toDate}
				/>

				{isLoading ? (
					<div className="rounded-3xl border border-dashed border-border bg-card p-6 text-sm text-muted-foreground shadow-sm">
						Ładowanie zamówień...
					</div>
				) : isError ? (
					<div className="rounded-3xl border border-dashed border-destructive/30 bg-card p-6 text-sm text-muted-foreground shadow-sm">
						Nie udało się pobrać zamówień dla wybranego zakresu.
					</div>
				) : orders.length === 0 ? (
					<OrdersEmptyState />
				) : filteredOrders.length === 0 ? (
					<OrdersFilteredEmptyState onClearFilters={() => updateSearch({ query: undefined })} />
				) : (
					<div className="space-y-3">
						{isDesktop ? (
							<OrdersTable
								onToggleSelect={toggleOrderSelection}
								onToggleSelectAll={toggleSelectAll}
								orders={filteredOrders}
								selectedOrderIds={selectedOrderIds}
							/>
						) : (
							<OrderCardList
								onToggleSelect={toggleOrderSelection}
								orders={filteredOrders}
								selectedOrderIds={selectedOrderIds}
							/>
						)}
					</div>
				)}
			</section>

			<Drawer
				direction={isDesktop ? "right" : "bottom"}
				open={isInvoiceDrawerOpen}
				onOpenChange={(open) => {
					if (!open) setIsInvoiceDrawerOpen(false);
				}}
			>
				<DrawerContent className={isDesktop ? "px-6 pb-6 [&>div:first-child]:hidden" : "px-4 pb-4"}>
					<DrawerTitle className="sr-only">Wystaw fakturę</DrawerTitle>
					<DrawerDescription className="sr-only">
						Formularz do wystawienia faktury dla wybranych zamówień.
					</DrawerDescription>
					<div className="mt-2 min-h-0 flex-1 overflow-y-auto">
						<CreateInvoiceForm
							selectedOrders={selectedOrders}
							onCancel={() => setIsInvoiceDrawerOpen(false)}
							onSuccess={() => {
								setIsInvoiceDrawerOpen(false);
								setSelectedOrderIds(new Set());
							}}
						/>
					</div>
				</DrawerContent>
			</Drawer>
		</div>
	);
}
