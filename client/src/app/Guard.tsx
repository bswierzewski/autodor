'use client';

import { signIn, useSession } from 'next-auth/react';

import Logo from '@/components/nav/components/Logo';
import { Button } from '@/components/ui/button';

export default function Guard({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  const session = useSession();

  if (session.status != 'authenticated')
    return (
      <div className="flex justify-center">
        <div className="h-[40vh] mt-[5vh] w-[80vw] flex flex-col gap-2 justify-center items-center shadow-lg">
          <div className="mb-10">
            <Logo size={200} />
          </div>
          <div className="text-center">
            {session.status === 'unauthenticated' ? (
              <>
                <div className="text-2xl font-bold">Musisz być zalogowany by korzystać z tej strony</div>
                <div className="mt-2">Kliknij poniżej by się zalogować</div>
                <div className="mt-4">
                  <Button variant={'outline'} onClick={() => signIn('auth0', {}, { prompt: 'login' })}>
                    Zaloguj
                  </Button>
                </div>
              </>
            ) : (
              <>
                <div className="text-2xl font-bold">Inicjalizacja sesji</div>
                <div className="mt-2">Proszę czekać ...</div>
              </>
            )}
          </div>
        </div>
      </div>
    );

  return children;
}
