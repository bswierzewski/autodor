import { toast } from "sonner";
import { useCreateManualInvoice } from "#/api/invoicing/invoicing";
import { getProblemDetailsMessages } from "#/lib/api-errors";
import type { ManualInvoiceCsvItem } from "../lib/manualInvoiceCsvImporter";
import { InvoiceDetailsForm } from "./InvoiceDetailsForm";

type CreateManualInvoiceFormProps = {
	items: ManualInvoiceCsvItem[];
	onSuccess: () => void;
	onCancel: () => void;
};

export function CreateManualInvoiceForm({ items, onSuccess, onCancel }: CreateManualInvoiceFormProps) {
	const createInvoiceMutation = useCreateManualInvoice({
		mutation: {
			onSuccess: () => {
				toast.success("Faktura została wystawiona.");
				onSuccess();
			},
		},
	});
	const errorMessages = getProblemDetailsMessages(createInvoiceMutation.error, "Nie udało się wystawić faktury.");

	return (
		<InvoiceDetailsForm
			description={
				items.length === 1
					? "Faktura zostanie wystawiona dla 1 pozycji z pliku CSV."
					: `Faktura zostanie wystawiona dla ${items.length} pozycji z pliku CSV.`
			}
			errorMessages={errorMessages}
			isPending={createInvoiceMutation.isPending}
			onCancel={onCancel}
			onSubmit={async (details) => {
				await createInvoiceMutation.mutateAsync({
					data: {
						...details,
						items: items.map(({ itemNumber, quantity, netUnitPrice }) => ({ itemNumber, quantity, netUnitPrice })),
					},
				});
			}}
		/>
	);
}
