# Sorteo - ASP.NET Core + React (TS)

Aplicación de inscripción a sorteo con panel de administración, autenticación JWT, subida de documentos y notificación por correo al cambiar estado.

## Requisitos
- .NET 9 SDK
- Node 18+
- SQL Server LocalDB (o SQL Server)

## Backend
`ash
cd SorteoAPI
# Config SMTP en user-secrets (recomendado, no en appsettings)
dotnet user-secrets set  Smtp:Host smtp.gmail.com
dotnet user-secrets set Smtp:Port 587
dotnet user-secrets set Smtp:User tu_correo
dotnet user-secrets set Smtp:Pass app_password
dotnet user-secrets set Smtp:From tu_correo
dotnet user-secrets set Smtp:Ssl true

# Ejecutar
dotnet run --urls http://localhost:5028
`
Swagger: http://localhost:5028/swagger

Credenciales admin por defecto: dmin / admin123.

## Frontend
`ash
cd sorteofrontend
copy .env.example .env.local
npm install
npm start
`
App: http://localhost:3000

## Flujo
1. Inscripción pública con documento (JPG/PNG/PDF).
2. Login admin, dashboard, detalle y cambio de estado.
3. Email opcional al aceptar/rechazar (si SMTP configurado).

## Notas
- DB: SQL Server LocalDB SorteoDB. Se crea al iniciar.
- Uploads locales en SorteoAPI/wwwroot/uploads (ignorados en git).
- Seguridad: JWT + BCrypt. CORS abierto en dev.

## Pruebas rápidas de SMTP
- Inicia sesión para obtener token, Authorize en Swagger.
- POST /api/Usuarios/test-email?to=destinatario.

## Presentación
Lee Presentacion_Sorteo.txt para más detalles de arquitectura y decisiones.
