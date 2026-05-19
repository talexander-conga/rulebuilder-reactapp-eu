import axios from "axios";

export const getClauses = async (authToken) => {
  try {
    const url = "https://prod-rls10.congacloud.com/api/data/v1/objects/Template";

    const response = await axios.get(url, {
      headers: {
        Authorization: authToken,
      }
    });

    const templates = response.data?.Data || [];

    const clauseNames = templates
      .filter(item => item.Type === "Clause"&&
        item.TextContent && 
        item.TextContent.trim() !== "")
      .map(item => item.Name);

    return clauseNames;

  } catch (error) {
    console.log("getClauses ~ error:", error);
    throw error;
  }
};
