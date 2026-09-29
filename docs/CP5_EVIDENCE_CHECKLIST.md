# Checklist de evidências do CP5 (a preencher)

O código do CP5 (versionamento, paginação e rate limit) já está implementado. Estes itens
dependem da API rodando de fato (Claude não tem acesso para executar `dotnet run` nem fazer
requisições HTTP reais neste ambiente) — preencha marcando `[x]` e anexando o arquivo/print
correspondente nesta pasta (`docs/`).

## A) Versionamento

- [ ] `docs/v1-listagem.json` — resposta de `GET /api/game?api-version=1.0` (lista simples, sem paginação).
- [ ] `docs/v2-listagem.json` — resposta de `GET /api/game?api-version=2.0` (envelope paginado).
- [ ] `docs/headers-versionamento.png` (ou `.txt`) — print/trecho dos headers de resposta mostrando
      `api-supported-versions: 1.0, 2.0` e `api-deprecated-versions: 1.0`.
- [ ] `docs/swagger-v1-v2.png` — print do Swagger UI (`http://localhost:<porta>/`) mostrando o
      seletor com os dois grupos (v1 marcado como obsoleto na descrição, v2).

## B) Paginação

- [ ] `docs/pagina-invalida-400.png` (ou `.json`) — resposta `400` de, por exemplo,
      `GET /api/v2/game?page=0` ou `?pageSize=500`.
- [ ] `docs/pagina-1-vs-2.png` (ou `.json`) — `GET /api/v2/game?page=1&pageSize=5` e
      `GET /api/v2/game?page=2&pageSize=5` lado a lado, mostrando `items` diferentes e
      `totalItems`/`totalPages` coerentes entre as duas chamadas.
- [ ] (opcional) `GET /api/v2/game?page=999&pageSize=20` com poucos registros no banco —
      confirmar `200 OK` com `items: []`, nunca `404`.

## C) Rate limiting

- [ ] `docs/rate-limit-429.png` (ou `.json`) — 11ª chamada em menos de 1 minuto a
      `POST /api/game` retornando `429` com o header `Retry-After`.
- [ ] `docs/health-apos-429.png` — `GET /health` chamado imediatamente após o `429` acima,
      confirmando `200` (prova de que o rate limit não afetou o health check).

## D) Testes

- [ ] `docs/dotnet-test.txt` (ou print) — saída de `dotnet test GameStore.sln` com todos os
      testes (CP4 + CP5) passando, incluindo os novos `GetPaged_...` em `GameServiceTests`.

## Como gerar cada evidência (roteiro sugerido)

```bash
# 1) Subir a API (a partir da pasta GameStore, onde está o .sln)
dotnet restore GameStore.sln
dotnet build GameStore.sln
dotnet ef database update --project GameStore.Infrastructure --startup-project GameStore.Api
dotnet run --project GameStore.Api
```

Em outro terminal (troque `<porta>` pela porta real, em `GameStore.Api/Properties/launchSettings.json`,
e `<studioId>` por um Guid de Studio já existente no banco):

```bash
# v1 vs v2
curl -s "http://localhost:<porta>/api/game?api-version=1.0" | tee docs/v1-listagem.json
curl -s "http://localhost:<porta>/api/game?api-version=2.0" | tee docs/v2-listagem.json

# headers de versionamento
curl -sD - -o /dev/null "http://localhost:<porta>/api/game?api-version=1.0"

# paginação inválida -> 400
curl -s -w "\n%{http_code}\n" "http://localhost:<porta>/api/v2/game?pageSize=0"

# página 1 vs página 2
curl -s "http://localhost:<porta>/api/v2/game?page=1&pageSize=5" | tee docs/pagina1.json
curl -s "http://localhost:<porta>/api/v2/game?page=2&pageSize=5" | tee docs/pagina2.json

# rate limit -> dispare 11 POSTs em menos de 1 minuto
for i in $(seq 1 11); do
  curl -s -o /dev/null -w "tentativa $i -> %{http_code}\n" -X POST "http://localhost:<porta>/api/game" \
    -H "Content-Type: application/json" \
    -d "{\"name\":\"Teste $i\",\"description\":\"Descricao valida de teste\",\"launchDate\":\"2020-01-01\",\"studioId\":\"<studioId>\",\"contentTypeEnum\":0}"
done

# health imediatamente depois
curl -s -w "\n%{http_code}\n" "http://localhost:<porta>/health"

# testes
dotnet test GameStore.sln | tee docs/dotnet-test.txt
```

No Windows (PowerShell), use `curl.exe` (não o alias `Invoke-WebRequest`) para os exemplos acima
funcionarem como estão.
