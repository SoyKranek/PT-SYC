export interface UsuarioInscripcion {
  tipoDocumento: string;
  numeroDocumento: string;
  nombresApellidos: string;
  fechaNacimiento: string;
  direccion: string;
  telefono: string;
  correoElectronico: string;
}

export interface UsuarioLista {
  id: number;
  nombresApellidos: string;
  fechaSolicitud: string;
  estado: string;
}

export interface UsuarioDetalle {
  id: number;
  tipoDocumento: string;
  numeroDocumento: string;
  nombresApellidos: string;
  fechaNacimiento: string;
  direccion: string;
  telefono: string;
  correoElectronico: string;
  rutaDocumento: string;
  fechaSolicitud: string;
  estado: string;
  comentarioAdmin?: string;
  fechaRevision?: string;
}

export interface CambioEstado {
  usuarioId: number;
  estado: string;
  comentarioAdmin?: string;
}

export interface LoginRequest {
  usuario: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  usuario: string;
  nombre: string;
  expiracion: string;
}

export interface ApiResponse<T> {
  message?: string;
  data?: T;
  error?: string;
}
