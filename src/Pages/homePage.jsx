import React, { useEffect, useState } from "react";
// import { useNavigate } from "react-router-dom";
import { getQueryData } from "../Services/getQueryData";
import { getToken } from "../Services/authToken";
import { deleteQueryTerm } from "../Services/deleteQueryData";
import "./HomePage.css";
import { FaTrash } from "react-icons/fa";
import { CiEdit } from "react-icons/ci";
import DynamicForm from "./createNew";

const HomePage = () => {
  // const navigate = useNavigate();
  const [savedData, setSavedData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [showFormModal, setShowFormModal] = useState(false);
  const [formToEdit, setFormToEdit] = useState(null);

  const [popup, setPopup] = useState({
    show: false,
    type: "",
    title: "",
    message: "",
    onConfirm: null,
  });

  const showPopup = (type, title, message, onConfirm = null) => {
    setPopup({ show: true, type, title, message, onConfirm });

    if (!onConfirm) {
      setTimeout(() => {
        setPopup((prev) => (prev.onConfirm ? prev : { show: false }));
      }, 5000);
    }
  };
  const closePopup = () => setPopup({ show: false });

  const fetchData = async () => {
    try {
      setLoading(true);
      const authToken = await getToken();
      const data = await getQueryData(authToken);

      const formattedData = data.map((item) => ({
        clause: item.clause,
        conditions: item.fields,
        expression: item.expression || "",
        type: item.type, 
        template: item.CNGCU_Clauses_c, 
        Id: item.Id,
        sequence: item.sequence,
      description: item.description,
      signer: item.signer,
      }));
console.log("format data: ", formattedData);

      setSavedData(formattedData);
    } catch (error) {
      console.error("Error fetching API data:", error);
      showPopup("error", "Error", "Failed to fetch data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleEdit = (record, index) => {
    setFormToEdit({ ...record, index});
    setShowFormModal(true);
  };

  const handleDelete = async (item) => {
    showPopup(
      "warning",
      "Confirm Delete",
      "Are you sure you want to delete this record?",
      async () => {
        try {
          const authToken = await getToken();
          await deleteQueryTerm(authToken, item.Id);

          showPopup("success", "Success!!", "Record deleted successfully.");
          fetchData();
        } catch (err) {
          console.error("Delete failed:", err);
          showPopup("error", "Error", "Failed to delete record.");
        }
      }
    );
  };

  return (
    <div className="homepage-wrapper">
      {popup.show && (
        <div className="custom-popup-overlay">
          <div className={`custom-popup popup-${popup.type}`}>
            <div className="popup-title">{popup.title}</div>
            <div className="popup-message">{popup.message}</div>

            {popup.onConfirm ? (
              <div style={{ display: "flex", gap: "10px" }}>
                <button
                  className="popup-btn"
                  onClick={() => {
                    popup.onConfirm();
                    closePopup();
                  }}
                >
                  Yes
                </button>

                <button
                  className="popup-btn"
                  style={{ background: "#555" }}
                  onClick={closePopup}
                >
                  No
                </button>
              </div>
            ) : (
              <button className="popup-btn" onClick={closePopup}>
                Close
              </button>
            )}
          </div>
        </div>
      )}

      {/* <div className="button-row"> */}
      <div className="header-row">
        <h6 className="page-title" style={{ fontSize: "20px" }}>
          Rule Builder
        </h6>

        <button
          onClick={() => {
            setFormToEdit(null);
            setShowFormModal(true);
          }}
          className="create-btn"
        >
          Create New
        </button>
      </div>

      {loading && (
        <div className="loader-wrapper">
          <div className="loader"></div>
        </div>
      )}

      {!loading && (
        <>
          {savedData.length === 0 ? (
            <p className="no-records">No records saved yet.</p>
          ) : (
            <div className="table-container">
              <table className="records-table">
                <thead>
                  <tr>
                    <th></th>
                    <th>Template/Clause/Signer</th>
                    <th>Conditions</th>
                    <th>Expression</th>
                  </tr>
                </thead>
                <tbody>
                  {savedData.map((item, index) => (
                    <tr key={index}>
                      <td className="action-buttons">
                        <button
                          className="edit-btn"
                          onClick={() => handleEdit(item, index)}
                        >
                          <CiEdit />
                        </button>

                        <button
                          className="delete-btn"
                          onClick={() => handleDelete(item)}
                        >
                          <FaTrash />
                        </button>
                      </td>

                      <td>{item.clause}</td>

                      <td>
                        {item.conditions.map((c, i) => (
                          <div key={i}>
                            {i + 1}. {c.field} {c.operator} {c.value}
                          </div>
                        ))}
                      </td>

                      <td>
                        {item.expression
                          ?.replace(/\band\b/gi, "AND")
                          .replace(/\bor\b/gi, "OR")}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </>
      )}
      {showFormModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <DynamicForm
              closeModal={() => setShowFormModal(false)}
              editData={formToEdit}
              refresh={fetchData}
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default HomePage;
