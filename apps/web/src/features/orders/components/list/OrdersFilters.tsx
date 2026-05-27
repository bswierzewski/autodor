import { MagnifyingGlassIcon, XIcon } from "@phosphor-icons/react";
import dayjs from "dayjs";
import type { ReactNode } from "react";
import { DatePickerField } from "#/components/common/DatePickerField";
import { Label } from "#/components/ui/label";

type OrdersFiltersProps = {
	query: string;
	fromDate: Date;
	toDate: Date;
	onQueryChange: (value: string) => void;
	onClearQuery: () => void;
	onFromDateChange: (date: Date) => void;
	onToDateChange: (date: Date) => void;
	actions?: ReactNode;
};

export function OrdersFilters({
	query,
	fromDate,
	toDate,
	onQueryChange,
	onClearQuery,
	onFromDateChange,
	onToDateChange,
	actions,
}: OrdersFiltersProps) {
	return (
		<>
			<div className="flex flex-col gap-4">
				<label className="relative block w-full">
					<MagnifyingGlassIcon
						className="pointer-events-none absolute top-1/2 left-4 -translate-y-1/2 text-muted-foreground"
						size={18}
					/>
					<input
						className="h-11 w-full rounded-2xl border border-border bg-background pl-11 pr-12 text-sm outline-none transition focus:border-foreground/30"
						placeholder="Szukaj po numerze, nr klienta lub osobie"
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
							<span className="sr-only">Wyczyść wyszukiwanie zamówień</span>
						</button>
					) : null}
				</label>
			</div>

			<div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
				<div className="grid gap-4 sm:grid-cols-2 lg:flex-1">
					<div className="grid gap-2">
						<Label htmlFor="orders-date-from">Od</Label>
						<DatePickerField
							id="orders-date-from"
							value={fromDate}
							onChange={(date) => {
								if (!date) {
									return;
								}

								onFromDateChange(date);
							}}
							disabled={(date) => dayjs(date).isAfter(toDate, "day")}
						/>
					</div>
					<div className="grid gap-2">
						<Label htmlFor="orders-date-to">Do</Label>
						<DatePickerField
							id="orders-date-to"
							value={toDate}
							onChange={(date) => {
								if (!date) {
									return;
								}

								onToDateChange(date);
							}}
							disabled={(date) => dayjs(date).isBefore(fromDate, "day")}
						/>
					</div>
				</div>
				{actions ? <div className="flex justify-end lg:shrink-0">{actions}</div> : null}
			</div>
		</>
	);
}
