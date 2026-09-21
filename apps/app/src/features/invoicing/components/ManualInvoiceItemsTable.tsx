import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
import { formatCurrency } from "#/lib/formatters";
import type { ManualInvoiceCsvItem } from "../lib/manualInvoiceCsvImporter";

type ManualInvoiceItemsTableProps = {
	items: ManualInvoiceCsvItem[];
};

export function ManualInvoiceItemsTable({ items }: ManualInvoiceItemsTableProps) {
	return (
		<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
			<Table>
				<TableHeader>
					<TableRow>
						<TableHead className="pl-5">Numer pozycji</TableHead>
						<TableHead className="text-right">Ilość</TableHead>
						<TableHead className="text-right">Cena jedn. netto</TableHead>
						<TableHead className="text-right">Wartość netto</TableHead>
						<TableHead className="pr-5 text-right">Wartość brutto</TableHead>
					</TableRow>
				</TableHeader>
				<TableBody>
					{items.length === 0 ? (
						<TableRow>
							<TableCell className="h-32 text-center text-muted-foreground" colSpan={5}>
								Zaimportuj plik CSV, aby wyświetlić pozycje faktury.
							</TableCell>
						</TableRow>
					) : (
						items.map((item) => {
							const netValue = item.quantity * item.netUnitPrice;
							const grossValue = netValue * 1.23;

							return (
								<TableRow key={item.sourceRow}>
									<TableCell className="pl-5 font-medium">{item.itemNumber}</TableCell>
									<TableCell className="text-right">{item.quantity}</TableCell>
									<TableCell className="text-right">{formatCurrency(item.netUnitPrice)}</TableCell>
									<TableCell className="text-right font-medium">{formatCurrency(netValue)}</TableCell>
									<TableCell className="pr-5 text-right font-medium">{formatCurrency(grossValue)}</TableCell>
								</TableRow>
							);
						})
					)}
				</TableBody>
			</Table>
		</div>
	);
}
