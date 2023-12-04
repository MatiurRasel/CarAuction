import { DefaultSession } from "next-auth";

declare module 'next-auth' {
    interface Session {
        user: {
            id: string
            userName: string
        } & DefaultSession['user']
    }

    interface Profile {
        userName:string
    }

    interface User {
        userName: string
    }
}

declare module 'next-auth/jwt' {
    interface JWT {
        userName: string
        access_token?: string
    }
}