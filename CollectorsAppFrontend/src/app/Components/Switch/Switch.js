"use client"

import { useState, useEffect } from 'react';
import Style from './ToggleSwitch.module.css';
import cn from "classnames";
export default function ToggleSwitch({ active, setActive }) {
    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true);
    }, []);

    if (!mounted) return null;
    const handleToggle = () => {
        if (active == "dark") {
            setActive("light");
        }
        if (active == "light") {
            setActive("dark");
        }
    };

    return (
        <div className={cn(Style.toggleContainer)} onClick={handleToggle}>
            <div className={cn(
                Style.toggleSwitch,              
        { [Style.theme]: active === 'dark' }  // only add "theme" when active==="dark"
      )}>
                {/* "toggle-switch ${active == "dark" ? 'theme' : ''}}>*/}
                <div className={cn(Style.toggleKnob)} />
            </div>
        </div>
    );
}