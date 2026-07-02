import { CaretLeftIcon, CaretRightIcon } from "@phosphor-icons/react";
import type * as React from "react";
import { type ChevronProps, DayPicker } from "react-day-picker";

import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";

function Calendar({ className, classNames, showOutsideDays = true, ...props }: React.ComponentProps<typeof DayPicker>) {
	return (
		<DayPicker
			showOutsideDays={showOutsideDays}
			className={cn("p-3", className)}
			classNames={{
				root: "w-full",
				months: "flex flex-col",
				month_caption: "relative mb-3 flex items-center justify-center",
				caption_label: "text-sm font-medium",
				nav: "flex items-center gap-1",
				button_previous: cn(
					buttonVariants({ variant: "ghost", size: "icon-sm" }),
					"absolute left-0 size-7 bg-transparent p-0 opacity-80 hover:opacity-100",
				),
				button_next: cn(
					buttonVariants({ variant: "ghost", size: "icon-sm" }),
					"absolute right-0 size-7 bg-transparent p-0 opacity-80 hover:opacity-100",
				),
				chevron: "size-4",
				month_grid: "w-full border-collapse",
				weekday: "h-9 text-[0.8rem] font-normal text-muted-foreground",
				day: "p-0 text-center text-sm",
				day_button: cn(buttonVariants({ variant: "ghost" }), "size-9 rounded-md p-0 font-normal"),
				today: "[&>button]:border [&>button]:border-border",
				selected:
					"rounded-md bg-primary text-primary-foreground [&>button]:bg-primary [&>button]:text-primary-foreground [&>button]:hover:bg-primary [&>button]:hover:text-primary-foreground",
				outside: "text-muted-foreground opacity-50",
				disabled: "text-muted-foreground opacity-40",
				hidden: "invisible",
				range_start: "rounded-l-md bg-muted [&>button]:bg-primary [&>button]:text-primary-foreground",
				range_middle: "bg-muted [&>button]:bg-muted [&>button]:text-foreground",
				range_end: "rounded-r-md bg-muted [&>button]:bg-primary [&>button]:text-primary-foreground",
				...classNames,
			}}
			components={{
				Chevron: ({ className: chevronClassName, orientation, ...chevronProps }: ChevronProps) =>
					orientation === "left" ? (
						<CaretLeftIcon className={cn("size-4", chevronClassName)} {...chevronProps} />
					) : (
						<CaretRightIcon className={cn("size-4", chevronClassName)} {...chevronProps} />
					),
			}}
			{...props}
		/>
	);
}

export { Calendar };
