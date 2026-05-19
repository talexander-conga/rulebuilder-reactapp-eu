import axios from "axios";

export const getQueryData = async (authToken) => {
  try {
    const url = "https://prod-rls10.congacloud.com/api/data/v1/objects/QueryTerms_c";

    const response = await axios.get(url, {
      headers: {
        Authorization: authToken,
      }
    });

    if (response.data && response.data.Success && response.data.Data) {
      const simplifiedData = response.data.Data.map((item) => {
        const fields = [];
        for (let i = 1; i <= 10; i++) {
          const field = item[`CNGCU_Field_${i}_c`];
          const operator = item[`CNGCU_Operator_${i}_c`];
          const value = item[`CNGCU_Value_${i}_c`];

          if (field || operator || value) {
            fields.push({ field, operator, value });
          }
        }

        return {
          Id: item.Id,
          type: item.CNGCU_Type_c || "",
          clause: item.CNGCU_Clauses_c,
          fields,
          expression: item.CNGCU_Expression_Logic_c || "",
          templateId: item.CNGCU_TemplateId_c || null,
          sequence: item.Sequence_c || null,
          description: item.Description_c || null,
          signer: item.Signer_c||null,
          signingOrder: item.Signing_Order_c||null,
        };
      });

      return simplifiedData;
    }

    return [];
  } catch (error) {
    console.error("getQueryData ~ error:", error);
    throw error;
  }
};
