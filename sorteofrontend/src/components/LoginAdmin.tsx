import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/api';
import './LoginAdmin.css';

/**
 * Pantalla de inicio de sesión para administradores.
 * Al autenticarse, guarda el token y redirige al dashboard.
 */
const LoginAdmin: React.FC = () => {
  const [credentials, setCredentials] = useState({
    usuario: '',
    password: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setCredentials(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const response = await authService.login(credentials);
      localStorage.setItem('token', response.token);
      localStorage.setItem('adminInfo', JSON.stringify({
        usuario: response.usuario,
        nombre: response.nombre
      }));
      navigate('/admin/dashboard');
    } catch (error: any) {
      setError(error.response?.data?.message || 'Error al iniciar sesión');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          <h1>🔐 Administrador</h1>
          <p>Inicie sesión para acceder al panel de administración</p>
        </div>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label htmlFor="usuario">Usuario</label>
            <input
              type="text"
              id="usuario"
              name="usuario"
              value={credentials.usuario}
              onChange={handleInputChange}
              required
              placeholder="Ingrese su usuario"
              autoComplete="username"
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Contraseña</label>
            <input
              type="password"
              id="password"
              name="password"
              value={credentials.password}
              onChange={handleInputChange}
              required
              placeholder="Ingrese su contraseña"
              autoComplete="current-password"
            />
          </div>

          <button type="submit" className="btn-login" disabled={loading}>
            {loading ? 'Iniciando sesión...' : 'Iniciar Sesión'}
          </button>
        </form>

        <div className="login-footer">
          <p>Credenciales por defecto:</p>
          <p><strong>Usuario:</strong> admin</p>
          <p><strong>Contraseña:</strong> admin123</p>
        </div>
      </div>
    </div>
  );
};

export default LoginAdmin;
