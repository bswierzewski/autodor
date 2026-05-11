import { SignIn } from "@clerk/react";
import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/login")({
	beforeLoad: ({ context }) => {
		if (!context.auth.isSignedIn) {
			return;
		}

		if (context.auth.isApproved) {
			throw redirect({ to: "/" });
		}

		throw redirect({ to: "/pending" });
	},
	component: SignInPage,
});

function SignInPage() {
	return <SignIn />;
}
