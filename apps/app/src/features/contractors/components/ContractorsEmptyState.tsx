import { PlusIcon } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";
import { Button } from "#/components/ui/button";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyTitle } from "#/components/ui/empty";

type ContractorsEmptyStateProps = {
	hasContractors: boolean;
	onClearFilters: () => void;
};

export function ContractorsEmptyState({ hasContractors, onClearFilters }: ContractorsEmptyStateProps) {
	return (
		<div className="rounded-3xl border border-dashed border-border bg-card p-6 shadow-sm">
			<Empty>
				<EmptyHeader>
					<EmptyTitle>{hasContractors ? "Brak kontrahentów dla podanych filtrów" : "Brak kontrahentów"}</EmptyTitle>
					<EmptyDescription>
						{hasContractors
							? "Zmień frazę wyszukiwania aby zobaczyć wyniki."
							: "Dodaj pierwszego kontrahenta, aby rozpocząć pracę z bazą klientów."}
					</EmptyDescription>
				</EmptyHeader>
				<EmptyContent>
					{hasContractors ? (
						<Button type="button" variant="outline" onClick={onClearFilters}>
							Wyczyść filtry
						</Button>
					) : (
						<Button asChild type="button">
							<Link
								onClick={(event) => {
									event.currentTarget.blur();
								}}
								search={true}
								to="/contractors/create"
							>
								<PlusIcon size={16} />
								Dodaj kontrahenta
							</Link>
						</Button>
					)}
				</EmptyContent>
			</Empty>
		</div>
	);
}
