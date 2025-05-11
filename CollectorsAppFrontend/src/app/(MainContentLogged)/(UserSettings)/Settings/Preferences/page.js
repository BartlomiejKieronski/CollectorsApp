"use client"
import { useState, useEffect } from "react"
import "./Preferences.css"
import ToggleSwitch from "@/app/Components/Switch/Switch"
import { useTheme } from "next-themes"

export default function Preferencje() {
    const { theme, setTheme } = useTheme();
    const [mounted, setMounted] = useState(false);
    
    useEffect(() => {
        setMounted(true);
    }, []);

    if (!mounted) return null;

    
    return (

        <div className="preferences-style-container">
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