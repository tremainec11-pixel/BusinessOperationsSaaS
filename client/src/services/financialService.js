import api from "./api";

export const getFinancialTransactions = async () => {
  const response = await api.get("/Financials");
  return response.data;
};

export const getFinancialTransactionById = async (id) => {
  const response = await api.get(`/Financials/${id}`);
  return response.data;
};

export const createFinancialTransaction = async (transaction) => {
  const response = await api.post("/Financials", transaction);
  return response.data;
};

export const updateFinancialTransaction = async (id, transaction) => {
  const response = await api.put(`/Financials/${id}`, transaction);
  return response.data;
};

export const deleteFinancialTransaction = async (id) => {
  const response = await api.delete(`/Financials/${id}`);
  return response.data;
};
