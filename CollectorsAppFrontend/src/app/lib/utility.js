"use client"
import axios from "axios";
import instance from "../axiosInstance";

export async function getItem(itemId, userId) {
    return await instance.get(`api/CollectableItems/GetCollectableItemsByUserIdAndItemId/${itemId}/${userId}`);
}

export async function getItems(userId, collectionId) {
    return await instance.get(`api/CollectableItems/GetCollectableItemsByUserIdAndCollectionId/${userId}/${collectionId}`);
}

export async function getPaginatedItems(page, userId, collectionId, numberOfItems) {
    return await instance.get(`api/CollectableItems/${page}/${userId}/${collectionId}/${numberOfItems}`)
}

export async function deleteItem(itemId, userId) {
    const response = await imagePaths(itemId, userId);
    if (response.data.length > 0) {
        response.data.forEach(async (item) => {
            await deleteImage(item, userId);
        })
    }
    return await instance.delete(`api/CollectableItems/${itemId}`);
}

export async function updateItem(itemId, data) {
    return await instance.put(`api/CollectableItems/${itemId}`, data);
}

export async function addItem(data) {
    return await instance.post(`api/CollectableItems`, data)
}

export async function collectionItemCount(collectionId, userId) {
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
    var collectionData = await getItems(userId, collectionId);
    if (collectionData.data.length > 0) {
        await deleteMultipleItems(collectionData.data, userId);
    }
    return await instance.delete(`api/Collections/${collectionId}`)
}

export async function addImage(formData, fileName) {
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

export async function deleteImage(image) {

    const URI = encodeURIComponent(image.path)

    const responseGCS = await axios.delete(`/api/DeleteFromGCS/${URI}`);

    return (responseGCS)
}

export async function getSignedImageUrl(image) {
    if (!image) return {error: "No file"};
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

export async function imagePaths(itemId, userId) {
    return await instance.get(`api/ImagePaths/GetImagePathsByItemId/${itemId}/${userId}`);
}

export async function getSignedImagesUrls(items) {
    if (!Array.isArray(items) || items.length === 0) {
        return ({error: "No items provided"});
    }
    try {
        const response = await axios.post("/api/HandleArrayUrlSigning", items);
        return response.data;
    } catch (error) {
        return ({error: "Failed to fetch signed URLs"});
    }
}

export async function getSignedItemImagesUrls(items) {
    if (!Array.isArray(items) || items.length === 0) {
        return ({error: "No items provided"});
    }
    try {
        const response = await axios.post("/api/HandleItemArrayUrlSigning", items);
        return response.data;
    } catch (error) {
        return ({error: "Failed to fetch signed URLs"});
    }
}

export async function deleteMultipleItems(items, userId) {
    items.forEach(async (element) => {
        await deleteItem(element.id, userId)
    });
}

export async function isPasswordCorrect(data){
    return await instance.post(`api/Authentication/IsPasswordCorrect`,data)
}
 
export async function deleteUserAccount(id){
    return await instance.delete(`api/Users/${id}`);
}