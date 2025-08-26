# Venice Teste Backend

Este projeto implementa uma API de pedidos com integração entre múltiplos componentes: SQL Server (dados principais do pedido), MongoDB (itens do pedido), Redis (cache de GET por 120s) e RabbitMQ (publicação de eventos PedidoCriado). A autenticação é via JWT e é obrigatória em todos os endpoints.

## Arquitetura (ASCII)

```
          +----------------------+            +--------------------+
          |      Cliente         |  HTTPS     |  Venice Web API    |
          | (curl/Swagger/UI)    +----------->|  (ASP.NET Core)    |
          +----------+-----------+            +---------+----------+
                     |                                 |
                     |                                 |
                     |                                 v
                     |                        +--------+--------+
                     |                        |    Redis (TTL   |
                     |                        |     120s)       |
                     |                        +--------+--------+
                     |                                 |
                     |                                 v
                     |                       Cache miss: carrega
                     |                       SQL + Mongo e seta cache
                     |                                 |
                     v                                 v
      +--------------+--------------+         +--------+--------+
      |    SQL Server (Pedidos)     |         |   MongoDB (Itens) |
      |  EF Core (EnsureCreated)    |         |   Coleção order_items
      +--------------+--------------+         +------------------+
                     ^                                 
                     |                                 
                     |                                 
                     +-------------------------+       
                                               |       
                                               v       
                                         +-----+------+
                                         | RabbitMQ   |
                                         | (eventos)  |
                                         +------------+
```

- POST /pedidos: persiste pedido (SQL), persiste itens (Mongo) e publica PedidoCriado (RabbitMQ).
- GET /pedidos/{id}: integra dados de SQL + Mongo e usa cache Redis com TTL de 120s.

## Decisões e Trade-offs

- Sem transação distribuída: a persistência ocorre em etapas (SQL -> Mongo -> publish). Em falhas parciais, a consistência é eventual. Estratégias de mitigação: logs, idempotência nos reprocessamentos, e possível uso de padrão Outbox (não implementado nesta versão para manter simplicidade).
- Cache de leitura (GET by id) com TTL fixo (120s): reduz carga em SQL/Mongo. Pode servir dados levemente defasados; invalida automaticamente após o TTL.
- Inicialização do banco relacional: o projeto executa EnsureCreated() no startup quando não está usando InMemory, permitindo subir sem rodar migrações manuais.

## Como executar localmente com Docker

Pré-requisitos: Docker Desktop com suporte a Docker Compose.

1) Suba todos os serviços:

- Windows/macOS/Linux: na raiz do repositório
  - docker compose up -d

2) API disponível em: http://localhost:8080
- Swagger: http://localhost:8080

3) RabbitMQ UI: http://localhost:15672 (user: guest / pass: guest)

4) Encerrar: docker compose down

Observações:
- O compose sobe: API, SQL Server (1433), MongoDB (27017), Redis (6379) e RabbitMQ (5672/15672).
- A API utiliza EnsureCreated() para criar a base/tabelas automaticamente no SQL Server.

## Configuração (principais variáveis já mapeadas no docker-compose)

- SQL Server: ConnectionStrings__ApplicationConnection=Server=sqlserver;Database=syc_erp;User Id=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True
- MongoDB: ConnectionStrings__MongoConnection=mongodb://mongo:27017, Mongo__Database=venice_orders
- Redis: ConnectionStrings__Redis=redis:6379
- RabbitMQ: RabbitMq__Host=rabbitmq, RabbitMq__Username=guest, RabbitMq__Password=guest
- JWT: Jwt__Issuer=venice.local, Jwt__Audience=venice.api, Jwt__Secret=super_secret_dev_key_please_change

## Endpoints + exemplos curl (com JWT)

Base URL: http://localhost:8080/api/v1

Importante: todos os endpoints requerem Authorization: Bearer <token>.

Como obter um JWT para testes (HS256):
- Header: { "alg": "HS256", "typ": "JWT" }
- Payload (exemplo): { "sub": "tester", "name": "Local Dev", "iat": 1710000000, "iss": "venice.local", "aud": "venice.api", "exp": 4700000000 }
- Secret: super_secret_dev_key_please_change
- Gere o token com HS256 (ex.: jwt.io) usando os campos acima. Alternativa: use qualquer ferramenta de geração de JWT HS256 apontando os mesmos issuer/audience/secret.

1) Criar pedido
- POST /pedidos/customer/{customerId}

Exemplo:
- set CUSTOMERID=11111111-1111-1111-1111-111111111111
- curl -i -X POST "http://localhost:8080/api/v1/pedidos/customer/%CUSTOMERID%" ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer <SEU_JWT_AQUI>" ^
  --data "{\n  \"itens\": [\n    { \"produtoId\": \"22222222-2222-2222-2222-222222222222\", \"quantidade\": 2, \"precoUnitario\": 10.5 }\n  ]\n}"

Resposta esperada:
- HTTP 201 Created
- Location: /api/v1/pedidos/{orderId}

2) Buscar pedido por id (usa Redis TTL 120s)
- GET /pedidos/{orderId}

Exemplo:
- curl -i "http://localhost:8080/api/v1/pedidos/{orderId}" -H "Authorization: Bearer <SEU_JWT_AQUI>"

3) (Opcional) Listar itens do pedido
- GET /pedidos/{orderId}/items

Exemplo:
- curl -i "http://localhost:8080/api/v1/pedidos/{orderId}/items" -H "Authorization: Bearer <SEU_JWT_AQUI>"

## Como ver a fila no RabbitMQ (UI 15672)

- URL: http://localhost:15672
- Usuário/Senha: guest / guest
- Após criar um pedido, verifique os Exchanges/Queues no painel do RabbitMQ Management. O evento PedidoCriado é publicado via MassTransit.

## Critérios de Aceite

- ✅ POST /pedidos retorna 201 + Location e publica PedidoCriado.
- ✅ Dados principais no SQL e itens no Mongo.
- ✅ GET /pedidos/{id} integra SQL+Mongo e usa Redis (TTL 120s).
- ✅ JWT obrigatório em todos os endpoints.
- ✅ 2+ testes unitários.
- ✅ docker compose up sobe tudo e a API funciona.

## Caminhos de código relevantes

- API e endpoints: <mcfile name="OrderController.cs" path="src/Venice.Teste.Backend.WebApi/Controllers/OrderController.cs"></mcfile>
- JWT e pipeline HTTP: <mcfile name="Program.cs" path="src/Venice.Teste.Backend.WebApi/Program.cs"></mcfile>
- DI de Infra (EF Core, Mongo, Redis, MassTransit): <mcfile name="ServiceCollectionExtensions.cs" path="src/Venice.Teste.Backend.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs"></mcfile>
- Cache 120s no GET by id: <mcfile name="CommandHandler.cs" path="src/Venice.Teste.Backend.Application/UseCases/Order/GetById/CommandHandler.cs"></mcfile>
- Publicação PedidoCriado: <mcfile name="MassTransitPedidoCriadoPublisher.cs" path="src/Venice.Teste.Backend.Infrastructure/Messaging/MassTransitPedidoCriadoPublisher.cs"></mcfile>
- Compose com todos os serviços: <mcfile name="docker-compose.yml" path="docker-compose.yml"></mcfile>

## Testes

- Existem projetos de teste em tests/ com cenários cobrindo validações, cálculo de total, cache e publicação de evento.
- Executar localmente: dotnet test