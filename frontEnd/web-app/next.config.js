/** @type {import('next').NextConfig} */
// const nextConfig = {
//     // experimental: {
//     //     serverActions: true
//     // },
//     images: {
//         //domains
//         remotePatterns:
//         //domains: 
//         [
//             'cdn.pixabay.com'
//         ]
//     },
//     //created on dockerimage 
//     //output: 'standalone'
// }

const nextConfig = {
    images: {
      remotePatterns: [
        {
          // Define the hostname for the remote pattern
          hostname: 'cdn.pixabay.com', // Replace with your hostname or pattern
          // Add any additional configurations if needed
        },
        // Add additional patterns if needed
      ],
      // Other image configuration options if needed
    },
    // Other Next.js configuration options
  };
  
  module.exports = nextConfig;
  