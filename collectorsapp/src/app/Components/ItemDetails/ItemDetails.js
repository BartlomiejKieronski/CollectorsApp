import { useEffect, useState } from "react"
import Image from "next/image"
export default function ItemDetails({ data,EditRedirect }) {
    const [itemData, setItemData] = useState(data)

    useEffect(() => {
        setItemData(data)
    },[data])
    
    return (<div className="item-page-content-area">

        <div><h2>Informacje</h2></div>
        {itemData&&(
        <div className="item-page-container">
        
            <div className="item-page-data-layout">
                <div className="element-size">Nazwa</div>
                <div className="element-size">{itemData.itemName}</div>
            </div>
            <div className="item-page-data-layout">
                <div className="element-size">Wartość</div>
                <div className="element-size">{itemData.itemValue}</div>
            </div>
            <div className="item-page-data-layout ">
                
                <div className="element-size text-wrap el-pos">Atrybuty kolekcjonerskie (np. Numizmat itp.)</div>
                <div className="element-size">{itemData.itemNumismat}</div>
            </div>
            <div className="item-page-data-layout">
                <div className="element-size">Kolekcja</div>
                <div className="element-size">{itemData.collectionId}</div>
            </div>
            <div className="item-page-data-layout">
                <div className="element-size">Stan</div>
                <div className="element-size">{itemData.state}</div>
            </div>
            <div className="item-page-data-layout">
                <div className="element-size">Data wydania</div>
                <div className="element-size">{itemData.itemYear}</div>
            </div>
            <div className="item-page-data-layout">
                <div className="element-size">Data nabycia</div>
                <div className="element-size">{itemData.dateOfAquire}</div>
            </div>
            <div className="item-page-data-layout">
                <div className="element-size">Data dodania</div>
                <div className="element-size">{itemData.insertDate}</div>
            </div>
        </div>
        )}
    </div>)
}