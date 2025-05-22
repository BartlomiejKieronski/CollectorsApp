"use client"

import ImageCarusel from "@/app/Components/Image-Carusel/ImgCarusel.js";
import ItemDetails from "@/app/Components/ItemDetails/ItemDetails";
import Linkify from "react-linkify";
import Style from"./itempage.module.css";
import { GetSignedImagesUrls, GetItem, ImagePaths, GetSignedImageUrl } from "@/app/lib/utility";
import { useEffect, useState } from "react";
import { useRouter, useParams } from "next/navigation";
import { useSession } from "next-auth/react";
import Image from "next/image";
import Button from "@/app/Components/Button/Button";
import cn from "classnames";
export default function ItemPage() {
    const { data: session, status } = useSession()

    const router = useRouter();
    const params = useParams();
    const { ItemPage } = params;

    const [imageData, setImageData] = useState();
    const [data, setData] = useState();
    const [imageItemData, setImageItemData] = useState();
    const [signedUrlImageData, setSignedUrlImageData] = useState();
    const [isLoading, setIsLoading] = useState(false)
    
    const toDateOnly = (datetimeString) => {
        const date = new Date(datetimeString);
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        return `${year}-${month}-${day}`;
    };

    useEffect(() => {
        let isActive = true;
        const getItemData = async () => {
            const [itemRes, imagesRes] = await Promise.all([
                GetItem(ItemPage, session.user.id),
                ImagePaths(ItemPage, session.user.id),
            ]);
            if (isActive) {
                const convertedItem = {
                    ...itemRes.data,
                    itemYear: itemRes.data.itemYear != null ? toDateOnly(itemRes.data.itemYear) : itemRes.data.itemYear,
                    dateOfAquire: itemRes.data.dateOfAquire != null ? toDateOnly(itemRes.data.dateOfAquire) : itemRes.data.dateOfAquire,
                    insertDate: itemRes.data.insertDate != null ? toDateOnly(itemRes.data.insertDate) : itemRes.data.insertDate

                };
                setData(convertedItem);
                setImageItemData(imagesRes.data)
            }
        }
        if (status == "authenticated" && isActive) {
            getItemData()
        }
        return () => {
            isActive = false;
            setIsLoading(false)
        }
    }, [status, ItemPage])

    useEffect(() => {
        let isActive = true;
        const signImageUrls = async () => {
            await GetSignedImagesUrls(imageItemData).then(res => {
                if (Array.isArray(res.responseData) && res.responseData.length > 0) {
                    setSignedUrlImageData(res.responseData);
                }
                else {
                    setSignedUrlImageData([{ id: 0, path: "/placeholder_image.png", itemId: imageItemData.id, url: "/placeholder_image.png" }])
                }
            })
        }
        if (status == "authenticated" && isActive && imageItemData) {
            console.log(imageItemData)
            signImageUrls();
        }
        return () => {
            isActive = false;
        }
    }, [imageItemData])

    const EditRedirect = () => router.push(`/Item/${ItemPage}/EditPage`);

    return (
        <div className={cn(Style.itemPageWrapper)}>
            {data && (
                <>
                    <Button classes={Style.editBtnLt} isLoading={isLoading} onClick={() => { setIsLoading(true); EditRedirect() }}>Edytuj dane</Button>
                    <div className={cn(Style.divDisplayPageLayout)}>
                        <div className={cn(Style.imageCaruselComponent)}>
                            <ImageCarusel signedUrlImageData={signedUrlImageData} />
                        </div>
                        <div className={cn(Style.itemDetailsComponent)}>
                            <ItemDetails data={data} EditRedirect={EditRedirect} />
                        </div>
                    </div>
                    <div className={cn(Style.descriptionLayout)}>
                        <div className={cn(Style.itemPageDescription)}>
                            <div>Opis <hr /></div>
                            <div><Linkify properties={{ className: {Style:Style.customLink} }}><p style={{ whiteSpace: 'pre-line', fontSize: "16px" }}>{data.description}</p></Linkify></div>
                        </div>
                    </div>
                </>)}
        </div>
    )
}