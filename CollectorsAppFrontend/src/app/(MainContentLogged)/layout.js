"use client"
import { useSession } from "next-auth/react";
import { MenuProvider } from "../Providers/MobileMenuProvider";
import "./MainLoggedContentLayout.css";
import MenuContent from "../Components/content-menu/content-menu";
import { ToastContainer, Slide } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { MenuItemsProvider } from "../Providers/MenuProvider/MenuProvider";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export default function MainContentLogged({ children }) {
    const { data: session, status } = useSession();
    const router = useRouter();

    useEffect(() => {
        if (status === "unauthenticated") {
            router.push("/Login")
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
            <div className="container">
                <div className="top-menu-item">
                    <MenuContent />
                </div>
                <div className="main-content-layout">
                    <MenuItemsProvider>
                        {children}
                    </MenuItemsProvider>
                </div>
            </div>
        </MenuProvider>
    </>
    )
}