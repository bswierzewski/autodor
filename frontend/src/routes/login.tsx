import { SignIn } from "@clerk/react";
import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/login")({
	beforeLoad: ({ context }) => {
		if (context.auth.isSignedIn && context.auth.isApproved) {
			throw redirect({ to: "/" });
		}
		if (context.auth.isSignedIn && !context.auth.isApproved) {
			throw redirect({ to: "/pending-approval" });
		}
	},
	component: SignInPage,
});

function SignInPage() {
	return <SignIn />;
}
