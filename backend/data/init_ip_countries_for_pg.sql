CREATE INDEX IF NOT EXISTS idx_ip_country_range ON ip_country (ip_from, ip_to);
COPY ip_country (ip_from, ip_to, country_code, country_name)
FROM '/tmp/IP2LOCATION-LITE-DB1.CSV'
DELIMITER ',' CSV HEADER;