import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { usuarioService } from '../services/api';
import { UsuarioLista } from '../types';
import './DashboardAdmin.css';

/**
 * Panel principal del administrador.
 * Muestra lista de inscripciones, estadísticas y permite navegar al detalle.
 */
const DashboardAdmin: React.FC = () => {
  const [usuarios, setUsuarios] = useState<UsuarioLista[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    cargarUsuarios();
  }, []);

  const cargarUsuarios = async () => {
    try {
      setLoading(true);
      const data = await usuarioService.obtenerLista();
      setUsuarios(data);
    } catch (error: any) {
      setError('Error al cargar la lista de usuarios');
      console.error('Error:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('adminInfo');
    navigate('/login');
  };

  const handleVerDetalle = (id: number) => {
    navigate(`/admin/usuario/${id}`);
  };

  const getEstadoColor = (estado: string) => {
    switch (estado) {
      case 'Pendiente':
        return 'estado-pendiente';
      case 'Aceptada':
        return 'estado-aceptada';
      case 'Rechazada':
        return 'estado-rechazada';
      default:
        return '';
    }
  };

  const getEstadoIcon = (estado: string) => {
    switch (estado) {
      case 'Pendiente':
        return '⏳';
      case 'Aceptada':
        return '✅';
      case 'Rechazada':
        return '❌';
      default:
        return '❓';
    }
  };

  const formatearFecha = (fecha: string) => {
    return new Date(fecha).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  if (loading) {
    return (
      <div className="dashboard-container">
        <div className="loading">Cargando usuarios...</div>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <div className="header-content">
          <h1>🎯 Panel de Administración</h1>
          <div className="header-actions">
            <button onClick={cargarUsuarios} className="btn-refresh">
              🔄 Actualizar
            </button>
            <button onClick={handleLogout} className="btn-logout">
              🚪 Cerrar Sesión
            </button>
          </div>
        </div>
      </header>

      <main className="dashboard-main">
        <div className="stats-container">
          <div className="stat-card">
            <h3>Total Inscripciones</h3>
            <p className="stat-number">{usuarios.length}</p>
          </div>
          <div className="stat-card">
            <h3>Pendientes</h3>
            <p className="stat-number">{usuarios.filter(u => u.estado === 'Pendiente').length}</p>
          </div>
          <div className="stat-card">
            <h3>Aceptadas</h3>
            <p className="stat-number">{usuarios.filter(u => u.estado === 'Aceptada').length}</p>
          </div>
          <div className="stat-card">
            <h3>Rechazadas</h3>
            <p className="stat-number">{usuarios.filter(u => u.estado === 'Rechazada').length}</p>
          </div>
        </div>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        <div className="usuarios-container">
          <h2>Lista de Inscripciones</h2>
          
          {usuarios.length === 0 ? (
            <div className="no-usuarios">
              <p>No hay inscripciones registradas</p>
            </div>
          ) : (
            <div className="usuarios-grid">
              {usuarios.map((usuario) => (
                <div 
                  key={usuario.id} 
                  className="usuario-card"
                  onClick={() => handleVerDetalle(usuario.id)}
                >
                  <div className="usuario-header">
                    <span className={`estado-badge ${getEstadoColor(usuario.estado)}`}>
                      {getEstadoIcon(usuario.estado)} {usuario.estado}
                    </span>
                    <span className="usuario-id">#{usuario.id}</span>
                  </div>
                  
                  <div className="usuario-info">
                    <h3>{usuario.nombresApellidos}</h3>
                    <p className="fecha-solicitud">
                      📅 {formatearFecha(usuario.fechaSolicitud)}
                    </p>
                  </div>
                  
                  <div className="usuario-actions">
                    <button className="btn-ver-detalle">
                      👁️ Ver Detalle
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default DashboardAdmin;
