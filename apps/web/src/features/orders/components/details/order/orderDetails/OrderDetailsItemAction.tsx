import { ArrowClockwiseIcon, XIcon } from "@phosphor-icons/react";
import type { OrderItemResponse } from "#/api/models/orderItemResponse";
import { Button } from "#/components/ui/button";

type ToggleOrderItemExclusion = (itemNumber: string, excluded: boolean) => void;

type OrderDetailsItemActionProps = {
	item: OrderItemResponse;
	isPending: boolean;
	onToggleOrderItemExclusion: ToggleOrderItemExclusion;
};

export function OrderDetailsItemAction({ item, isPending, onToggleOrderItemExclusion }: OrderDetailsItemActionProps) {
	return (
		<Button
			disabled={isPending}
			size="sm"
			type="button"
			variant={item.isExcluded ? "secondary" : "destructive"}
			onClick={() => onToggleOrderItemExclusion(item.itemNumber, !item.isExcluded)}
		>
			{item.isExcluded ? <ArrowClockwiseIcon size={16} /> : <XIcon size={16} />}
			{item.isExcluded ? "Przywróć" : "Wyklucz"}
		</Button>
	);
}