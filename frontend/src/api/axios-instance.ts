import Axios, { type AxiosRequestConfig } from "axios";

export const AXIOS_INSTANCE = Axios.create({
	baseURL: import.meta.env.VITE_API_URL || "http://localhost:5000",
});

export const customInstance = <T>(
	config: AxiosRequestConfig,
	options?: AxiosRequestConfig,
): Promise<T> => {
	const source = Axios.CancelToken.source();
	const promise = AXIOS_INSTANCE({
		...config,
		...options,
		cancelToken: source.token,
	}).then(({ data }) => data);

	// @ts-expect-error - adding cancel method
	promise.cancel = () => {
		source.cancel("Query was cancelled");
	};

	return promise;
};
