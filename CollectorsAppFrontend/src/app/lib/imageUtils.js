"use client"
import instance from "../axiosInstance";
import axios from "axios";

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