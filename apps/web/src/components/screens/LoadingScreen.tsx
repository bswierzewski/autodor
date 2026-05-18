import { Empty, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "@/components/ui/empty";
import { Spinner } from "@/components/ui/spinner";

export function LoadingScreen() {
	return (
		<Empty className="flex min-h-screen w-full items-center justify-center px-6">
			<EmptyHeader>
				<EmptyMedia variant="icon">
					<Spinner className="size-8" />
				</EmptyMedia>
				<EmptyTitle>Przygotowujemy logowanie</EmptyTitle>
				<EmptyDescription>
					Sprawdzamy Twoją sesję i inicjalizujemy bezpieczny dostęp do aplikacji. Ten ekran znika automatycznie, gdy
					Clerk zakończy ładowanie danych uwierzytelniania.
				</EmptyDescription>
			</EmptyHeader>
		</Empty>
	);
}
