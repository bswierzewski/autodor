import { Link } from "@tanstack/react-router";
import { useState } from "react";
import { useGetOrders } from "#/api/orders/orders";

function formatDate(date: Date): string {
	return date.toISOString().split("T")[0];
}

export function OrdersPage() {
	const today = new Date();
	const weekAgo = new Date(today);
	weekAgo.setDate(today.getDate() - 7);

	const [from, setFrom] = useState(formatDate(weekAgo));
	const [to, setTo] = useState(formatDate(today));

	const { data, isLoading, isError } = useGetOrders({ from, to });

	return (
		<div>
			<div className="flex items-center gap-4">
				<h1 className="text-2xl font-semibold">Autodor</h1>
				<Link to="/errors">Go to errors</Link>
			</div>
			<div className="space-y-4">
				<input id="from" type="date" value={from} max={to} onChange={(e) => setFrom(e.target.value)} />
				<input id="to" type="date" value={to} min={from} onChange={(e) => setTo(e.target.value)} />
				<pre className="overflow-auto text-sm">
					{isLoading ? "Loading..." : isError ? "Error" : JSON.stringify(data, null, 2)}
				</pre>
			</div>
		</div>
	);
}
