CREATE OR REPLACE FUNCTION ip_to_bigint(ip TEXT)
RETURNS BIGINT AS $$
DECLARE
  parts TEXT[];
BEGIN
  parts := string_to_array(ip, '.');
  RETURN (
    (parts[1])::BIGINT << 24
  ) + (
    (parts[2])::BIGINT << 16
  ) + (
    (parts[3])::BIGINT << 8
  ) + (
    (parts[4])::BIGINT
  );
END;
$$ LANGUAGE plpgsql IMMUTABLE;

UPDATE "PlayerIps" pi
SET "CountryCode" = ic.country_code
FROM ip_country ic
WHERE ip_to_bigint(pi."Ip") BETWEEN ic.ip_from AND ic.ip_to;