import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { toast } from "sonner";
import { getGetOrderQueryKey, getGetOrdersQueryKey, useGetOrder, useUpdateOrderItemExclusion } from "#/api/orders/orders";
import { OrderDetailsItemsTable } from "#/features/orders/components/details/order/orderDetails/OrderDetailsItemsTable";
import { OrderDetailsSummaryCard } from "#/features/orders/components/details/order/orderDetails/OrderDetailsSummaryCard";

type OrderDetailsSearch = {
	date: string;
};

export const Route = createFileRoute("/_app/orders/$orderId")({
	validateSearch: (search: Record<string, unknown>): OrderDetailsSearch => ({
		date: typeof search.date === "string" ? search.date : "",
	}),
	component: OrderDetailsRoute,
});

function OrderDetailsRoute() {
	const { orderId } = Route.useParams();
	const { date } = Route.useSearch();
	const queryClient = useQueryClient();

	const updateOrderItemExclusionMutation = useUpdateOrderItemExclusion({
		mutation: {
			onSuccess: async (_, variables) => {
				await Promise.all([
					queryClient.invalidateQueries({ queryKey: getGetOrdersQueryKey() }),
					queryClient.invalidateQueries({ queryKey: getGetOrderQueryKey(orderId, { date }) }),
				]);

				toast.success(
					variables.data.excluded
						? `Pozycja ${variables.itemNumber} została wyłączona z fakturowania.`
						: `Pozycja ${variables.itemNumber} została przywrócona do fakturowania.`,
				);
			},
			onError: () => {
				toast.error("Nie udało się zmienić statusu pozycji zamówienia.");
			},
		},
	});

	const toggleOrderItemExclusion = (itemNumber: string, excluded: boolean) => {
		updateOrderItemExclusionMutation.mutate({
			id: orderId,
			itemNumber,
			data: { excluded },
		});
	};
    
	const { data: order, isError, isLoading } = useGetOrder(
		orderId,
		{ date },
		{
			query: {
				enabled: Boolean(date),
			},
		},
	);

	const isPending = updateOrderItemExclusionMutation.isPending;

	if (!date) {
		return (
			<div className="space-y-4">
				<div className="rounded-3xl border border-dashed border-destructive/30 bg-card p-6 text-sm text-muted-foreground shadow-sm">
					Brakuje daty zamówienia potrzebnej do pobrania szczegółów.
				</div>
			</div>
		);
	}

	if (isLoading) {
		return (
			<div className="space-y-4">
				<div className="rounded-3xl border border-dashed border-border bg-card p-6 text-sm text-muted-foreground shadow-sm">
					Ładowanie szczegółów zamówienia...
				</div>
			</div>
		);
	}

	if (isError || !order) {
		return (
			<div className="space-y-4">
				<div className="rounded-3xl border border-dashed border-destructive/30 bg-card p-6 text-sm text-muted-foreground shadow-sm">
					Nie udało się pobrać szczegółów zamówienia.
				</div>
			</div>
		);
	}

	return (
		<div className="space-y-6">
			<OrderDetailsSummaryCard order={order} />
			<OrderDetailsItemsTable isPending={isPending} items={order.items} onToggleOrderItemExclusion={toggleOrderItemExclusion} />
		</div>
	);
}