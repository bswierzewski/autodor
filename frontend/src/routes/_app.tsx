import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";
import { Box, Container } from "@mantine/core";

import { Navbar } from "../components/layouts/Navabr";

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
		<Box
			style={{
				minHeight: "100vh",
				background: "linear-gradient(180deg, #f6f8fc 0%, #eef2f8 100%)",
			}}
		>
			<Box px="lg" py="md">
				<Navbar />
			</Box>
			<Container fluid px="lg" pb="lg">
				<Outlet />
			</Container>
		</Box>
	);
}
