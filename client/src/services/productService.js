import api from "./api";

export const getProducts = async () => {
  const response = await api.get("/Products");
  return response.data;
};

export const getProductById = async (id) => {
  const response = await api.get(`/Products/${id}`);
  return response.data;
};

export const createProduct = async (product) => {
  const response = await api.post("/Products", product);
  return response.data;
};

export const updateProduct = async (id, product) => {
  const response = await api.put(`/Products/${id}`, product);
  return response.data;
};

export const deleteProduct = async (id) => {
  const response = await api.delete(`/Products/${id}`);
  return response.data;
};
