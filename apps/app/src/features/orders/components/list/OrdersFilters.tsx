import dayjs from "dayjs";
import type { ReactNode } from "react";
import { DatePickerField } from "#/components/common/DatePickerField";
import { DebouncedSearchField } from "#/components/common/DebouncedSearchField";
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
			<div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
				<div className="grid grid-cols-2 gap-4 lg:flex-1">
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
			</div>

			<div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
				<DebouncedSearchField
					className="lg:flex-1"
					clearLabel="Wyczyść wyszukiwanie zamówień"
					onClear={onClearQuery}
					onValueChange={onQueryChange}
					placeholder="Szukaj po numerze, nr klienta lub osobie"
					value={query}
				/>
				{actions ? <div className="flex justify-end lg:shrink-0">{actions}</div> : null}
			</div>
		</>
	);
}
