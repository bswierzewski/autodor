import { EnvelopeSimpleIcon, IdentificationCardIcon, MapPinLineIcon } from "@phosphor-icons/react";
import type { GetContractorsQueryResult } from "#/api/contractors/contractors";
import { ContractorActions } from "#/features/contractors/components/ContractorActions";

type ContractorsCardListProps = {
	contractors: GetContractorsQueryResult;
};

export function ContractorsCardList({ contractors }: ContractorsCardListProps) {
	return (
		<div className="grid gap-4">
			{contractors.map((contractor) => (
				<article
					className="rounded-3xl border bg-card p-5 text-left shadow-sm transition hover:-translate-y-0.5 hover:shadow-md"
					key={contractor.id}
				>
					<div className="flex items-start justify-between gap-4">
						<p className="text-lg font-semibold tracking-tight">{contractor.name}</p>
						<ContractorActions contractor={contractor} />
					</div>
					<div className="mt-5 space-y-2 text-sm text-muted-foreground">
						<div className="flex items-center gap-2">
							<IdentificationCardIcon size={16} />
							<span>NIP {contractor.nip}</span>
						</div>
						<div className="flex items-center gap-2">
							<MapPinLineIcon size={16} />
							<span>
								{contractor.street}, {contractor.zipCode} {contractor.city}
							</span>
						</div>
						<div className="flex items-center gap-2">
							<EnvelopeSimpleIcon size={16} />
							<span>{contractor.email}</span>
						</div>
					</div>
				</article>
			))}
		</div>
	);
}
