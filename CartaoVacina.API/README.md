# CartaoVacina API

## Autenticação JWT

Esta API utiliza autenticação JWT (JSON Web Token) para proteger as rotas. Apenas as rotas de login e registro não requerem autenticação.

### Rotas Públicas (sem autenticação)
- `POST /api/auth/login` - Login do usuário
- `POST /api/auth/register` - Registro de novo usuário

### Rotas Protegidas (com autenticação)
Todas as outras rotas requerem um token JWT válido no header `Authorization`.

### Como usar a autenticação

1. **Fazer login ou registro** para obter um token JWT:
   ```http
   POST /api/auth/login
   Content-Type: application/json
   
   {
     "email": "usuario@exemplo.com",
     "password": "senha123"
   }
   ```

2. **Usar o token** nas requisições subsequentes:
   ```http
   GET /api/user/1
   Authorization: Bearer {seu_token_jwt_aqui}
   ```

### Exemplo de resposta de login
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64_encoded_refresh_token",
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

### Testando no Swagger
1. Acesse o Swagger UI em `/swagger`
2. Faça login usando o endpoint `/api/auth/login`
3. Copie o token da resposta
4. Clique no botão "Authorize" no Swagger
5. Digite `Bearer {seu_token}` no campo
6. Agora você pode testar todas as rotas protegidas

### Endpoints de Usuário
- `GET /api/auth/me` - Obter dados do usuário logado
- `GET /api/user/{id}` - Obter usuário por ID
- `GET /api/user` - Listar todos os usuários
- `POST /api/user` - Criar novo usuário
- `PATCH /api/user/{id}` - Atualizar usuário
- `DELETE /api/user/{id}` - Deletar usuário

### Endpoints de Vacinação
- `POST /api/user/{userId}/vaccinations` - Criar vacinação para usuário
- `PATCH /api/user/{userId}/vaccinations/{vaccinationId}` - Atualizar vacinação
- `DELETE /api/user/{userId}/vaccinations/{vaccinationId}` - Deletar vacinação

### Endpoints de Vacinas
- `GET /api/vaccine/{id}` - Obter vacina por ID
- `GET /api/vaccine` - Listar todas as vacinas
- `POST /api/vaccine` - Criar nova vacina
- `PATCH /api/vaccine/{id}` - Atualizar vacina
- `DELETE /api/vaccine/{id}` - Deletar vacina 