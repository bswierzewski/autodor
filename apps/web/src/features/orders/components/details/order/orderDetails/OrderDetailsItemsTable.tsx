import type { OrderItemResponse } from "#/api/models/orderItemResponse";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
import { formatCurrency } from "#/lib/formatters";
import { OrderDetailsItemAction } from "./OrderDetailsItemAction";

type ToggleOrderItemExclusion = (itemNumber: string, excluded: boolean) => void;

type OrderDetailsItemsTableProps = {
	items: OrderItemResponse[];
	isPending: boolean;
	onToggleOrderItemExclusion: ToggleOrderItemExclusion;
};

export function OrderDetailsItemsTable({ items, isPending, onToggleOrderItemExclusion }: OrderDetailsItemsTableProps) {
	return (
		<section className="space-y-3">
			<div>
				<h2 className="text-lg font-semibold tracking-tight">Pozycje zamówienia</h2>
				<p className="text-sm text-muted-foreground">Lista pozycji z możliwością wykluczenia pojedynczej pozycji z fakturowania.</p>
			</div>

			<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
				<Table>
					<TableHeader>
						<TableRow>
							<TableHead>Nr pozycji</TableHead>
							<TableHead>Produkt</TableHead>
							<TableHead className="text-right">Ilość</TableHead>
							<TableHead className="text-right">Cena</TableHead>
							<TableHead className="text-right">Wartość</TableHead>
							<TableHead className="w-32 pr-5 text-right">Akcje</TableHead>
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
									<TableCell className="pl-5 font-medium">{item.itemNumber}</TableCell>
									<TableCell className="text-muted-foreground">{item.productDisplayName}</TableCell>
									<TableCell className="text-right">{item.quantity}</TableCell>
									<TableCell className="text-right">{formatCurrency(Number(item.price))}</TableCell>
									<TableCell className="text-right font-medium">{formatCurrency(lineTotal)}</TableCell>
									<TableCell className="pr-5 text-right">
										<OrderDetailsItemAction
											isPending={isPending}
											item={item}
											onToggleOrderItemExclusion={onToggleOrderItemExclusion}
										/>
									</TableCell>
								</TableRow>
							);
						})}
					</TableBody>
				</Table>
			</div>
		</section>
	);
}