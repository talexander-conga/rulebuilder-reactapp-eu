import axios from "axios";
import { base_url } from "./baseUrl";

export const deleteQueryTerm = async (authToken, id) => {
  try {
    const url = `${base_url}/api/data/v1/objects/QueryTerms_c/${id}`;

    const response = await axios.delete(url, {
      headers: {
        Authorization: authToken,
      }
    });

    return response.data;
  } catch (error) {
    console.error("deleteQueryTerm ~ error:", error.response?.data || error.message);
    throw error;
  }
};
