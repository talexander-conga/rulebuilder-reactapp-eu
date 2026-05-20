import axios from "axios";
import { base_url } from "./baseUrl";

export const updateQueryTerm = async (authToken, id, body) => {
  try {
    const url = `${base_url}/api/data/v1/objects/QueryTerms_c/${id}`;

    const response = await axios.put(url, body, {
      headers: {
        Authorization: authToken,
        "Content-Type": "application/json"
      }
    });

    return response.data;
  } catch (error) {
    console.error("updateQueryTerm ~ error:", error.response?.data || error.message);
    throw error;
  }
};