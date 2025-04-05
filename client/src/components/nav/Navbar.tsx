'use client';

import { useSession } from 'next-auth/react';

import Avatar from './components/Avatar';
import LoginButton from './components/LoginButton';
import Logo from './components/Logo';
import { ThemePicker } from './components/ThemePicker';

type Props = {
  className?: string;
};

export default function Navbar({ className }: Props) {
  const session = useSession();

  return (
    <div className={`${className} flex justify-center sticky top-0 p-4 shadow-lg bg-background z-40`}>
      <div className="flex w-full items-center justify-between">
        <Logo size={115} />
        <div className="flex gap-2">
          <ThemePicker />
          {session.status == 'authenticated' ? <Avatar /> : <LoginButton />}
        </div>
      </div>
    </div>
  );
}
