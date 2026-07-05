import { Button } from "#/components/ui/button";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyTitle } from "#/components/ui/empty";

type OrdersFilteredEmptyStateProps = {
	onClearFilters: () => void;
};

export function OrdersFilteredEmptyState({ onClearFilters }: OrdersFilteredEmptyStateProps) {
	return (
		<div className="rounded-3xl border border-dashed border-border bg-card p-6 shadow-sm">
			<Empty>
				<EmptyHeader>
					<EmptyTitle>Brak zamówień dla podanych filtrów</EmptyTitle>
					<EmptyDescription>Zmień frazę wyszukiwania aby zobaczyć wyniki.</EmptyDescription>
				</EmptyHeader>
				<EmptyContent>
					<Button type="button" variant="outline" onClick={onClearFilters}>
						Wyczyść filtry
					</Button>
				</EmptyContent>
			</Empty>
		</div>
	);
}
