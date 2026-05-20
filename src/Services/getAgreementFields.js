import axios from "axios";
import { base_url } from "./baseUrl";

export const getAgreementFields = async (authToken) => {
  try {
    const url = `${base_url}/api/schema/v1/objects/Agreement`;

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
