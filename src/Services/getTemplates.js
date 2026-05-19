import axios from "axios";

export const getTemplates = async (authToken) => {
  try {
    const url = "https://prod-rls10.congacloud.com/api/drive/v1/templates";

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
