import NextAuth from 'next-auth';
import Auth0 from 'next-auth/providers/auth0';

export const { handlers, signIn, signOut, auth } = NextAuth({
  providers: [
    Auth0({
      clientId: process.env.AUTH_AUTH0_ID,
      clientSecret: process.env.AUTH_AUTH0_SECRET,
      issuer: process.env.AUTH_AUTH0_ISSUER,
      authorization: {
        params: {
          audience: process.env.AUTH_AUTH0_AUDIENCE
        }
      }
    })
  ],
  callbacks: {
    async jwt({ token, account }) {
      if (account) token.access_token = account.access_token;

      return token;
    },
    async session({ session, token }) {
      if (token) session.access_token = token.access_token as string;
      return session;
    }
  }
});
