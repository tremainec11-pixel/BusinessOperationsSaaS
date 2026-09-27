import api from "./api";

export const getEmployees = async () => {
  const response = await api.get("/Employees");

  return response.data;
};

export const getEmployee = async (id) => {
  const response = await api.get(`/Employees/${id}`);

  return response.data;
};

export const createEmployee = async (data) => {
  const response = await api.post("/Employees", data);

  return response.data;
};

export const updateEmployee = async (id, data) => {
  const response = await api.put(`/Employees/${id}`, data);

  return response.data;
};

export const deleteEmployee = async (id) => {
  await api.delete(`/Employees/${id}`);
};