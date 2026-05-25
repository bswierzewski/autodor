import dayjs from "dayjs";
import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { OrderActions } from "./OrderActions";

type ToggleOrderExclusion = (orderId: string, excluded: boolean) => void;

const currencyFormatter = new Intl.NumberFormat("pl-PL", {
	style: "currency",
	currency: "PLN",
	minimumFractionDigits: 2,
	maximumFractionDigits: 2,
});

function formatDate(date: Date | string): string {
	return dayjs(date).format("YYYY-MM-DD");
}

function formatCurrency(value: number | string): string {
	return currencyFormatter.format(Number(value));
}

type OrderCardListProps = {
	orders: OrderSummaryResponse[];
	isPending: boolean;
	onToggleOrderExclusion: ToggleOrderExclusion;
};

export function OrderCardList({ orders, isPending, onToggleOrderExclusion }: OrderCardListProps) {
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