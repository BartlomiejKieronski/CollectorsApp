"use client";
import Image from "next/image";
import "./content-menu.css";
import image from "@/../public/image.png";
import { useMenu } from "@/app/Providers/MobileMenuProvider";
import { usePathname, useRouter } from "next/navigation";
import { signOut } from "next-auth/react";
import { useState, useRef, useEffect } from "react";
import Link from "next/link";
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
    <div className="menu-content">
      <div className="menu-content-start">
        {urlContainsItemsView && (
          <button className="hamburger-side-menu" onClick={toggleMenu}>
            ☰
          </button>
        )}
        <div className="menu-content-logo" tabIndex={0}>
          <Link href={"/ViewItems"}>
            <div className="menu-content-logo">
              <Image
                src={image}
                width={50}
                height={50}
                alt="App Logo"
                className="logo-image"
              />
              <div className="app-name" >App Name</div>
            </div>
          </Link>
        </div>
      </div>
      <button
        ref={menuIconRef}
        className="menu-icon"
        onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
      >
        ☰
      </button>

      <div ref={menuContentEndRef} className={`menu-content-end ${!isMobileMenuOpen ? "" : "mobile-menu-open"}`}>
        <ul className="ul-menu">
          <li tabIndex={0} role="button"><Link href={"/ViewItems"}>Kolekcje</Link></li>
          <li className="user-menu" tabIndex={0} role="button" onKeyDown={(e) => {
            if (e.key === "Enter") {
              setIsUserMenuOpen(!isUserMenuOpen);
            }
          }}>
            <span ref={userToggleRef} className="user-toggle"
              onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}>
              Użytkownik
            </span>
            <ul
              ref={userSubmenuRef}
              className={`user-submenu ${isUserMenuOpen ? "open" : ""}`}
            >
              <li tabIndex={0} role="button"><Link href={"/Settings/Preferences"}>Preferencje</Link></li>
              <li tabIndex={0} role="button"><Link href={"/Settings/Account"}>Konto</Link></li>
              <li className="logout-item">
                <button
                  className="lg-bt"
                  onClick={() => {
                    signOut({ redirect: false }).then(() => {
                      router.push("/Login");
                    });
                  }}
                >
                  <div className="ul-flex fl-gap">
                    <span className="bt-txt">Wyloguj się</span>
                    <span className="img-lt">
                      <img
                        className="lg-img svg-cl"
                        src="/logout.svg"
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