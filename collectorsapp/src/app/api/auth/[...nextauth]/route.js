"use server"
import NextAuth from "next-auth";
import { authOptions } from "@/app/lib/CredentialsOptions/Credentials";

const handler = NextAuth(authOptions);

export async function GET(req, res) {
  return handler(req, res);
}

export async function POST(req, res) {
  return handler(req, res);
}
