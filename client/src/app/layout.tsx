import Guard from './Guard';
import { Providers } from './providers';
import { APP_VERSION, BUILD_DATE } from '@/lib/appVersion';
import type { Metadata } from 'next';
import { NextIntlClientProvider } from 'next-intl';
import { getLocale } from 'next-intl/server';
import Link from 'next/link';

import { siteConfig } from '@/config/site';

import Navbar from '@/components/nav/Navbar';
import Sidebar from '@/components/nav/Sidebar';

import '../styles/globals.css';

export const metadata: Metadata = {
  title: siteConfig.name,
  description: siteConfig.description
};

export default async function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  const locale = await getLocale();
  const buildDateFormatted = BUILD_DATE
    ? new Date(BUILD_DATE).toLocaleString('pl-PL', {
        day: '2-digit',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      })
    : 'Nieznana';

  return (
    <html suppressHydrationWarning lang={locale}>
      <body className="min-h-screen">
        <NextIntlClientProvider>
          <Providers>
            <Guard>
              <div className="flex flex-col md:flex-row w-full min-h-screen">
                <Navbar className="md:hidden" />
                <Sidebar className="hidden md:flex" />
                <div className="flex flex-col flex-1">
                  <main className="p-10 flex-1">{children}</main>
                  <footer className="flex items-center justify-center py-3">
                    <span className="text-default-600">
                      <Link href={'/about'}>Informacje o aplikacji</Link>
                    </span>
                  </footer>
                </div>
              </div>
            </Guard>
          </Providers>
        </NextIntlClientProvider>
      </body>
    </html>
  );
}
