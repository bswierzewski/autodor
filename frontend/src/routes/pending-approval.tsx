import { useClerk, useUser } from "@clerk/react";
import { createFileRoute, redirect } from "@tanstack/react-router";
import { LogOut, RefreshCw } from "lucide-react";
import { useState } from "react";
import { Button } from "../components/ui/button";

export const Route = createFileRoute("/pending-approval")({
	beforeLoad: ({ context }) => {
		if (!context.auth.isSignedIn) {
			throw redirect({ to: "/login" });
		}
		if (context.auth.isApproved) {
			throw redirect({ to: "/" });
		}
	},
	component: PendingApprovalPage,
});

function PendingApprovalPage() {
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
		<div className="flex min-h-screen flex-col items-center justify-center gap-6 p-8">
			<div className="flex max-w-md flex-col items-center gap-4 text-center">
				<div className="flex size-16 items-center justify-center rounded-full bg-muted">
					<svg
						xmlns="http://www.w3.org/2000/svg"
						className="size-8 text-muted-foreground"
						fill="none"
						viewBox="0 0 24 24"
						stroke="currentColor"
					>
						<title>Oczekiwanie</title>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							strokeWidth={2}
							d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
						/>
					</svg>
				</div>
				<h1 className="text-2xl font-semibold">Oczekiwanie na zatwierdzenie</h1>
				<p className="text-muted-foreground">
					Twoje konto zostało zarejestrowane i oczekuje na zatwierdzenie przez administratora. Otrzymasz dostęp do
					aplikacji po weryfikacji konta.
				</p>
			</div>
			<div className="flex gap-3">
				<Button variant="outline" onClick={handleCheckStatus} disabled={isChecking}>
					<RefreshCw className={isChecking ? "animate-spin" : ""} />
					{isChecking ? "Sprawdzanie..." : "Sprawdź status"}
				</Button>
				<Button variant="ghost" onClick={() => signOut()}>
					<LogOut />
					Wyloguj się
				</Button>
			</div>
		</div>
	);
}
