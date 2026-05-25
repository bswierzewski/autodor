import type { GetContractorsQueryResult } from "#/api/contractors/contractors";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
import { ContractorActions } from "#/features/contractors/components/ContractorActions";

type ContractorsTableProps = {
	contractors: GetContractorsQueryResult;
};

export function ContractorsTable({ contractors }: ContractorsTableProps) {
	return (
		<div className="space-y-3">
			<p className="text-sm text-muted-foreground">
				Użyj akcji w ostatniej kolumnie, aby edytować lub usunąć kontrahenta.
			</p>
			<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
				<Table>
					<TableHeader>
						<TableRow>
							<TableHead className="pl-5">Nazwa</TableHead>
							<TableHead>NIP</TableHead>
							<TableHead>Adres</TableHead>
							<TableHead>Email</TableHead>
							<TableHead className="w-28 pr-5" />
						</TableRow>
					</TableHeader>
					<TableBody>
						{contractors.map((contractor) => (
							<TableRow className="transition-colors hover:bg-muted/40" key={contractor.id}>
								<TableCell className="pl-5 font-medium">{contractor.name}</TableCell>
								<TableCell className="text-muted-foreground">{contractor.nip}</TableCell>
								<TableCell className="text-muted-foreground">
									{contractor.street}, {contractor.zipCode} {contractor.city}
								</TableCell>
								<TableCell className="text-muted-foreground">{contractor.email}</TableCell>
								<TableCell className="pr-5">
									<div className="flex items-center justify-end gap-2">
										<ContractorActions contractor={contractor} />
									</div>
								</TableCell>
							</TableRow>
						))}
					</TableBody>
				</Table>
			</div>
		</div>
	);
}