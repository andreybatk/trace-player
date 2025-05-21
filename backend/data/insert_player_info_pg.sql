CREATE OR REPLACE FUNCTION int_to_ip(ip BIGINT)
RETURNS TEXT AS $$
BEGIN
  RETURN 
    (ip >> 24 & 255)::TEXT || '.' ||
    (ip >> 16 & 255)::TEXT || '.' ||
    (ip >> 8 & 255)::TEXT || '.' ||
    (ip & 255)::TEXT;
END;
$$ LANGUAGE plpgsql IMMUTABLE;

INSERT INTO "Players" ("SteamId", "SteamId64")
SELECT DISTINCT worldid, NULL
FROM ps_plr_ids_worldid;

INSERT INTO "PlayerNames" ("Name", "AddedAt", "Server", "PlayerId")
SELECT 
    n.name,
    n.lastseen AT TIME ZONE 'UTC',
    'Frallion Servers',
    p."Id"
FROM ps_plr_ids_name n
JOIN ps_plr_ids_worldid w ON n.plrid = w.plrid
JOIN "Players" p ON p."SteamId" = w.worldid
WHERE (w.worldid LIKE 'STEAM_0:%' OR w.worldid LIKE 'STEAM_1:%')
ORDER BY n.plrid, n.lastseen;

INSERT INTO "PlayerIps" ("Ip", "CountryCode", "AddedAt", "PlayerId")
SELECT 
    int_to_ip(i.ipaddr) AS ip,
    NULL,
    i.firstseen AT TIME ZONE 'UTC',
    p."Id"
FROM ps_plr_ids_ipaddr i
JOIN ps_plr_ids_worldid w ON i.plrid = w.plrid
JOIN "Players" p ON p."SteamId" = w.worldid;

TRUNCATE TABLE ps_plr_ids_name RESTART IDENTITY;
DROP TABLE IF EXISTS ps_plr_ids_name;

TRUNCATE TABLE ps_plr_ids_worldid RESTART IDENTITY;
DROP TABLE IF EXISTS ps_plr_ids_worldid;

TRUNCATE TABLE ps_plr_ids_ipaddr RESTART IDENTITY;
DROP TABLE IF EXISTS ps_plr_ids_ipaddr;