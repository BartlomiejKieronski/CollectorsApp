"use client";
import Image from "next/image";
import { useEffect, useState } from "react";
import ImageCroper from "@/app/Components/ImageCroper/page.js";
import { DeleteImage } from "@/app/lib/utility";
import { toast } from "react-toastify";
import Button from "../Button/Button";
import Style from "@/app/(MainContentLogged)/Item/[ItemPage]/[EditPage]/EditPage.module.css";
import cn from "classnames";

export default function EditPictures({
    onHandleImages,
    onCroppedImageChange,
    imageData,
    profileCollectablePath,
    addNewImage,
    isLoading,
    fetchUpdatedImages
}) {
    const [images, setImages] = useState(null);
    const [image, setImage] = useState(null);

    useEffect(() => {
        setImages(imageData);
    }, [imageData]);

    const ChangeDisplayedImage=(index)=> setImage(images[index]);

    function UpdateDefaultImage() {
        if (!image) {
            toast("Wybierz zdjęcie, które powinno być nowym profilowym dla wybranej rzeczy kolekcjonerskiej",{autoClose:5000});
        } else {
            onHandleImages(image);
        }
    }

    async function deletePicture() {
        if (image.path === profileCollectablePath) {
            toast("Najpierw wybierz inne zdjęcie jako domyślne dla kolekcjonerskiej rzeczy.", {autoClose:5000});
        } else {
            await DeleteImage(image).then((res) =>{
                fetchUpdatedImages();    
                toast("Zdjęcie usuniete pomyślnie", { autoClose: 3000 });
                setImage(null)
            });
        }
    }

    function handleAddImage() {
        if (image) {
            setImage(null);
        } else {
            addNewImage();
        }
    }

    return (
        <div className={cn(Style.editImageDisplayDiv)}>
            <div className={cn(Style.btnLt)}>
                <Button type="button" onClick={handleAddImage} isLoading={isLoading}>
                    Dodaj zdjęcie
                </Button>
                <Button type="button" isLoading={isLoading} onClick={UpdateDefaultImage}>
                    Zaaktualizuj domyślne zdjęcie
                </Button>
            </div>
            {(!images || images.length < 1) ? (
                <div className={cn(Style.imgCropperContainer)}>
                    <div className={cn(Style.imgCropper)}>
                        <ImageCroper onCroppedImageChange={onCroppedImageChange} />
                    </div>
                </div>
            ) : (
                <div>
                    <div className={cn(Style.smallImages)}>
                        {images.map((img, index) => (
                            <div key={index} onClick={() => ChangeDisplayedImage(index)}>
                                <img src={img.url} height="100px" alt={`Image ${index}`} />
                            </div>
                        ))}
                        <div className={cn('icon')}>
                            <img
                                src="/add_circle.svg"
                                height="100px"
                                alt="Add new"
                                onClick={() => setImage(null)}
                            />
                        </div>
                    </div>
                    <div className={cn(Style.selectedImage)}>
                        {image ? (
                            <>
                                <Image src={image.url} fill alt="Selected" />
                                <div className={cn(Style.garbageBinIcon)} onClick={deletePicture}>
                                    <Image src="/delete-bin.svg" width={30} height={30} alt="Delete" />
                                </div>
                            </>
                        ) : (
                            <div className={cn(Style.imgCropper)}>
                                <ImageCroper onCroppedImageChange={onCroppedImageChange} />
                            </div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}