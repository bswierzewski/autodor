import { PlusIcon } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";
import { DebouncedSearchField } from "#/components/common/DebouncedSearchField";
import { Button } from "#/components/ui/button";

type ContractorsFiltersProps = {
	query: string;
	onQueryChange: (value: string) => void;
	onClearQuery: () => void;
};

export function ContractorsFilters({ query, onQueryChange, onClearQuery }: ContractorsFiltersProps) {
	return (
		<div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
			<DebouncedSearchField
				clearLabel="Wyczyść wyszukiwanie"
				onClear={onClearQuery}
				onValueChange={onQueryChange}
				placeholder="Szukaj po nazwie, NIP, mieście lub emailu"
				value={query}
			/>
			<Button asChild className="h-11 w-full rounded-2xl px-4 lg:w-56" type="button">
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
		</div>
	);
}
