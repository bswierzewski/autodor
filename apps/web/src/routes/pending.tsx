import { useClerk, useUser } from "@clerk/react";
import { ArrowClockwiseIcon, HourglassMediumIcon, SignOutIcon } from "@phosphor-icons/react";
import { createFileRoute, redirect } from "@tanstack/react-router";
import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "@/components/ui/empty";

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
		<Empty className="flex min-h-screen w-full items-center justify-center px-6">
			<EmptyHeader>
				<EmptyMedia variant="icon">
					<HourglassMediumIcon size={20} weight="duotone" />
				</EmptyMedia>
				<EmptyTitle>Konto oczekuje na zatwierdzenie</EmptyTitle>
				<EmptyDescription>
					Twoje konto zostało utworzone, ale jeszcze nie ma dostępu do aplikacji. Sprawdź status po akceptacji albo
					wyloguj się i wróć później.
				</EmptyDescription>
			</EmptyHeader>
			<EmptyContent className="sm:flex-row sm:justify-center">
				<Button type="button" size="lg" onClick={handleCheckStatus} disabled={isChecking}>
					<ArrowClockwiseIcon size={18} weight="duotone" className={isChecking ? "animate-spin" : undefined} />
					{isChecking ? "Sprawdzanie..." : "Sprawdź status"}
				</Button>
				<Button type="button" size="lg" variant="outline" onClick={() => signOut()}>
					<SignOutIcon size={18} weight="duotone" />
					Wyloguj się
				</Button>
			</EmptyContent>
		</Empty>
	);
}
