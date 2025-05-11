"use client";
import { useEffect, useState } from "react";

export default function EditData({ onDataChange, itemData, collections }) {
    const [itemDataFields, setItemData] = useState({
        itemName: "",
        itemValue: null,
        itemYear: null,
        dateOfAquire: null,
        photoFilePath: null,
        itemNumismat: null,
        collectionId: "",
        ownerId: null,
        state: "Normalny",
        description: null
    });

    const [value, setValue] = useState(false)

    useEffect(() => {

        setValue(collections)
        setItemData(itemData)
    
    }, [collections,itemData])

    const handleChange = (e) => {
        console.log(itemDataFields)
        const { name, value } = e.target;
        const newData = { ...itemDataFields, [name]: value };
        
        setItemData(newData);
        if (onDataChange) {
            onDataChange(newData);
        }
    }

    return (
        <div className="add-item-layout">
            {itemData && (<>
                <div>
                    <label>
                        Nazwa
                        <input type="text" name="itemName" onChange={handleChange} defaultValue={itemData.itemName} required />
                    </label>
                </div>
                <div>
                    <label>
                        Wartość przedmiotu
                        <input type="text" name="itemValue" onChange={handleChange} defaultValue={itemData.itemValue} />
                    </label>
                </div>
                <div>
                    <label>
                        Data Produkcji
                        <input type="date" name="itemYear" onChange={handleChange} defaultValue={itemData.itemYear} />
                    </label>
                </div>
                <div>
                    <label>
                        Data Zdobycia
                        <input type="date" name="dateOfAquire" onChange={handleChange} defaultValue={itemData.dateOfAquire} />
                    </label>
                </div>
                <div>
                    <label>
                        Atrybuty kolekcjonerskie (nominał/stopień medalu itp.): 
                        <input type="text" name="itemNumismat" onChange={handleChange} defaultValue={itemData.itemNumismat} />
                    </label>
                </div>
                <div>
                    <label>
                        Stan
                        <select id="state" name="state" defaultValue={itemData.state} onChange={handleChange} >
                            <option value="Normalny">Normalny</option>
                            <option value="Bardzo Zły">Bardzo zły</option>
                            <option value="Zły">Zły</option>
                            <option value="Normalny">Normalny</option>
                            <option value="Dobry">Dobry</option>
                            <option value="Bardzo dobry">Bardzo dobry</option>
                        </select>
                    </label>
                </div>
                {value && (
                    <div>
                        <label>
                            Kolekcja
                            <select id="collectionId" name="collectionId" value={itemData.collectionId} onChange={handleChange} required>
                                {collections && collections.map((collection) => (
                                    <option key={collection.id} value={collection.id}>{collection.name}</option>
                                ))}
                            </select>
                        </label>
                    </div>
                )}
                <div>
                    <label>
                        Opis
                        <textarea type="textarea" rows={15} cols={20} name="description" defaultValue={itemData.description} onChange={handleChange} />
                    </label>

                </div>
            </>)}
        </div>
    );
}
