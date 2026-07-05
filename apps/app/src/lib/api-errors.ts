import type { HttpValidationProblemDetails } from "#/api/models";

export function getProblemDetailsMessages(
	problemDetails: HttpValidationProblemDetails | null | undefined,
	fallbackMessage: string,
): string[] {
	if (!problemDetails) {
		return [];
	}

	if (problemDetails.errors) {
		const messages = Object.values(problemDetails.errors).flat();

		if (messages.length > 0) {
			return messages;
		}
	}

	return [problemDetails.detail ?? problemDetails.title ?? fallbackMessage];
}
