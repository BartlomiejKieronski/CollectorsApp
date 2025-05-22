"use client";
import { useState, useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import { useSession } from "next-auth/react";
import Style from "./EditPage.module.css";
import EditPictures from "@/app/Components/EditItemData/EditPictures";
import EditData from "@/app/Components/EditItemData/EditData";
import {
  UpdateItem,
  AddImage,
  GetItem,
  ImagePaths,
  GetSignedImagesUrls,
  addImagePath,
  DeleteItem
} from "@/app/lib/utility";

import { useMenuItemsProvider } from "@/app/Providers/MenuProvider/MenuProvider";
import { toast } from "react-toastify";
import Button from "@/app/Components/Button/Button";
import cn from "classnames";

export default function EditItem() {
  const { data: session, status } = useSession();

  const router = useRouter();
  const params = useParams();

  const { EditPage, ItemPage } = params;
  const { menuItems, error } = useMenuItemsProvider();

  const [imageData, setImageData] = useState(null);
  const [itemData, setItemData] = useState(null);
  const [imagePath, setImagePath] = useState(null);
  const [formData, setFormData] = useState(null);
  const [signedUrls, setSignedUrls] = useState(null);
  const [keyData, setKeyData] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const toDateOnly = (datetimeString) => {
    const date = new Date(datetimeString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
  };

  useEffect(() => {
    const abort = new AbortController();
    let isMounted = true;
    const fetchData = async () => {
      try {
        const [itemRes, imagesRes] = await Promise.all([
          GetItem(ItemPage, session.user.id),
          ImagePaths(ItemPage, session.user.id),
        ]);
        console.log(itemRes.data)
        if (isMounted) {
          const convertedItem = {
            ...itemRes.data,
            itemYear: itemRes.data.itemYear != null ? toDateOnly(itemRes.data.itemYear) : itemRes.data.itemYear,
            dateOfAquire: itemRes.data.dateOfAquire != null ? toDateOnly(itemRes.data.dateOfAquire) : itemRes.data.dateOfAquire,
          };
          setItemData(convertedItem);
          setFormData(convertedItem);
          setImagePath(imagesRes.data);
        }
      } catch (err) {
        if (isMounted) {
          toast("Wystąpił błąd podczas pobierania informacji z serwera: " + err, { autoClose: false });
        }
      }
    };

    if (status === "authenticated" && ItemPage && session?.user?.id) {
      fetchData();
    }
    return () => {
      isMounted = false;
    };
  }, [ItemPage, session?.user?.id]);

  useEffect(() => {
    let isActive = true;


    if (status === "authenticated" && (imageData || keyData !== 0)) {
      refreshImagePaths();
    }

    return () => {
      isActive = false;
    };
  }, [status, imageData, keyData, ItemPage]);

  const refreshImagePaths = async () => {
    try {
      setIsLoading(true);
      const res = await ImagePaths(ItemPage, session.user.id);

      setImagePath(res.data);
      setIsLoading(false);
    } catch (err) {
      toast("wystąpił błąd: " + err, { autoClose: false })
    }
  };

  useEffect(() => {
    let isActive = true;
    const fetchSignedUrls = async () => {
      try {
        const res = await GetSignedImagesUrls(imagePath);
        if (isActive) {
          setSignedUrls(res.responseData);
        }
      } catch (err) {
        toast("wystąpił błąd: " + err, { autoClose: false })
      }
    };

    if (imagePath && status === "authenticated") {
      fetchSignedUrls();
    }

    return () => {
      isActive = false;
    };
  }, [imagePath, status]);

  const isFormDataUnchanged = () => {
    return JSON.stringify(formData) === JSON.stringify(itemData);
  };

  const handleItemInfoChange = (updatedData) => setFormData(updatedData);

  const updateItemInfo = async (dataToUpdate) => {
    setIsLoading(true);
    console.log(dataToUpdate)
    try {
      const res = await UpdateItem(dataToUpdate.id, dataToUpdate);
      setItemData(dataToUpdate);
      toast("Zaaktualizowano pomyślnie", { autoClose: 3000 });
    } catch (err) {
      toast("Wystąpił błąd podczas aktualizacji", { autoClose: 3000 });
    }
    setIsLoading(false);
  };

  const updateDefaultItemImage = async (croppedImage) => {
    console.log(croppedImage)
    setIsLoading(true)
    if (isFormDataUnchanged()) {
      const newData = { ...formData, photoFilePath: croppedImage.path };
      setFormData(newData);
      await updateItemInfo(newData);
    } else {
      toast(
        "Dane obiektu w formularzu zostały zmienione, przed dodaniem zdjęcia prześlij nowe dane obiektu"
        , { autoClose: false });
    }
    setIsLoading(false)
  };

  const determineFileName = () => {
    const timestamp = Date.now();
    let collectionId = menuItems.find(item => item.id == formData.collectionId);
    if (!collectionId) {
      toast("Brak kolekcji", { autoClose: false });
      return "";
    }
    return `${session.user.id}/${collectionId.name}/${timestamp}.jpeg`;
  };

  const onCroppedImageChange = (image) => setImageData(image);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!isFormDataUnchanged()) {
      await updateItemInfo(formData);
    } else {
      toast("Najpierw zmień dane obiektu w formularzu", { autoClose: false });
    }
  };

  const uploadImage = async (fileName, imageDataUpload) => {
    setIsLoading(true);
    try {
      const result = await AddImage(imageData, fileName);
      if (result.message === "File uploaded successfully") {
        await uploadData(imageDataUpload);
      }
      return result;
    } catch (err) {
      toast("Wystąpił błąd: " + err, { autoClose: false });
    }
    setIsLoading(false)
  };

  const uploadData = async (imageDataUpload) => {
    try {
      setIsLoading(true)
      const res = await addImagePath(imageDataUpload);

      if (res.status === 201) {
        toast("Zdjęcie dodane pomyślnie", { autoClose: 3000 });
        setKeyData((prev) => prev + 1);
      } else {
        toast("Wystąpił błąd, spróbuj ponownie", { autoClose: false });
      }
      setIsLoading(false)
    } catch (err) {
      toast("Wystąpił błąd, spróbuj ponownie", { autoClose: false });
    }
  };

  const addNewImage = async () => {
    if (imageData) {
      if (isFormDataUnchanged()) {
        const fileName = determineFileName();
        if (!fileName) return;
        const imageValues = { path: fileName, itemId: formData.id, ownerId: session.user?.id };
        await uploadImage(fileName, imageValues);
        if (!itemData.photoFilePath) {
          await updateDefaultItemImage({id:0,path:fileName,itemId:formData.id})
        }
      } else {
        toast("Dane obiektu w formularzu zostały zmienione, przed dodaniem zdjęcia prześlij nowe dane obiektu", { autoClose: false });
      }
    } else {
      toast("Nie został wybrany żaden obraz do przesłania", { autoClose: false });
    }
  };

  const DeleteCurrentItem = async() => {
    setIsLoading(true)
    await DeleteItem(itemData.id,session.user.id);
    var collectionName = menuItems.find(x=>x.id==itemData.collectionId)
    setIsLoading(false)
    router.push(`/ViewItems/${collectionName.name}/${itemData.collectionId}`)
  }
  
  if (status !== "authenticated") {
    return <div>Loading...</div>;
  }

  return (
    <div className={cn(Style.editItem)}>
      <form onSubmit={handleSubmit}>
        {error && <p>{error}</p>}
        <div className={cn(Style.displayFlex)}>
          <div className={cn(Style.imgCropperPosition)}>
            {itemData && (
              <EditPictures
                key={keyData}
                onHandleImages={updateDefaultItemImage}
                onCroppedImageChange={onCroppedImageChange}
                imageData={signedUrls}
                userData={ItemPage}
                profileCollectablePath={itemData.photoFilePath}
                addNewImage={addNewImage}
                isLoading={isLoading}
                fetchUpdatedImages={refreshImagePaths}
              />
            )}
          </div>
          <div className={cn(Style.addItemsPosition)}>
            <div>
              <div className={cn(Style.deleteButton)}>
                <Button type={"button"} isLoading={isLoading} disabled={isLoading} onClick={() => DeleteCurrentItem()}>Usuń</Button>
              </div>
              {menuItems && (
                <EditData
                  onDataChange={handleItemInfoChange}
                  itemData={itemData}
                  collections={menuItems}

                />)}
            </div>
            <div className="addItemsButton">
              <Button className={cn(Style.addItemSubmitButton)} isLoading={isLoading} disabled={isLoading} type="submit">Aktualizuj</Button>
            </div>
          </div>
        </div>
      </form>
    </div>
  );
}