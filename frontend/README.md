# Front-end — Locadora de Veículos (TP2)

React + Vite + Bootstrap, consumindo a API ASP.NET Core do TP1.

## Como rodar
1. **Back-end:** rode `dotnet ef database update` e `dotnet run` (porta 5273).
2. **Front-end:** `cd frontend && npm install && npm run dev` → http://localhost:5173
3. Se a porta da API for outra: `cp .env.example .env` e ajuste `VITE_API_URL`.

## Telas
| Entidade | CRUD | Filtros (front) | Consultas (rotas do back) |
|---|---|---|---|
| Veículos | ✔ | modelo, placa, fabricante, status | `/veiculos/disponiveis`, `/veiculos/por-fabricante/{id}` |
| Fabricantes | ✔ | nome, país | — |
| Categorias | ✔ | nome, descrição | — |
| Clientes | ✔ | nome, CPF, e-mail | `/clientes/{id}/historico` |
| Aluguéis | abrir, editar (em aberto), devolver, excluir | cliente, placa, veículo | `/alugueis/em-aberto`, `/alugueis/por-periodo` |

Wireframes: https://www.figma.com/design/Pz6ScsLEmc0BYXTxuMihyh