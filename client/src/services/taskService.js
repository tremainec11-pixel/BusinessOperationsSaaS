import api from "./api";

export const getTasks = async () => {
  const response = await api.get("/Tasks");
  return response.data;
};

export const getTaskById = async (id) => {
  const response = await api.get(`/Tasks/${id}`);
  return response.data;
};

export const createTask = async (task) => {
  const response = await api.post("/Tasks", task);
  return response.data;
};

export const updateTask = async (id, task) => {
  const response = await api.put(`/Tasks/${id}`, task);
  return response.data;
};

export const deleteTask = async (id) => {
  const response = await api.delete(`/Tasks/${id}`);
  return response.data;
};
