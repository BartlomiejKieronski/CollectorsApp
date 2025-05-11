"use client"

import { useSession } from "next-auth/react"
import { useParams, useRouter } from "next/navigation"
import { useEffect, useState } from "react"
import { useMenuItemsProvider } from "../Providers/MenuProvider/MenuProvider";

export default function Home() {
  const {menuItems,error } = useMenuItemsProvider();
  const router = useRouter();
  const params = useParams();
  const { data: session, status } = useSession();
  useEffect(() => {
    let isActive = true;
    
    if (isActive) {
      router.push("/ViewItems")
    }
    return () => {
      isActive = false
    }
  }, [params])

  return (
    <div>
    </div>
  )
}
