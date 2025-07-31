"use client";

import { useState, useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import { useSession } from "next-auth/react";
import ImageCroper from "@/app/Components/ImageCroper/page";
import ItemInfo from "@/app/Components/AddItem/AddItemInfo";
import { AddImage, AddItem } from "@/app/lib/utility";
import { toast } from "react-toastify";
import { useMenuItemsProvider } from "@/app/Providers/MenuProvider/MenuProvider";
import Style from "./AddItem.module.css";
import Button from "@/app/Components/Button/Button";
import Link from "next/link";
import cn from "classnames";
export default function AddCollectableItem() {

    const router = useRouter();
    const params = useParams();

    const { ItemCollection } = params;
    const { data: session, status } = useSession();
    const { menuItems, error } = useMenuItemsProvider();

    const [imageData, setImageData] = useState();
    const [formData, setFormData] = useState(null);
    const [selectedCollection, setSelectedCollection] = useState();
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        let isActive = true;
        if (status === "authenticated" && ItemCollection && menuItems) {
            if (isActive) {
                var value = menuItems.find(data => data.name == ItemCollection);
                if (value) {
                    setSelectedCollection(value);
                } else {
                    router.push("/ViewItems");
                }
            }
        }
        return () => {
            isActive = false;
        }
    }, [status, ItemCollection, menuItems]);

    const handleItemInfoChange = (data) => setFormData((prev) => ({ ...prev, itemInfo: data }));
    
    const handleCroppedImageChange = (croppedImage) => setImageData(croppedImage);

    const determineFileName = () => {
        const userId = session.user.id;
        const timestamp = Date.now();
        return `${userId}/${ItemCollection}/${timestamp}.jpeg`;
    };

    const uploadImage = async (fileName, newData) => {
        var toastImageUpload = toast("Dodawanie zdjęcia", { autoClose: false })
        var i = await AddImage(imageData, fileName);
        if (i.message == "File uploaded successfully") {
            await uploadData(newData);
            toast.update(toastImageUpload, "Zdjęcie dodane pomyślnie", { autoClose: 3000 });
        }
        else {
            toast.update(toastImageUpload, "wystąpił błąd", { autoClose: false });
        }
        return i;
    };

    const uploadData = async (newData) => {
        var ItemAdd = toast("Dodawanie pozycji", {autoClose:false})
        await AddItem(newData).then(res => {
            if (res.status == 201) {
                toast.update(ItemAdd, "Pozycja dodana pomyślnie", {autoClose:3000})
                window.location.reload()
            }
            else (
                toast("Wystąpił błąd, spróbuj ponownie", {autoClose:false})
            )
        })
    }

    async function postItems(e) {
        e.preventDefault();
        setIsLoading(true);
        try {
            if (imageData && status == "authenticated") {
                const fileName = determineFileName();
                const newData = { ...formData.itemInfo, ["photoFilePath"]: fileName, ["ownerId"]: session.user.id };
                setFormData({ itemInfo: newData })
                await uploadImage(fileName, newData)
            } else if (status == "authenticated") {
                const newData = { ...formData.itemInfo, ["ownerId"]: session.user.id };
                setFormData({ itemInfo: newData })
                await uploadData(newData);
            }
        } catch (error) {
            toast("Error podczas dodawania pozycji" + " " + error, {autoClose:false});
        }
        setIsLoading(false)
    }
    
    if (status == "authenticated") {
        return (
            <div className={cn(Style.addItemMargin)}>
                
                <form onSubmit={postItems}>
                    <div className={cn(Style.displayFlex)}>
                        
                        <div className={cn(Style.imgCropperPosition)}>
                        <div className={cn(Style.backToViewItems)}><Button isLoading={isLoading} onClick={()=>router.back()}>Wróć</Button></div>
                            <ImageCroper onCroppedImageChange={handleCroppedImageChange} />
                        </div>
                        <div className={cn(Style.addItemsPosition)}>
                            <div>{menuItems && (
                                <ItemInfo onDataChange={handleItemInfoChange} collections={menuItems} selectedCollection={selectedCollection} />
                            )}
                            </div>
                            <div className={cn(Style.addItemsButton)}>
                                <Button className={cn(Style.addItemSubmitButton)} isLoading={isLoading} type="submit">Dodaj</Button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        );
    }
}
