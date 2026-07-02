import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { Checkbox } from "#/components/ui/checkbox";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
import { formatOrderItemsCount } from "#/features/orders/lib/ordersFormatters";
import { formatCurrency, formatDate } from "#/lib/formatters";
import { cn } from "#/lib/utils";
import { OrderActions } from "./OrderActions";

type OrdersTableProps = {
	orders: OrderSummaryResponse[];
	selectedOrderIds: ReadonlySet<string>;
	onToggleSelect: (id: string) => void;
	onToggleSelectAll: (checked: boolean) => void;
};

export function OrdersTable({ orders, selectedOrderIds, onToggleSelect, onToggleSelectAll }: OrdersTableProps) {
	const selectedCount = orders.filter((o) => selectedOrderIds.has(o.id)).length;
	const allSelected = selectedCount === orders.length && orders.length > 0;
	const someSelected = selectedCount > 0 && !allSelected;

	return (
		<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
			<Table>
				<TableHeader>
					<TableRow>
						<TableHead className="w-12 pl-5">
							<Checkbox
								aria-label="Zaznacz wszystkie"
								checked={allSelected ? true : someSelected ? "indeterminate" : false}
								onCheckedChange={(checked) => onToggleSelectAll(!!checked)}
							/>
						</TableHead>
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
							className={cn("cursor-pointer", order.isExcluded && "bg-destructive/5 hover:bg-destructive/10")}
							key={order.id}
							onClick={() => onToggleSelect(order.id)}
						>
							<TableCell className="pl-5" onClick={(e) => e.stopPropagation()}>
								<Checkbox
									aria-label={`Zaznacz zamówienie ${order.number ?? order.id}`}
									checked={selectedOrderIds.has(order.id)}
									onCheckedChange={() => onToggleSelect(order.id)}
								/>
							</TableCell>
							<TableCell className="font-medium">{order.number ?? "-"}</TableCell>
							<TableCell className="text-muted-foreground">{formatDate(order.date)}</TableCell>
							<TableCell className="text-muted-foreground">{order.person ?? "-"}</TableCell>
							<TableCell className="text-muted-foreground">{order.customerNumber ?? "-"}</TableCell>
							<TableCell className="text-right">{formatOrderItemsCount(order)}</TableCell>
							<TableCell className="text-right font-medium">{formatCurrency(order.totalAmount)}</TableCell>
							<TableCell className="pr-5 text-right" onClick={(e) => e.stopPropagation()}>
								<OrderActions order={order} />
							</TableCell>
						</TableRow>
					))}
				</TableBody>
			</Table>
		</div>
	);
}
