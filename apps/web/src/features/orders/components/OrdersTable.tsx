import dayjs from "dayjs";
import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
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

type OrdersTableProps = {
	orders: OrderSummaryResponse[];
	isPending: boolean;
	onToggleOrderExclusion: ToggleOrderExclusion;
};

export function OrdersTable({ orders, isPending, onToggleOrderExclusion }: OrdersTableProps) {
	return (
		<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
			<Table>
				<TableHeader>
					<TableRow>
						<TableHead>Numer</TableHead>
						<TableHead>Data</TableHead>
						<TableHead>Osoba</TableHead>
						<TableHead>Nr klienta</TableHead>
						<TableHead className="text-right">Pozycje</TableHead>
						<TableHead className="text-right">Kwota</TableHead>
						<TableHead className="w-14 pr-5 text-right">Akcje</TableHead>
					</TableRow>
				</TableHeader>
				<TableBody>
					{orders.map((order) => (
						<TableRow
							className={[
								"cursor-pointer",
								order.isExcluded ? "bg-destructive/5 hover:bg-destructive/10" : "",
							]
								.filter(Boolean)
								.join(" ")}
							key={order.id}
						>
							<TableCell className="pl-5 font-medium">{order.number ?? "-"}</TableCell>
							<TableCell className="text-muted-foreground">{formatDate(order.date)}</TableCell>
							<TableCell className="text-muted-foreground">{order.person ?? "-"}</TableCell>
							<TableCell className="text-muted-foreground">{order.customerNumber ?? "-"}</TableCell>
							<TableCell className="text-right">{order.itemsCount}</TableCell>
							<TableCell className="text-right font-medium">{formatCurrency(order.totalAmount)}</TableCell>
							<TableCell className="pr-5 text-right" onClick={(event) => event.stopPropagation()}>
								<OrderActions
									isPending={isPending}
									order={order}
									onToggleOrderExclusion={onToggleOrderExclusion}
								/>
							</TableCell>
						</TableRow>
					))}
				</TableBody>
			</Table>
		</div>
	);
}