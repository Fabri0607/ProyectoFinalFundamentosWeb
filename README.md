<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
</head>
<body>

  <h1>Sistema Inventario Ventas - Backend</h1>

  <p><strong>Repositorio del Frontend:</strong> <a href="https://github.com/Fabri0607/ProyectoFinal-FrontEnd">ProyectoFinal-FrontEnd</a></p>

  <h2>📋 Descripción General</h2>
  <p>
    Este sistema es una aplicación web empresarial diseñada con ASP.NET Core para la gestión de inventario de productos,
    ventas y reportes de negocio. Implementa una arquitectura por capas, control de acceso basado en roles
    y seguimiento de inventario en tiempo real mediante registros automáticos de movimiento.
  </p>

  <h2>🎯 Funcionalidades Principales</h2>
  <ul>
    <li><strong>Gestión de Productos:</strong> CRUD completo con clasificación por categorías.</li>
    <li><strong>Procesamiento de Ventas:</strong> Manejo de transacciones, actualización automática del inventario y generación de facturas.</li>
    <li><strong>Control de Inventario:</strong> Seguimiento en tiempo real, historial de movimientos y ajustes manuales.</li>
    <li><strong>Reportes Empresariales:</strong> Informes de ventas e inventario con parámetros configurables.</li>
    <li><strong>Gestión de Usuarios:</strong> Control de acceso basado en roles: Administrador, Colaborador y Vendedor.</li>
  </ul>

  <h2>🧱 Arquitectura General</h2>
  <p>El sistema está dividido en cuatro capas principales:</p>
  <ul>
    <li><strong>API Controllers:</strong> AuthController, ProductoController, VentaController, etc.</li>
    <li><strong>Servicios:</strong> ProductoService, VentaService, MovimientoInventarioService, etc.</li>
    <li><strong>Unidad de Trabajo:</strong> Coordinación de operaciones de datos (UnidadDeTrabajo).</li>
    <li><strong>Acceso a Datos:</strong> Implementaciones DAL como ProductoDALImpl, VentaDALImpl, etc.</li>
  </ul>

  <h2>⚙️ Tecnologías Utilizadas</h2>
  <table border="1" cellpadding="8">
    <thead>
      <tr>
        <th>Componente</th>
        <th>Tecnología</th>
        <th>Implementación</th>
      </tr>
    </thead>
    <tbody>
      <tr><td>Backend</td><td>ASP.NET Core 6+</td><td>Web API</td></tr>
      <tr><td>Base de Datos</td><td>SQL Server</td><td>Entity Framework Core</td></tr>
      <tr><td>Autenticación</td><td>ASP.NET Identity</td><td>JWT Bearer Tokens</td></tr>
      <tr><td>Autorización</td><td>Basado en Roles</td><td>Admin, Colaborador, Vendedor</td></tr>
      <tr><td>Logs</td><td>Serilog</td><td>Archivos con rotación diaria</td></tr>
      <tr><td>Email</td><td>SMTP</td><td>Integración con Gmail vía CorreoHelper</td></tr>
      <tr><td>CORS</td><td>ASP.NET Core CORS</td><td>Soporte a frontend en React</td></tr>
    </tbody>
  </table>

  <h2>🏗️ Estructura del Proyecto</h2>
  <ul>
    <li><strong>Proyecto Web API:</strong> <code>BackEnd</code></li>
    <li><strong>Capa de Acceso a Datos:</strong> <code>DAL</code></li>
    <li><strong>Modelos de Dominio:</strong> <code>Entities</code></li>
    <li><strong>Configuración de Contextos:</strong> SistemaInventarioVentasContext y AuthDBContext</li>
  </ul>

  <h2>📦 Entidades Principales</h2>
  <ul>
    <li><strong>Producto:</strong> Gestión del catálogo. Relación con <em>Parametro</em> y <em>MovimientoInventario</em>.</li>
    <li><strong>Venta:</strong> Registro de ventas. Contiene múltiples <em>DetalleVenta</em>.</li>
    <li><strong>DetalleVenta:</strong> Ítems individuales por venta. Relación con <em>Producto</em>.</li>
    <li><strong>MovimientoInventario:</strong> Registro de entradas y salidas de inventario.</li>
    <li><strong>Parametro:</strong> Valores de configuración (categorías, tipos de movimiento, etc.).</li>
  </ul>

  <h2>🔐 Autenticación y Autorización</h2>
  <ul>
    <li>Gestión de identidad mediante <code>ApplicationUser</code> extendiendo <code>IdentityUser</code>.</li>
    <li>Roles: <strong>Administrador</strong>, <strong>Colaborador</strong>, <strong>Vendedor</strong>.</li>
    <li>Uso de <strong>JWT</strong> con expiración configurable.</li>
    <li>Base de datos de autenticación separada (<code>AuthDBContext</code>).</li>
    <li>Inicialización automática de roles y usuario admin en el arranque (SeedRolesAndAdmin).</li>
  </ul>

  <h2>🔗 Integraciones Externas</h2>
  <ul>
    <li><strong>Email:</strong> Envío de correos usando SMTP y Gmail.</li>
    <li><strong>Logging:</strong> Integración con Serilog para manejo de errores.</li>
    <li><strong>Frontend:</strong> Configuración CORS para desarrollo con React.</li>
  </ul>

  <h2>🚀 Configuración para Desarrollo</h2>
  <ol>
    <li>Clonar este repositorio y restaurar paquetes NuGet.</li>
    <li>Actualizar las cadenas de conexión en <code>appsettings.json</code>.</li>
    <li>Ejecutar migraciones EF Core para crear la base de datos.</li>
    <li>Levantar el servidor API con IIS Express o Kestrel.</li>
    <li>Verificar los endpoints en <code>/swagger</code> si está habilitado.</li>
  </ol>

  <hr>
  <p>Desarrollado por: <strong>Fabricio Alfaro Arce</strong>, Nahum Mora, Juan Rodríguez y Yehudy Moreira | Curso: Fundamentos de Programación Web</p>

</body>
</html>
