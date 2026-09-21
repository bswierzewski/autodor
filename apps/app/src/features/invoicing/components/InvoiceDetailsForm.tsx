import { CheckIcon, XIcon } from "@phosphor-icons/react";
import { useForm } from "@tanstack/react-form";
import dayjs from "dayjs";
import * as zod from "zod";
import { DatePickerField } from "#/components/common/DatePickerField";
import { Button } from "#/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "#/components/ui/field";
import { Input } from "#/components/ui/input";
import { Spinner } from "#/components/ui/spinner";

const invoiceDetailsFormSchema = zod.object({
	invoiceNumber: zod.string(),
	issueDate: zod.date(),
	saleDate: zod.date(),
	contractorNIP: zod.string().trim().min(1, "NIP kontrahenta jest wymagany"),
});

export type InvoiceDetails = {
	invoiceNumber: number | null;
	issueDate: string;
	saleDate: string;
	contractorNIP: string;
};

type InvoiceDetailsFormProps = {
	defaultContractorNIP?: string;
	description: string;
	errorMessages?: string[];
	isPending?: boolean;
	onCancel: () => void;
	onSubmit: (details: InvoiceDetails) => Promise<void> | void;
};

export function InvoiceDetailsForm({
	defaultContractorNIP = "",
	description,
	errorMessages = [],
	isPending = false,
	onCancel,
	onSubmit,
}: InvoiceDetailsFormProps) {
	const today = dayjs().startOf("day").toDate();
	const form = useForm({
		defaultValues: {
			invoiceNumber: "",
			issueDate: today,
			saleDate: today,
			contractorNIP: defaultContractorNIP,
		},
		validators: { onSubmit: invoiceDetailsFormSchema },
		onSubmit: async ({ value }) => {
			const rawNumber = value.invoiceNumber.trim();
			await onSubmit({
				invoiceNumber: rawNumber === "" ? null : Number(rawNumber),
				issueDate: dayjs(value.issueDate).format("YYYY-MM-DD"),
				saleDate: dayjs(value.saleDate).format("YYYY-MM-DD"),
				contractorNIP: value.contractorNIP.trim(),
			});
		},
	});
	const isSubmitting = form.state.isSubmitting;

	return (
		<section>
			<form
				className="flex h-full flex-col gap-6"
				onSubmit={(event) => {
					event.preventDefault();
					event.stopPropagation();
					void form.handleSubmit();
				}}
			>
				<header className="space-y-1">
					<h2 className="text-2xl font-semibold tracking-tight">Wystaw fakturę</h2>
					<p className="text-sm text-muted-foreground">{description}</p>
				</header>

				{errorMessages.length > 0 ? (
					<div className="rounded-2xl border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
						{errorMessages.map((message) => (
							<p key={message}>{message}</p>
						))}
					</div>
				) : null}

				<FieldGroup className="grid gap-4">
					<form.Field name="invoiceNumber">
						{(field) => (
							<Field>
								<FieldLabel htmlFor={field.name}>Numer faktury</FieldLabel>
								<Input
									disabled={isPending || isSubmitting}
									id={field.name}
									inputMode="numeric"
									name={field.name}
									onBlur={field.handleBlur}
									onChange={(event) => field.handleChange(event.target.value)}
									placeholder="Pozostaw puste, aby przypisać automatycznie"
									value={field.state.value}
								/>
							</Field>
						)}
					</form.Field>

					<form.Field name="issueDate">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>Data wystawienia</FieldLabel>
									<DatePickerField
										id={field.name}
										value={field.state.value}
										onChange={(date) => {
											if (date) field.handleChange(date);
										}}
									/>
									<FieldError errors={field.state.meta.errors} />
								</Field>
							);
						}}
					</form.Field>

					<form.Field name="saleDate">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>Data sprzedaży</FieldLabel>
									<DatePickerField
										id={field.name}
										value={field.state.value}
										onChange={(date) => {
											if (date) field.handleChange(date);
										}}
									/>
									<FieldError errors={field.state.meta.errors} />
								</Field>
							);
						}}
					</form.Field>

					<form.Field name="contractorNIP">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>NIP kontrahenta</FieldLabel>
									<Input
										aria-invalid={isInvalid}
										disabled={isPending || isSubmitting}
										id={field.name}
										name={field.name}
										onBlur={field.handleBlur}
										onChange={(event) => field.handleChange(event.target.value)}
										placeholder="np. 1234567890"
										value={field.state.value}
									/>
									<FieldError errors={field.state.meta.errors} />
								</Field>
							);
						}}
					</form.Field>
				</FieldGroup>

				<footer className="mt-auto grid gap-3 border-t border-border/60 pt-5 sm:grid-cols-2">
					<Button
						className="h-11 w-full"
						type="button"
						variant="outline"
						onClick={onCancel}
						disabled={isPending || isSubmitting}
					>
						<XIcon size={16} />
						Anuluj
					</Button>
					<Button className="h-11 w-full" type="submit" disabled={isPending || isSubmitting}>
						{isPending || isSubmitting ? <Spinner className="size-4" /> : <CheckIcon size={16} />}
						{isPending || isSubmitting ? "Wystawianie..." : "Wystaw fakturę"}
					</Button>
				</footer>
			</form>
		</section>
	);
}
