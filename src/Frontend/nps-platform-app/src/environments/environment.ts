export const environment = {
  production: false,
  // CORRECCIÓN CLAVE AQUÍ: Usamos un puerto HTTPS válido, asumiendo 7056
  // (Si 7056 fuera solo para el Dashboard, usarías 7176 o el puerto HTTPS real del Identity Service)
  identityApiUrl: 'https://localhost:7176/api/identity',

  // Asumiendo que esta es la URL base del Identity Service (aunque no se usa en el AuthService)
  npsApiUrl: 'https://localhost:57282/api/nps',
  dashboardApiUrl: 'https://localhost:7056/api/dashboard',
};
