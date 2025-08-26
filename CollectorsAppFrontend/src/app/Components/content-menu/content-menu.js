"use client";
import Image from "next/image";
import Style from "./content-menu.module.css";
import image from "@/../public/image.png";
import { useMenu } from "@/app/Providers/MobileMenuProvider";
import { usePathname, useRouter } from "next/navigation";
import { signOut } from "next-auth/react";
import { useState, useRef, useEffect } from "react";
import Link from "next/link";
import cn from "classnames";

export default function MenuContent() {

  const { toggleMenu } = useMenu();
  const path = usePathname();
  const router = useRouter();
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const urlContainsItemsView = path.includes("ViewItems");

  const userToggleRef = useRef(null);
  const userSubmenuRef = useRef(null);
  const menuIconRef = useRef(null);
  const menuContentEndRef = useRef(null);

  useEffect(() => {
    const handleClickOutsideUserMenu = (event) => {
      if (
        isUserMenuOpen &&
        userToggleRef.current &&
        !userToggleRef.current.contains(event.target) &&
        userSubmenuRef.current &&
        !userSubmenuRef.current.contains(event.target)
      ) {
        setIsUserMenuOpen(false);
      }
    };

    document.addEventListener("click", handleClickOutsideUserMenu);
    return () => document.removeEventListener("click", handleClickOutsideUserMenu);
  }, [isUserMenuOpen]);

  useEffect(() => {
    const handleClickOutsideMobileMenu = (event) => {
      if (
        isMobileMenuOpen &&
        menuIconRef.current &&
        !menuIconRef.current.contains(event.target) &&
        menuContentEndRef.current &&
        !menuContentEndRef.current.contains(event.target)
      ) {
        setIsMobileMenuOpen(false);
      }
    };

    document.addEventListener("click", handleClickOutsideMobileMenu);
    return () => document.removeEventListener("click", handleClickOutsideMobileMenu);
  }, [isMobileMenuOpen]);

  return (
    <div className={cn(Style.menuContent)}>
      <div className={cn(Style.menuContentStart)}>
        {urlContainsItemsView && (
          <button className={cn(Style.hamburgerSideMenu)} onClick={toggleMenu}>
            ☰
          </button>
        )}
        <div className={cn(Style.menuContentLogo)} tabIndex={0}>
          <Link href={"/ViewItems"}>
            <div className={cn(Style.menuContentLogo)}>
              <Image
                src={image}
                width={50}
                height={50}
                alt="App Logo"
                className={cn(Style.logoImage)}
              />
              <div className={cn(Style.appName)} >App Name</div>
            </div>
          </Link>
        </div>
      </div>
      <button
        ref={menuIconRef}
        className={cn(Style.menuIcon)}
        onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
      >
        ☰
      </button>
      <div ref={menuContentEndRef} className={cn(Style.menuContentEnd,
        {
          [Style.mobileMenuOpen]: isMobileMenuOpen
        })
      }>
        <ul className={cn(Style.ulMenu)}>
          <li tabIndex={0} role="button"><Link href={"/ViewItems"}>Kolekcje</Link></li>
          <li className={cn(Style.userMenu)} tabIndex={0} role="button" onKeyDown={(e) => {
            if (e.key === "Enter") {
              setIsUserMenuOpen(!isUserMenuOpen);
            }
          }}>
            <span ref={userToggleRef} className={cn(Style.userToggle)}
              onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}>
              Użytkownik
            </span>
            <ul
              ref={userSubmenuRef}
              className={cn(Style.userSubmenu, { [Style.open]: isUserMenuOpen })
              }>
              <li tabIndex={0} role="button"><Link href={"/Settings/Preferences"}>Preferencje</Link></li>
              <li tabIndex={0} role="button"><Link href={"/Settings/Account"}>Konto</Link></li>
              <li className={cn(Style.logoutItem)}>
                <button
                  className={cn(Style.lgBt)}
                  onClick={() => {
                    router.push("/Logout/Logged-Out");
                  }}
                >
                  <div className={cn(Style.ulFlex, Style.flGap)}>
                    <span className={cn(Style.btTxt)}>Wyloguj się</span>
                    <span className={cn(Style.imgLt)}>
                      <Image
                        className={cn(Style.lgImg, Style.svgCl, 'icon')}
                        src="/logout.svg"
                        width={20}
                        height={20}
                        alt="logout"
                      />
                    </span>
                  </div>
                </button>
              </li>
            </ul>
          </li>
        </ul>
      </div>
    </div>
  );
}