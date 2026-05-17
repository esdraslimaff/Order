// src/api/apiClient.ts
import axios from 'axios';

export const apiClient = axios.create({
    baseURL: 'https://good-hamburger.onrender.com/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken'); 
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});