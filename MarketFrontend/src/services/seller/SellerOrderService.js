import api from "../../lib/axios";

export const SellerOrderService = {
    getOrders : async ({page =1, pageSize=20}) => {
        const res = await api.get('/order/seller', {
            params: {
                page,
                pageSize
            },
            silent: true
        });
        return res.data;
    },
    updateStatus : async (orderId,payload) => {
        const res = await api.put(`/order/${orderId}/status`, payload,{silent: true});
        return res.data
    },
    getOrderById : async (orderId) => {
        const res = await api.get(`/order/${orderId}`,{silent: true});
        return res.data;
    }

}