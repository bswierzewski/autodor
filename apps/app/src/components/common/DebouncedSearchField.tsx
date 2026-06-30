import { MagnifyingGlassIcon, XIcon } from "@phosphor-icons/react";
import { useDebounce } from "@uidotdev/usehooks";
import { useEffect, useRef, useState } from "react";
import { cn } from "#/lib/utils";

type DebouncedSearchFieldProps = {
	value: string;
	onValueChange: (value: string) => void;
	onClear: () => void;
	placeholder: string;
	clearLabel: string;
	className?: string;
};

export function DebouncedSearchField({
	value,
	onValueChange,
	onClear,
	placeholder,
	clearLabel,
	className,
}: DebouncedSearchFieldProps) {
	const [localValue, setLocalValue] = useState(value);
	const [synchronizedValue, setSynchronizedValue] = useState(value);
	const debouncedValue = useDebounce(localValue, 300);
	const onValueChangeRef = useRef(onValueChange);

	if (value !== synchronizedValue) {
		setSynchronizedValue(value);
		setLocalValue(value);
	}

	useEffect(() => {
		onValueChangeRef.current = onValueChange;
	}, [onValueChange]);

	useEffect(() => {
		if (debouncedValue !== localValue || debouncedValue === value) {
			return;
		}

		onValueChangeRef.current(debouncedValue);
	}, [debouncedValue, localValue, value]);

	return (
		<label className={cn("relative block w-full", className)}>
			<MagnifyingGlassIcon
				className="pointer-events-none absolute top-1/2 left-4 -translate-y-1/2 text-muted-foreground"
				size={18}
			/>
			<input
				className="h-11 w-full rounded-2xl border border-border bg-background pl-11 pr-12 text-sm outline-none transition focus:border-foreground/30"
				placeholder={placeholder}
				type="text"
				value={localValue}
				onChange={(event) => setLocalValue(event.target.value)}
			/>
			{localValue ? (
				<button
					className="absolute top-1/2 right-3 flex size-7 -translate-y-1/2 cursor-pointer items-center justify-center rounded-full text-muted-foreground transition hover:bg-muted hover:text-foreground"
					onClick={() => {
						setLocalValue("");
						onClear();
					}}
					type="button"
				>
					<XIcon size={14} />
					<span className="sr-only">{clearLabel}</span>
				</button>
			) : null}
		</label>
	);
}
