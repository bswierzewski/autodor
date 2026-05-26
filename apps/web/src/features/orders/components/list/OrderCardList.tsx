import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { Checkbox } from "#/components/ui/checkbox";
import { formatCurrency, formatDate } from "#/lib/formatters";
import { OrderActions } from "./OrderActions";

type ToggleOrderExclusion = (orderId: string, excluded: boolean) => void;
type PrintOrderPdf = (orderId: string, date: string) => void;

type OrderCardListProps = {
	orders: OrderSummaryResponse[];
	isPending: boolean;
	selectedOrderIds: ReadonlySet<string>;
	onToggleSelect: (id: string) => void;
	onPrintOrderPdf: PrintOrderPdf;
	onToggleOrderExclusion: ToggleOrderExclusion;
};

export function OrderCardList({
	orders,
	isPending,
	selectedOrderIds,
	onToggleSelect,
	onPrintOrderPdf,
	onToggleOrderExclusion,
}: OrderCardListProps) {
	return (
		<div className="grid gap-4">
			{orders.map((order) => (
				<div
					className={[
						"rounded-3xl border bg-card p-5 shadow-sm transition",
						order.isExcluded ? "border-destructive/30 bg-destructive/5" : "",
						selectedOrderIds.has(order.id) ? "border-primary/40 bg-primary/5" : "",
					]
						.filter(Boolean)
						.join(" ")}
					key={order.id}
				>
					<div className="flex items-start justify-between gap-3">
						<div className="flex min-w-0 items-start gap-3">
							<div className="mt-0.5">
								<Checkbox
									aria-label={`Zaznacz zamówienie ${order.number ?? order.id}`}
									checked={selectedOrderIds.has(order.id)}
									onCheckedChange={() => onToggleSelect(order.id)}
								/>
							</div>
							<div className="min-w-0">
								<p className="truncate text-base font-semibold tracking-tight">{order.number ?? order.id}</p>
								<p className="text-sm text-muted-foreground">{formatDate(order.date)}</p>
							</div>
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
