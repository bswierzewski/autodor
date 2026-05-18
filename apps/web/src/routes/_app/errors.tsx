import { createFileRoute } from "@tanstack/react-router";
import { ErrorsPage } from "#/features/errors/pages/ErrorsPage";

export const Route = createFileRoute("/_app/errors")({
	component: ErrorsPage,
});
