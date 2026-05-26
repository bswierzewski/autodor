import { useGetContractors } from "#/api/contractors/contractors";
import type { GetOrderResponse } from "#/api/models/getOrderResponse";
import { formatCurrency, formatDate } from "#/lib/formatters";

type OrderDetailsSummaryCardProps = {
	order: GetOrderResponse;
};

type DetailRowProps = {
	label: string;
	value: string | number;
	multiline?: boolean;
};

function DetailRow({ label, value, multiline = false }: DetailRowProps) {
	return (
		<div className="flex flex-col gap-1 border-b border-border/60 py-3 last:border-b-0 last:pb-0 first:pt-0 sm:flex-row sm:items-start sm:justify-between sm:gap-6">
			<dt className="text-sm text-muted-foreground">{label}</dt>
			<dd className={multiline ? "text-sm font-medium sm:max-w-88 sm:text-right" : "font-medium sm:text-right"}>
				{value}
			</dd>
		</div>
	);
}

export function OrderDetailsSummaryCard({ order }: OrderDetailsSummaryCardProps) {
	const totalAmount = order.items.reduce((sum, item) => sum + Number(item.quantity) * Number(item.price), 0);
	const excludedItemsCount = order.items.filter((item) => item.isExcluded).length;
	const contractorNip = order.customerNumber?.trim() || undefined;
	const contractorQuery = useGetContractors(contractorNip ? { nips: [contractorNip] } : undefined, {
		query: {
			enabled: Boolean(contractorNip),
		},
	});
	const contractor = contractorQuery.data?.[0];

	return (
		<section className="rounded-3xl border bg-card p-6 shadow-sm">
			<div className="space-y-6">
				<div>
					<p className="text-sm text-muted-foreground">Zamówienie</p>
					<h1 className="text-2xl font-semibold tracking-tight">{order.number ?? order.id}</h1>
				</div>

				<div className="grid gap-6 lg:grid-cols-2 lg:gap-8">
					<div className="space-y-3">
						<h2 className="text-sm font-semibold uppercase tracking-[0.12em] text-muted-foreground">
							Szczegóły zamówienia
						</h2>
						<dl>
							<DetailRow label="Data" value={formatDate(order.date)} />
							<DetailRow label="Pozycje" value={order.items.length} />
							<DetailRow label="Kwota" value={formatCurrency(totalAmount)} />
							<DetailRow label="Wykluczone pozycje" value={excludedItemsCount} />
						</dl>
					</div>

					<div className="space-y-3">
						<h2 className="text-sm font-semibold uppercase tracking-[0.12em] text-muted-foreground">Kontrahent</h2>
						<dl>
							<DetailRow label="Nazwa" value={contractor?.name ?? contractorNip ?? "Brak numeru klienta"} multiline />
							<DetailRow label="Nr klienta" value={contractor?.nip ?? contractorNip ?? "-"} />
							<DetailRow
								label="Adres"
								multiline
								value={
									contractor
										? `${contractor.street}, ${contractor.zipCode} ${contractor.city}`
										: contractorQuery.isLoading
											? "Ładowanie danych kontrahenta..."
											: "-"
								}
							/>
							<DetailRow
								label="Email"
								multiline
								value={contractor?.email ?? (contractorQuery.isLoading ? "Ładowanie danych kontrahenta..." : "-")}
							/>
						</dl>

						{contractorQuery.isError ? (
							<p className="text-sm text-destructive">Nie udało się pobrać danych kontrahenta.</p>
						) : null}
					</div>
				</div>
			</div>
		</section>
	);
}
