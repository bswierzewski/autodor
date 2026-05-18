export const customFetch = async <T>(
	url: string,
	options: RequestInit = {},
): Promise<T> => {
	const token = await window.Clerk?.session?.getToken();

	const headers = new Headers(options.headers);
	if (token) {
		headers.set("Authorization", `Bearer ${token}`);
	}

	const response = await fetch(url, { ...options, headers });

	if ([204, 205, 304].includes(response.status)) {
		return null as T;
	}

	const body = await response.text();
	const data = body ? JSON.parse(body) : null;

	if (!response.ok) {
		throw data;
	}

	return data as T;
};
