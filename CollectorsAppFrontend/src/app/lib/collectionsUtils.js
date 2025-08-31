"use client"
import instance from "../axiosInstance";
import axios from "axios";

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