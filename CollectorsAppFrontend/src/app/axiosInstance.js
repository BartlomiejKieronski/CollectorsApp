import { getCookie } from "cookies-next";
import axios from "axios";
import createAuthRefreshInterceptor from 'axios-auth-refresh';

const instance = axios.create({
  baseURL: "https://localhost:44302/",
  withCredentials: true,
  origin: "localhost:3000",
})

const refreshAuthLogic = async (failedRequest) => {
  try {
    await instance.post('api/Authentication/Reauthenticate');

    const newAccessToken = await getCookie("AuthToken");
    if (newAccessToken) {
      failedRequest.response.config.headers['Authorization'] = `Bearer ${newAccessToken}`;
    }
    return Promise.resolve();
  } catch (error) {
    return Promise.reject(error);
  }
};

createAuthRefreshInterceptor(instance, refreshAuthLogic);

instance.interceptors.request.use((config) => {
  const token = getCookie('AuthToken');
  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  return config;
});

export default instance;
