import { DotsThreeOutlineVerticalIcon } from "@phosphor-icons/react";
import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { Button } from "#/components/ui/button";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "#/components/ui/dropdown-menu";

type ToggleOrderExclusion = (orderId: string, excluded: boolean) => void;

type OrderActionsProps = {
	order: OrderSummaryResponse;
	isPending: boolean;
	onToggleOrderExclusion: ToggleOrderExclusion;
};

export function OrderActions({ order, isPending, onToggleOrderExclusion }: OrderActionsProps) {
	return (
		<DropdownMenu>
			<DropdownMenuTrigger asChild>
				<Button className="ml-auto" size="icon-sm" type="button" variant="ghost">
					<DotsThreeOutlineVerticalIcon size={16} />
					<span className="sr-only">Otwórz menu akcji dla zamówienia {order.number ?? order.id}</span>
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end">
				<DropdownMenuItem
					disabled={isPending}
					onSelect={() => onToggleOrderExclusion(order.id, !order.isExcluded)}
					variant={order.isExcluded ? "default" : "destructive"}
				>
					{order.isExcluded ? "Przywróć do fakturowania" : "Wyłącz z fakturowania"}
				</DropdownMenuItem>
			</DropdownMenuContent>
		</DropdownMenu>
	);
}