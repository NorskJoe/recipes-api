# Recipes API

ASP.NET Core solution for managing recipes. Four projects: `Presentation`
(web API), `Application` (MediatR handlers + interfaces), `Domain`, and
`Infrastructure` (SQL Server + Dapper persistence).

## Prerequisites

- .NET 9 SDK
- SQL Server (e.g. in Docker) reachable at `localhost:1433`
- A database named `RecipesDB` that already exists

Example SQL Server on Docker:

```sh
docker run -d --name sqlserver -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=..." -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

## Database password (MSSQL_SA_PASSWORD)

The app connects with the SQL Server `sa` login. The password is read from the
`MSSQL_SA_PASSWORD` environment variable - it is **not** stored in this repo.

The connection string is built by `run.sh` as:

```
Server=localhost,1433;Database=RecipesDB;UserId=sa;Password=$MSSQL_SA_PASSWORD;Encrypt=True;TrustServerCertificate=True;
```

### Setting it up

1. Copy `home/secrets.template.env` from the dotfiles repo to `~/.secrets.env`:
   ```sh
   cp ~/dotfiles/home/secrets.template.env ~/.secrets.env
   ```
2. Edit `~/.secrets.env` and set your actual SA password (keep the single
   quotes; they prevent zsh from interpreting `!`).
3. Load it. If your shell sources `~/.secrets.env` (configured in
   `dotfiles/home/shell.nix`), restart your shell or run a rebuild. Otherwise
   source it manually:
   ```sh
   source ~/.secrets.env
   ```

Alternatively, for a single session:

```sh
export MSSQL_SA_PASSWORD='your-password-here'
```

## Running

```sh
./run.sh
```

`run.sh` fails fast with a clear message if `MSSQL_SA_PASSWORD` is not set.
On startup the API connects to `RecipesDB` and applies any pending SQL
migrations in `Infrastructure/Migrations/`.