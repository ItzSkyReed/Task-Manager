# Task Manager
Реализовано в рамках тестового задания с разделением на Backend, Frontend и БД.


## Стек технологий

* **Backend:** C# 14, .NET 10, ASP.NET Core Minimal APIs, MediatR, FluentValidation, Entity Framework Core, JWT + Refresh Tokens (HttpOnly Cookie), Scalar (вместо Swagger).
* **Frontend:** Angular, Angular Material, Nginx (раздача и reverse proxy).
* **Database:** PostgreSQL 18.
* **Инфраструктура:** Docker, Docker Compose.

## Запуск

Для запуска требуется Docker.

1. **Клонируйте репозиторий и перейдите в его корневую папку:**
   ```bash
   git clone <URL_ВАШЕГО_РЕПОЗИТОРИЯ>
   cd TaskManager
   ```

2. **Настройте переменные окружения:**
   ```bash
   cp .env.example .env
   ```
   При необходимости измените `DB_PASSWORD` и `JWT_SECRET_KEY`. Если пропустить этот шаг, Docker использует значения по умолчанию из `compose.yaml`.

3. **Запустите приложение:**
   ```bash
   docker compose up -d --build
   ```

4. **Откройте приложение в браузере:**
   * **Веб-интерфейс:** [http://localhost:4200](http://localhost:4200)
   * **Документация API:** [http://localhost:8080/scalar/v1](http://localhost:8080/scalar/v1)

## Структура проекта

- `backend/` - серверная часть на .NET 10 с разделением на слои (`Domain`, `Application`, `Infrastructure`, `API`).
- `frontend/` - клиент на Angular. Внутри `src/app/` код разбит на `core` (модели, сервисы, интерсепторы), `layout` (шапка и вкладки) и `features` (авторизация, проекты, задачи, пользователи). Для сборки и проксирования `/api/` используется Nginx.
- `compose.yaml` - запуск PostgreSQL, API и фронтенда в Docker.
