/**
 * Generated by orval v7.8.0 🍺
 * Do not edit manually.
 * Web
 * OpenAPI spec version: 1.0
 */
import { useMutation } from '@tanstack/react-query';
import type { MutationFunction, QueryClient, UseMutationOptions, UseMutationResult } from '@tanstack/react-query';

import { customInstance } from '../axios';
import type { CreateInvoiceCommand, InvoiceResponseDto, PrintInvoiceCommand } from '../models';

type SecondParameter<T extends (...args: never) => unknown> = Parameters<T>[1];

export const printInvoice = (
  printInvoiceCommand: PrintInvoiceCommand,
  options?: SecondParameter<typeof customInstance>,
  signal?: AbortSignal
) => {
  return customInstance<Blob>(
    {
      url: `/api/Invoices/PrintInvoice`,
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      data: printInvoiceCommand,
      responseType: 'blob',
      signal
    },
    options
  );
};

export const getPrintInvoiceMutationOptions = <TError = unknown, TContext = unknown>(options?: {
  mutation?: UseMutationOptions<
    Awaited<ReturnType<typeof printInvoice>>,
    TError,
    { data: PrintInvoiceCommand },
    TContext
  >;
  request?: SecondParameter<typeof customInstance>;
}): UseMutationOptions<Awaited<ReturnType<typeof printInvoice>>, TError, { data: PrintInvoiceCommand }, TContext> => {
  const mutationKey = ['printInvoice'];
  const { mutation: mutationOptions, request: requestOptions } = options
    ? options.mutation && 'mutationKey' in options.mutation && options.mutation.mutationKey
      ? options
      : { ...options, mutation: { ...options.mutation, mutationKey } }
    : { mutation: { mutationKey }, request: undefined };

  const mutationFn: MutationFunction<Awaited<ReturnType<typeof printInvoice>>, { data: PrintInvoiceCommand }> = (
    props
  ) => {
    const { data } = props ?? {};

    return printInvoice(data, requestOptions);
  };

  return { mutationFn, ...mutationOptions };
};

export type PrintInvoiceMutationResult = NonNullable<Awaited<ReturnType<typeof printInvoice>>>;
export type PrintInvoiceMutationBody = PrintInvoiceCommand;
export type PrintInvoiceMutationError = unknown;

export const usePrintInvoice = <TError = unknown, TContext = unknown>(
  options?: {
    mutation?: UseMutationOptions<
      Awaited<ReturnType<typeof printInvoice>>,
      TError,
      { data: PrintInvoiceCommand },
      TContext
    >;
    request?: SecondParameter<typeof customInstance>;
  },
  queryClient?: QueryClient
): UseMutationResult<Awaited<ReturnType<typeof printInvoice>>, TError, { data: PrintInvoiceCommand }, TContext> => {
  const mutationOptions = getPrintInvoiceMutationOptions(options);

  return useMutation(mutationOptions, queryClient);
};
export const createInvoice = (
  createInvoiceCommand: CreateInvoiceCommand,
  options?: SecondParameter<typeof customInstance>,
  signal?: AbortSignal
) => {
  return customInstance<InvoiceResponseDto>(
    {
      url: `/api/Invoices`,
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      data: createInvoiceCommand,
      signal
    },
    options
  );
};

export const getCreateInvoiceMutationOptions = <TError = unknown, TContext = unknown>(options?: {
  mutation?: UseMutationOptions<
    Awaited<ReturnType<typeof createInvoice>>,
    TError,
    { data: CreateInvoiceCommand },
    TContext
  >;
  request?: SecondParameter<typeof customInstance>;
}): UseMutationOptions<Awaited<ReturnType<typeof createInvoice>>, TError, { data: CreateInvoiceCommand }, TContext> => {
  const mutationKey = ['createInvoice'];
  const { mutation: mutationOptions, request: requestOptions } = options
    ? options.mutation && 'mutationKey' in options.mutation && options.mutation.mutationKey
      ? options
      : { ...options, mutation: { ...options.mutation, mutationKey } }
    : { mutation: { mutationKey }, request: undefined };

  const mutationFn: MutationFunction<Awaited<ReturnType<typeof createInvoice>>, { data: CreateInvoiceCommand }> = (
    props
  ) => {
    const { data } = props ?? {};

    return createInvoice(data, requestOptions);
  };

  return { mutationFn, ...mutationOptions };
};

export type CreateInvoiceMutationResult = NonNullable<Awaited<ReturnType<typeof createInvoice>>>;
export type CreateInvoiceMutationBody = CreateInvoiceCommand;
export type CreateInvoiceMutationError = unknown;

export const useCreateInvoice = <TError = unknown, TContext = unknown>(
  options?: {
    mutation?: UseMutationOptions<
      Awaited<ReturnType<typeof createInvoice>>,
      TError,
      { data: CreateInvoiceCommand },
      TContext
    >;
    request?: SecondParameter<typeof customInstance>;
  },
  queryClient?: QueryClient
): UseMutationResult<Awaited<ReturnType<typeof createInvoice>>, TError, { data: CreateInvoiceCommand }, TContext> => {
  const mutationOptions = getCreateInvoiceMutationOptions(options);

  return useMutation(mutationOptions, queryClient);
};
