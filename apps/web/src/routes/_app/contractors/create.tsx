import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Drawer, DrawerContent, DrawerTitle } from "#/components/ui/drawer";
import { CreateContractorForm } from "#/features/contractors/components/CreateContractorForm";
import { useMediaQuery } from "#/hooks/use-media-query";

export const Route = createFileRoute("/_app/contractors/create")({
	component: ContractorCreateRoute,
});

function ContractorCreateRoute() {
	const navigate = useNavigate();
	const isDesktop = useMediaQuery("(min-width: 1024px)");

	return (
		<Drawer
			direction={isDesktop ? "right" : "bottom"}
			open
			onOpenChange={(open) => {
				if (!open) {
					void navigate({ to: "/contractors" });
				}
			}}
		>
			<DrawerContent className={isDesktop ? "px-6 pb-6 [&>div:first-child]:hidden" : "px-4 pb-4"}>
				<DrawerTitle className="sr-only">Dodaj kontrahenta</DrawerTitle>
				<div
					className={
						isDesktop
							? "h-full overflow-y-auto px-1 pb-2 pt-6"
							: "mx-auto w-full max-w-2xl overflow-y-auto px-1 pb-2 pt-4"
					}
				>
					<CreateContractorForm />
				</div>
			</DrawerContent>
		</Drawer>
	);
}
