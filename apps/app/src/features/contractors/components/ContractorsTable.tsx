import type { GetContractorsQueryResult } from "#/api/contractors/contractors";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "#/components/ui/table";
import { ContractorActions } from "#/features/contractors/components/ContractorActions";

type ContractorsTableProps = {
	contractors: GetContractorsQueryResult;
};

export function ContractorsTable({ contractors }: ContractorsTableProps) {
	return (
		<div className="space-y-3">
			<div className="overflow-hidden rounded-3xl border bg-card shadow-sm">
				<Table className="table-fixed">
					<TableHeader>
						<TableRow>
							<TableHead className="w-[26%] pl-5">Nazwa</TableHead>
							<TableHead>NIP</TableHead>
							<TableHead className="w-[32%]">Adres</TableHead>
							<TableHead className="w-[24%]">Email</TableHead>
							<TableHead className="w-28 pr-5" />
						</TableRow>
					</TableHeader>
					<TableBody>
						{contractors.map((contractor) => {
							const address = `${contractor.street}, ${contractor.zipCode} ${contractor.city}`;

							return (
								<TableRow className="transition-colors hover:bg-muted/40" key={contractor.id}>
									<TableCell className="pl-5 font-medium">
										<span className="block truncate" title={contractor.name}>
											{contractor.name}
										</span>
									</TableCell>
									<TableCell className="text-muted-foreground">{contractor.nip}</TableCell>
									<TableCell className="text-muted-foreground">
										<span className="block truncate" title={address}>
											{address}
										</span>
									</TableCell>
									<TableCell className="text-muted-foreground">
										<span className="block truncate" title={contractor.email}>
											{contractor.email}
										</span>
									</TableCell>
									<TableCell className="pr-5">
										<div className="flex items-center justify-end gap-2">
											<ContractorActions contractor={contractor} />
										</div>
									</TableCell>
								</TableRow>
							);
						})}
					</TableBody>
				</Table>
			</div>
		</div>
	);
}
