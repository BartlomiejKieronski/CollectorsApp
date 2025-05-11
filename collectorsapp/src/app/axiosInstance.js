
import { getCookie } from "cookies-next";
import axios from "axios";
import { TestNav } from "./lib/testnav";
let routerRef = null;
export function setRouter(router) {
    routerRef = router;
}

const instance = axios.create({
    baseURL: "https://localhost:44302/",
    withCredentials: true,
    origin: "localhost:3000",
})

instance.interceptors.request.use((config) => {
    var cookie = getCookie("AuthToken");
    if (cookie === null) {
        return config;
    } else {
        config.headers['Authorization'] = "bearer " + getCookie("AuthToken");
        return config;
    }
}, (error) => {
    return Promise.reject(error);
})

instance.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;
        if (originalRequest.url.includes("/Reauthenticate")) 
            {
                return Promise.reject(error);
            }
        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;
            try {
                await instance.post("https://localhost:44302/api/Authentication/Reauthenticate");
                return instance(originalRequest)
            }
            catch (refreshError) {
                window.location.href = "/Logout";
                if (typeof window !== 'undefined') {
                    
                }
                return Promise.reject(refreshError);
            }
        }
        window.location.href = "/Settings";
        return Promise.reject(error);
    });

export default instance;