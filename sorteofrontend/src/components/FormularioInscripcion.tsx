import React, { useState } from 'react';
import { UsuarioInscripcion } from '../types';
import { usuarioService } from '../services/api';
import './FormularioInscripcion.css';

/**
 * Formulario público de inscripción al sorteo.
 * Envía datos por multipart/form-data incluyendo el archivo de documento.
 */
const FormularioInscripcion: React.FC = () => {
  const [formData, setFormData] = useState<UsuarioInscripcion>({
    tipoDocumento: '',
    numeroDocumento: '',
    nombresApellidos: '',
    fechaNacimiento: '',
    direccion: '',
    telefono: '',
    correoElectronico: '',
  });

  const [documento, setDocumento] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setDocumento(e.target.files[0]);
    }
  };

  // Envía el formulario y maneja respuesta/errores
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!documento) {
      setMessage({ type: 'error', text: 'Debe seleccionar un documento' });
      return;
    }

    setLoading(true);
    setMessage(null);

    try {
      await usuarioService.inscripcion(formData, documento);
      setMessage({ type: 'success', text: 'Inscripción realizada exitosamente' });
      
      // Limpiar formulario
      setFormData({
        tipoDocumento: '',
        numeroDocumento: '',
        nombresApellidos: '',
        fechaNacimiento: '',
        direccion: '',
        telefono: '',
        correoElectronico: '',
      });
      setDocumento(null);
      
      // Limpiar input de archivo
      const fileInput = document.getElementById('documento') as HTMLInputElement;
      if (fileInput) fileInput.value = '';
      
    } catch (error: any) {
      setMessage({ 
        type: 'error', 
        text: error.response?.data?.message || 'Error al procesar la inscripción' 
      });
    } finally {
      setLoading(false);
    }
  };

  const calcularEdadMinima = () => {
    const fecha = new Date();
    fecha.setFullYear(fecha.getFullYear() - 18);
    return fecha.toISOString().split('T')[0];
  };

  return (
    <div className="formulario-container">
      <h2>Inscripción al Sorteo</h2>
      <p className="formulario-descripcion">
        Complete el formulario para participar en nuestro sorteo. Todos los campos son obligatorios.
      </p>

      {message && (
        <div className={`mensaje ${message.type}`}>
          {message.text}
        </div>
      )}

      <form onSubmit={handleSubmit} className="formulario">
        <div className="form-group">
          <label htmlFor="tipoDocumento">Tipo de Documento *</label>
          <select
            id="tipoDocumento"
            name="tipoDocumento"
            value={formData.tipoDocumento}
            onChange={handleInputChange}
            required
          >
            <option value="">Seleccione un tipo</option>
            <option value="CC">Cédula de Ciudadanía</option>
            <option value="CE">Cédula de Extranjería</option>
            <option value="TI">Tarjeta de Identidad</option>
            <option value="PP">Pasaporte</option>
          </select>
        </div>

        <div className="form-group">
          <label htmlFor="numeroDocumento">Número de Documento *</label>
          <input
            type="text"
            id="numeroDocumento"
            name="numeroDocumento"
            value={formData.numeroDocumento}
            onChange={handleInputChange}
            required
            placeholder="Ej: 12345678"
          />
        </div>

        <div className="form-group">
          <label htmlFor="nombresApellidos">Nombres y Apellidos *</label>
          <input
            type="text"
            id="nombresApellidos"
            name="nombresApellidos"
            value={formData.nombresApellidos}
            onChange={handleInputChange}
            required
            placeholder="Ej: Juan Carlos Pérez López"
          />
        </div>

        <div className="form-group">
          <label htmlFor="fechaNacimiento">Fecha de Nacimiento *</label>
          <input
            type="date"
            id="fechaNacimiento"
            name="fechaNacimiento"
            value={formData.fechaNacimiento}
            onChange={handleInputChange}
            required
            max={calcularEdadMinima()}
          />
          <small>Debe ser mayor de edad (18 años o más)</small>
        </div>

        <div className="form-group">
          <label htmlFor="direccion">Dirección *</label>
          <input
            type="text"
            id="direccion"
            name="direccion"
            value={formData.direccion}
            onChange={handleInputChange}
            required
            placeholder="Ej: Calle 123 # 45-67, Ciudad"
          />
        </div>

        <div className="form-group">
          <label htmlFor="telefono">Teléfono *</label>
          <input
            type="tel"
            id="telefono"
            name="telefono"
            value={formData.telefono}
            onChange={handleInputChange}
            required
            placeholder="Ej: 3001234567"
          />
        </div>

        <div className="form-group">
          <label htmlFor="correoElectronico">Correo Electrónico *</label>
          <input
            type="email"
            id="correoElectronico"
            name="correoElectronico"
            value={formData.correoElectronico}
            onChange={handleInputChange}
            required
            placeholder="Ej: usuario@email.com"
          />
        </div>

        <div className="form-group">
          <label htmlFor="documento">Documento de Identidad *</label>
          <input
            type="file"
            id="documento"
            name="documento"
            onChange={handleFileChange}
            required
            accept=".jpg,.jpeg,.png,.pdf"
          />
          <small>Formatos permitidos: JPG, PNG, PDF. Máximo 5MB</small>
        </div>

        <button type="submit" className="btn-enviar" disabled={loading}>
          {loading ? 'Procesando...' : 'Enviar Inscripción'}
        </button>
      </form>
    </div>
  );
};

export default FormularioInscripcion;
