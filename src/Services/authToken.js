import axios from "axios"
import QueryString from "qs"

export const getToken = async () => {
    try {  
        const params = new URLSearchParams();
        params.append("client_id", import.meta.env.VITE_CONGA_CLIENT_ID);
        params.append("client_secret", import.meta.env.VITE_CONGA_CLIENT_SECRET);
        params.append("grant_type", "client_credentials");
        const axiosConfig = {
            method: "post",
            url: import.meta.env.VITE_CONGA_AUTH_URL,
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            data: params
        }
        const platformTokenResponse = await axios.request(axiosConfig)
        console.log("token ~ platformTokenResponse:", platformTokenResponse)
        return ("Bearer " + platformTokenResponse.data.access_token)

    } catch (error) {
        console.log("~ token ~ error:", error?.response ? error.response.data : error)
        throw error
    }
}
