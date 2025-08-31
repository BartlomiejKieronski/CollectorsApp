"use client"
import instance from "../axiosInstance";

export async function isPasswordCorrect(data){
    return await instance.post(`api/Authentication/IsPasswordCorrect`,data)
}
 
export async function deleteUserAccount(id){
    return await instance.delete(`api/Users/${id}`);
}