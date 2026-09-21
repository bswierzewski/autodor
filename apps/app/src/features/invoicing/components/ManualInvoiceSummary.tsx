import { ReceiptIcon } from "@phosphor-icons/react";
import { Button } from "#/components/ui/button";
import { formatCurrency } from "#/lib/formatters";
import type { ManualInvoiceCsvItem } from "../lib/manualInvoiceCsvImporter";

type ManualInvoiceSummaryProps = {
	items: ManualInvoiceCsvItem[];
	onCreateInvoice: () => void;
};

export function ManualInvoiceSummary({ items, onCreateInvoice }: ManualInvoiceSummaryProps) {
	const totalQuantity = items.reduce((total, item) => total + Number(item.quantity), 0);
	const totalNet = items.reduce((total, item) => total + Number(item.quantity) * Number(item.netUnitPrice), 0);
	const totalVat = totalNet * 0.23;
	const totalGross = totalNet + totalVat;

	return (
		<section className="ml-auto w-full max-w-sm rounded-2xl border bg-card p-4 shadow-sm">
			<h2 className="mb-3 text-sm font-semibold">Podsumowanie</h2>

			<dl className="space-y-2 text-sm">
				<SummaryRow label="Liczba pozycji" value={String(items.length)} />
				<SummaryRow label="Łączna ilość" value={totalQuantity.toLocaleString("pl-PL")} />
				<SummaryRow label="Wartość netto" value={formatCurrency(totalNet)} />
				<SummaryRow label="VAT 23%" value={formatCurrency(totalVat)} />
				<div className="mt-3 flex items-center justify-between border-t pt-3">
					<dt className="font-semibold">Do zapłaty</dt>
					<dd className="text-lg font-bold tabular-nums">{formatCurrency(totalGross)}</dd>
				</div>
			</dl>

			<Button className="mt-4 h-10 w-full" disabled={items.length === 0} type="button" onClick={onCreateInvoice}>
				<ReceiptIcon size={16} />
				Wystaw fakturę
			</Button>
		</section>
	);
}

type SummaryRowProps = {
	label: string;
	value: string;
};

function SummaryRow({ label, value }: SummaryRowProps) {
	return (
		<div className="flex items-center justify-between gap-6">
			<dt className="text-muted-foreground">{label}</dt>
			<dd className="font-medium tabular-nums">{value}</dd>
		</div>
	);
}
