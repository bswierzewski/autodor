import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { Checkbox } from "#/components/ui/checkbox";
import { formatCurrency } from "#/lib/formatters";
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
							"relative rounded-3xl border bg-card p-5 shadow-sm transition",
							order.isExcluded ? "border-destructive/30 bg-destructive/5" : "",
							selectedOrderIds.has(order.id) ? "border-primary/40 bg-primary/5" : "",
						]
							.filter(Boolean)
							.join(" ")}
						key={order.id}
					>
						<label className="block cursor-pointer" htmlFor={checkboxId}>
							<div className="flex items-start gap-3 pr-12">
								<div className="flex min-w-0 flex-1 items-start gap-3">
									<div className="mt-0.5">
										<Checkbox
											aria-label={`Zaznacz zamówienie ${order.number ?? order.id}`}
											checked={selectedOrderIds.has(order.id)}
											id={checkboxId}
											onCheckedChange={() => onToggleSelect(order.id)}
										/>
									</div>
									<div className="grid min-w-0 flex-1 gap-1">
										<p className="min-w-0 truncate text-base font-semibold">{order.number ?? order.id}</p>
										<div className="grid min-w-0 w-full grid-cols-2 gap-3 text-sm">
											<div className="min-w-0">
												<p className="text-muted-foreground">NIP:</p>
												<p className="truncate font-medium">{order.customerNumber ?? "-"}</p>
											</div>
											<div className="min-w-0 text-right">
												<p className="text-muted-foreground">Kwota:</p>
												<p className="truncate font-medium">{formatCurrency(order.totalAmount)}</p>
											</div>
										</div>
									</div>
								</div>
							</div>
						</label>

						<div className="absolute top-5 right-5">
							<OrderActions
								isPending={isPending}
								order={order}
								onPrintOrderPdf={onPrintOrderPdf}
								onToggleOrderExclusion={onToggleOrderExclusion}
							/>
						</div>
					</div>
				);
			})}
		</div>
	);
}
