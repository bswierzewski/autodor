import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/_app")({
	beforeLoad: ({ context }) => {
		if (!context.auth.isSignedIn) {
			throw redirect({ to: "/login" });
		}

		if (!context.auth.isApproved) {
			throw redirect({ to: "/pending" });
		}
	},
	component: () => <Outlet />,
});
