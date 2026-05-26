import { ReceiptIcon } from "@phosphor-icons/react";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import dayjs from "dayjs";
import { useState } from "react";
import { toast } from "sonner";
import {
	getGetOrdersQueryKey,
	useGenerateDeliveryNote,
	useGetOrders,
	useUpdateOrderExclusion,
} from "#/api/orders/orders";
import { Button } from "#/components/ui/button";
import { Drawer, DrawerContent, DrawerTitle } from "#/components/ui/drawer";
import { CreateInvoiceForm } from "#/features/orders/components/invoice/CreateInvoiceForm";
import { OrderCardList } from "#/features/orders/components/list/OrderCardList";
import { OrdersEmptyState } from "#/features/orders/components/list/OrdersEmptyState";
import { OrdersFilteredEmptyState } from "#/features/orders/components/list/OrdersFilteredEmptyState";
import { OrdersFilters } from "#/features/orders/components/list/OrdersFilters";
import { OrdersTable } from "#/features/orders/components/list/OrdersTable";
import { useMediaQuery } from "#/hooks/use-media-query";
import { formatDate } from "#/lib/formatters";
import { downloadBlob } from "#/lib/utils";

export const Route = createFileRoute("/_app/")({
	component: OrdersRoute,
});

function OrdersRoute() {
	const today = dayjs().startOf("day").toDate();
	const weekAgo = dayjs(today).subtract(7, "day").toDate();
	const isDesktop = useMediaQuery("(min-width: 1024px)");

	const [query, setQuery] = useState("");
	const [fromDate, setFromDate] = useState(weekAgo);
	const [toDate, setToDate] = useState(today);
	const from = formatDate(fromDate);
	const to = formatDate(toDate);
	const queryClient = useQueryClient();

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

	const generateDeliveryNoteMutation = useGenerateDeliveryNote({
		mutation: {
			onSuccess: (pdf, variables) => {
				downloadBlob(pdf, `WZ_${variables.data.orderId}.pdf`);
				toast.success("PDF został pobrany.");
			},
			onError: () => {
				toast.error("Nie udało się wygenerować pliku PDF.");
			},
		},
	});

	const printOrderPdf = (orderId: string, date: string) => {
		generateDeliveryNoteMutation.mutate({
			data: {
				orderId,
				date,
			},
		});
	};

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

	const toggleOrderExclusion = (orderId: string, excluded: boolean) => {
		updateOrderExclusionMutation.mutate({
			id: orderId,
			data: { excluded },
		});
	};

	const { data, isLoading, isError } = useGetOrders({ from, to });
	const orders = data?.orders ?? [];
	const isPending = updateOrderExclusionMutation.isPending || generateDeliveryNoteMutation.isPending;
	const filteredOrders = orders.filter((order) => {
		const searchableValue = [order.number, order.customerNumber, order.person].join(" ").toLowerCase();

		return searchableValue.includes(query);
	});

	const selectedOrders = orders.filter((o) => selectedOrderIds.has(o.id));

	return (
		<div className="space-y-6">
			<section className="grid gap-6">
				<OrdersFilters
					fromDate={fromDate}
					onClearQuery={() => setQuery("")}
					onFromDateChange={setFromDate}
					onQueryChange={setQuery}
					onToDateChange={setToDate}
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
					<OrdersFilteredEmptyState onClearFilters={() => setQuery("")} />
				) : (
					<div className="space-y-3">
						<div className="flex justify-end gap-2">
							{selectedOrderIds.size > 0 && (
								<Button size="sm" variant="outline" onClick={() => setSelectedOrderIds(new Set())}>
									Wyczyść
								</Button>
							)}
							<Button disabled={selectedOrderIds.size === 0} size="sm" onClick={() => setIsInvoiceDrawerOpen(true)}>
								<ReceiptIcon />
								Wystaw fakturę
							</Button>
						</div>
						{isDesktop ? (
							<OrdersTable
								isPending={isPending}
								onPrintOrderPdf={printOrderPdf}
								onToggleOrderExclusion={toggleOrderExclusion}
								onToggleSelect={toggleOrderSelection}
								onToggleSelectAll={toggleSelectAll}
								orders={filteredOrders}
								selectedOrderIds={selectedOrderIds}
							/>
						) : (
							<OrderCardList
								isPending={isPending}
								onPrintOrderPdf={printOrderPdf}
								onToggleOrderExclusion={toggleOrderExclusion}
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
					<div
						className={
							isDesktop
								? "h-full overflow-y-auto px-1 pb-2 pt-6"
								: "mx-auto w-full max-w-2xl overflow-y-auto px-1 pb-2 pt-4"
						}
					>
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
