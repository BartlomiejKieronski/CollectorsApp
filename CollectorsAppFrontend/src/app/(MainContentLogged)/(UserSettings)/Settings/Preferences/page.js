"use client"
import { useState, useEffect } from "react";
import Style from "./Preferences.module.css";
import ToggleSwitch from "@/app/Components/Switch/Switch";
import { useTheme } from "next-themes";
import cn from "classnames";

export default function Preferencje() {
    const { theme, setTheme } = useTheme();
    const [mounted, setMounted] = useState(false);
    
    useEffect(() => {
        setMounted(true);
    }, []);

    if (!mounted) return null;

    
    return (
        <div className={cn(Style.preferencesStyleContainer)}>
            <div>
                <div>Motyw:</div>
                <div>
                    {theme == "dark" && <div>Ciemny</div>}
                    {theme == "light" && <div>Jasny</div>}
                    <div>
                        {theme && <ToggleSwitch setActive={setTheme} active={theme} />}
                    </div>
                </div>
            </div>
        </div>
    )
}