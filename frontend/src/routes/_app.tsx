import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";

import { Navbar } from "../components/layouts/Navbar";

export const Route = createFileRoute("/_app")({
	beforeLoad: ({ context }) => {
		if (!context.auth.isSignedIn) {
			throw redirect({ to: "/login" });
		}

		if (!context.auth.isApproved) {
			throw redirect({ to: "/pending" });
		}
	},
	component: AppLayout,
});

function AppLayout() {
	return (
		<div
			className="min-h-screen"
		>
			<div>
				<Navbar />
			</div>
			<div className="px-4 pt-4 pb-6 lg:px-6">
				<Outlet />
			</div>
		</div>
	);
}
