import { Empty, EmptyDescription, EmptyHeader, EmptyTitle } from "#/components/ui/empty";

export function OrdersEmptyState() {
	return (
		<div className="rounded-3xl border border-dashed border-border bg-card p-6 shadow-sm">
			<Empty>
				<EmptyHeader>
					<EmptyTitle>Brak zamówień w wybranym zakresie</EmptyTitle>
					<EmptyDescription>Wybierz inny przedział dat, aby zobaczyć wyniki.</EmptyDescription>
				</EmptyHeader>
			</Empty>
		</div>
	);
}
