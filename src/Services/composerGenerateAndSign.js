import axios from "axios";
import { base_url } from "./baseUrl";
 
export const composerGenerateAndSign = async (agreementId,authToken) => {
  const url = `${base_url}/api/custom-api/v1/GenerateSignComposer/sign`;
 
  const data = {
    "recordId": agreementId,
  };
 
  try {
    const response = await axios.post(url, data, {
      headers: {
        accept: "application/json",
        Authorization: authToken,
        "Content-Type":"application/json"
      },
    });
 
    console.log("Signing Url:", response.data?.Data);
    return response.data?.Data;
  } catch (err) {
    console.error("Error calling Signing Url API:", err.response?.data || err.message);
    if (err.response?.status === 500) {
      console.error("Backend Stack Trace/Message:", err.response.data);
    }
    throw err;
  }
};