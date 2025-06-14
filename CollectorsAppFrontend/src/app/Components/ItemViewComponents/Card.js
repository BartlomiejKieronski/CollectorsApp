import { useState } from "react";
import Image from "next/image";
import Link from "next/link";
import cn from "classnames";
import Style from "@/app/(MainContentLogged)/(Content)/ViewItems/ItemsView.module.css"
export default function Card({cardData}){
    const [data,setData] = useState(null);
    
    useState(()=>{
        setData(cardData)
    },[cardData]);

    return(<>{data &&(
    <div className={cn(Style.cardLayout)}>
        <div className={cn(Style.cardImg)}>
            <Image sizes="100%" src={data.url} fill alt={data.itemName}/> 
            
        </div>
        <div className={cn(Style.cardData)}>
            <p>{data.itemName}</p>
            
            <p><Link href={`/Item/${data.id}`}>pokaż szczegóły</Link></p>
            
        </div>
    </div>)
}</>)
}