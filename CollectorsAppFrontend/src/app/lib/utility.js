"use client"
import axios from "axios";
import instance from "../axiosInstance";

export async function GetItem(itemId, userId) {
    return await instance.get(`api/CollectableItems/GetCollectableItemsByUserIdAndItemId/${itemId}/${userId}`);
}

export async function GetItems(userId, collectionId) {
    return await instance.get(`api/CollectableItems/GetCollectableItemsByUserIdAndCollectionId/${userId}/${collectionId}`);
}

export async function GetPaginatedItems(page, userId, collectionId, numberOfItems) {
    return await instance.get(`api/CollectableItems/${page}/${userId}/${collectionId}/${numberOfItems}`)
}

export async function DeleteItem(itemId, userId) {
    const response = await ImagePaths(itemId, userId);
    if (response.data.length > 0) {
        response.data.forEach(async (item) => {
            await DeleteImage(item, userId);
        })
    }
    return await instance.delete(`api/CollectableItems/${itemId}`);
}

export async function UpdateItem(itemId, data) {
    return await instance.put(`api/CollectableItems/${itemId}`, data);
}

export async function AddItem(data) {
    return await instance.post(`api/CollectableItems`, data)
}

export async function CollectionItemCount(collectionId, userId) {
    return await instance.get(`api/CollectableItems/getData?collection=${collectionId}&userId=${userId}`);
}

export async function getCollectionData(userId) {
    return await instance.get(`api/Collections/GetCollectionsByUserId/${userId}`);
}

export async function addCollection(data) {
    return await instance.post(`api/Collections`, data)
}

export async function updateCollection(id, data) {
    return await instance.put(`api/Collections/${id}`, data)
}

export async function deleteCollection(collectionId, userId) {
    var collectionData = await GetItems(userId, collectionId);
    if (collectionData.data.length > 0) {
        await DeleteMultipleItems(collectionData.data, userId);
    }
    return await instance.delete(`api/Collections/${collectionId}`)
}

export async function AddImage(formData, fileName) {
    const croppedImage = formData;
    if (!croppedImage) return;

    const base64Data = croppedImage.replace(/^data:image\/\w+;base64,/, "");
    try {
        const response = await axios.post("/api/UploadToGcs", {
            fileName,
            fileData: base64Data,
        });
        return response.data;
    } catch (error) {
        return error;
    }
}

export async function DeleteImage(image) {

    const URI = encodeURIComponent(image.path)

    const responseGCS = await axios.delete(`/api/DeleteFromGCS/${URI}`);

    return (responseGCS)
}

export async function GetSignedImageUrl(image) {
    if (!image) return "No file";
    try {
        const response = await axios.post("/api/SignUrls", {
            fileName: image
        });
        return await response.data;
    } catch (error) {
        return error;
    }
}

export async function addImagePath(data) {
    return await instance.post(`api/ImagePaths`, data);
}

export async function deleteImagePath(imagePathId) {
    return await instance.delete(`api/ImagePaths/${imagePathId}`);
}

export async function updateImagePath(imagePathId, data) {
    return await instance.update(`api/ImagePaths/${imagePathId}`, data);
}

export async function ImagePaths(itemId, userId) {
    return await instance.get(`api/ImagePaths/GetImagePathsByItemId/${itemId}/${userId}`);
}

export async function GetSignedImagesUrls(items) {
    if (!Array.isArray(items) || items.length === 0) {
        return ("No items provided");
    }
    try {
        const response = await axios.post("/api/HandleArrayUrlSigning", items);
        return response.data;
    } catch (error) {
        return ("Failed to fetch signed URLs");
    }
}

export async function GetSignedItemImagesUrls(items) {
    if (!Array.isArray(items) || items.length === 0) {
        return ("No items provided");
    }
    try {
        const response = await axios.post("/api/HandleItemArrayUrlSigning", items);
        return response.data;
    } catch (error) {
        return ("Failed to fetch signed URLs");
    }
}

export async function DeleteMultipleItems(items, userId) {
    items.forEach(async (element) => {
        await DeleteItem(element.id, userId)
    });
}

export async function IsPasswordCorrect(data){
    return await instance.post(`api/Authentication/IsPasswordCorrect`,data)
}
 
export async function DeleteUserAccount(id){
    return await instance.delete(`api/Users/${id}`);
}