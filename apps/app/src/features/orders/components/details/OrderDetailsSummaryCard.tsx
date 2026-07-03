import { useGetContractors } from "#/api/contractors/contractors";
import type { GetOrderResponse } from "#/api/models/getOrderResponse";
import { formatCurrency, formatDate } from "#/lib/formatters";

type OrderDetailsSummaryCardProps = {
	order: GetOrderResponse;
};

type DetailItemProps = {
	label: string;
	value: string | number;
};

function DetailItem({ label, value }: DetailItemProps) {
	return (
		<div className="grid gap-1.5">
			<dt className="text-xs font-medium uppercase tracking-[0.12em] text-muted-foreground">{label}</dt>
			<dd className="min-w-0 wrap-break-word text-sm font-medium leading-snug text-foreground">{value}</dd>
		</div>
	);
}

export function OrderDetailsSummaryCard({ order }: OrderDetailsSummaryCardProps) {
	const contractorNip = order.customerNumber?.trim() || undefined;
	const contractorQuery = useGetContractors(contractorNip ? { nips: [contractorNip] } : undefined, {
		query: {
			enabled: Boolean(contractorNip),
		},
	});
	const contractor = contractorQuery.data?.[0];

	return (
		<section className="rounded-3xl border bg-card p-6 shadow-sm">
			<dl className="grid grid-cols-2 gap-4 sm:grid-cols-3 sm:gap-6 lg:grid-cols-7">
				<DetailItem label="Numer" value={order.number ?? order.id} />
				<DetailItem label="Data" value={formatDate(order.date)} />
				<DetailItem label="NIP" value={contractor?.nip ?? contractorNip ?? "-"} />
				<DetailItem
					label="Kontrahent"
					value={contractor?.name ?? order.person ?? (contractorQuery.isLoading ? "Ładowanie..." : "-")}
				/>
				<DetailItem label="Pozycje" value={order.items.length} />
				<DetailItem label="Netto" value={formatCurrency(order.netAmount)} />
				<DetailItem label="Brutto" value={formatCurrency(order.grossAmount)} />
			</dl>

			{contractorQuery.isError ? (
				<p className="mt-4 text-sm text-destructive">Nie udało się pobrać danych kontrahenta.</p>
			) : null}
		</section>
	);
}
