import { MagnifyingGlassIcon, PlusIcon, XIcon } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";
import { Button } from "#/components/ui/button";

type ContractorsFiltersProps = {
	query: string;
	onQueryChange: (value: string) => void;
	onClearQuery: () => void;
};

export function ContractorsFilters({ query, onQueryChange, onClearQuery }: ContractorsFiltersProps) {
	return (
		<div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
			<label className="relative block w-full">
				<MagnifyingGlassIcon
					className="pointer-events-none absolute top-1/2 left-4 -translate-y-1/2 text-muted-foreground"
					size={18}
				/>
				<input
					className="h-11 w-full rounded-2xl border border-border bg-background pl-11 pr-12 text-sm outline-none transition focus:border-foreground/30"
					placeholder="Szukaj po nazwie, NIP, mieście lub emailu"
					type="text"
					value={query}
					onChange={(event) => onQueryChange(event.target.value)}
				/>
				{query ? (
					<button
						className="absolute top-1/2 right-3 flex size-7 -translate-y-1/2 cursor-pointer items-center justify-center rounded-full text-muted-foreground transition hover:bg-muted hover:text-foreground"
						onClick={onClearQuery}
						type="button"
					>
						<XIcon size={14} />
						<span className="sr-only">Wyczyść wyszukiwanie</span>
					</button>
				) : null}
			</label>
			<Button asChild className="h-11 w-full rounded-2xl px-4 lg:w-auto" type="button">
				<Link to="/contractors/create">
					<PlusIcon size={16} />
					Dodaj kontrahenta
				</Link>
			</Button>
		</div>
	);
}