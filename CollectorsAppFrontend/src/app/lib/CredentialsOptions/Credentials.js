import axios from "axios";
import CredentialsProvider from "next-auth/providers/credentials";
import setCookieParser from "set-cookie-parser"
import { cookies } from "next/headers";
import instance from "@/app/axiosInstance";

export const authOptions = {
    providers: [
        CredentialsProvider({
            name: "Credentials",
            credentials: {
                username: { label: "Username", type: "text", placeholder: "Nazwa użytkownika" },
                password: { label: "Password", type: "password", placeholder: "Hasło" },
            },
            async authorize(credentials, req) {

                const header = req.headers['cookie']
                try {
                    const response = await instance.post("api/Authentication", {
                        "name": credentials.username,
                        "password": credentials.password
                    }, {
                        headers: {
                            'Cookie': header
                        }});
                    if (response.status === 200) {
                        const data = setCookieParser(response.headers["set-cookie"])
                        const cookieStore = await cookies()
                        await data.forEach(element => {
                            const expiresDate = new Date(element.expires);
                            cookieStore.set({
                                name: element.name,
                                value: element.value,
                                expires: expiresDate,
                                path: element.path,
                                secure: element.secure,
                                sameSite: element.sameSite,
                                httpOnly: element.httpOnly
                            });
                        });
                        return response.data
                    }
                } catch (err) {
                    console.log("catch", err)
                }
                return null
            }
        },
        )],
    session: {
        strategy: "jwt",
        maxAge: 30 * 24 * 60 * 60
    },
    jwt: {
        maxAge: 30 * 24 * 60 * 60,
    },
    callbacks: {
        async jwt({ token, user }) {
            if (user) {
                token.id = user.id;
                token.email = user.email;
                token.name = user.name;
            }
            return token;
        },
        async session({ session, token }) {
            session.user = {
                id: token.id,
                name: token.name,
                email: token.email,
            };
            return session;
        },
    },
}