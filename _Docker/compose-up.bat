@ECHO OFF

docker compose pull
docker compose --project-name "beer-db" up -d
docker image prune -f