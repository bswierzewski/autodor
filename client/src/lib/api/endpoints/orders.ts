/**
 * Generated by orval v7.8.0 🍺
 * Do not edit manually.
 * Web
 * OpenAPI spec version: 1.0
 */
import { useMutation, useQuery } from '@tanstack/react-query';
import type {
  DataTag,
  DefinedInitialDataOptions,
  DefinedUseQueryResult,
  MutationFunction,
  QueryClient,
  QueryFunction,
  QueryKey,
  UndefinedInitialDataOptions,
  UseMutationOptions,
  UseMutationResult,
  UseQueryOptions,
  UseQueryResult
} from '@tanstack/react-query';

import { customInstance } from '../axios';
import type { ExcludeOrderCommand, GetOrdersParams, OrderDto, Void } from '../models';

type SecondParameter<T extends (...args: never) => unknown> = Parameters<T>[1];

export const getOrders = (
  params: GetOrdersParams,
  options?: SecondParameter<typeof customInstance>,
  signal?: AbortSignal
) => {
  return customInstance<OrderDto[]>({ url: `/api/Orders`, method: 'GET', params, signal }, options);
};

export const getGetOrdersQueryKey = (params: GetOrdersParams) => {
  return [`/api/Orders`, ...(params ? [params] : [])] as const;
};

export const getGetOrdersQueryOptions = <TData = Awaited<ReturnType<typeof getOrders>>, TError = unknown>(
  params: GetOrdersParams,
  options?: {
    query?: Partial<UseQueryOptions<Awaited<ReturnType<typeof getOrders>>, TError, TData>>;
    request?: SecondParameter<typeof customInstance>;
  }
) => {
  const { query: queryOptions, request: requestOptions } = options ?? {};

  const queryKey = queryOptions?.queryKey ?? getGetOrdersQueryKey(params);

  const queryFn: QueryFunction<Awaited<ReturnType<typeof getOrders>>> = ({ signal }) =>
    getOrders(params, requestOptions, signal);

  return { queryKey, queryFn, ...queryOptions } as UseQueryOptions<
    Awaited<ReturnType<typeof getOrders>>,
    TError,
    TData
  > & { queryKey: DataTag<QueryKey, TData> };
};

export type GetOrdersQueryResult = NonNullable<Awaited<ReturnType<typeof getOrders>>>;
export type GetOrdersQueryError = unknown;

export function useGetOrders<TData = Awaited<ReturnType<typeof getOrders>>, TError = unknown>(
  params: GetOrdersParams,
  options: {
    query: Partial<UseQueryOptions<Awaited<ReturnType<typeof getOrders>>, TError, TData>> &
      Pick<
        DefinedInitialDataOptions<Awaited<ReturnType<typeof getOrders>>, TError, Awaited<ReturnType<typeof getOrders>>>,
        'initialData'
      >;
    request?: SecondParameter<typeof customInstance>;
  },
  queryClient?: QueryClient
): DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData> };
export function useGetOrders<TData = Awaited<ReturnType<typeof getOrders>>, TError = unknown>(
  params: GetOrdersParams,
  options?: {
    query?: Partial<UseQueryOptions<Awaited<ReturnType<typeof getOrders>>, TError, TData>> &
      Pick<
        UndefinedInitialDataOptions<
          Awaited<ReturnType<typeof getOrders>>,
          TError,
          Awaited<ReturnType<typeof getOrders>>
        >,
        'initialData'
      >;
    request?: SecondParameter<typeof customInstance>;
  },
  queryClient?: QueryClient
): UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData> };
export function useGetOrders<TData = Awaited<ReturnType<typeof getOrders>>, TError = unknown>(
  params: GetOrdersParams,
  options?: {
    query?: Partial<UseQueryOptions<Awaited<ReturnType<typeof getOrders>>, TError, TData>>;
    request?: SecondParameter<typeof customInstance>;
  },
  queryClient?: QueryClient
): UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData> };

export function useGetOrders<TData = Awaited<ReturnType<typeof getOrders>>, TError = unknown>(
  params: GetOrdersParams,
  options?: {
    query?: Partial<UseQueryOptions<Awaited<ReturnType<typeof getOrders>>, TError, TData>>;
    request?: SecondParameter<typeof customInstance>;
  },
  queryClient?: QueryClient
): UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData> } {
  const queryOptions = getGetOrdersQueryOptions(params, options);

  const query = useQuery(queryOptions, queryClient) as UseQueryResult<TData, TError> & {
    queryKey: DataTag<QueryKey, TData>;
  };

  query.queryKey = queryOptions.queryKey;

  return query;
}

export const excludeOrder = (
  excludeOrderCommand: ExcludeOrderCommand,
  options?: SecondParameter<typeof customInstance>,
  signal?: AbortSignal
) => {
  return customInstance<Void>(
    {
      url: `/api/Orders`,
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      data: excludeOrderCommand,
      signal
    },
    options
  );
};

export const getExcludeOrderMutationOptions = <TError = unknown, TContext = unknown>(options?: {
  mutation?: UseMutationOptions<
    Awaited<ReturnType<typeof excludeOrder>>,
    TError,
    { data: ExcludeOrderCommand },
    TContext
  >;
  request?: SecondParameter<typeof customInstance>;
}): UseMutationOptions<Awaited<ReturnType<typeof excludeOrder>>, TError, { data: ExcludeOrderCommand }, TContext> => {
  const mutationKey = ['excludeOrder'];
  const { mutation: mutationOptions, request: requestOptions } = options
    ? options.mutation && 'mutationKey' in options.mutation && options.mutation.mutationKey
      ? options
      : { ...options, mutation: { ...options.mutation, mutationKey } }
    : { mutation: { mutationKey }, request: undefined };

  const mutationFn: MutationFunction<Awaited<ReturnType<typeof excludeOrder>>, { data: ExcludeOrderCommand }> = (
    props
  ) => {
    const { data } = props ?? {};

    return excludeOrder(data, requestOptions);
  };

  return { mutationFn, ...mutationOptions };
};

export type ExcludeOrderMutationResult = NonNullable<Awaited<ReturnType<typeof excludeOrder>>>;
export type ExcludeOrderMutationBody = ExcludeOrderCommand;
export type ExcludeOrderMutationError = unknown;

export const useExcludeOrder = <TError = unknown, TContext = unknown>(
  options?: {
    mutation?: UseMutationOptions<
      Awaited<ReturnType<typeof excludeOrder>>,
      TError,
      { data: ExcludeOrderCommand },
      TContext
    >;
    request?: SecondParameter<typeof customInstance>;
  },
  queryClient?: QueryClient
): UseMutationResult<Awaited<ReturnType<typeof excludeOrder>>, TError, { data: ExcludeOrderCommand }, TContext> => {
  const mutationOptions = getExcludeOrderMutationOptions(options);

  return useMutation(mutationOptions, queryClient);
};
