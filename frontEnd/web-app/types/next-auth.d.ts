import { DefaultSession } from "next-auth";

declare module 'next-auth' {
    interface Session {
        user: {
            userName: string
        } & DefaultSession['user']
    }

    interface Profile {
        userName:string
    }
}

declare module 'next-auth/jwt' {
    interface JWT {
        userName: string
    }
}