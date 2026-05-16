// src/api/apiClient.ts
import axios from 'axios';

export const apiClient = axios.create({
  baseURL: 'https://good-hamburger.onrender.com/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Intercepta todas as requisições e injeta o token se o usuário estiver logado
apiClient.interceptors.request.use((config) => {
  // Mantive a mesma key "authToken" que você usava no JSInterop do Blazor
  const token = localStorage.getItem('authToken'); 
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});