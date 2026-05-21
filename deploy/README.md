# Deployment Notes

## Standard Convention

This deployment layout is intended to work the same way on every VPS.

Keep these names stable across environments:
- the application service is always `app`
- the database service is always `postgres`
- the backup service is always `postgres-backup`
- the default database name is `app_db`
- the default database user is `app_user`

Only project-specific values should change in `.env`:
- credentials and application secrets remain environment-specific

The public hostname is configured directly in [deploy/docker-compose.yml](c:\repos\autodor\deploy\docker-compose.yml) in the `caddy` label on the `app` service. Replace `app.example.com` with the real domain before deployment.

Container names visible in `docker ps` follow the same convention:
- `app`
- `postgres`
- `postgres-backup`
- `caddy`
- `watchtower`

Docker network names are also fixed:
- `internal`
- `edge`

The commands below are intended to be run from the main project folder after logging into the VPS.

## Backups

The `postgres-backup` service creates logical PostgreSQL backups with `pg_dump` and stores them in the `./backups` directory next to `docker-compose.yml`, mounted as `/backups` inside the container.

On the VPS, backup files are visible directly on the host in `./backups`.

Current behavior:
- a backup runs automatically once per day via `SCHEDULE: "@daily"`
- a backup also runs when the `postgres-backup` container starts because `BACKUP_ON_START` is enabled
- retention keeps 7 daily backups, 4 weekly backups, and 6 monthly backups
- backups are compressed as `.sql.gz`

Backup folders inside the container:
- `/backups/last` contains timestamped backups from every run
- `/backups/daily` contains the latest backup for each day
- `/backups/weekly` contains the latest backup for each ISO week
- `/backups/monthly` contains the latest backup for each month

Useful commands:

Create a manual backup:
```bash
docker compose exec postgres-backup /backup.sh
```

List available backup files:
```bash
docker compose exec postgres-backup sh -lc 'find /backups -type f | sort'
```

List backup files directly on the host:
```bash
find ./backups -maxdepth 2 \( -type f -o -type l \) | sort
```

Check backup container logs:
```bash
docker compose logs postgres-backup
```

## Restore

Before restoring, stop the application to avoid writes during recovery:

```bash
docker compose stop app
```

Command flags used below:
- `-T` disables pseudo-TTY allocation so piping backup data into `psql` works cleanly

How backup file selection works:
- the template uses database `app_db` and user `app_user`
- `/backups/last/app_db-latest.sql.gz` restores the newest backup pointed to by the `latest` symlink
- `/backups/daily/app_db-YYYYMMDD.sql.gz` restores a specific daily backup file for the configured database

Restore the latest backup for the configured database:
```bash
docker compose exec -T postgres psql -U app_user -d postgres -c "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'app_db' AND pid <> pg_backend_pid();"
docker compose exec -T postgres psql -U app_user -d postgres -c 'DROP DATABASE IF EXISTS "app_db";' -c 'CREATE DATABASE "app_db";'
docker compose exec -T postgres-backup zcat /backups/last/app_db-latest.sql.gz | docker compose exec -T postgres psql -U app_user -d app_db
docker compose start app
```

Restore a specific backup file:
```bash
docker compose exec -T postgres-backup zcat /backups/daily/app_db-YYYYMMDD.sql.gz | docker compose exec -T postgres psql -U app_user -d app_db
```

## Troubleshooting

If backups are not being created:
- verify the backup service is running with `docker compose ps`
- inspect logs with `docker compose logs postgres-backup`
- confirm PostgreSQL is healthy with `docker compose ps postgres`
- verify the database credentials in `.env`

If the latest backup file is missing:
- list `./backups/last`, `./backups/daily`, `./backups/weekly`, and `./backups/monthly` on the host or `/backups/last`, `/backups/daily`, `/backups/weekly`, and `/backups/monthly` inside the container
- trigger a manual backup with `docker compose exec postgres-backup /backup.sh`
- re-check logs for authentication or connectivity failures

If restore fails:
- make sure `app` is stopped before restore
- make sure the target database was dropped and recreated cleanly
- verify the selected backup file exists and can be decompressed with `zcat`
- review PostgreSQL logs with `docker compose logs postgres`

If the backup storage must be inspected from inside the container:
```bash
docker compose exec postgres-backup sh -lc 'df -h /backups && find /backups -maxdepth 2 -type f | sort'
```

The backup service stores logical dumps, not physical PostgreSQL data files. That makes backups portable and easy to upload to external storage, but restore time depends on dump size.
