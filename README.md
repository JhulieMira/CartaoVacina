# Cartão de Vacina - Documentação da API

## Visão Geral
Esta aplicação é uma API ASP.NET Core 9 que gerencia usuários, vacinas e registros de vacinação, utilizando banco de dados SQLite. A arquitetura é dividida em camadas, separando responsabilidades de API, domínio, infraestrutura e contratos.

Principais tecnologias e ferramentas:
- **ASP.NET Core 9**: Framework para construção da API REST.
- **Entity Framework Core 9**: ORM para acesso ao banco de dados SQLite.
- **JWT (JSON Web Token)**: Autenticação e autorização.
- **FluentValidation**: Validação de dados de entrada.
- **MediatR**: Implementação do padrão Mediator para desacoplamento entre camadas.
- **UnitOfWork**: Gerenciamento de transações e repositórios.

---

## Endpoints e Funcionalidades

### 1. Autenticação (`/api/auth`)
- **POST /login**
  - Autentica um usuário com email e senha.
  - Retorna um token JWT e refresh token.
  - Regras:
    - Email e senha obrigatórios e validados.
    - Conta deve estar ativa.
    - Senha é verificada via hash seguro.

- **POST /register**
  - Cria uma nova conta de usuário.
  - Retorna token JWT e refresh token.
  - Regras:
    - Email único (não pode já estar cadastrado).
    - Senha forte e confirmação obrigatória.
    - Dados validados pelo FluentValidation.

### 2. Usuários (`/api/user`)
- **GET /{userId}**
  - Busca usuário por ID, incluindo vacinações.
  - Retorna 404 se não encontrado.

- **GET /**
  - Lista todos os usuários.

- **POST /**
  - Cria um novo usuário.
  - Regras:
    - Nome, data de nascimento e gênero obrigatórios.
    - Gênero deve ser válido.
    - Data de nascimento não pode ser futura.

- **PATCH /{userId}**
  - Atualiza dados do usuário.
  - Regras:
    - Pelo menos um campo (nome ou data de nascimento) deve ser informado.
    - Validações similares à criação.

- **DELETE /{userId}**
  - Remove usuário.
  - Retorna 404 se não encontrado.

#### Vacinações do Usuário
- **POST /{userId}/vaccinations**
  - Registra uma vacinação para o usuário.
  - Regras de negócio:
    - O usuário não pode tomar a mesma dose da vacina duas vezes.
    - O usuário não pode tomar uma vacina fora da sua faixa etária (idade mínima/máxima da vacina).
    - Não pode tomar mais doses do que o total definido para a vacina.
    - Data da vacinação deve ser válida.

- **PATCH /{userId}/vaccinations/{vaccinationId}**
  - Atualiza a data de uma vacinação.
  - Regras:
    - Apenas a data pode ser alterada.
    - Data deve ser válida.

- **DELETE /{userId}/vaccinations/{vaccinationId}**
  - Remove um registro de vacinação.
  - Regras:
    - Usuário e vacinação devem existir.

### 3. Vacinas (`/api/vaccine`)
- **GET /{vaccineId}**
  - Busca vacina por ID.
  - Retorna 404 se não encontrada.

- **GET /**
  - Lista todas as vacinas.

- **POST /**
  - Cria uma nova vacina.
  - Regras:
    - Nome, código e número de doses obrigatórios.
    - Código deve ser único.
    - Faixa etária mínima/máxima pode ser definida.

- **PATCH /{vaccineId}**
  - Atualiza dados da vacina.
  - Regras:
    - Pelo menos um campo deve ser informado.
    - Código deve continuar único.

- **DELETE /{vaccineId}**
  - Remove uma vacina.
  - Retorna 404 se não encontrada.

---

## Regras de Negócio Importantes
- **Vacinação**:
  - Um usuário não pode tomar a mesma dose da mesma vacina mais de uma vez.
  - Um usuário não pode tomar uma vacina fora da sua faixa etária (idade mínima/máxima definida na vacina).
  - Não pode tomar mais doses do que o total definido para a vacina.
- **Usuário**:
  - Nome, data de nascimento e gênero obrigatórios.
  - Gênero deve ser um valor válido.
- **Vacina**:
  - Código único.
  - Nome e doses obrigatórios.

---

## Ferramentas e Padrões de Projeto

### JWT (JSON Web Token)
- Utilizado para autenticação e autorização dos endpoints protegidos.
- O token é gerado no login/registro e deve ser enviado no header Authorization (Bearer).
- Implementação customizada em `JwtService`.

### Entity Framework Core
- ORM para acesso ao banco SQLite.
- Mapeamento das entidades: Usuário, Vacina, Vacinação, Conta.
- Migrations para versionamento do banco.

### FluentValidation
- Validação de todos os DTOs de entrada (criação/atualização de usuário, vacina, vacinação, login, registro).
- Regras de validação centralizadas em classes específicas.

### UnitOfWork
- Interface e implementação para garantir atomicidade das operações de banco.
- Permite agrupar múltiplas operações em uma única transação.
- Facilita testes e manutenção.

### MediatR (Mediator Pattern)
- Toda a lógica de negócio (commands/queries) é desacoplada dos controllers usando o MediatR.
- Controllers apenas recebem requisições e delegam para handlers via `mediator.Send()`.
- Handlers implementam a lógica de cada operação (criação, atualização, deleção, listagem, etc).
- Facilita testes, manutenção e extensibilidade.

---

## Estrutura de Pastas (Resumo)
- **CartaoVacina.API**: Controllers e configuração da API.
- **CartaoVacina.Core**: Handlers, serviços, validações e regras de negócio.
- **CartaoVacina.Infrastructure**: Implementação de repositórios, UnitOfWork e acesso ao banco.
- **CartaoVacina.Contracts**: DTOs, entidades e interfaces compartilhadas.
- **CartaoVacina.Migrations**: Migrations e contexto do banco de dados.

---

## Observações Finais
- A API segue boas práticas de arquitetura limpa, separando responsabilidades e facilitando testes.
- O uso de MediatR e UnitOfWork garante baixo acoplamento e alta coesão.
- Todas as validações são centralizadas e automáticas via FluentValidation.
- O acesso ao banco é seguro e transacional via Entity Framework e UnitOfWork.
- JWT garante segurança e autenticação robusta.

---

Para dúvidas ou contribuições, consulte o código-fonte ou entre em contato com o responsável pelo projeto. 
