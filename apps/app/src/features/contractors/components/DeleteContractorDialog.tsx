import { useQueryClient } from "@tanstack/react-query";
import { type ReactNode, useState } from "react";

import { getGetContractorsQueryKey, useDeleteContractor } from "#/api/contractors/contractors";
import type { GetContractorsResponse } from "#/api/models";
import {
	AlertDialog,
	AlertDialogAction,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
	AlertDialogTrigger,
} from "#/components/ui/alert-dialog";
import { Spinner } from "#/components/ui/spinner";

type DeleteContractorDialogProps = {
	contractor: GetContractorsResponse;
	children: ReactNode;
};

export function DeleteContractorDialog({ contractor, children }: DeleteContractorDialogProps) {
	const [open, setOpen] = useState(false);
	const queryClient = useQueryClient();
	const deleteContractorMutation = useDeleteContractor({
		mutation: {
			onSuccess: async () => {
				await queryClient.invalidateQueries({
					queryKey: getGetContractorsQueryKey(),
				});
				setOpen(false);
			},
			onError: (error) => {
				console.error("Failed to delete contractor", error);
			},
		},
	});

	const handleOpenChange = (nextOpen: boolean) => {
		if (!nextOpen && !deleteContractorMutation.isPending) {
			deleteContractorMutation.reset();
		}

		setOpen(nextOpen);
	};

	const handleDelete = (event: React.MouseEvent<HTMLButtonElement>) => {
		event.preventDefault();
		deleteContractorMutation.mutate({ id: contractor.id });
	};

	return (
		<AlertDialog open={open} onOpenChange={handleOpenChange}>
			<AlertDialogTrigger asChild>{children}</AlertDialogTrigger>
			<AlertDialogContent className="max-w-md rounded-3xl p-6 sm:max-w-md">
				<AlertDialogHeader>
					<AlertDialogTitle className="text-xl font-semibold tracking-tight">Usuń kontrahenta</AlertDialogTitle>
					<AlertDialogDescription>
						Czy na pewno chcesz usunąć kontrahenta{" "}
						<strong className="font-semibold text-foreground">{contractor.name}</strong> z bazy?
					</AlertDialogDescription>
				</AlertDialogHeader>

				{deleteContractorMutation.error?.detail || deleteContractorMutation.error?.title ? (
					<div className="rounded-2xl border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
						{deleteContractorMutation.error.detail ?? deleteContractorMutation.error.title}
					</div>
				) : null}

				<AlertDialogFooter className="pt-2">
					<AlertDialogCancel disabled={deleteContractorMutation.isPending}>Anuluj</AlertDialogCancel>
					<AlertDialogAction disabled={deleteContractorMutation.isPending} onClick={handleDelete}>
						{deleteContractorMutation.isPending ? <Spinner className="size-4" /> : null}
						{deleteContractorMutation.isPending ? "Usuwanie..." : "Usuń"}
					</AlertDialogAction>
				</AlertDialogFooter>
			</AlertDialogContent>
		</AlertDialog>
	);
}
