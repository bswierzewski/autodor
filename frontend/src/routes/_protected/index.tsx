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

	return (
		<div>
			<div>
				<input
					id="from"
					type="date"
					value={from}
					max={to}
					onChange={(e) => setFrom(e.target.value)}
				/>
				<input
					id="to"
					type="date"
					value={to}
					min={from}
					onChange={(e) => setTo(e.target.value)}
				/>
				<pre>
					{isLoading
						? "Loading..."
						: isError
							? "Error"
							: JSON.stringify(data, null, 2)}
				</pre>
			</div>
		</div>
	);
}
