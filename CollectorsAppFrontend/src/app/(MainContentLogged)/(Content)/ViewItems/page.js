"use client"
import { useEffect } from "react";
import { useRouter, usePathname } from "next/navigation";
import { useMenuItemsProvider } from "@/app/Providers/MenuProvider/MenuProvider";

export default function ItemView() {
    const { menuItems } = useMenuItemsProvider();
    const params = usePathname();
    const router = useRouter();

    useEffect(() => {
        if (params === "/ViewItems" && Array.isArray(menuItems) && menuItems) {
            const sortedItems = menuItems.filter((item) => item.parentId == 0)
                .sort((a, b) => a.name.localeCompare(b.name))[0]
            if (sortedItems) {
                router.push(`/ViewItems/${sortedItems.name}/${sortedItems.id}`)
            }
            else {
            }
        }
    }, [menuItems])

    return (<div>

    </div>)
}