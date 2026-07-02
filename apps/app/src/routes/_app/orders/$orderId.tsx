import { createFileRoute } from "@tanstack/react-router";
import { useGetOrder } from "#/api/orders/orders";
import { OrderDetailsItemsTable } from "#/features/orders/components/details/OrderDetailsItemsTable";
import { OrderDetailsSummaryCard } from "#/features/orders/components/details/OrderDetailsSummaryCard";

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

	const {
		data: order,
		isError,
		isLoading,
	} = useGetOrder(
		orderId,
		{ date },
		{
			query: {
				enabled: Boolean(date),
			},
		},
	);

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
		<div className="space-y-4">
			<OrderDetailsSummaryCard order={order} />
			<OrderDetailsItemsTable items={order.items} orderDate={date} orderId={orderId} />
		</div>
	);
}
