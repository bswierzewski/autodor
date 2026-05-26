import { CheckIcon, XIcon } from "@phosphor-icons/react";
import { useForm } from "@tanstack/react-form";
import { CreateContractorBody } from "#/api/contractors/contractors.zod";
import { Button } from "#/components/ui/button";
import { Field, FieldDescription, FieldError, FieldGroup, FieldLabel } from "#/components/ui/field";
import { Input } from "#/components/ui/input";
import { Spinner } from "#/components/ui/spinner";

export type ContractorFormValues = {
	name: string;
	nip: string;
	street: string;
	city: string;
	zipCode: string;
	email: string;
};

export const emptyContractorFormValues: ContractorFormValues = {
	name: "",
	nip: "",
	street: "",
	city: "",
	zipCode: "",
	email: "",
};

type ContractorFormProps = {
	description: string;
	errorMessages?: string[];
	initialValues: ContractorFormValues;
	isPending?: boolean;
	onCancel: () => void;
	onSubmit: (values: ContractorFormValues) => Promise<void> | void;
	submitLabel: string;
	submittingLabel: string;
	title: string;
};

export function ContractorForm({
	description,
	errorMessages = [],
	initialValues,
	isPending = false,
	onCancel,
	onSubmit,
	submitLabel,
	submittingLabel,
	title,
}: ContractorFormProps) {
	const form = useForm({
		defaultValues: initialValues,
		validators: {
			onSubmit: CreateContractorBody,
		},
		onSubmit: async ({ value }) => {
			await onSubmit(value);
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
				<header className="space-y-2">
					<div className="space-y-1">
						<h2 className="text-2xl font-semibold tracking-tight">{title}</h2>
						<p className="text-sm text-muted-foreground">{description}</p>
					</div>
				</header>

				{errorMessages.length ? (
					<div className="rounded-2xl border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
						{errorMessages.map((message) => (
							<p key={message}>{message}</p>
						))}
					</div>
				) : null}

				<FieldGroup className="grid gap-4 md:grid-cols-2">
					<form.Field name="name">
						{(field) => {
							const isInvalid = field.state.meta.isTouched && !field.state.meta.isValid;

							return (
								<Field className="md:col-span-2" data-invalid={isInvalid}>
									<FieldLabel htmlFor={field.name}>Nazwa firmy</FieldLabel>
									<Input
										aria-invalid={isInvalid}
										disabled={isPending}
										id={field.name}
										name={field.name}
										onBlur={field.handleBlur}
										onChange={(event) => field.handleChange(event.target.value)}
										placeholder="AUTODOR Sp. z o.o."
										value={field.state.value}
									/>
									<FieldDescription>Pełna nazwa kontrahenta widoczna na dokumentach.</FieldDescription>
									<FieldError errors={field.state.meta.errors} />
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
									<Input
										aria-invalid={isInvalid}
										disabled={isPending}
										id={field.name}
										inputMode="numeric"
										name={field.name}
										onBlur={field.handleBlur}
										onChange={(event) => field.handleChange(event.target.value)}
										placeholder="1234567890"
										value={field.state.value}
									/>
									<FieldError errors={field.state.meta.errors} />
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
									<Input
										autoComplete="email"
										aria-invalid={isInvalid}
										disabled={isPending}
										id={field.name}
										name={field.name}
										onBlur={field.handleBlur}
										onChange={(event) => field.handleChange(event.target.value)}
										placeholder="biuro@autodor.pl"
										type="email"
										value={field.state.value}
									/>
									<FieldError errors={field.state.meta.errors} />
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
									<Input
										autoComplete="street-address"
										aria-invalid={isInvalid}
										disabled={isPending}
										id={field.name}
										name={field.name}
										onBlur={field.handleBlur}
										onChange={(event) => field.handleChange(event.target.value)}
										placeholder="ul. Przemyslowa 12"
										value={field.state.value}
									/>
									<FieldError errors={field.state.meta.errors} />
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
									<Input
										autoComplete="postal-code"
										aria-invalid={isInvalid}
										disabled={isPending}
										id={field.name}
										name={field.name}
										onBlur={field.handleBlur}
										onChange={(event) => field.handleChange(event.target.value)}
										placeholder="00-001"
										value={field.state.value}
									/>
									<FieldError errors={field.state.meta.errors} />
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
									<Input
										autoComplete="address-level2"
										aria-invalid={isInvalid}
										disabled={isPending}
										id={field.name}
										name={field.name}
										onBlur={field.handleBlur}
										onChange={(event) => field.handleChange(event.target.value)}
										placeholder="Warszawa"
										value={field.state.value}
									/>
									<FieldError errors={field.state.meta.errors} />
								</Field>
							);
						}}
					</form.Field>
				</FieldGroup>

				<footer className="mt-auto grid gap-3 border-t border-border/60 pt-5 sm:grid-cols-2">
					<Button className="h-11 w-full" type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>
						<XIcon size={16} />
						Anuluj
					</Button>
					<Button className="h-11 w-full" type="submit" disabled={isPending || isSubmitting}>
						{isSubmitting ? <Spinner className="size-4" /> : <CheckIcon size={16} />}
						{isSubmitting ? submittingLabel : submitLabel}
					</Button>
				</footer>
			</form>
		</section>
	);
}
