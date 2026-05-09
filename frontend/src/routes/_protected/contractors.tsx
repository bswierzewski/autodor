import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_protected/contractors")({
	component: ContractorsPage,
});

function ContractorsPage() {
	return <h1>Contractors</h1>;
}
