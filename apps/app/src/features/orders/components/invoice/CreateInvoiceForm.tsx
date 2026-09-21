import dayjs from "dayjs";
import { toast } from "sonner";
import { useCreateInvoice } from "#/api/invoicing/invoicing";
import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { InvoiceDetailsForm } from "#/features/invoicing/components/InvoiceDetailsForm";
import { getProblemDetailsMessages } from "#/lib/api-errors";

type CreateInvoiceFormProps = {
	selectedOrders: OrderSummaryResponse[];
	onSuccess: () => void;
	onCancel: () => void;
};

export function CreateInvoiceForm({ selectedOrders, onSuccess, onCancel }: CreateInvoiceFormProps) {
	const selectedNIPs = selectedOrders.map((order) => order.customerNumber?.trim() ?? "");
	const contractorNIP = selectedNIPs[0] && selectedNIPs.every((nip) => nip === selectedNIPs[0]) ? selectedNIPs[0] : "";
	const createInvoiceMutation = useCreateInvoice({
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
			defaultContractorNIP={contractorNIP}
			description={
				selectedOrders.length === 1
					? "Faktura zostanie wystawiona dla 1 zamówienia."
					: `Faktura zostanie wystawiona dla ${selectedOrders.length} zamówień.`
			}
			errorMessages={errorMessages}
			isPending={createInvoiceMutation.isPending}
			onCancel={onCancel}
			onSubmit={async (details) => {
				await createInvoiceMutation.mutateAsync({
					data: {
						...details,
						orderIds: selectedOrders.map((order) => order.id),
						dates: selectedOrders.map((order) => dayjs(order.date).toISOString()),
					},
				});
			}}
		/>
	);
}
