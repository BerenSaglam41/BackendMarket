import api from "../lib/axios";

export const fetchAdminOrdersApi = async ({
    page = 1,
    pageSize = 20,
    orderStatus,
    paymentStatus,
}) => {
    const res = await api.get("/order/all",{
        params : {
            page,
            pageSize,
            orderStatus,
            paymentStatus
        },
        silent : true
    });
    return res.data;
}
export const fetchMyOrdersApi = async ( page = 1 , pageSize = 10) => {
    return api.get("/order",{
        params : {
            page,
            pageSize
        },
        silent : true
    })
}
export const fetchSellerOrdersApi = async ( page = 1 , pageSize = 20) => {
    return api.get("/order/seller",{
        params : {
            page,
            pageSize
        },
        silent : true
    })
}