"use client"
import { usePathname, useRouter } from "next/navigation";
import Style from "./SettingsLayout.module.css";
import { useEffect } from "react";
import Link from "next/link";
import cn from "classnames";

export default function UserSettings({ children }) {
    const path = usePathname();
    const router = useRouter();
    
    useEffect(() => {
        if (path == "/Settings") {
            router.push("/Settings/Account")
        }
    }, [path])
    
    return (
        <div className={cn(Style.SettingsContainer)}>
            <div>
                <div><Link href={"/Settings/Account"}>Konto</Link></div>
                <div><Link href={"/Settings/Preferences"}>Preferencje</Link></div>
            </div>
            <hr />
            <div>
                {children}
            </div>
        </div>
    )
}