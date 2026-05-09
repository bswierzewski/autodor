import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { useGetOrders } from "#/api/orders/orders";

export const Route = createFileRoute("/_protected/")({
	component: HomePage,
});

function formatDate(date: Date): string {
	return date.toISOString().split("T")[0];
}

function HomePage() {
	const today = new Date();
	const weekAgo = new Date(today);
	weekAgo.setDate(today.getDate() - 7);

	const [from, setFrom] = useState(formatDate(weekAgo));
	const [to, setTo] = useState(formatDate(today));

	const { data, isLoading, isError } = useGetOrders({ from, to });

	const orders = data?.orders ?? [];

	return (
		<div className="p-6 space-y-6">
			<h1 className="text-2xl font-semibold">Zamówienia</h1>

			<div className="flex flex-wrap gap-4">
				<div className="flex flex-col gap-1.5">
					<label htmlFor="from" className="text-sm font-medium">
						Data od
					</label>
					<input
						id="from"
						type="date"
						value={from}
						max={to}
						onChange={(e) => setFrom(e.target.value)}
						className="rounded-md border border-input bg-background px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-2 focus:ring-ring"
					/>
				</div>
				<div className="flex flex-col gap-1.5">
					<label htmlFor="to" className="text-sm font-medium">
						Data do
					</label>
					<input
						id="to"
						type="date"
						value={to}
						min={from}
						onChange={(e) => setTo(e.target.value)}
						className="rounded-md border border-input bg-background px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-2 focus:ring-ring"
					/>
				</div>
			</div>

			{isError && <p className="text-sm text-destructive">Nie udało się pobrać zamówień.</p>}

			<div className="rounded-md border overflow-x-auto">
				<table className="w-full text-sm">
					<thead>
						<tr className="border-b bg-muted/50 text-muted-foreground">
							<th className="px-4 py-3 text-left font-medium">Numer</th>
							<th className="px-4 py-3 text-left font-medium">Data</th>
							<th className="px-4 py-3 text-left font-medium">Osoba</th>
							<th className="px-4 py-3 text-left font-medium">Nr klienta</th>
							<th className="px-4 py-3 text-right font-medium">Pozycje</th>
							<th className="px-4 py-3 text-right font-medium">Wartość</th>
							<th className="px-4 py-3 text-center font-medium">Wykluczone</th>
						</tr>
					</thead>
					<tbody>
						{isLoading ? (
							<tr>
								<td colSpan={7} className="px-4 py-8 text-center text-muted-foreground">
									Ładowanie...
								</td>
							</tr>
						) : orders.length === 0 ? (
							<tr>
								<td colSpan={7} className="px-4 py-8 text-center text-muted-foreground">
									Brak zamówień w wybranym zakresie dat.
								</td>
							</tr>
						) : (
							orders.map((order) => (
								<tr key={order.id} className="border-b last:border-0 hover:bg-muted/30 transition-colors">
									<td className="px-4 py-3">{order.number ?? "—"}</td>
									<td className="px-4 py-3">{order.date}</td>
									<td className="px-4 py-3">{order.person ?? "—"}</td>
									<td className="px-4 py-3">{order.customerNumber ?? "—"}</td>
									<td className="px-4 py-3 text-right">{String(order.itemsCount)}</td>
									<td className="px-4 py-3 text-right">{String(order.totalAmount)}</td>
									<td className="px-4 py-3 text-center">
										{order.isExcluded ? (
											<span className="text-destructive">Tak</span>
										) : (
											<span className="text-muted-foreground">Nie</span>
										)}
									</td>
								</tr>
							))
						)}
					</tbody>
				</table>
			</div>
		</div>
	);
}
