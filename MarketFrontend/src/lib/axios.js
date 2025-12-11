import axios from 'axios';

const api = axios.create({
    baseURL : "http://localhost:5062/api",
    withCredentials: false,
})
export default api;