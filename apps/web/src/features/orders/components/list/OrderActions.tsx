import { ArrowClockwiseIcon, DotsThreeOutlineVerticalIcon, EyeIcon, PrinterIcon, XIcon } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";
import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { Button } from "#/components/ui/button";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "#/components/ui/dropdown-menu";

type ToggleOrderExclusion = (orderId: string, excluded: boolean) => void;
type PrintOrderPdf = (orderId: string, date: string) => void;

type OrderActionsProps = {
	order: OrderSummaryResponse;
	isPending: boolean;
	onPrintOrderPdf: PrintOrderPdf;
	onToggleOrderExclusion: ToggleOrderExclusion;
};

export function OrderActions({ order, isPending, onPrintOrderPdf, onToggleOrderExclusion }: OrderActionsProps) {
	return (
		<DropdownMenu>
			<DropdownMenuTrigger asChild>
				<Button className="ml-auto" size="icon-sm" type="button" variant="ghost">
					<DotsThreeOutlineVerticalIcon size={16} />
					<span className="sr-only">Otwórz menu akcji dla zamówienia {order.number ?? order.id}</span>
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end">
				<DropdownMenuItem asChild>
					<Link params={{ orderId: order.id }} search={{ date: order.date }} to="/orders/$orderId">
						<EyeIcon size={16} />
						Podgląd zamówienia
					</Link>
				</DropdownMenuItem>
				<DropdownMenuItem disabled={isPending} onSelect={() => onPrintOrderPdf(order.id, order.date)}>
					<PrinterIcon size={16} />
					Drukuj PDF
				</DropdownMenuItem>
				<DropdownMenuItem
					disabled={isPending}
					onSelect={() => onToggleOrderExclusion(order.id, !order.isExcluded)}
					variant={order.isExcluded ? "default" : "destructive"}
				>
					{order.isExcluded ? <ArrowClockwiseIcon size={16} /> : <XIcon size={16} />}
					{order.isExcluded ? "Przywróć" : "Wyłącz"}
				</DropdownMenuItem>
			</DropdownMenuContent>
		</DropdownMenu>
	);
}
