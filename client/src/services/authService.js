import api from "./api";

export const login = async (email, password) => {
  const response = await api.post("/Auth/login", {
    email,
    password,
  });

  return response.data;
};

export const register = async (data) => {
  const response = await api.post("/Auth/register", data);

  return response.data;
};