"use client"

import { useRouter } from "next/navigation"

    
export function TestNav(nav){
    const router = useRouter();
    router.push("/Logout");
}