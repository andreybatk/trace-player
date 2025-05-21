#!/bin/bash

set -e  # Остановить выполнение при ошибке

# Название контейнера и БД
CONTAINER_NAME="traceplayer-postgres"
DB_NAME="traceplayer"
DB_USER="postgres"

# Копируем файлы в контейнер
docker cp ./IP2LOCATION-LITE-DB1.CSV $CONTAINER_NAME:/tmp/IP2LOCATION-LITE-DB1.CSV
docker cp ./init_tran_hnsblock_for_pg.sql $CONTAINER_NAME:/init_tran_hnsblock_for_pg.sql
docker cp ./init_ip_countries_for_pg.sql $CONTAINER_NAME:/init_ip_countries_for_pg.sql
docker cp ./insert_player_info_pg.sql $CONTAINER_NAME:/insert_player_info_pg.sql
docker cp ./update_player_ips_pg.sql $CONTAINER_NAME:/update_player_ips_pg.sql

# Выполняем SQL-файлы
docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME -f /init_tran_hnsblock_for_pg.sql
docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME -f /init_ip_countries_for_pg.sql
docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME -f /insert_player_info_pg.sql
docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME -f /update_player_ips_pg.sql

echo "✅ Инициализация завершена."
