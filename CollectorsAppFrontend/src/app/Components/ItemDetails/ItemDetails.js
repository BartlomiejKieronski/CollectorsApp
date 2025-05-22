import { useEffect, useState } from "react"
import Image from "next/image"
import Style from "@/app/(MainContentLogged)/Item/[ItemPage]/itempage.module.css";
import cn from "classnames";
export default function ItemDetails({ data,EditRedirect }) {
    const [itemData, setItemData] = useState(data)

    useEffect(() => {
        setItemData(data)
    },[data])
    
    return (<div className={cn(Style.itemPageContentArea)}>

        <div><h2>Informacje</h2></div>
        {itemData&&(
        <div className={cn(Style.itemPageContainer)}>
        
            <div className={cn(Style.itemPageDataLayout)}>
                <div className={cn(Style.elementSize)}>Nazwa</div>
                <div className={cn(Style.elementSize)}>{itemData.itemName}</div>
            </div>
            <div className={cn(Style.itemPageDataLayout)}>
                <div className={cn(Style.elementSize)}>Wartość</div>
                <div className={cn(Style.elementSize)}>{itemData.itemValue}</div>
            </div>
            <div className={cn(Style.itemPageDataLayout)}>
                
                <div className={cn(Style.elementSize, Style.textWrap, Style.elPos)}>Atrybuty kolekcjonerskie (np. Numizmat itp.)</div>
                <div className={cn(Style.elementSize)}>{itemData.itemNumismat}</div>
            </div>
            <div className={cn(Style.itemPageDataLayout)}>
                <div className={cn(Style.elementSize)}>Kolekcja</div>
                <div className={cn(Style.elementSize)}>{itemData.collectionId}</div>
            </div>
            <div className={cn(Style.itemPageDataLayout)}>
                <div className={cn(Style.elementSize)}>Stan</div>
                <div className={cn(Style.elementSize)}>{itemData.state}</div>
            </div>
            <div className={cn(Style.itemPageDataLayout)}>
                <div className={cn(Style.elementSize)}>Data wydania</div>
                <div className={cn(Style.elementSize)}>{itemData.itemYear}</div>
            </div>
            <div className={cn(Style.itemPageDataLayout)}>
                <div className={cn(Style.elementSize)}>Data nabycia</div>
                <div className={cn(Style.elementSize)}>{itemData.dateOfAquire}</div>
            </div>
            <div className={cn(Style.itemPageDataLayout)}>
                <div className={cn(Style.elementSize)}>Data dodania</div>
                <div className={cn(Style.elementSize)}>{itemData.insertDate}</div>
            </div>
        </div>
        )}
    </div>)
}