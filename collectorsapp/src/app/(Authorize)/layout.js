"use client"
import { useSession } from "next-auth/react"
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export default function AuthorizeLayout({ children }) {
  const { data: session, status } = useSession();
  const router = useRouter()
  useEffect(() => {
    if (status === 'authenticated') {
      router.push('/');
    }
  }, [status]);
  if (status === "loading") {
    return;
  } else if (status === "unauthenticated") {
    return (<div style={{width:"100vw", height:"100vh"}}>{children}</div>)
  }
}