// Servicio centralizado de API con Axios.
// - Adjunta JWT automáticamente si existe en localStorage
// - Redirige a /login en caso de 401
// - Expone métodos para auth y usuarios
import axios from 'axios';
import { UsuarioInscripcion, UsuarioLista, UsuarioDetalle, CambioEstado, LoginRequest, LoginResponse } from '../types';

// URL base del backend leída desde variables de entorno (.env.local)
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://localhost:7001/api';

// Configurar axios
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para agregar el token JWT
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor para manejar errores
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/auth/login', credentials);
    return response.data;
  },
};

export const usuarioService = {
  // Inscripción de usuario
  inscripcion: async (data: UsuarioInscripcion, documento: File): Promise<any> => {
    const formData = new FormData();
    
    // Agregar datos del usuario
    Object.entries(data).forEach(([key, value]) => {
      formData.append(key, value);
    });
    
    // Agregar documento
    formData.append('documento', documento);
    
    const response = await api.post('/usuarios/inscripcion', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  // Obtener lista de usuarios (para admin)
  obtenerLista: async (): Promise<UsuarioLista[]> => {
    const response = await api.get<UsuarioLista[]>('/usuarios/lista');
    return response.data;
  },

  // Obtener detalle de usuario (para admin)
  obtenerDetalle: async (id: number): Promise<UsuarioDetalle> => {
    const response = await api.get<UsuarioDetalle>(`/usuarios/detalle/${id}`);
    return response.data;
  },

  // Cambiar estado de usuario (para admin)
  cambiarEstado: async (data: CambioEstado): Promise<any> => {
    const response = await api.put('/usuarios/cambiar-estado', data);
    return response.data;
  },

  // Descargar documento protegido (retorna Blob)
  descargarDocumento: async (nombreArchivo: string): Promise<Blob> => {
    const response = await api.get(`/usuarios/documento/${nombreArchivo}`, { responseType: 'blob' });
    return response.data as Blob;
  },

  // URL base si se requiere
  obtenerDocumentoUrl: (nombreArchivo: string): string => {
    return `${API_BASE_URL}/usuarios/documento/${nombreArchivo}`;
  },
};

export default api;
