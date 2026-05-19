import React, { useState, useEffect } from "react";
// import { useNavigate, useLocation } from "react-router-dom";
import Select from "react-select"; 
import { getClauses } from "../Services/getClauses";
import { getToken } from "../Services/authToken";
import { getAgreementFields } from "../Services/getAgreementFields";
import { createQueryTerm } from "../Services/postQueryRecords";
import { updateQueryTerm } from "../Services/updateQueryData";
import { getTemplates } from "../Services/getTemplates";
import "./DynamicForm.css";

const DynamicForm = ({ closeModal, editData, refresh }) => {
  const [clause, setClause] = useState([]);
  const [clauseOptions, setClauseOptions] = useState([]);
  const [rows, setRows] = useState([{ field: "", operator: "", value: "" }]);
  const [expression, setExpression] = useState("");
  const [editIndex, setEditIndex] = useState(undefined);
  const [editId, setEditId] = useState(null);
  const [fieldOptions, setFieldOptions] = useState([]);
  const [expressionEdited, setExpressionEdited] = useState(false);
  const operatorOptions = ["=", "!=", ">", "<", ">=", "<=", "IN", "LIKE"];
  const [type, setType] = useState(editData ? "" : "Template");
  const [template, setTemplate] = useState("");
  const [sequence, setSequence] = useState("");
  const [description, setDescription] = useState("");
  const [templateOptions, setTemplateOptions] = useState([]);
  const [templateIdMap, setTemplateIdMap] = useState({});
  const [signer, setSigner] = useState("");
  const [signingOrder, setSigningOrder] = useState("");
  useEffect(() => {
    if (editData) {
      setExpressionEdited(true);
      setType(editData.type);

      if (editData.type === "Clause") {
        setClause(
          editData.clause
            ? editData.clause.split(",").map((c) => ({ value: c, label: c }))
            : [],
        );
      }

      if (editData.type === "Template") {
        setTemplate(editData.clause || "");
        setSequence(editData.Sequence_c || editData.sequence || "");
        setDescription(editData.Description_c || editData.description || "");
      }
      if (editData.type === "Signer") {
        setSigner(editData.signer || "");
        setSigningOrder(editData.Signing_Order_c || "3");
      }

      setRows(
        editData.conditions?.length
          ? editData.conditions
          : [{ field: "", operator: "", value: "" }],
      );

      setExpression(editData.expression || "");
      setEditId(editData.Id);
    }
    
    if (editData && editData.type === "Template") {
      console.log("Sequence fields:", {
        sequence: editData.sequence,
        Sequence_c: editData.Sequence_c,
        description: editData.description,
        Description_c: editData.Description_c,
      });
    }
    if (editData && editData.type === "Signer") {
      console.log("signer:", {
        signer: editData.signer,
      });
    }
  }, [editData]);
  
  useEffect(() => {
    const fetchTemplates = async () => {
      try {
        const authToken = await getToken();
        const templates = await getTemplates(authToken);  
        
        const options = templates.map(t => ({
          value: t.templateName,
          label: t.templateName
        }));
        
        const idMap = templates.reduce((map, t) => {
          map[t.templateName] = t.templateId;
          return map;
        }, {});
        
        setTemplateOptions(options);
        setTemplateIdMap(idMap);
        console.log("Dynamic templates loaded:", options);
      } catch (err) {
        console.error("Error fetching templates:", err);
        showPopup("error", "Templates Error", "Failed to load templates.");
      }
    };
    
    fetchTemplates();
  }, []);
const signerOptions = [
    { value: "Signer3", label: "Signer3" }
  ];

  const showPopup = (type, title, message) => {
    const overlay = document.createElement("div");
    overlay.className = "custom-popup-overlay";

    overlay.innerHTML = `
    <div class="custom-popup popup-${type}">
      <div class="popup-title">${title}</div>
      <div class="popup-message">${message}</div>
      <button class="popup-btn btn-${type}">OK</button>
    </div>
  `;

    document.body.appendChild(overlay);

    const closePopup = () => {
      overlay.remove();
    };
    overlay.querySelector("button").addEventListener("click", closePopup);
    setTimeout(closePopup, 5000);
  };

  // Fetch fields
  useEffect(() => {
    const fetchFields = async () => {
      try {
        const authToken = await getToken();
        const fields = await getAgreementFields(authToken);
        setFieldOptions(fields);
      } catch (err) {
        console.error("Error fetching fields:", err);
      }
    };
    fetchFields();
  }, []);

  // Fetch clause names
  useEffect(() => {
    const fetchClauses = async () => {
      try {
        const authToken = await getToken();
        const clauses = await getClauses(authToken);
        const options = clauses.map((c) => ({ value: c, label: c }));
        setClauseOptions(options);
      } catch (err) {
        console.error("Error fetching clauses:", err);
      }
    };
    fetchClauses();
  }, []);
  // Auto-generate expression when rows change
  useEffect(() => {
    if (expressionEdited || editId || editData) return;

    const count = rows.length;
    if (count === 0) return;

    const autoExp = Array.from({ length: count }, (_, i) => i + 1).join(
      " AND ",
    );
    setExpression(autoExp);
  }, [rows, editId, expressionEdited, editData]);

  // Prefill data if editing
  // useEffect(() => {
  //   if (location.state?.formToEdit) {
  //     setExpressionEdited(true);
  //     const { formToEdit, index } = location.state;

  //     setClause(
  //       formToEdit.clause
  //         ? formToEdit.clause.split(",").map((c) => ({ value: c, label: c }))
  //         : [],
  //     );
  //     setRows(
  //       formToEdit.conditions.length
  //         ? formToEdit.conditions
  //         : [{ field: "", operator: "", value: "" }],
  //     );
  //     setExpression(formToEdit.expression);

  //     setEditIndex(index);
  //     setEditId(formToEdit.Id);
  //   }
  // }, [location.state]);

  const addRow = () => {
    if (rows.length >= 10) {
      showPopup(
        "warning",
        "Limit Reached",
        "You can only add up to 10 conditions.",
      );
      return;
    }
    setRows([...rows, { field: "", operator: "", value: "" }]);
  };

  const updateRow = (index, key, value) => {
    const updatedRows = [...rows];
    updatedRows[index][key] = value;
    setRows(updatedRows);
  };

  const removeRow = (index) => {
    if (rows.length === 1) return;
    const updated = rows.filter((_, i) => i !== index);
    setRows(updated);
  };

  const buildReadableExpression = (rows, expression) => {
    const conditionMap = {};

    rows.forEach((row, index) => {
      const idx = index + 1;
      let valueFormatted = row.value;
if (row.operator === "IN") {
      const trimmedValues = row.value
        .split(",")
        .map(val => val.trim())
        .filter(val => val !== "");
      valueFormatted = `(${trimmedValues.join(",")})`; 
    } else if (isNaN(valueFormatted)){
        valueFormatted = `'${valueFormatted}'`;
    }
      

      if (row.operator === "IN") {
      conditionMap[idx] = `${row.field} IN ${valueFormatted}`;
    } else {
      conditionMap[idx] = `${row.field}${row.operator}${valueFormatted}`;
    }
    });

    let finalReadable = expression.replace(/\b\d+\b/g, (num) => {
      const key = parseInt(num, 10);
      return conditionMap[key] ? `(${conditionMap[key]})` : "";
    });

    finalReadable = finalReadable.replace(/\s+/g, " ").trim();

    return finalReadable;
  };

  const handleSave = async () => {
    if (!type) {
      showPopup("warning", "Missing Type", "Please select Type.");
      return;
    }

    if (type === "Clause" && (!clause || clause.length === 0)) {
      showPopup(
        "warning",
        "Missing Clauses",
        "Please select at least one clause.",
      );
      return;
    }

    if (type === "Template") {
      if (!template) {
        showPopup("warning", "Missing Template", "Please select a template.");
        return;
      }
      const sequenceStr = String(sequence || "").trim();
      if (!sequenceStr) {
        showPopup(
          "warning",
          "Missing Sequence",
          "Please enter Sequence for template.",
        );
        return;
      }
    }
    if (type === "Signer" && !signer) {
      showPopup(
        "warning",
        "Missing Signer",
        "Please select a signer field.",
      );
      return;
    }

    for (let i = 0; i < rows.length; i++) {
      const r = rows[i];
      if (!r.field.trim() || !r.operator.trim() || !r.value.trim()) {
        showPopup(
          "warning",
          "Incomplete Row",
          `Row ${i + 1}: Field, Operator, and Value cannot be empty.`,
        );

        return;
      }
    }

    let finalExpression = expression.trim();

    finalExpression = finalExpression
      .replace(/\s+/g, " ")
      .replace(/\bAND\b/gi, "and")
      .replace(/\bOR\b/gi, "or")
      .trim();

    // Allowed pattern: numbers, AND/OR, parentheses
    const validPattern = /^[\d\s()andor]+$/i;

    if (!validPattern.test(finalExpression)) {
      showPopup(
        "error",
        "Invalid Expression",
        "Only numbers, parentheses, and AND/OR are allowed.",
      );

      return;
    }

    //parentheses pairing
    const stack = [];
    for (const ch of finalExpression) {
      if (ch === "(") stack.push(ch);
      else if (ch === ")") {
        if (!stack.length) {
          showPopup(
            "error",
            "Invalid Parentheses",
            "Invalid parentheses placement.",
          );

          return;
        }
        stack.pop();
      }
    }
    if (stack.length > 0) {
      showPopup("error", "Parentheses Error", "Parentheses are not balanced.");

      return;
    }

    //referenced numbers
    const usedNumbers = finalExpression.match(/\b\d+\b/g)?.map(Number) || [];
    const maxRow = rows.length;

    for (let num of usedNumbers) {
      if (num < 1 || num > maxRow) {
        showPopup(
          "error",
          "Invalid Row Reference",
          `Expression invalid: You referenced ${num}, but only ${maxRow} rows exist.`,
        );

        return;
      }
    }

    try {
      const authToken = await getToken();
      const readableExpression = buildReadableExpression(rows, finalExpression);

      // console.log("Readable Expression: ", readableExpression);
      // const selectedClauses = clause.map((c) => c.value).join(",");
      let clauseOrTemplateValue = "";
      let templateIdValue = null;
      if (type === "Clause") {
        clauseOrTemplateValue = clause.map((c) => c.value).join(",");
      } else if (type === "Template") {
        clauseOrTemplateValue = template;
        templateIdValue = templateIdMap[template];
      }else if (type === "Signer") {
        clauseOrTemplateValue = signer; 
      }

      const body = {
        CNGCU_Type_c: type,
        CNGCU_Clauses_c: clauseOrTemplateValue,
        Name: "AutoGeneratedQueryTerm_" + Date.now(),
        CNGCU_Expression_Logic_c: finalExpression,
        CNGCU_Expression_Condition_c: readableExpression,
      };

      if (templateIdValue) {
        body.CNGCU_TemplateId_c = templateIdValue;
        body.Sequence_c = sequence;
        body.Description_c = description;
      }
      if (type === "Signer") {
        body.Signer_c = signer;
        body.Signing_Order_c = signingOrder || "3";
      }
      rows.forEach((row, index) => {
        const idx = index + 1;
        if (row.field) body[`CNGCU_Field_${idx}_c`] = row.field;
        if (row.operator) body[`CNGCU_Operator_${idx}_c`] = row.operator;
        if (row.value) {
        let cleanValue = row.value;
        if (row.operator === "IN") {
          cleanValue = row.value
            .split(",")
            .map(val => val.trim())     
            .filter(val => val !== "")  
            .join(",");          
        }
        body[`CNGCU_Value_${idx}_c`] = cleanValue;
      }
      });

      console.log("Sending body to API:", body);
      if (editId) {
        await updateQueryTerm(authToken, editId, body);
        showPopup("success", "Success!!", "Query term updated successfully!");
      } else {
        await createQueryTerm(authToken, body);
        showPopup("success", "Success!!", "Query term created successfully!");
      }

      //       showPopup(
      //   "success",
      //   "Saved Successfully",
      //   "Query term saved successfully!"
      // );

      // navigate("/", {
      //   state: {
      //     formResult: {
      //       data: {
      //         clause: selectedClauses,
      //         conditions: rows,
      //         expression: finalExpression,
      //       },
      //     },
      //   },
      // });
      refresh();
      closeModal();
    } catch (err) {
      console.error("Error saving query term:", err);
      showPopup(
        "error",
        "Save Failed",
        "Failed to save query term. Check console for details.",
      );
    }
  };

  return (
    <div className="dynamic-form-container">
      <div style={{ marginBottom: "20px" }}>
        <label>
          <strong>Type</strong> <span style={{ color: "red" }}>*</span>
        </label>
        <select
          value={type}
          onChange={(e) => {
            setType(e.target.value);
            setClause([]);
            setTemplate("");
            setSigner("");
            setSigningOrder("");
            setSequence("");
            setDescription("");
          }}
          style={{
            display: "block",
            width: "auto",
            minWidth: "140px",
            padding: "8px",
            marginTop: "5px",
          }}
        >
          <option value="">Select Type</option>
          <option value="Clause">Clause</option>
          <option value="Template">Template</option>
          <option value="Signer">Signer</option>
        </select>
      </div>

      {/*SHOW CLAUSE PICKLIST IF TYPE IS CLAUSE*/}
      {type === "Clause" && (
        <div style={{ marginBottom: "20px" }}>
          <label>
            <strong>Select Clauses</strong>{" "}
            <span style={{ color: "red" }}>*</span>
          </label>
          <Select
            value={clause}
            onChange={setClause}
            options={clauseOptions}
            isClearable
            isSearchable
            isMulti
            placeholder="Select one or more clauses..."
            styles={{
              multiValue: (provided) => ({
                ...provided,
                backgroundColor: "#F9593B",
                color: "#ffffff",
                fontSize: "12px",
              }),
              multiValueLabel: (provided) => ({
                ...provided,
                color: "#ffffff",
                fontWeight: 500,
                fontSize: "12px",
              }),
              multiValueRemove: (provided) => ({
                ...provided,
                color: "#ffffff",
                fontSize: "12px",
                ":hover": {
                  backgroundColor: "#e04f33",
                  color: "white",
                },
              }),
              placeholder: (provided) => ({
                ...provided,
                fontSize: "13px",
              }),
              option: (provided) => ({
                ...provided,
                fontSize: "13px",
              }),
            }}
          />
        </div>
      )}

      {/*SHOW TEMPLATE PICKLIST IF TYPE IS TEMPLATE*/}
      {type === "Template" && (
        <>
          <div style={{ marginBottom: "20px" }}>
            <label>
              <strong>Select Template</strong>{" "}
              <span style={{ color: "red" }}>*</span>
            </label>
            <Select
              value={templateOptions.find((t) => t.value === template) || null}
              onChange={(selected) => setTemplate(selected?.value || "")}
              options={templateOptions}
              isClearable
              isSearchable
              placeholder="Select template..."
              styles={{
                valueContainer: (provided) => ({
                  ...provided,
                  display: "flex",
                  alignItems: "center",
                  gap: "4px",
                }),

                singleValue: (provided) => ({
                  ...provided,
                  backgroundColor: "#F9593B",
                  color: "#ffffff",
                  padding: "2px 6px",
                  borderRadius: "4px",
                  width: "auto",
                  display: "inline-flex",
                  maxWidth: "fit-content",
                  fontSize: "12px",
                }),

                placeholder: (provided) => ({
                  ...provided,
                  fontSize: "13px",
                }),
                option: (provided) => ({
                  ...provided,
                  fontSize: "13px",
                  color: "#000000",
                }),
              }}
            />
          </div>
          <div style={{ marginBottom: "20px" }}>
            <label>
              <strong>Sequence</strong> <span style={{ color: "red" }}>*</span>
            </label>
            <input
              type="text"
              className="query-box"
              placeholder="Enter Sequence"
              value={sequence}
              onChange={(e) => {
                // Only allow numbers
                const value = e.target.value;
                if (value === "" || /^\d*$/.test(value)) {
                  setSequence(value);
                }
              }}
              min="0"
              style={{
                width: "140px", 
                padding: "8px 12px", 
                marginTop: "5px", 
                display: "block",
                fontSize: "13px",
                height: "38px"
              }}
            />
          </div>

          <div style={{ marginBottom: "20px" }}>
            <label>
              <strong>Description</strong>
            </label>
            <textarea
              className="query-box"
              placeholder="Enter Description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              style={{
                width: "100%",
                padding: "8px",
                marginTop: "5px",
                minHeight: "60px",
                resize: "vertical",
                fontSize: "14px",
              }}
              rows={3}
            />
          </div>
        </>
      )}

{type === "Signer" && (
  <div
    style={{
      display: "flex",
      alignItems: "flex-end",
      gap: "20px",
      marginBottom: "20px",
      flexWrap: "wrap", // responsive on small screens
    }}
  >
    {/* SELECT SIGNER */}
    <div style={{ display: "flex", flexDirection: "column" }}>
      <label>
        <strong>Select Signer</strong>{" "}
        <span style={{ color: "red" }}>*</span>
      </label>

      <Select
        value={signerOptions.find((s) => s.value === signer) || null}
        onChange={(selected) => {
          const selectedSigner = selected?.value || "";
          setSigner(selectedSigner);
          if (selectedSigner === "Signer3") {
            setSigningOrder("3");
          } else {
            setSigningOrder("");
          }
        }}
        options={signerOptions}
        isClearable
        isSearchable={false}
        placeholder="Select..."
        styles={{
          container: (provided) => ({
            ...provided,
            width: "160px",
            marginTop: "5px",
          }),
          control: (provided) => ({
            ...provided,
            minHeight: "38px",
            height: "38px",
            borderRadius: "8px",
            borderColor: "#ccc",
            boxShadow: "none",
          }),
          valueContainer: (provided) => ({
            ...provided,
            padding: "0 8px",
            fontSize: "13px",
          }),
          indicatorsContainer: (provided) => ({
            ...provided,
            height: "38px",
            gap: "2px",
          }),
          clearIndicator: (provided) => ({
            ...provided,
            padding: "0 4px",
            transform: "scale(0.8)",
          }),
          dropdownIndicator: (provided) => ({
            ...provided,
            padding: "0 4px",
            transform: "scale(0.8)",
          }),
          indicatorSeparator: () => ({ display: "none" }),
          singleValue: (provided) => ({
            ...provided,
            backgroundColor: "#F9593B",
            color: "#fff",
            padding: "2px 6px",
            borderRadius: "4px",
            fontSize: "12px",
          }),
        }}
      />
    </div>

    {/* SIGNING ORDER */}
    <div style={{ display: "flex", flexDirection: "column" }}>
      <label>
        <strong>Signing Order</strong>{" "}
        <span style={{ color: "red" }}>*</span>
      </label>

      <input
        type="text"
        className="query-box"
        placeholder="Signing Order"
        value={signer === "Signer3" ? "3" : signingOrder}
        disabled={signer === "Signer3"}
        style={{
          width: "140px",
          padding: "8px 12px",
          marginTop: "5px",
          fontSize: "13px",
          height: "38px",
          backgroundColor: signer === "Signer3" ? "#f5f5f5" : "#fff",
          cursor: signer === "Signer3" ? "not-allowed" : "text",
        }}
        onChange={(e) => {
          if (signer !== "Signer3") {
            const value = e.target.value;
            if (value === "" || /^\d*$/.test(value)) {
              setSigningOrder(value);
            }
          }
        }}
      />
    </div>
  </div>
)}

      <strong style={{ fontWeight: "bold" }}>Conditions</strong> <span style={{ color: "red" }}>*</span>

      {rows.map((row, index) => (
        <div className="condition-row" key={index}>
          <strong>{index + 1}.</strong>

          <select
            className="query-box"
            value={row.field}
            onChange={(e) => updateRow(index, "field", e.target.value)}
          >
            <option value="">Field</option>
            {fieldOptions.map((f) => (
              <option key={f.FieldName} value={f.FieldName}>
                {f.DisplayName}
              </option>
            ))}
          </select>

          <select
            className="query-box"
            value={row.operator}
            onChange={(e) => updateRow(index, "operator", e.target.value)}
          >
            <option value="">Operator</option>
            {operatorOptions.map((op) => (
              <option key={op} value={op}>
                {op}
              </option>
            ))}
          </select>

          <input
            type="text"
            className="query-box"
            placeholder="Value"
            value={row.value}
            onChange={(e) => updateRow(index, "value", e.target.value)}
          />

          <div className="row-actions">
            {index === rows.length - 1 && (
              <button className="add-row-btn" onClick={addRow}>
                +
              </button>
            )}
            <button
              className="remove-row-btn"
              onClick={() => removeRow(index)}
              disabled={rows.length === 1}
            >
              -
            </button>
          </div>
        </div>
      ))}

      <label>
        <strong>Expression</strong>
      </label>
      <input
        type="text"
        placeholder="e.g. 1 AND 2 OR 3"
        value={expression}
        onChange={(e) => {
          const val = e.target.value;

          const upperCased = val
            .replace(/\band\b/gi, "AND")
            .replace(/\bor\b/gi, "OR");

          setExpression(upperCased);
          setExpressionEdited(true);
        }}
      />

      <div className="form-buttons">
        <button className="back-btn" onClick={closeModal}>
          Cancel
        </button>

        <button className="save-btn" onClick={handleSave}>
          {editId ? "Update" : "Save"}
        </button>
      </div>
    </div>
  );
};

export default DynamicForm;
