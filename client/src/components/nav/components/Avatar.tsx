'use client';

import { LogOut } from 'lucide-react';
import { signOut, useSession } from 'next-auth/react';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import React from 'react';

import { Urls } from '@/config/site';

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger
} from '@/components/ui/dropdown-menu';

export default function Avatar() {
  const session = useSession();
  const router = useRouter();
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Image className="rounded-full" src={session.data?.user.image ?? ''} width={40} height={40} alt="Picture of the author" />
      </DropdownMenuTrigger>
      <DropdownMenuContent className="w-56">
        <DropdownMenuLabel>{session.data?.user.name}</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {Urls.map((url) => (
          <DropdownMenuItem key={url.id} onClick={() => router.push(url.route)}>
            {url.iconSmall}
            <span className="mx-2">{url.label}</span>
          </DropdownMenuItem>
        ))}
        <DropdownMenuSeparator />
        <DropdownMenuItem onClick={() => signOut()}>
          <LogOut className="mr-2 h-4 w-4" />
          <span>Wyloguj</span>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
