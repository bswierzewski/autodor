import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { formatCurrency, formatDate } from "#/lib/formatters";
import { OrderActions } from "./OrderActions";

type ToggleOrderExclusion = (orderId: string, excluded: boolean) => void;
type PrintOrderPdf = (orderId: string, date: string) => void;

type OrderCardListProps = {
	orders: OrderSummaryResponse[];
	isPending: boolean;
	onPrintOrderPdf: PrintOrderPdf;
	onToggleOrderExclusion: ToggleOrderExclusion;
};

export function OrderCardList({ orders, isPending, onPrintOrderPdf, onToggleOrderExclusion }: OrderCardListProps) {
	return (
		<div className="grid gap-4">
			{orders.map((order) => (
				<div
					className={[
						"rounded-3xl border bg-card p-5 shadow-sm transition",
						order.isExcluded ? "border-destructive/30 bg-destructive/5" : "",
					]
						.filter(Boolean)
						.join(" ")}
					key={order.id}
				>
					<div className="flex items-start justify-between gap-3">
						<div className="min-w-0">
							<p className="truncate text-base font-semibold tracking-tight">{order.number ?? order.id}</p>
							<p className="text-sm text-muted-foreground">{formatDate(order.date)}</p>
						</div>
						<div>
							<OrderActions
								isPending={isPending}
								order={order}
								onPrintOrderPdf={onPrintOrderPdf}
								onToggleOrderExclusion={onToggleOrderExclusion}
							/>
						</div>
					</div>

					<div className="mt-4 grid gap-3 text-sm sm:grid-cols-2">
						<div>
							<p className="text-muted-foreground">Osoba</p>
							<p className="font-medium">{order.person ?? "-"}</p>
						</div>
						<div>
							<p className="text-muted-foreground">Nr klienta</p>
							<p className="font-medium">{order.customerNumber ?? "-"}</p>
						</div>
						<div>
							<p className="text-muted-foreground">Pozycje</p>
							<p className="font-medium">{order.itemsCount}</p>
						</div>
						<div>
							<p className="text-muted-foreground">Kwota</p>
							<p className="font-medium">{formatCurrency(order.totalAmount)}</p>
						</div>
					</div>
				</div>
			))}
		</div>
	);
}