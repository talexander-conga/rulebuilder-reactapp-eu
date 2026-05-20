import axios from "axios";
import { base_url } from "./baseUrl";

export const createQueryTerm = async (authToken, body) => {
  try {
    // const body = {
    //   CNGCU_Clauses_c: clause,    
    //   Name: name,                 
    //   CNGCU_Field_1_c: field || "",    
    //   CNGCU_Value_1_c: value || "",    
    //   Operator_1_c: operator || ""     
    // };

    const response = await axios.post(
      `${base_url}/api/data/v1/objects/QueryTerms_c`,
      body,
      {
        headers: {
          Authorization: authToken,
          "Content-Type": "application/json"
        }
      }
    );

    return response.data;
  } catch (error) {
    console.error("createQueryTerm ~ error:", error.response?.data || error.message);
    throw error;
  }
};