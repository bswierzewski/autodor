import { createFileRoute } from "@tanstack/react-router";
import { ContractorsPage } from "#/features/contractors/pages/ContractorsPage";

export const Route = createFileRoute("/_app/contractors")({
	component: ContractorsPage,
});
