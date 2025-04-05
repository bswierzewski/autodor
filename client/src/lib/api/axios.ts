'use client';

import Axios, { AxiosError, AxiosInstance, AxiosRequestConfig } from 'axios';
import { getSession, signOut } from 'next-auth/react';
import toast from 'react-hot-toast';

const axiosInstance: AxiosInstance = Axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL
});

axiosInstance.interceptors.request.use(async (config) => {
  const session = await getSession();
  const token = session?.access_token;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

axiosInstance.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    // Ensure error is an AxiosError before accessing its properties
    if (Axios.isCancel(error)) {
      return Promise.resolve(); // Ignore canceled requests
    }

    const axiosError = error as AxiosError; // Explicitly cast error to AxiosError

    if (!axiosError.response) {
      toast.error('Network error. Please check your connection.');
    } else {
      const { status } = axiosError.response;
      if (status === 401) {
        signOut({ redirect: false });
        toast.error('Session expired. Please log in again.');
      } else if (status >= 500) {
        toast.error('Something went wrong. Please try again later.');
      }
    }

    return Promise.reject(axiosError);
  }
);

export const customInstance = async <T>(config: AxiosRequestConfig, options?: AxiosRequestConfig): Promise<T> => {
  return axiosInstance({
    ...config,
    ...options
  }).then(({ data }) => data);
};
