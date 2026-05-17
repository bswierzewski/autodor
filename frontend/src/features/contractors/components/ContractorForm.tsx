import { CheckIcon, PlusIcon, XIcon } from "@phosphor-icons/react";
import { useForm } from "@tanstack/react-form";
import { useQueryClient } from "@tanstack/react-query";

import {
	getGetContractorQueryKey,
	getGetContractorsQueryKey,
	useCreateContractor,
	useGetContractor,
	useUpdateContractor,
} from "#/api/contractors/contractors";
import { CreateContractorBody, UpdateContractorBody } from "#/api/contractors/contractors.zod";
import { Button } from "#/components/ui/button";
import { Field, FieldError, FieldLabel } from "#/components/ui/field";
import { Spinner } from "#/components/ui/spinner";
import { cn } from "#/lib/utils";

type ContractorFormProps = {
	contractorId?: string;
	onClose: () => void;
};

export function ContractorForm({ contractorId, onClose }: ContractorFormProps) {
	const queryClient = useQueryClient();
	const isEditMode = contractorId !== undefined;
	const contractorQuery = useGetContractor(contractorId ?? "", {
		query: {
			enabled: isEditMode,
		},
	});
	const createMutation = useCreateContractor();
	const updateMutation = useUpdateContractor();
	const mutation = isEditMode ? updateMutation : createMutation;
	const mutationError = mutation.error ?? null;

	const form = useForm({
		defaultValues: {
			name: contractorQuery.data?.name ?? "",
			nip: contractorQuery.data?.nip ?? "",
			street: contractorQuery.data?.street ?? "",
			city: contractorQuery.data?.city ?? "",
			zipCode: contractorQuery.data?.zipCode ?? "",
			email: contractorQuery.data?.email ?? "",
		},
		validators: {
			onSubmit: isEditMode ? UpdateContractorBody : CreateContractorBody,
		},
		onSubmit: async ({ value }) => {
			try {
				if (isEditMode && contractorId) {
					await updateMutation.mutateAsync({ id: contractorId, data: value });
					await queryClient.invalidateQueries({ queryKey: getGetContractorQueryKey(contractorId) });
				} else {
					await createMutation.mutateAsync({ data: value });
				}
				await queryClient.invalidateQueries({ queryKey: getGetContractorsQueryKey() });
				onClose();
			} catch {
				// Error displayed via mutation.error
			}
		},
	});

	const isLoadingContractor = isEditMode && contractorQuery.isLoading && !contractorQuery.data;
	const isSubmitting = form.state.isSubmitting;
	const isFieldDisabled = isLoadingContractor || contractorQuery.isError;
	const submitLabel = isEditMode ? "Zapisz zmiany" : "Dodaj kontrahenta";
	const title = isEditMode ? "Edytuj kontrahenta" : "Dodaj kontrahenta";
	const subtitle = isEditMode
		? (contractorQuery.data?.name ?? "Zaktualizuj dane wybranego kontrahenta.")
		: "Uzupełnij dane, aby dodać nowego kontrahenta do bazy.";

	const handleClose = () => {
		createMutation.reset();
		updateMutation.reset();
		onClose();
	};

	return (
		<section>
			<form
				className="flex h-full flex-col gap-6"
				onSubmit={(e) => {
					e.preventDefault();
					e.stopPropagation();
					void form.handleSubmit();
				}}
			>
				<header className="space-y-2">
					<div className="space-y-1">
						<h2 className="text-2xl font-semibold tracking-tight">{title}</h2>
						<p className="text-sm text-muted-foreground">{subtitle}</p>
					</div>
				</header>

				{contractorQuery.isError ? (
					<div className="rounded-2xl border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
						Nie udało się pobrać danych kontrahenta. Zamknij formularz i spróbuj ponownie.
					</div>
				) : null}

				{mutationError?.errors || mutationError?.detail || mutationError?.title ? (
					<div className="rounded-2xl border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
						{mutationError.errors
							? Object.values(mutationError.errors)
									.flat()
									.map((msg) => <p key={msg}>{msg}</p>)
							: mutationError.detail
								? mutationError.detail
								: mutationError.title}
					</div>
				) : null}

				<div className="grid gap-4 md:grid-cols-2">
					<form.Field name="name">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field className="md:col-span-2" data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>Nazwa firmy</FieldLabel>
									<input
										id={field.name}
										name={field.name}
										value={field.state.value}
										onChange={(e) => field.handleChange(e.target.value)}
										onBlur={field.handleBlur}
										disabled={isFieldDisabled}
										aria-invalid={isInvalid}
										placeholder="AUTODOR Sp. z o.o."
										className={cn(
											"h-11 rounded-2xl border border-border bg-background px-4 text-sm outline-none transition placeholder:text-muted-foreground focus:border-foreground/30 disabled:cursor-not-allowed disabled:opacity-70",
											isInvalid && "border-destructive/40 focus:border-destructive/40",
										)}
									/>
									<FieldError errors={field.state.meta.errors as Array<{ message?: string }>} />
								</Field>
							);
						}}
					</form.Field>
					<form.Field name="nip">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>NIP</FieldLabel>
									<input
										id={field.name}
										name={field.name}
										inputMode="numeric"
										value={field.state.value}
										onChange={(e) => field.handleChange(e.target.value)}
										onBlur={field.handleBlur}
										disabled={isFieldDisabled}
										aria-invalid={isInvalid}
										placeholder="1234567890"
										className={cn(
											"h-11 rounded-2xl border border-border bg-background px-4 text-sm outline-none transition placeholder:text-muted-foreground focus:border-foreground/30 disabled:cursor-not-allowed disabled:opacity-70",
											isInvalid && "border-destructive/40 focus:border-destructive/40",
										)}
									/>
									<FieldError errors={field.state.meta.errors as Array<{ message?: string }>} />
								</Field>
							);
						}}
					</form.Field>
					<form.Field name="email">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>Email</FieldLabel>
									<input
										id={field.name}
										name={field.name}
										type="email"
										autoComplete="email"
										value={field.state.value}
										onChange={(e) => field.handleChange(e.target.value)}
										onBlur={field.handleBlur}
										disabled={isFieldDisabled}
										aria-invalid={isInvalid}
										placeholder="biuro@autodor.pl"
										className={cn(
											"h-11 rounded-2xl border border-border bg-background px-4 text-sm outline-none transition placeholder:text-muted-foreground focus:border-foreground/30 disabled:cursor-not-allowed disabled:opacity-70",
											isInvalid && "border-destructive/40 focus:border-destructive/40",
										)}
									/>
									<FieldError errors={field.state.meta.errors as Array<{ message?: string }>} />
								</Field>
							);
						}}
					</form.Field>
					<form.Field name="street">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field className="md:col-span-2" data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>Ulica i numer</FieldLabel>
									<input
										id={field.name}
										name={field.name}
										autoComplete="street-address"
										value={field.state.value}
										onChange={(e) => field.handleChange(e.target.value)}
										onBlur={field.handleBlur}
										disabled={isFieldDisabled}
										aria-invalid={isInvalid}
										placeholder="ul. Przemyslowa 12"
										className={cn(
											"h-11 rounded-2xl border border-border bg-background px-4 text-sm outline-none transition placeholder:text-muted-foreground focus:border-foreground/30 disabled:cursor-not-allowed disabled:opacity-70",
											isInvalid && "border-destructive/40 focus:border-destructive/40",
										)}
									/>
									<FieldError errors={field.state.meta.errors as Array<{ message?: string }>} />
								</Field>
							);
						}}
					</form.Field>
					<form.Field name="zipCode">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>Kod pocztowy</FieldLabel>
									<input
										id={field.name}
										name={field.name}
										autoComplete="postal-code"
										value={field.state.value}
										onChange={(e) => field.handleChange(e.target.value)}
										onBlur={field.handleBlur}
										disabled={isFieldDisabled}
										aria-invalid={isInvalid}
										placeholder="00-001"
										className={cn(
											"h-11 rounded-2xl border border-border bg-background px-4 text-sm outline-none transition placeholder:text-muted-foreground focus:border-foreground/30 disabled:cursor-not-allowed disabled:opacity-70",
											isInvalid && "border-destructive/40 focus:border-destructive/40",
										)}
									/>
									<FieldError errors={field.state.meta.errors as Array<{ message?: string }>} />
								</Field>
							);
						}}
					</form.Field>
					<form.Field name="city">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;
							return (
								<Field data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>Miasto</FieldLabel>
									<input
										id={field.name}
										name={field.name}
										autoComplete="address-level2"
										value={field.state.value}
										onChange={(e) => field.handleChange(e.target.value)}
										onBlur={field.handleBlur}
										disabled={isFieldDisabled}
										aria-invalid={isInvalid}
										placeholder="Warszawa"
										className={cn(
											"h-11 rounded-2xl border border-border bg-background px-4 text-sm outline-none transition placeholder:text-muted-foreground focus:border-foreground/30 disabled:cursor-not-allowed disabled:opacity-70",
											isInvalid && "border-destructive/40 focus:border-destructive/40",
										)}
									/>
									<FieldError errors={field.state.meta.errors as Array<{ message?: string }>} />
								</Field>
							);
						}}
					</form.Field>
				</div>

				<footer className="mt-auto grid gap-3 border-t border-border/60 pt-5 sm:grid-cols-2">
					<Button className="h-11 w-full" type="button" variant="outline" onClick={handleClose} disabled={isSubmitting}>
						<XIcon size={16} />
						Anuluj
					</Button>
					<Button className="h-11 w-full" type="submit" disabled={isFieldDisabled || isSubmitting}>
						{isSubmitting ? <Spinner className="size-4" /> : null}
						{!isSubmitting ? isEditMode ? <CheckIcon size={16} /> : <PlusIcon size={16} /> : null}
						{isSubmitting ? "Zapisywanie..." : submitLabel}
					</Button>
				</footer>
			</form>
		</section>
	);
}
