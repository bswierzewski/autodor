import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import {
	getGetContractorQueryKey,
	getGetContractorQueryOptions,
	getGetContractorsQueryKey,
	useUpdateContractor,
} from "#/api/contractors/contractors";
import { queryClient } from "#/config/query-client";
import { ContractorForm } from "#/features/contractors/components/ContractorForm";
import { getProblemDetailsMessages } from "#/lib/api-errors";

export const Route = createFileRoute("/_app/contractors/$contractorId/edit")({
	loader: ({ params }) => queryClient.ensureQueryData(getGetContractorQueryOptions(params.contractorId)),
	pendingComponent: ContractorEditPending,
	errorComponent: ContractorEditError,
	component: ContractorEditRoute,
});

function ContractorEditRoute() {
	const contractor = Route.useLoaderData();
	const { contractorId } = Route.useParams();
	const navigate = useNavigate();
	const routeQueryClient = useQueryClient();
	const updateMutation = useUpdateContractor();
	const errorMessages = getProblemDetailsMessages(updateMutation.error, "Nie udało się zapisać kontrahenta.");
	const initialValues = {
		name: contractor.name,
		nip: contractor.nip,
		street: contractor.street,
		city: contractor.city,
		zipCode: contractor.zipCode,
		email: contractor.email,
	};

	const handleClose = () => {
		updateMutation.reset();
		void navigate({ to: "/contractors", search: true });
	};

	return (
		<ContractorForm
			key={contractorId}
			description={contractor.name}
			errorMessages={errorMessages}
			initialValues={initialValues}
			isPending={updateMutation.isPending}
			onCancel={handleClose}
			onSubmit={async (value) => {
				await updateMutation.mutateAsync({ id: contractorId, data: value });
				await routeQueryClient.invalidateQueries({ queryKey: getGetContractorQueryKey(contractorId) });
				await routeQueryClient.invalidateQueries({ queryKey: getGetContractorsQueryKey() });
				await navigate({ to: "/contractors", search: true });
			}}
			submitLabel="Zapisz zmiany"
			submittingLabel="Zapisywanie..."
			title="Edytuj kontrahenta"
		/>
	);
}

function ContractorEditPending() {
	return (
		<div className="rounded-3xl border border-dashed border-border bg-card p-6 text-sm text-muted-foreground shadow-sm">
			Ładowanie danych kontrahenta...
		</div>
	);
}

function ContractorEditError() {
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
