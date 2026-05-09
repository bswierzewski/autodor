import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/contractors/")({
	component: ContractorsPage,
});

function ContractorsPage() {
	return <h1>Contractors</h1>;
}
