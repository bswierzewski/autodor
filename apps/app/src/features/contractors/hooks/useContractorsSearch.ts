import { getRouteApi, useNavigate } from "@tanstack/react-router";
import { z } from "zod";

export const contractorsSearchSchema = z.object({
	query: z.string().optional().catch(undefined),
});

type ContractorsSearch = z.infer<typeof contractorsSearchSchema>;

const routeApi = getRouteApi("/_app/contractors");

export function useContractorsSearch() {
	const navigate = useNavigate({ from: routeApi.fullPath });
	const search = routeApi.useSearch();

	const updateSearch = (updates: Partial<ContractorsSearch>) => {
		void navigate({
			replace: true,
			search: (previous) => {
				const next = { ...previous, ...updates };

				if (!next.query?.trim()) delete next.query;

				return next;
			},
		});
	};

	return {
		query: search.query ?? "",
		updateSearch,
	};
}
