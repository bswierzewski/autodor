import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/contractors")({
	component: RouteComponent,
});

function RouteComponent() {
	return <h1>Hello contractors!</h1>;
}
