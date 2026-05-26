import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { getGetContractorsQueryKey, useCreateContractor } from "#/api/contractors/contractors";
import { ContractorForm, emptyContractorFormValues } from "#/features/contractors/components/ContractorForm";
import { getProblemDetailsMessages } from "#/lib/api-errors";

export const Route = createFileRoute("/_app/contractors/create")({
	component: ContractorCreateRoute,
});

function ContractorCreateRoute() {
	const navigate = useNavigate();
	const queryClient = useQueryClient();
	const createMutation = useCreateContractor();
	const errorMessages = getProblemDetailsMessages(createMutation.error, "Nie udało się dodać kontrahenta.");

	const handleClose = () => {
		createMutation.reset();
		void navigate({ to: "/contractors" });
	};

	return (
		<ContractorForm
			description="Uzupełnij dane, aby dodać nowego kontrahenta do bazy."
			errorMessages={errorMessages}
			initialValues={emptyContractorFormValues}
			isPending={createMutation.isPending}
			onCancel={handleClose}
			onSubmit={async (value) => {
				await createMutation.mutateAsync({ data: value });
				await queryClient.invalidateQueries({ queryKey: getGetContractorsQueryKey() });
				await navigate({ to: "/contractors" });
			}}
			submitLabel="Dodaj kontrahenta"
			submittingLabel="Dodawanie..."
			title="Dodaj kontrahenta"
		/>
	);
}
