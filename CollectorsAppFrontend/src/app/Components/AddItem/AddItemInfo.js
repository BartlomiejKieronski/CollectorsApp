"use client";
import { useEffect, useState } from "react";

export default function ItemInfo({ onDataChange, collections, selectedCollection }) {
    const [collectionData, setCollectionData] = useState(collections)
    const [value, setValue] = useState(selectedCollection)
    const [itemData, setItemData] = useState({
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

    useEffect(() => {
        let isActive = true;

        setCollectionData(collections)
        if (selectedCollection) {
            if (isActive) {
                const newData = { ...itemData, ["collectionId"]: selectedCollection.id };
                setItemData(newData);
                setValue(selectedCollection)
            }
        }

        return () => {
            isActive = false
        }
    }, [collectionData, value, selectedCollection, collections])

    const handleChange = (e) => {
        const { name, value } = e.target;
        const newData = { ...itemData, [name]: value };
        setItemData(newData);
        onDataChange(newData);
    };
    
    return (
        <div className="add-item-layout">
            <div>
                <label>
                    Nazwa
                    <input type="text" name="itemName" onChange={handleChange} required />
                </label>
            </div>
            <div>
                <label>
                    Wartość przedmiotu
                    <input type="text" name="itemValue" onChange={handleChange} />
                </label>
            </div>
            <div>
                <label>
                    Data Produkcji
                    <input type="date" name="itemYear" onChange={handleChange} />
                </label>
            </div>
            <div>
                <label>
                    Data Zdobycia
                    <input type="date" name="dateOfAquire" onChange={handleChange} />
                </label>
            </div>
            <div>
                <label>
                    Atrybuty kolekcjonerskie: {"("}nominał/Stopień medalu{")"}
                    <input type="text" name="itemNumismat" onChange={handleChange} />
                </label>
            </div>
            <div>
                <label>
                    Stan
                    <select id="state" name="state" onChange={handleChange} >
                        <option value="Normalny">Normalny</option>
                        <option value="Bardzo Zły">Bardzo zły</option>
                        <option value="Zły">Zły</option>
                        <option value="normalny">Normalny</option>
                        <option value="Dobry">Dobry</option>
                        <option value="Bardzo dobry">Bardzo dobry</option>
                    </select>
                </label>
            </div>
            {value && (
                <div>
                    <label>
                        Kolekcja
                        <select id="collectionId" name="collectionId" onChange={handleChange} required>
                            <option value={value.id}>{value.name}</option>
                            {
                                collections && (
                                    collections.map((option) =>
                                    (
                                        <option key={option.id} value={option.id} >
                                            {option.name}
                                        </option>
                                    ))
                                )
                            }
                        </select>
                    </label>
                </div>
            )}
            <div>
                <label>
                    Opis
                    <textarea type="textarea" rows={15} cols={20} name="description" onChange={handleChange} />
                </label>

            </div>
        </div>
    );
}
