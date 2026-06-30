import { PencilSimpleLineIcon, TrashIcon } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";
import type { GetContractorsQueryResult } from "#/api/contractors/contractors";
import { Button } from "#/components/ui/button";
import { DeleteContractorDialog } from "#/features/contractors/components/DeleteContractorDialog";

type ContractorActionsProps = {
	contractor: GetContractorsQueryResult[number];
};

export function ContractorActions({ contractor }: ContractorActionsProps) {
	return (
		<div className="flex items-center gap-2">
			<Button asChild size="icon-sm" type="button" variant="outline">
				<Link
					onClick={(event) => {
						event.currentTarget.blur();
					}}
					params={{ contractorId: contractor.id }}
					search={true}
					to="/contractors/$contractorId/edit"
				>
					<PencilSimpleLineIcon size={16} />
					<span className="sr-only">Edytuj kontrahenta {contractor.name}</span>
				</Link>
			</Button>
			<DeleteContractorDialog contractor={contractor}>
				<Button size="icon-sm" type="button" variant="destructive">
					<TrashIcon size={16} />
					<span className="sr-only">Usuń kontrahenta {contractor.name}</span>
				</Button>
			</DeleteContractorDialog>
		</div>
	);
}
