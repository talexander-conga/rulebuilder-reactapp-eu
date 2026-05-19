import axios from "axios";

export const getAgreementFields = async (authToken) => {
  try {
    const url = "https://prod-rls10.congacloud.com/api/schema/v1/objects/Agreement";

    const response = await axios.get(url, {
      headers: {
        Authorization: authToken,
      }
    });

    const fieldMetadata = response.data?.Data?.FieldMetadata || [];

    const allowedTypes = ["String", "Int", "Currency", "Picklist", "Double", "Decimal"];

    const fields = fieldMetadata
      .filter(field => allowedTypes.includes(field.DataType))
      .map(field => ({
        FieldName: field.FieldName,
        DisplayName: field.DisplayName,
        DataType: field.DataType,
      }));

    return fields;

  } catch (error) {
    console.log("getAgreement ~ error:", error);
    throw error;
  }
};
