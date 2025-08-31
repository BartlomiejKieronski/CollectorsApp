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

export async function deleteMultipleItems(items, userId) {
    items.forEach(async (element) => {
        await deleteItem(element.id, userId)
    });
}