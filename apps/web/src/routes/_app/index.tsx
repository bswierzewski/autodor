import { createFileRoute } from "@tanstack/react-router";
import { OrdersPage } from "#/features/orders/pages/OrdersPage";

export const Route = createFileRoute("/_app/")({
	component: OrdersPage,
});
