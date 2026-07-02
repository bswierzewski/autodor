import { ArrowClockwiseIcon, DotsThreeOutlineVerticalIcon, EyeIcon, PrinterIcon, XIcon } from "@phosphor-icons/react";
import { useQueryClient } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { toast } from "sonner";
import type { OrderSummaryResponse } from "#/api/models/orderSummaryResponse";
import { getGetOrdersQueryKey, useGenerateDeliveryNote, useUpdateOrderExclusion } from "#/api/orders/orders";
import { Button } from "#/components/ui/button";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "#/components/ui/dropdown-menu";
import { Spinner } from "#/components/ui/spinner";
import { downloadBlob } from "#/lib/utils";

type OrderActionsProps = {
	order: OrderSummaryResponse;
};

export function OrderActions({ order }: OrderActionsProps) {
	const queryClient = useQueryClient();
	const generateDeliveryNoteMutation = useGenerateDeliveryNote({
		mutation: {
			onSuccess: (pdf) => {
				downloadBlob(pdf, `WZ_${order.id}.pdf`);
				toast.success("PDF został pobrany.");
			},
			onError: () => toast.error("Nie udało się wygenerować pliku PDF."),
		},
	});
	const updateOrderExclusionMutation = useUpdateOrderExclusion({
		mutation: {
			onSuccess: async (_, variables) => {
				await queryClient.invalidateQueries({ queryKey: getGetOrdersQueryKey() });
				toast.success(
					variables.data.excluded
						? "Zamówienie zostało pominięte przy fakturowaniu."
						: "Zamówienie zostało przywrócone do fakturowania.",
				);
			},
			onError: () => toast.error("Nie udało się zmienić statusu zamówienia."),
		},
	});
	const isPending = generateDeliveryNoteMutation.isPending || updateOrderExclusionMutation.isPending;

	return (
		<DropdownMenu>
			<DropdownMenuTrigger asChild>
				<Button
					aria-busy={isPending}
					className="ml-auto"
					disabled={isPending}
					size="icon-sm"
					type="button"
					variant="ghost"
					onClick={(event) => event.stopPropagation()}
					onKeyDown={(event) => event.stopPropagation()}
				>
					{isPending ? <Spinner /> : <DotsThreeOutlineVerticalIcon size={16} />}
					<span className="sr-only">Otwórz menu akcji dla zamówienia {order.number ?? order.id}</span>
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end">
				<DropdownMenuItem asChild>
					<Link params={{ orderId: order.id }} search={{ date: order.date }} to="/orders/$orderId">
						<EyeIcon size={16} />
						Podgląd
					</Link>
				</DropdownMenuItem>
				<DropdownMenuItem
					onSelect={() => generateDeliveryNoteMutation.mutate({ data: { orderId: order.id, date: order.date } })}
				>
					<PrinterIcon size={16} />
					Pobierz
				</DropdownMenuItem>
				<DropdownMenuItem
					onSelect={() =>
						updateOrderExclusionMutation.mutate({
							id: order.id,
							data: { excluded: !order.isExcluded },
						})
					}
					variant={order.isExcluded ? "default" : "destructive"}
				>
					{order.isExcluded ? <ArrowClockwiseIcon size={16} /> : <XIcon size={16} />}
					{order.isExcluded ? "Przywróć" : "Pomiń"}
				</DropdownMenuItem>
			</DropdownMenuContent>
		</DropdownMenu>
	);
}
