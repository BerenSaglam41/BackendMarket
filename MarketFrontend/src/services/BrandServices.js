import api from "../lib/axios";

export const fetchBrands = () => {
    return api.get("/brand",{ silent: true });
}