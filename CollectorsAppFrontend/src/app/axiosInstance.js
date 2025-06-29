import { getCookie } from "cookies-next";
import axios from "axios";
import createAuthRefreshInterceptor from 'axios-auth-refresh';

const instance = axios.create({
  baseURL: "https://localhost:44302/",
  withCredentials: true,
})
const reauthInstance = axios.create({
  withCredentials: true,
})

const refreshAuthLogic = async (failedRequest) => {
  try {
    await reauthInstance.post('https://localhost:44302/api/Authentication/Reauthenticate');

    const newAccessToken = await getCookie("AuthToken");
    if (newAccessToken) {
      failedRequest.response.config.headers['Authorization'] = `Bearer ${newAccessToken}`;
    }
    return Promise.resolve();
  } catch (error) {
    if (typeof window !== 'undefined') {
      window.location.href = '/Logout';
    }
    return Promise.reject(error);
  }
};

createAuthRefreshInterceptor(instance, refreshAuthLogic,{statusCodes:[401]});

instance.interceptors.request.use((config) => {
  const token = getCookie('AuthToken');
  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  return config;
});

export default instance;
