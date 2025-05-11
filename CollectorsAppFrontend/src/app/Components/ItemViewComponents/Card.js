import { useState } from "react"
import Image from "next/image"
import "./Card.css"
import Link from "next/link"
export default function Card({cardData}){
    const [data,setData] = useState(null);
    
    useState(()=>{
        setData(cardData)
    },[cardData]);

    return(<>{data &&(
    <div className="card-layout">
        <div className="card-img">
            <Image style={{ objectFit: 'cover' }} sizes="100%" src={data.url} priority={false} fill alt={data.itemName} />
        </div>
        <div className="card-data">
            <p>{data.itemName}</p>
            
            <p><Link href={`/Item/${data.id}`}>pokaż szczegóły</Link></p>
            
        </div>
    </div>)
}</>)
}