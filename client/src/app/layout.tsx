import Guard from './Guard';
import { Providers } from './providers';
import type { Metadata } from 'next';
import { NextIntlClientProvider } from 'next-intl';
import { getLocale } from 'next-intl/server';

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
                      Wytworzone przez <strong className="text-primary">{siteConfig.company}</strong>
                      {`${siteConfig.version} - ${process.env.ENVIRONMENT}`}
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
