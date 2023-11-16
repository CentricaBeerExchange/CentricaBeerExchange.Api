@ECHO OFF

docker compose pull
docker compose --project-name "beer-db" up --force-recreate -d
docker image prune -f