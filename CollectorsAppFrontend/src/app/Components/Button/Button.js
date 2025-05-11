"use client"
import { Ring2 } from 'ldrs/react'
import 'ldrs/react/Ring2.css'
export default function Button({ children, required = false, type = "button", disabled = false, classes, isLoading = false, ...props }) {

    return (
        <button type={type} className={classes} required={required} disabled={disabled} {...props}>
            {isLoading ?
                <Ring2
                    size="20"
                    stroke="3"
                    strokeLength="0.25"
                    bgOpacity="0.1"
                    speed="0.8"
                    color="white"
                /> : children}
        </button>
    )
}