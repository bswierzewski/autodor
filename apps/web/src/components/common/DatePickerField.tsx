import { CalendarBlankIcon } from "@phosphor-icons/react";
import dayjs from "dayjs";
import { useId, useState } from "react";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { cn } from "@/lib/utils";

type DatePickerFieldProps = {
	disabled?: (date: Date) => boolean;
	displayFormat?: string;
	id?: string;
	onChange: (date: Date | undefined) => void;
	placeholder?: string;
	value?: Date;
	className?: string;
};

function DatePickerField({
	disabled,
	displayFormat = "DD.MM.YYYY",
	id,
	onChange,
	placeholder = "Wybierz datę",
	value,
	className,
}: DatePickerFieldProps) {
	const generatedId = useId();
	const buttonId = id ?? generatedId;
	const [open, setOpen] = useState(false);

	return (
		<Popover open={open} onOpenChange={setOpen}>
			<PopoverTrigger asChild>
				<Button
					className={cn("h-11 justify-between rounded-2xl px-4 font-normal", className)}
					id={buttonId}
					type="button"
					variant="outline"
				>
					<span>{value ? dayjs(value).format(displayFormat) : placeholder}</span>
					<CalendarBlankIcon className="text-muted-foreground" size={18} />
				</Button>
			</PopoverTrigger>
			<PopoverContent align="start" className="w-auto p-0">
				<Calendar
					autoFocus
					mode="single"
					selected={value}
					onSelect={(date) => {
						onChange(date ? dayjs(date).startOf("day").toDate() : undefined);
						setOpen(false);
					}}
					disabled={disabled}
				/>
			</PopoverContent>
		</Popover>
	);
}

export { DatePickerField };
