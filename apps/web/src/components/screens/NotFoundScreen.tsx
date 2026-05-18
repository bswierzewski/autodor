import { HouseIcon, MagnifyingGlassIcon } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "@/components/ui/empty";

export function NotFoundScreen() {
	return (
		<Empty className="flex min-h-screen w-full items-center justify-center px-6">
			<EmptyHeader>
				<EmptyMedia variant="icon">
					<MagnifyingGlassIcon size={20} weight="duotone" />
				</EmptyMedia>
				<EmptyTitle>404 - Nie znaleziono strony</EmptyTitle>
				<EmptyDescription>
					Nie udało się odnaleźć strony pod tym adresem. Sprawdź, czy link jest poprawny, albo wróć na stronę główną
					aplikacji.
				</EmptyDescription>
			</EmptyHeader>
			<EmptyContent>
				<Button asChild size="lg">
					<Link to="/">
						<HouseIcon size={18} weight="duotone" />
						Strona główna
					</Link>
				</Button>
			</EmptyContent>
		</Empty>
	);
}
