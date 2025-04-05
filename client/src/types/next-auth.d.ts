import { DefaultSession } from 'next-auth';

declare module 'next-auth' {
  interface Session {
    access_token: string | undefined;
    user: {
      id: string;
      username: string;
    } & DefaultSession['user'];
  }

  interface Profile {
    username: string;
  }

  interface User {
    username: string;
  }
}

declare module 'next-auth/jwt' {
  interface JWT {
    username: string;
    access_token?: string;
    expires_at?: number;
  }
}
