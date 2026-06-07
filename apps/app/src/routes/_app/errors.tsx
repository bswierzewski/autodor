import { createFileRoute } from "@tanstack/react-router";
import {
	useCreateValidationModelError,
	useGetBadRequestError,
	useGetDomainExceptionError,
	useGetForbiddenError,
	useGetInternalServerError,
	useGetNotFoundError,
	useGetSecuredError,
	useGetUnauthorizedError,
} from "#/api/errors/errors";

export const Route = createFileRoute("/_app/errors")({
	component: ErrorsRoute,
});

type ErrorPanelState = {
	label: string;
	value: unknown;
	isLoading: boolean;
};

function ErrorsRoute() {
	const validationModel = useCreateValidationModelError(
		{
			Email: "not-an-email",
			Name: "a",
			Quantity: 0,
		},
		{ query: { retry: false } },
	);
	const badRequest = useGetBadRequestError({ query: { retry: false } });
	const domainException = useGetDomainExceptionError({
		query: { retry: false },
	});
	const unauthorized = useGetUnauthorizedError({ query: { retry: false } });
	const secured = useGetSecuredError({ query: { retry: false } });
	const forbidden = useGetForbiddenError({ query: { retry: false } });
	const notFound = useGetNotFoundError({ query: { retry: false } });
	const internalServerError = useGetInternalServerError({
		query: { retry: false },
	});

	const items: ErrorPanelState[] = [
		{
			label: "CreateValidationModelError (AsParameters + FluentValidation)",
			value: validationModel.error ?? validationModel.data ?? { message: "No response" },
			isLoading: validationModel.isLoading,
		},
		{
			label: "GetBadRequestError",
			value: badRequest.error ?? badRequest.data ?? { message: "No response" },
			isLoading: badRequest.isLoading,
		},
		{
			label: "GetDomainExceptionError",
			value: domainException.error ?? domainException.data ?? { message: "No response" },
			isLoading: domainException.isLoading,
		},
		{
			label: "GetUnauthorizedError",
			value: unauthorized.error ?? unauthorized.data ?? { message: "No response" },
			isLoading: unauthorized.isLoading,
		},
		{
			label: "GetSecuredError (requires role errors-debugger)",
			value: secured.error ?? secured.data ?? { message: "No response" },
			isLoading: secured.isLoading,
		},
		{
			label: "GetForbiddenError",
			value: forbidden.error ?? forbidden.data ?? { message: "No response" },
			isLoading: forbidden.isLoading,
		},
		{
			label: "GetNotFoundError",
			value: notFound.error ?? notFound.data ?? { message: "No response" },
			isLoading: notFound.isLoading,
		},
		{
			label: "GetInternalServerError",
			value: internalServerError.error ?? internalServerError.data ?? { message: "No response" },
			isLoading: internalServerError.isLoading,
		},
	];

	return (
		<div className="space-y-6 p-6">
			<div className="space-y-2">
				<h1 className="text-3xl font-semibold">Errors</h1>
				<p className="text-sm opacity-70">This page intentionally calls demo endpoints that return problem details.</p>
			</div>

			<div className="grid gap-4">
				{items.map((item) => (
					<section className="rounded-box border border-base-300 bg-base-100 p-4 shadow-sm" key={item.label}>
						<span className="label-text font-semibold">{item.label}</span>
						<pre className="mt-3 overflow-auto rounded-box bg-base-200 p-4 text-sm">
							{item.isLoading ? "Loading..." : JSON.stringify(item.value, null, 2)}
						</pre>
					</section>
				))}
			</div>
		</div>
	);
}
