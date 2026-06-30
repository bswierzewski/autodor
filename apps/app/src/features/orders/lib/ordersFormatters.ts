import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";

export function formatOrderItemsCount(order: OrderSummaryResponse) {
	const itemsCount = Number(order.itemsCount);
	const includedItemsCount = Math.max(0, itemsCount - Number(order.excludedItemsCount));

	return `${includedItemsCount}/${itemsCount}`;
}
