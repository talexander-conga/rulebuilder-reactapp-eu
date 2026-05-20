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
        "Content-Type":"application/json",
        "User-Id": window.currentOrgConfig?.user_id,
      },
    });
 
    console.log("Signing Url:", response.data?.Data);
    return response.data?.Data;
  } catch (err) {
    const serverError = err.response?.data;
    console.error("Error calling Signing Url API:", serverError || err.message);
    
    if (serverError?.Errors) {
      serverError.Errors.forEach((e, i) => console.error(`Server Error [${i}]:`, e));
    }

    throw err;
  }
};