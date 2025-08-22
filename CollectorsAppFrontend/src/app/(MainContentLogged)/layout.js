"use client"

import { useSession } from "next-auth/react";
import { MenuProvider } from "../Providers/MobileMenuProvider";
import Style from "./MainLoggedContentLayout.module.css";
import MenuContent from "../Components/content-menu/content-menu";
import { ToastContainer, Slide } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { MenuItemsProvider } from "../Providers/MenuProvider/MenuProvider";
import { useRouter, useParams, usePathname } from "next/navigation";
import { useEffect } from "react";
import cn from "classnames"

export default function MainContentLogged({ children }) {
    const { data: session, status } = useSession();
    const router = useRouter();
    const params = useParams();
    const pathname = usePathname();

    useEffect(() => {
        if (status === "unauthenticated") {
            const segments = params.LogoutMessage;
            if (Array.isArray(segments) && segments[0] != null) {
                router.push(`/Login/${segments[0]}`)
            }
            else if(pathname.includes("/Logout")){
                router.push("/Login/Logged-Out")
            }
            else {
                router.push("/Login")
            }
        }
    }, [status])

    if (status === "loading") {
        return (<></>)
    }

    if (status === "unauthenticated") return (<></>)

    return (<>
        <ToastContainer
            position="bottom-right"
            autoClose={5000}
            hideProgressBar
            newestOnTop={false}
            closeOnClick
            rtl={false}
            pauseOnFocusLoss
            draggable={false}
            pauseOnHover={false}
            theme="dark"
            transition={Slide}
        />
        <MenuProvider>
            <div className={cn(Style.container)}>
                <div className={cn(Style.topMenuItem)}>
                    <MenuContent />
                </div>
                <div className={cn(Style.mainContentLayout)}>
                    <MenuItemsProvider>
                        {children}
                    </MenuItemsProvider>
                </div>
            </div>
        </MenuProvider>
    </>
    )
}