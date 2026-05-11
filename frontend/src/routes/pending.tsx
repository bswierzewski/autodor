import { useClerk, useUser } from "@clerk/react";
import { createFileRoute, redirect } from "@tanstack/react-router";
import { useState } from "react";

export const Route = createFileRoute("/pending")({
	beforeLoad: ({ context }) => {
		if (!context.auth.isSignedIn) {
			throw redirect({ to: "/login" });
		}

		if (context.auth.isApproved) {
			throw redirect({ to: "/" });
		}
	},
	component: PendingPage,
});

function PendingPage() {
	const { signOut } = useClerk();
	const { user } = useUser();
	const [isChecking, setIsChecking] = useState(false);

	const handleCheckStatus = async () => {
		setIsChecking(true);
		try {
			await user?.reload();
		} finally {
			setIsChecking(false);
		}
	};

	return (
		<div>
			<button type="button" onClick={handleCheckStatus} disabled={isChecking}>
				{isChecking ? "Sprawdzanie..." : "Sprawdź status"}
			</button>
			<button type="button" onClick={() => signOut()}>
				Wyloguj się
			</button>
		</div>
	);
}
