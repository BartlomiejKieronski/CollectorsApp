"use client"
import { signOut } from "next-auth/react";
import { useEffect } from "react";

export default function Logout(){
    useEffect(()=>{
        signOut({redirect:false});
    },[])
    return;
}