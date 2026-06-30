import { getRouteApi, useNavigate } from "@tanstack/react-router";
import dayjs from "dayjs";
import { z } from "zod";
import { formatDate } from "#/lib/formatters";

function getDefaultDateRange() {
	const to = dayjs().startOf("day");

	return {
		from: formatDate(to.subtract(7, "day").toDate()),
		to: formatDate(to.toDate()),
	};
}

const defaultDateRange = getDefaultDateRange();

export const ordersSearchSchema = z
	.object({
		from: z.iso.date().optional().catch(undefined),
		to: z.iso.date().optional().catch(undefined),
		query: z.string().optional().catch(undefined),
	})
	.transform((search) => {
		const from = search.from ?? defaultDateRange.from;
		const to = search.to ?? defaultDateRange.to;

		return dayjs(from).isAfter(to, "day") ? { ...search, from: undefined, to: undefined } : search;
	});

type OrdersSearch = z.infer<typeof ordersSearchSchema>;

const routeApi = getRouteApi("/_app/");

export function useOrdersSearch() {
	const navigate = useNavigate({ from: routeApi.fullPath });
	const search = routeApi.useSearch();

	const updateSearch = (updates: Partial<OrdersSearch>) => {
		void navigate({
			replace: true,
			search: (previous) => {
				const next = { ...previous, ...updates };

				if (!next.query?.trim()) delete next.query;
				if (next.from === defaultDateRange.from) delete next.from;
				if (next.to === defaultDateRange.to) delete next.to;

				return next;
			},
		});
	};

	return {
		from: search.from ?? defaultDateRange.from,
		to: search.to ?? defaultDateRange.to,
		query: search.query ?? "",
		updateSearch,
	};
}
