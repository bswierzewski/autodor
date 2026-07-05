function isJsonContentType(contentType: string | null) {
	return !!contentType && (contentType.includes("application/json") || contentType.includes("+json"));
}

function isTextContentType(contentType: string | null) {
	return !!contentType && (contentType.startsWith("text/") || contentType.includes("xml"));
}

async function parseResponse<T>(response: Response) {
	const contentType = response.headers.get("content-type");
	if (isJsonContentType(contentType)) {
		const body = await response.text();
		return (body ? JSON.parse(body) : null) as T;
	}

	if (isTextContentType(contentType)) {
		return (await response.text()) as T;
	}

	if (contentType) {
		return (await response.blob()) as T;
	}

	const body = await response.text();
	return (body || null) as T;
}

export const customFetch = async <T>(url: string, options: RequestInit = {}): Promise<T> => {
	const token = await window.Clerk?.session?.getToken();

	const headers = new Headers(options.headers);
	if (token) {
		headers.set("Authorization", `Bearer ${token}`);
	}

	const response = await fetch(url, { ...options, headers });

	if ([204, 205, 304].includes(response.status)) {
		return null as T;
	}

	const data = await parseResponse<unknown>(response);

	if (!response.ok) {
		throw data;
	}

	return data as T;
};
