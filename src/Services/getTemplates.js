import axios from "axios";
import { base_url } from "./baseUrl";

export const getTemplates = async (authToken) => {
  try {
    const url = `${base_url}/api/drive/v1/templates`;

    const response = await axios.get(url, {
      headers: {
        Authorization: authToken,
      }
    });

    const templates = response.data?.data || [];

    const templateList = templates.map(item => ({
      templateId: item.templateId,
      templateName: item.templateName
    }));

    return templateList;

  } catch (error) {
    console.log("getTemplates ~ error:", error.response?.data || error.message);
    throw error;
  }
};
