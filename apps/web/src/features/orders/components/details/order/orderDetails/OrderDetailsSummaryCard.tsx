import { useGetContractors } from "#/api/contractors/contractors";
import type { GetOrderResponse } from "#/api/models/getOrderResponse";
import { formatCurrency, formatDate } from "#/lib/formatters";

type OrderDetailsSummaryCardProps = {
	order: GetOrderResponse;
};

export function OrderDetailsSummaryCard({ order }: OrderDetailsSummaryCardProps) {
	const totalAmount = order.items.reduce((sum, item) => sum + Number(item.quantity) * Number(item.price), 0);
	const excludedItemsCount = order.items.filter((item) => item.isExcluded).length;
	const contractorNip = order.customerNumber?.trim() || undefined;
	const contractorQuery = useGetContractors(
		contractorNip ? { nips: [contractorNip] } : undefined,
		{
			query: {
				enabled: Boolean(contractorNip),
			},
		},
	);
	const contractor = contractorQuery.data?.[0];

	return (
		<section className="grid gap-4 xl:grid-cols-[minmax(0,1.4fr)_minmax(0,1fr)]">
			<div className="rounded-3xl border bg-card p-6 shadow-sm">
				<div className="space-y-4">
					<div>
						<p className="text-sm text-muted-foreground">Zamówienie</p>
						<h1 className="text-2xl font-semibold tracking-tight">{order.number ?? order.id}</h1>
					</div>

					<div className="grid gap-4 sm:grid-cols-2">
						<div className="rounded-2xl border bg-muted/20 p-4">
							<p className="text-sm text-muted-foreground">Data</p>
							<p className="mt-1 font-medium">{formatDate(order.date)}</p>
						</div>
						<div className="rounded-2xl border bg-muted/20 p-4">
							<p className="text-sm text-muted-foreground">Pozycje</p>
							<p className="mt-1 font-medium">{order.items.length}</p>
						</div>
						<div className="rounded-2xl border bg-muted/20 p-4">
							<p className="text-sm text-muted-foreground">Kwota</p>
							<p className="mt-1 font-medium">{formatCurrency(totalAmount)}</p>
						</div>
						<div className="rounded-2xl border bg-muted/20 p-4">
							<p className="text-sm text-muted-foreground">Wykluczone pozycje</p>
							<p className="mt-1 font-medium">{excludedItemsCount}</p>
						</div>
					</div>
				</div>
			</div>

			<div className="rounded-3xl border bg-card p-6 shadow-sm">
				<div className="space-y-4">
					<div>
						<p className="text-sm text-muted-foreground">Kontrahent</p>
						<h2 className="text-2xl font-semibold tracking-tight">
							{contractor?.name ?? contractorNip ?? "Brak numeru klienta"}
						</h2>
					</div>

					<div className="grid gap-4">
						<div className="rounded-2xl border bg-muted/20 p-4">
							<p className="text-sm text-muted-foreground">Nr klienta</p>
							<p className="mt-1 font-medium">{contractor?.nip ?? contractorNip ?? "-"}</p>
						</div>
						<div className="rounded-2xl border bg-muted/20 p-4">
							<p className="text-sm text-muted-foreground">Osoba kontaktowa</p>
							<p className="mt-1 font-medium">{order.person ?? "-"}</p>
						</div>
						<div className="rounded-2xl border bg-muted/20 p-4">
							<p className="text-sm text-muted-foreground">Adres</p>
							<p className="mt-1 font-medium">
								{contractor
									? `${contractor.street}, ${contractor.zipCode} ${contractor.city}`
									: contractorQuery.isLoading
										? "Ładowanie danych kontrahenta..."
										: "-"}
							</p>
						</div>
						<div className="rounded-2xl border bg-muted/20 p-4">
							<p className="text-sm text-muted-foreground">Email</p>
							<p className="mt-1 font-medium">{contractor?.email ?? (contractorQuery.isLoading ? "Ładowanie danych kontrahenta..." : "-")}</p>
						</div>
					</div>

					{contractorQuery.isError ? (
						<p className="text-sm text-destructive">Nie udało się pobrać danych kontrahenta.</p>
					) : null}
				</div>
			</div>
		</section>
	);
}