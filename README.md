# CompuTrabajo Redarbor

Resumen
- Proyecto .NET 10 que implementa una API para gestión de empleados con una separación en capas: API, Application, Domain e Infrastructure.
- Autenticación JWT y documentación OpenAPI/Swagger.

Estructura principal
- `CompuTrabajo.Redarbor.Api` — capa Web/API (controllers, configuración de Swagger y JWT).
- `CompuTrabajo.Redarbor.Application` — lógica de aplicación (commands, queries, handlers, interfaces para persistencia).
- `CompuTrabajo.Redarbor.Domain` — entidades del dominio y reglas (p. ej. `Employee`).
- `CompuTrabajo.Redarbor.Infrastruture` — implementación de persistencia (EF Core, repositorios, UnitOfWork).
- `CompuTrabajo.Redarbor.Application.Tests` — pruebas unitarias xUnit para la capa Application.

Requisitos
- .NET 10 SDK
- (Opcional) Docker para correr la BD y PlantUML si desea generar diagramas con contenedores.

Cómo compilar y ejecutar
1. Restaurar paquetes y compilar:
   - `dotnet restore`
   - `dotnet build`
2. Ejecutar la API (desde el proyecto `CompuTrabajo.Redarbor.Api`):
   - `dotnet run --project CompuTrabajo.Redarbor.Api`
3. Swagger UI estará disponible en entorno `Development` en `/swagger`.

Pruebas
- Ejecutar pruebas de la solución:
  - `dotnet test` (ejecuta xUnit tests en `CompuTrabajo.Redarbor.Application.Tests` y cualquier otro proyecto de test).

Diagramas de arquitectura
- Archivos de diagramas (PlantUML) en `diagrams/`:
  - `architecture.puml` — vista de contenedores (API, Application, Domain, Infrastructure, DB, Tests).
  - `components.puml` — vista de componentes/fuentes (handlers, repositorios, UoW, DTOs).

Generar los diagramas
- Usar extensión PlantUML en VSCode o el contenedor Docker:
  - Con VSCode: instalar "PlantUML" y abrir los `.puml`.
  - Con Docker:
    - `docker run --rm -v "$PWD":/workspace -w /workspace plantuml/plantuml -tpng diagrams/architecture.puml`

Notas importantes
- La configuración de Swagger y OpenAPI puede cambiar según las versiones de paquetes (`Swashbuckle`, `Microsoft.OpenApi`).
- Las pruebas unitarias usan `Moq` para simular repositorios y `xUnit` como runner.


Generar token de autenticación (JWT)
-----------------------------------

La API incluye un endpoint de autenticación de demo en `POST /api/auth/login` que devuelve un `access_token` JWT. Este token debe enviarse en la cabecera `Authorization: Bearer {token}` para acceder a los endpoints protegidos (p. ej. `api/Redarbor`).

Pasos rápidos (local):

1. Arranca la API:
   - `dotnet run --project CompuTrabajo.Redarbor.Api`  (o `docker-compose up -d --build` si usas Docker)

2. Solicita el token (credenciales de demo):

   - Usuario/Password de ejemplo: `admin` / `1234` (solo demo)

   - Ejemplo con `curl`:

     ```bash
     curl -s -X POST http://localhost:8080/api/auth/login \
       -H "Content-Type: application/json" \
       -d '{"username":"admin","password":"1234"}'
     ```

     Respuesta de ejemplo:

     ```json
     { "access_token": "eyJhbGciOiJI..." }
     ```

   - Para extraer el token en shell (requiere `jq`):

     ```bash
     TOKEN=$(curl -s -X POST http://localhost:8080/api/auth/login \
       -H "Content-Type: application/json" \
       -d '{"username":"admin","password":"1234"}' | jq -r .access_token)
     echo $TOKEN
     ```

3. Usar el token en llamadas autenticadas:

   - Ejemplo con `curl` a la lista de empleados:

     ```bash
     curl -H "Authorization: Bearer $TOKEN" http://localhost:8080/api/Redarbor
     ```

   - En Swagger UI: presiona "Authorize" e introduce `Bearer {tu_token}` (incluye la palabra `Bearer`).

Notas:
 - La generación del token usa la sección `Jwt` de `appsettings.json` (clave `Key`, `Issuer`, `Audience`, `ExpiresMinutes`). En entornos Docker estos valores pueden venir de variables de entorno.
 - El endpoint de login es únicamente para desarrollo/demo (usuarios hardcode). En producción debes integrar con un proveedor de identidad seguro (Identity Server, Azure AD, etc.).
 - Se recomienda usar el entorno Docker con el orquestador docker compose  `docker-compose up  --build` ya que levanta ambiente completo con imagen de sql server

