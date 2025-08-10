import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import FormularioInscripcion from './components/FormularioInscripcion';
import LoginAdmin from './components/LoginAdmin';
import DashboardAdmin from './components/DashboardAdmin';
import DetalleUsuarioAdmin from './components/DetalleUsuarioAdmin';

// Ruta protegida: exige que exista token en localStorage
const ProtectedRoute: React.FC<{ children: React.ReactElement }> = ({ children }) => {
  const token = localStorage.getItem('token');
  if (!token) {
    return <Navigate to="/login" replace />;
  }
  return children;
};

function App() {
  return (
    <Routes>
      <Route path="/" element={<FormularioInscripcion />} />
      <Route path="/inscripcion" element={<FormularioInscripcion />} />
      <Route path="/login" element={<LoginAdmin />} />
      <Route
        path="/admin/dashboard"
        element={
          <ProtectedRoute>
            <DashboardAdmin />
          </ProtectedRoute>
        }
      />
      <Route
        path="/admin/usuario/:id"
        element={
          <ProtectedRoute>
            <DetalleUsuarioAdmin />
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
