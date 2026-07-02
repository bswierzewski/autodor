import type { OrderItemResponse } from "#/api/models/orderItemResponse";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
import { formatCurrency } from "#/lib/formatters";
import { OrderDetailsItemAction } from "./OrderDetailsItemAction";

type OrderDetailsItemsTableProps = {
	orderId: string;
	orderDate: string;
	items: OrderItemResponse[];
};

export function OrderDetailsItemsTable({ orderId, orderDate, items }: OrderDetailsItemsTableProps) {
	return (
		<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
			<Table className="[&_td]:px-5 [&_th]:px-5">
				<TableHeader>
					<TableRow>
						<TableHead className="hidden sm:table-cell">Nr pozycji</TableHead>
						<TableHead>Produkt</TableHead>
						<TableHead className="hidden text-right sm:table-cell">Ilość</TableHead>
						<TableHead className="hidden text-right sm:table-cell">Cena</TableHead>
						<TableHead className="hidden text-right sm:table-cell">Wartość</TableHead>
						<TableHead className="w-16 text-right">Akcje</TableHead>
					</TableRow>
				</TableHeader>
				<TableBody>
					{items.map((item) => {
						const lineTotal = Number(item.quantity) * Number(item.price);

						return (
							<TableRow
								className={item.isExcluded ? "bg-destructive/5 hover:bg-destructive/10" : undefined}
								key={item.itemNumber}
							>
								<TableCell className="hidden font-medium sm:table-cell">{item.itemNumber}</TableCell>
								<TableCell className="text-muted-foreground">{item.productDisplayName}</TableCell>
								<TableCell className="hidden text-right sm:table-cell">{item.quantity}</TableCell>
								<TableCell className="hidden text-right sm:table-cell">{formatCurrency(Number(item.price))}</TableCell>
								<TableCell className="hidden text-right font-medium sm:table-cell">
									{formatCurrency(lineTotal)}
								</TableCell>
								<TableCell className="text-right">
									<OrderDetailsItemAction item={item} orderDate={orderDate} orderId={orderId} />
								</TableCell>
							</TableRow>
						);
					})}
				</TableBody>
			</Table>
		</div>
	);
}
