import { LogIn } from 'lucide-react';
import { signIn } from 'next-auth/react';
import React from 'react';

import { Button } from '../../ui/button';

export default function LoginButton() {
  return (
    <Button className="w-full" variant="outline" onClick={() => signIn('auth0', {}, { prompt: 'login' })}>
      <LogIn className="mr-2" />
      Zaloguj
    </Button>
  );
}
