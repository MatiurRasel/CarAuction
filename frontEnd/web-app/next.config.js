/** @type {import('next').NextConfig} */
const nextConfig = {
    // experimental: {
    //     serverActions: true
    // },
    images: {
        domains: [
            'cdn.pixabay.com'
        ]
    },
    //created on dockerimage 
    output: 'standalone'
}

module.exports = nextConfig
