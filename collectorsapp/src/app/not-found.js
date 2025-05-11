"use client"
import { useSession } from "next-auth/react"
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import ContentMenu from "@/app/Components/content-menu/content-menu.js"
import "./NotFound.css"
import { MenuProvider } from "@/app/Providers/MobileMenuProvider"

export default function notFound() {
    const { data: session, status } = useSession();
    
    const router = useRouter();
    
    useEffect(() => {
        if (status == "unauthenticated") {
            router.push("/Login")
        }
    }, [status])

    if (status == "authenticated") {
        return (
            <div className="not-found-page">
                <MenuProvider>
                    <ContentMenu />
                </MenuProvider>
                <div className="not-found-content">
                    <div className="fourofour">404</div>
                    <div className="not-found">Nie znaleziono</div> 
                </div>

            </div>)
    }
}