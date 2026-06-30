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
			<div className="space-y-3">
				<div>
					<h1 className="text-2xl font-semibold tracking-tight">{order.number ?? order.id}</h1>
				</div>

				<div className="grid grid-cols-2 gap-4 sm:gap-6 lg:gap-8">
					<div className="space-y-3">
						<h3 className="text-sm font-semibold uppercase tracking-[0.12em] text-muted-foreground">Zamówienie</h3>
						<dl className="grid gap-3">
							<DetailItem label="Data" value={formatDate(order.date)} />
							<DetailItem label="Kwota" value={formatCurrency(totalAmount)} />
							<DetailItem label="Pozycje" value={order.items.length} />
							<DetailItem label="Pominięte pozycje" value={excludedItemsCount} />
						</dl>
					</div>

					<div className="space-y-3">
						<h3 className="text-sm font-semibold uppercase tracking-[0.12em] text-muted-foreground">Kontrahent</h3>
						<dl className="grid gap-3">
							<DetailItem label="Nazwa" value={contractor?.name ?? contractorNip ?? "Brak NIP"} />
							<DetailItem label="NIP" value={contractor?.nip ?? contractorNip ?? "-"} />
							<DetailItem
								label="Adres"
								value={
									contractor
										? `${contractor.street}, ${contractor.zipCode} ${contractor.city}`
										: contractorQuery.isLoading
											? "Ładowanie danych kontrahenta..."
											: "-"
								}
							/>
							<DetailItem
								label="Email"
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
