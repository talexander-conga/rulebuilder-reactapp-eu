import axios from "axios";

export const deleteQueryTerm = async (authToken, id) => {
  try {
    const url = `https://prod-rls10.congacloud.com/api/data/v1/objects/QueryTerms_c/${id}`;

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
