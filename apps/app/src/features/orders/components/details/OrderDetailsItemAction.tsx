import { ArrowClockwiseIcon, XIcon } from "@phosphor-icons/react";
import { useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import type { OrderItemResponse } from "#/api/models/orderItemResponse";
import { getGetOrderQueryKey, getGetOrdersQueryKey, useUpdateOrderItemExclusion } from "#/api/orders/orders";
import { Button } from "#/components/ui/button";
import { Spinner } from "#/components/ui/spinner";

type OrderDetailsItemActionProps = {
	orderId: string;
	orderDate: string;
	item: OrderItemResponse;
};

export function OrderDetailsItemAction({ orderId, orderDate, item }: OrderDetailsItemActionProps) {
	const actionLabel = item.isExcluded ? "Przywróć" : "Pomiń";
	const queryClient = useQueryClient();
	const updateExclusionMutation = useUpdateOrderItemExclusion({
		mutation: {
			onSuccess: async (_, variables) => {
				await Promise.all([
					queryClient.invalidateQueries({ queryKey: getGetOrdersQueryKey() }),
					queryClient.invalidateQueries({ queryKey: getGetOrderQueryKey(orderId, { date: orderDate }) }),
				]);
				toast.success(
					variables.data.excluded
						? `Pozycja ${item.itemNumber} została pominięta przy fakturowaniu.`
						: `Pozycja ${item.itemNumber} została przywrócona do fakturowania.`,
				);
			},
			onError: () => toast.error("Nie udało się zmienić statusu pozycji zamówienia."),
		},
	});
	const isPending = updateExclusionMutation.isPending;

	return (
		<Button
			aria-label={actionLabel}
			aria-busy={isPending}
			disabled={isPending}
			size="icon-sm"
			type="button"
			variant={item.isExcluded ? "secondary" : "destructive"}
			onClick={() =>
				updateExclusionMutation.mutate({
					id: orderId,
					itemNumber: item.itemNumber,
					data: { excluded: !item.isExcluded },
				})
			}
		>
			{isPending ? <Spinner /> : item.isExcluded ? <ArrowClockwiseIcon size={16} /> : <XIcon size={16} />}
		</Button>
	);
}
