import { CheckIcon, XIcon } from "@phosphor-icons/react";
import { useForm } from "@tanstack/react-form";
import dayjs from "dayjs";
import { toast } from "sonner";

import { useCreateInvoice } from "#/api/invoicing/invoicing";
import { DatePickerField } from "#/components/common/DatePickerField";
import { Button } from "#/components/ui/button";
import { Field, FieldError, FieldLabel } from "#/components/ui/field";
import { Spinner } from "#/components/ui/spinner";
import { cn } from "#/lib/utils";

type OrderInvoiceFormProps = {
	contractorNip?: string;
	onClose?: () => void;
	onSuccess?: () => void;
	orderDates: string[];
	orderIds: string[];
};

type OrderInvoiceFormValues = {
	contractorNIP: string;
	invoiceNumber: string;
	issueDate: Date;
	saleDate: Date;
};

function normalizeDate(date: Date): Date {
	return dayjs(date).startOf("day").toDate();
}

function toApiDateTime(value: Date | string): string {
	return dayjs(value).startOf("day").format("YYYY-MM-DDTHH:mm:ssZ");
}

function getDefaultValues(orderDates: string[], contractorNip?: string): OrderInvoiceFormValues {
	const today = normalizeDate(new Date());
	const saleDate =
		orderDates.length > 0
			? orderDates.reduce(
					(latestDate, orderDate) => {
						const currentDate = normalizeDate(dayjs(orderDate).toDate());

						return currentDate > latestDate ? currentDate : latestDate;
					},
					normalizeDate(dayjs(orderDates[0]).toDate()),
				)
			: today;

	return {
		contractorNIP: contractorNip ?? "",
		invoiceNumber: "",
		issueDate: today,
		saleDate,
	};
}

export function OrderInvoiceForm({ contractorNip, onClose, onSuccess, orderDates, orderIds }: OrderInvoiceFormProps) {
	const createInvoiceMutation = useCreateInvoice({
		mutation: {
			onSuccess: () => {
				toast.success("Faktura została wystawiona.");
				onSuccess?.();
			},
			onError: () => {
				toast.error("Nie udało się wystawić faktury.");
			},
		},
	});
	const mutationError = createInvoiceMutation.error ?? null;
	const mutationErrorWithDetails = mutationError as
		| ({ details?: string | string[] | null } & typeof mutationError)
		| null;
	const mutationErrorDetails =
		typeof mutationErrorWithDetails?.details === "string"
			? mutationErrorWithDetails.details
			: Array.isArray(mutationErrorWithDetails?.details)
				? mutationErrorWithDetails.details.join(" ")
				: null;
	const mutationErrorMessages = mutationError?.errors ? Object.values(mutationError.errors).flat() : null;
	const mutationErrorMessage = mutationErrorMessages?.length
		? null
		: (mutationErrorDetails ?? mutationError?.detail ?? mutationError?.title ?? null);
	const hasSelectedOrders = orderIds.length > 0;
	const selectedOrderDates = [...new Set(orderDates.map((date) => toApiDateTime(date)))];
	const defaultValues = getDefaultValues(orderDates, contractorNip);
	const form = useForm({
		defaultValues,
		onSubmit: async ({ value }) => {
			const trimmedContractorNip = value.contractorNIP.trim();
			const trimmedInvoiceNumber = value.invoiceNumber.trim();

			if (!hasSelectedOrders || trimmedContractorNip.length === 0) {
				return;
			}

			await createInvoiceMutation.mutateAsync({
				data: {
					contractorNIP: trimmedContractorNip,
					dates: selectedOrderDates,
					invoiceNumber: trimmedInvoiceNumber === "" ? null : Number.parseInt(trimmedInvoiceNumber, 10),
					issueDate: toApiDateTime(value.issueDate),
					orderIds,
					saleDate: toApiDateTime(value.saleDate),
				},
			});
		},
	});
	const invoiceNumberValue = form.state.values.invoiceNumber.trim();
	const hasInvalidInvoiceNumber = invoiceNumberValue !== "" && !/^-?\d+$/.test(invoiceNumberValue);
	const isSubmitting = form.state.isSubmitting;
	const isSubmitDisabled =
		!hasSelectedOrders ||
		hasInvalidInvoiceNumber ||
		form.state.values.contractorNIP.trim().length === 0 ||
		isSubmitting;

	const handleClose = () => {
		createInvoiceMutation.reset();
		onClose?.();
	};

	if (!hasSelectedOrders) {
		return null;
	}

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
				<header className="space-y-2">
					<div className="space-y-1">
						<h2 className="text-2xl font-semibold tracking-tight">Wystaw fakturę</h2>
						<p className="text-sm text-muted-foreground">Uzupełnij dane potrzebne do wystawienia faktury.</p>
					</div>
				</header>

				{mutationErrorMessages?.length || mutationErrorMessage ? (
					<div className="rounded-2xl border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
						{mutationErrorMessages?.length
							? mutationErrorMessages.map((message) => <p key={message}>{message}</p>)
							: mutationErrorMessage}
					</div>
				) : null}

				<div className="grid gap-4 md:grid-cols-2">
					<form.Field name="contractorNIP">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;

							return (
								<Field data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>NIP kontrahenta</FieldLabel>
									<input
										id={field.name}
										name={field.name}
										value={field.state.value}
										onBlur={field.handleBlur}
										onChange={(event) => field.handleChange(event.target.value)}
										placeholder="1234567890"
										className={cn(
											"h-11 rounded-2xl border border-border bg-background px-4 text-sm outline-none transition placeholder:text-muted-foreground focus:border-foreground/30",
											isInvalid && "border-destructive/40 focus:border-destructive/40",
										)}
									/>
								</Field>
							);
						}}
					</form.Field>

					<form.Field name="invoiceNumber">
						{(field) => (
							<Field data-invalid={hasInvalidInvoiceNumber}>
								<FieldLabel htmlFor={field.name}>Numer faktury</FieldLabel>
								<input
									id={field.name}
									name={field.name}
									inputMode="numeric"
									value={field.state.value}
									onBlur={field.handleBlur}
									onChange={(event) => field.handleChange(event.target.value)}
									placeholder="Opcjonalnie"
									className={cn(
										"h-11 rounded-2xl border border-border bg-background px-4 text-sm outline-none transition placeholder:text-muted-foreground focus:border-foreground/30",
										hasInvalidInvoiceNumber && "border-destructive/40 focus:border-destructive/40",
									)}
								/>
								<FieldError
									errors={
										hasInvalidInvoiceNumber ? [{ message: "Numer faktury musi być liczbą całkowitą." }] : undefined
									}
								/>
							</Field>
						)}
					</form.Field>

					<form.Field name="saleDate">
						{(field) => (
							<Field>
								<FieldLabel htmlFor={field.name}>Data sprzedaży</FieldLabel>
								<DatePickerField
									id={field.name}
									value={field.state.value}
									onChange={(date) => {
										if (!date) {
											return;
										}

										field.handleChange(date);
									}}
								/>
							</Field>
						)}
					</form.Field>

					<form.Field name="issueDate">
						{(field) => (
							<Field>
								<FieldLabel htmlFor={field.name}>Data wystawienia</FieldLabel>
								<DatePickerField
									id={field.name}
									value={field.state.value}
									onChange={(date) => {
										if (!date) {
											return;
										}

										field.handleChange(date);
									}}
								/>
							</Field>
						)}
					</form.Field>
				</div>

				<footer className="mt-auto grid gap-3 border-t border-border/60 pt-5 sm:grid-cols-2">
					<Button className="h-11 w-full" type="button" variant="outline" onClick={handleClose} disabled={isSubmitting}>
						<XIcon size={16} />
						Anuluj
					</Button>
					<Button className="h-11 w-full" type="submit" disabled={isSubmitDisabled}>
						{isSubmitting ? <Spinner className="size-4" /> : <CheckIcon size={16} />}
						{isSubmitting ? "Wystawianie..." : "Wystaw fakturę"}
					</Button>
				</footer>
			</form>
		</section>
	);
}
