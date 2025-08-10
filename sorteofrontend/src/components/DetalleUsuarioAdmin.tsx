import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { usuarioService } from '../services/api';
import { UsuarioDetalle } from '../types';

/**
 * Vista de detalle para una inscripción específica.
 * Previsualiza/descarga el documento y permite cambiar el estado.
 */
const DetalleUsuarioAdmin: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [detalle, setDetalle] = useState<UsuarioDetalle | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [estado, setEstado] = useState('');
  const [comentario, setComentario] = useState('');
  const [saving, setSaving] = useState(false);
  const [docUrl, setDocUrl] = useState<string | null>(null);

  useEffect(() => {
    const cargar = async () => {
      try {
        if (!id) return;
        setLoading(true);
        const data = await usuarioService.obtenerDetalle(Number(id));
        setDetalle(data);
        setEstado(data.estado);
      } catch (e: any) {
        setError(e.response?.data?.message || 'Error al cargar el detalle');
      } finally {
        setLoading(false);
      }
    };
    cargar();
  }, [id]);

  useEffect(() => {
    const fetchDocumento = async () => {
      if (!detalle) return;
      try {
        const blob = await usuarioService.descargarDocumento(detalle.rutaDocumento);
        const url = URL.createObjectURL(blob);
        setDocUrl(url);
        return () => URL.revokeObjectURL(url);
      } catch (e) {
        // Ignorar si no se puede cargar
      }
    };
    fetchDocumento();
  }, [detalle]);

  const handleDescargar = async () => {
    if (!detalle) return;
    const blob = await usuarioService.descargarDocumento(detalle.rutaDocumento);
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = detalle.rutaDocumento;
    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(url);
  };

  const handleCambiarEstado = async () => {
    if (!detalle) return;
    if (estado !== 'Aceptada' && estado !== 'Rechazada') {
      setError("El estado debe ser 'Aceptada' o 'Rechazada'");
      return;
    }
    try {
      setSaving(true);
      await usuarioService.cambiarEstado({
        usuarioId: detalle.id,
        estado,
        comentarioAdmin: comentario || undefined,
      });
      const refreshed = await usuarioService.obtenerDetalle(detalle.id);
      setDetalle(refreshed);
    } catch (e: any) {
      setError(e.response?.data?.message || 'No se pudo cambiar el estado');
    } finally {
      setSaving(false);
    }
  };

  const renderDocumento = () => {
    if (!detalle || !docUrl) return <div style={{ padding: 8, opacity: 0.8 }}>Documento no disponible</div>;
    const isPdf = detalle.rutaDocumento.toLowerCase().endsWith('.pdf');
    if (isPdf) {
      return (
        <embed src={docUrl} type="application/pdf" width="100%" height="600px" />
      );
    }
    return (
      <img src={docUrl} alt="Documento" style={{ maxWidth: '100%', borderRadius: 8 }} />
    );
  };

  if (loading) return <div style={{ padding: 24, color: 'white' }}>Cargando...</div>;
  if (error) return <div style={{ padding: 24 }} className="error-message">{error}</div>;
  if (!detalle) return null;

  return (
    <div style={{ minHeight: '100vh', background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', color: 'white' }}>
      <div style={{ maxWidth: 1000, margin: '0 auto', padding: 24 }}>
        <button onClick={() => navigate(-1)} style={{ marginBottom: 16 }} className="btn-ver-detalle">⬅ Volver</button>
        <h1 style={{ textShadow: '2px 2px 4px rgba(0,0,0,0.3)' }}>Detalle de Inscripción #{detalle.id}</h1>

        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 24 }}>
          <div style={{ background: 'rgba(255,255,255,0.1)', border: '1px solid rgba(255,255,255,0.2)', borderRadius: 12, padding: 16 }}>
            <h2>Información del Usuario</h2>
            <p><strong>Nombre:</strong> {detalle.nombresApellidos}</p>
            <p><strong>Documento:</strong> {detalle.tipoDocumento} {detalle.numeroDocumento}</p>
            <p><strong>Fecha de Nacimiento:</strong> {new Date(detalle.fechaNacimiento).toLocaleDateString('es-ES')}</p>
            <p><strong>Dirección:</strong> {detalle.direccion}</p>
            <p><strong>Teléfono:</strong> {detalle.telefono}</p>
            <p><strong>Email:</strong> {detalle.correoElectronico}</p>
            <p><strong>Fecha Solicitud:</strong> {new Date(detalle.fechaSolicitud).toLocaleString('es-ES')}</p>
            <p><strong>Estado actual:</strong> {detalle.estado}</p>
            {detalle.comentarioAdmin && <p><strong>Comentario admin:</strong> {detalle.comentarioAdmin}</p>}
          </div>

          <div style={{ background: 'rgba(255,255,255,0.1)', border: '1px solid rgba(255,255,255,0.2)', borderRadius: 12, padding: 16 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <h2>Documento adjunto</h2>
              <button onClick={handleDescargar} className="btn-refresh">⬇ Descargar</button>
            </div>
            <div style={{ background: 'rgba(0,0,0,0.2)', padding: 8, borderRadius: 8 }}>
              {renderDocumento()}
            </div>
          </div>
        </div>

        <div style={{ marginTop: 24, background: 'rgba(255,255,255,0.1)', border: '1px solid rgba(255,255,255,0.2)', borderRadius: 12, padding: 16 }}>
          <h2>Cambiar estado</h2>
          <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap' }}>
            <select value={estado} onChange={(e) => setEstado(e.target.value)} style={{ padding: 8, borderRadius: 8 }}>
              <option value="Pendiente">Pendiente</option>
              <option value="Aceptada">Aceptada</option>
              <option value="Rechazada">Rechazada</option>
            </select>
            <input
              type="text"
              placeholder="Comentario (opcional)"
              value={comentario}
              onChange={(e) => setComentario(e.target.value)}
              style={{ flex: 1, minWidth: 240, padding: 8, borderRadius: 8 }}
            />
            <button onClick={handleCambiarEstado} disabled={saving} className="btn-refresh">
              {saving ? 'Guardando...' : 'Guardar cambio'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DetalleUsuarioAdmin;
