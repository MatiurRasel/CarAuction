import { getServerSession } from "next-auth";
import { authOptions } from "../api/auth/[...nextauth]/route";
import { headers } from 'next/headers';
export async function getSession() {
    return await getServerSession(authOptions);
}

export async function getCurrentUser() {
    try {
        const session = await getSession();
        if(!session) return null;

        return session.user;
    }
    catch(error) {
        return null;
    }
}

export async function getTokenWorkAround() {
    const req = {
        headers: Object.fromEntries(headers() as Headers)
    }
}