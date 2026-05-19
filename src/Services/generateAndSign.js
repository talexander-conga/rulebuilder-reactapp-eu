import axios from "axios";
 
export const GenerateAndSign = async (agreementId,authToken) => {
  const url =
    "https://prod-rls10.congacloud.com/api/custom-api/v1/GenerateComposerDoc/generateDocdellwood";
 
  const data = {
    "recordId": agreementId,
  };
 
  try {
    const response = await axios.post(url, data, {
      headers: {
        accept: "application/json",
        Authorization: authToken,
        "User-Id": window.currentOrgConfig.user_id,
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