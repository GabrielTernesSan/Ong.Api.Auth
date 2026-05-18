# 🔐 Ong.Api.Auth — Serviço de Autenticação

Microsserviço responsável pelo **cadastro de usuários** e pela **geração de tokens JWT**.  
Faz parte da plataforma **Conexão Solidária** desenvolvida para a ONG Esperança Solidária.

---

## 📐 Responsabilidades

| Recurso | Método | Acesso | Descrição |
|---|---|---|---|
| `/auth/register` | POST | Público | Cadastra novo usuário (Doador ou GestorONG) |
| `/auth/login` | POST | Público | Autentica e retorna JWT |
| `/auth/outbox` | GET | API Key | Lista mensagens Outbox pendentes (uso do Worker) |
| `/auth/outbox/{id}/processed` | PATCH | API Key | Marca mensagem como processada |
| `/auth/outbox/{id}/error` | PATCH | API Key | Registra erro no processamento |
| `/health` | GET | Público | Health check |
| `/metrics` | GET | Público | Métricas Prometheus (OpenTelemetry) |

---

## 🏗️ Arquitetura Interna

```
Ong.Api.Auth          → Camada de entrada (Minimal API, Swagger, JWT)
Ong.Application       → Handlers MediatR (LoginHandler, RegisterHandler)
Ong.Domain            → Entidades, contratos (User, IUserRepository, ITokenService)
Ong.Infra             → EF Core + PostgreSQL, TokenService (JWT), Outbox
Ong.Commom            → DTOs compartilhados (UserCreated, Response<T>)
```

### Padrão Outbox
Ao registrar um novo usuário, o serviço persiste um `OutboxMessage` no banco de dados (padrão Transactional Outbox). O **Worker** consulta esta tabela periodicamente e publica o evento `UserCreated` no RabbitMQ, garantindo consistência eventual sem perda de mensagens.

---

## ⚙️ Pré-requisitos

| Ferramenta | Versão mínima |
|---|---|
| .NET SDK | 10.0 (preview) |
| Docker + Docker Compose | 24+ |
| PostgreSQL | 15+ |

---

## 🚀 Rodando localmente

### 1. Clone o repositório

```bash
git clone https://github.com/<seu-org>/Ong.Api.Auth.git
cd Ong.Api.Auth
```

### 2. Configure as variáveis de ambiente

Crie o arquivo `src/Ong.Api.Auth/appsettings.Development.json` (ou use variáveis de ambiente):

### 3. Suba o banco via Docker

```bash
docker run -d \
  --name postgres-auth \
  -e POSTGRES_DB=ong_auth \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:15
```

### 4. Aplique as migrations

```bash
cd src/Ong.Api.Auth
dotnet ef database update --project ../Ong.Infra/Ong.Infra.csproj
```

### 5. Execute a API

```bash
dotnet run --project src/Ong.Api.Auth/Ong.Api.Auth.csproj
```

A API estará disponível em:
- **Swagger UI:** http://localhost:5000/swagger
- **Health:** http://localhost:5000/health
- **Métricas:** http://localhost:5000/metrics

---

## 🐳 Rodando via Docker

```bash
# Build da imagem
docker build -f src/Ong.Api.Auth/Dockerfile -t ong-api-auth:local .

# Executar
docker run -d \
  --name ong-api-auth \
  -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=ong_auth;Username=postgres;Password=postgres" \
  -e Jwt__Key="sua-chave-secreta-minimo-32-caracteres!!" \
  -e Jwt__Issuer="ong-auth" \
  -e Jwt__Audience="ong-platform" \
  -e ApiKeys__WorkerKey="chave-interna-worker-1234" \
  ong-api-auth:local
```

---

## ☸️ Kubernetes (Minikube / Kind)

Os manifests estão na pasta `k8s/` do repositório raiz de infraestrutura.

```bash
kubectl apply -f k8s/auth/
kubectl get pods -n conexao-solidaria
```

## 🔄 Pipeline CI/CD

| Pipeline | Gatilho | O que faz |
|---|---|---|
| `ci.yml` | Push/PR em `main` | Restore → Build → Testes → Publica resultados |
| `cd.yml` | CI com sucesso | Login GHCR → Docker Build → Push da imagem |

A imagem é publicada em: `ghcr.io/<org>/ong-api-auth:latest` e `ghcr.io/<org>/ong-api-auth:sha-<commit>`.

---

## 📊 Observabilidade

O serviço expõe métricas via **OpenTelemetry + Prometheus Exporter** no endpoint `/metrics`.

Métricas disponíveis:
- `http_server_request_duration_seconds` — latência das requisições HTTP
- `http_server_active_requests` — requisições ativas
- `process_cpu_time_seconds_total` — uso de CPU
- `dotnet_gc_*` — métricas de GC do .NET

Configure o Prometheus para fazer scrape em `http://ong-api-auth:8080/metrics`.  
Veja o dashboard Grafana em `observability/grafana/dashboards/`.

---

## 🔑 Autenticação com API Key (endpoints internos)

Os endpoints de Outbox são protegidos por API Key via header `x-api-key`.  
O Worker deve enviar a chave configurada em `ApiKeys:WorkerKey`.