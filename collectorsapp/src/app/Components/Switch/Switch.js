"use client"

import { useState, useEffect } from 'react';
import './ToggleSwitch.css';

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
        <div className="toggle-container" onClick={handleToggle}>
            <div className={`toggle-switch ${active == "dark" ? 'theme' : ''}`}>
                <div className="toggle-knob" />
            </div>
        </div>
    );
}