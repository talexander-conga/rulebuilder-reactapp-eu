import axios from "axios";
import { base_url } from "./baseUrl";
 
export const generateAndSignInternal = async (agreementId,authToken) => {
  const url = `${base_url}/api/custom-api/v1/GenerateSignComposer/sign/internal`;
 
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
    console.error("Error calling Signing Url API", err);
    throw err;
  }
};