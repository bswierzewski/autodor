import { CheckIcon, XIcon } from "@phosphor-icons/react";
import { useForm } from "@tanstack/react-form";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";

import {
	getGetContractorQueryKey,
	getGetContractorsQueryKey,
	useGetContractor,
	useUpdateContractor,
} from "#/api/contractors/contractors";
import { UpdateContractorBody } from "#/api/contractors/contractors.zod";
import type { UpdateContractorCommand } from "#/api/models/updateContractorCommand";
import { Button } from "#/components/ui/button";
import { Field, FieldDescription, FieldError, FieldGroup, FieldLabel } from "#/components/ui/field";
import { Input } from "#/components/ui/input";
import { Spinner } from "#/components/ui/spinner";

export const Route = createFileRoute("/_app/contractors/$contractorId/edit")({
	component: ContractorsEditRoute,
});

function ContractorsEditRoute() {
	const navigate = useNavigate();
	const { contractorId } = Route.useParams();
	const queryClient = useQueryClient();
	const contractorQuery = useGetContractor(contractorId);
	const updateMutation = useUpdateContractor();
	const errorMessages = updateMutation.error?.errors
		? Object.values(updateMutation.error.errors).flat()
		: updateMutation.error?.detail || updateMutation.error?.title
			? [updateMutation.error.detail ?? updateMutation.error.title ?? "Nie udało się zapisać kontrahenta."]
			: [];

	if (contractorQuery.isLoading && !contractorQuery.data) {
		return (
			<div className="rounded-3xl border border-dashed border-border bg-card p-6 text-sm text-muted-foreground shadow-sm">
				Ładowanie danych kontrahenta...
			</div>
		);
	}

	if (contractorQuery.isError || !contractorQuery.data) {
		return (
			<div className="space-y-4">
				<header className="space-y-1">
					<h2 className="text-2xl font-semibold tracking-tight">Edytuj kontrahenta</h2>
					<p className="text-sm text-muted-foreground">Nie udało się pobrać danych do edycji.</p>
				</header>
				<div className="rounded-2xl border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
					Zamknij formularz i spróbuj ponownie.
				</div>
			</div>
		);
	}

	const defaultValues: UpdateContractorCommand = {
		name: contractorQuery.data.name,
		nip: contractorQuery.data.nip,
		street: contractorQuery.data.street,
		city: contractorQuery.data.city,
		zipCode: contractorQuery.data.zipCode,
		email: contractorQuery.data.email,
	};
	const form = useForm({
		defaultValues,
		validators: {
			onSubmit: UpdateContractorBody,
		},
		onSubmit: async ({ value }) => {
			await updateMutation.mutateAsync({ id: contractorId, data: value });
			await queryClient.invalidateQueries({ queryKey: getGetContractorQueryKey(contractorId) });
			await queryClient.invalidateQueries({ queryKey: getGetContractorsQueryKey() });
			await navigate({ to: "/contractors" });
		},
	});
	const isSubmitting = form.state.isSubmitting;

	const handleClose = () => {
		form.reset();
		updateMutation.reset();
		void navigate({ to: "/contractors" });
	};

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
						<h2 className="text-2xl font-semibold tracking-tight">Edytuj kontrahenta</h2>
						<p className="text-sm text-muted-foreground">{contractorQuery.data.name}</p>
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
										disabled={updateMutation.isPending}
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
										disabled={updateMutation.isPending}
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
										disabled={updateMutation.isPending}
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
										disabled={updateMutation.isPending}
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
										disabled={updateMutation.isPending}
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
										disabled={updateMutation.isPending}
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
					<Button
						className="h-11 w-full"
						type="button"
						variant="outline"
						onClick={handleClose}
						disabled={isSubmitting}
					>
						<XIcon size={16} />
						Anuluj
					</Button>
					<Button className="h-11 w-full" type="submit" disabled={updateMutation.isPending || isSubmitting}>
						{isSubmitting ? <Spinner className="size-4" /> : <CheckIcon size={16} />}
						{isSubmitting ? "Zapisywanie..." : "Zapisz zmiany"}
					</Button>
				</footer>
			</form>
		</section>
	);
}
