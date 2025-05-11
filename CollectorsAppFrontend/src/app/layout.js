import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import SessionProviderWrapper from "@/app/Providers/SessionWrapper.js"
import Theme from "@/app/Providers/provider.js"
import { getServerSession } from "next-auth/next";
import { authOptions } from "./lib/CredentialsOptions/Credentials";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata = {
  description: "Collectors App",
};

export default async function RootLayout({ children }) {
  const session = await getServerSession(authOptions);
  return (
    <html lang="pl" suppressHydrationWarning>
      <body className={`${geistSans.variable} ${geistMono.variable}`}>
        <Theme>
          <SessionProviderWrapper session={session}>
            {children}
          </SessionProviderWrapper>
        </Theme>
      </body>
    </html>
  );
}