/** @type {import('next').NextConfig} */

const nextConfig = {
  images: {
    remotePatterns: [
      { protocol: 'https', hostname: 's.gravatar.com' },
      { protocol: 'http', hostname: 'res.cloudinary.com' }
    ]
  },
  output: 'standalone',
  async redirects() {
    return [
      {
        source: '/',
        destination: '/dashboard',
        permanent: false
      }
    ];
  }
};

export default nextConfig;
