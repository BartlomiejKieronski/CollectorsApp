import { getCookie } from "cookies-next";
import axios from "axios";
import { isTokenExpired } from "./lib/jwt-get-time";

const instance = axios.create({
  baseURL: "https://localhost:44302/",
  withCredentials: true,
  origin: "localhost:3000",
})

let isRefreshing = false;
let awaitingFetch = [];

function processQueue(error) {
  requestQueue.forEach(({ resolve, reject, config }) => {
    if (error) {
      reject(error);
    } else {
      
      var token = getCookie('AuthToken');
      if (token) config.headers.Authorization = `Bearer ${token}`;
      resolve(instance(config));
    }
  });
  requestQueue.length = 0;
}

instance.interceptors.request.use(async (config) => {
  if(!config.url){
    return config;
  }
  if(isRefreshing==true){
    return Promise((resolve,reject)=>{
      awaitingFetch.push({resolve,reject,config})
    })
  }
  if (config.url.includes("/Reauthenticate") || config.url.includes("/Authentication")) {
    return config;
  }

  var authCookie = await getCookie("AuthToken");
  if (authCookie) {
    var expired = isTokenExpired(authCookie);
    if (expired == false) {
      config.headers['Authorization'] = "bearer " + authCookie;
    }
    else {
      isRefreshing=true;
      var result = await instance.post("https://localhost:44302/api/Authentication/Reauthenticate");

      if (result.status != 200) {
        return Promise.reject(result.status);
      }

      else {
        isRefreshing=false;
        var newCookie = await getCookie("AuthToken");
        config.headers['Authorization'] = "bearer " + newCookie;
      }
    }
  }

  else{
    isRefreshing=true;
    var result = await instance.post("https://localhost:44302/api/Authentication/Reauthenticate")
    if (result.status != 200) {
      return Promise.reject(result.status);
    }
    else {
      isRefreshing=false
      var newCookie = await getCookie("AuthToken")
      config.headers['Authorization'] = "bearer " + newCookie;
    }
  }

  return config;
}, (error) => {
  return Promise.reject(error)
});

instance.interceptors.response.use(
  (response) => {
    if (response.config?.url?.includes("/Reauthenticate") && response.request?.status === 200){
      isRefreshing = false;
      processQueue(null);      
    }
    return response;
  },
  async (error) => {
    if (error.config?.url.includes("/Reauthenticate") && error.response?.status === 401) {
      processQueue(error.response?.status);
      window.location.href = "/Logout";
      return Promise.reject(refreshError);
    }
    return Promise.reject(error);
  });

  export default instance;
