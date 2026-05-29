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
			{orders.map((order) => {
				const checkboxId = `order-select-${order.id}`;

				return (
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
						<div className="grid min-w-0 gap-3">
							<div className="flex min-w-0 items-start gap-3">
								<label className="flex min-w-0 flex-1 cursor-pointer items-start gap-3" htmlFor={checkboxId}>
									<div className="mt-0.5">
										<Checkbox
											aria-label={`Zaznacz zamówienie ${order.number ?? order.id}`}
											checked={selectedOrderIds.has(order.id)}
											id={checkboxId}
											onCheckedChange={() => onToggleSelect(order.id)}
										/>
									</div>
									<div className="min-w-0 flex-1">
										<p className="min-w-0 truncate text-base font-semibold">{order.number ?? order.id}</p>
									</div>
								</label>

								<div className="shrink-0">
									<OrderActions
										isPending={isPending}
										order={order}
										onPrintOrderPdf={onPrintOrderPdf}
										onToggleOrderExclusion={onToggleOrderExclusion}
									/>
								</div>
							</div>

							<label className="block cursor-pointer" htmlFor={checkboxId}>
								<div className="grid min-w-0 gap-2 rounded-2xl bg-muted/40 text-sm">
									<div className="grid min-w-0 grid-cols-2 gap-3">
										<p className="min-w-0 truncate text-muted-foreground">
											Data: <span className="font-medium text-foreground">{formatDate(order.date)}</span>
										</p>
										<p className="min-w-0 truncate text-right text-muted-foreground">
											NIP: <span className="font-medium text-foreground">{order.customerNumber ?? "-"}</span>
										</p>
									</div>
									<div className="grid min-w-0 grid-cols-2 gap-3">
										<p className="min-w-0 truncate text-muted-foreground">
											Kwota: <span className="font-medium text-foreground">{formatCurrency(order.totalAmount)}</span>
										</p>
										<p className="min-w-0 truncate text-right text-muted-foreground">
											Pozycje: <span className="font-medium text-foreground">{order.itemsCount}</span>
										</p>
									</div>
								</div>
							</label>
						</div>
					</div>
				);
			})}
		</div>
	);
}
