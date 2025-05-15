/*
  Trace Player
  nvault_util https://forums.alliedmods.net/showthread.php?t=139584
  easy_http https://github.com/Next21Team/AmxxEasyHttp/releases/tag/1.4.0
*/

#include <amxmodx>
#include <nvault_util>
#include <easy_http>

#define PLUGIN "Trace Player API"
#define VERSION "1.3"
#define AUTHOR "yarmak"

new vault_existing;
new vault_new;
new vault_util_new;

new const VAULT_EXISTING[] = "api_player_cache";
new const VAULT_NEW[] = "api_player_new";

new const ApiServer[] = "http://localhost:5000";
new const ApiKey[] = "api_key";

public plugin_init()
{
  register_plugin(PLUGIN, VERSION, AUTHOR);

  vault_existing = nvault_open(VAULT_EXISTING);
  vault_new = nvault_open(VAULT_NEW);
  vault_util_new = nvault_util_open(VAULT_NEW);

  if (vault_existing == INVALID_HANDLE || vault_new == INVALID_HANDLE || vault_util_new == INVALID_HANDLE)
  {
    set_fail_state("[ERROR] Failed to open one of the nvaults.");
  }
}

public client_putinserver(id)
{
  if (is_user_bot(id) || is_user_hltv(id)) return;

  new szSteamID[32]; get_user_authid(id, szSteamID, charsmax(szSteamID));
  new szIP[32]; get_user_ip(id, szIP, charsmax(szIP), 1);
  new szName[32]; get_user_name(id, szName, charsmax(szName));

  new szKey[64];
  formatex(szKey, charsmax(szKey), "%s", szSteamID);

  new szData[128];
  formatex(szData, charsmax(szData), "%s|%s|%s", szSteamID, szIP, szName);

  new szExistingData[128];
  if (nvault_get(vault_existing, szKey, szExistingData, charsmax(szExistingData)))
  {
    return;
  }

  log_amx("[TracePlayer] Данные игрока записаны в vault_new %s;", szSteamID);
  nvault_set(vault_new, szKey, szData);
}

public plugin_end()
{
  nvault_close(vault_new);
  new iCount = nvault_util_count(vault_util_new);

  if(iCount <= 0)
  {
    log_amx("[TracePlayer] Count vault_util_new is 0;");
    return;
  }

  new EzJSON:jsonData = ezjson_init_object();
  if (jsonData == EzInvalid_JSON)
  {
    log_amx("[TracePlayer] Failed to initialize jsonData.");
    ezjson_free(jsonData);
    return;
  }

  new EzJSON:jsonPlayers = ezjson_init_array();
  if (jsonPlayers == EzInvalid_JSON)
  {
    log_amx("[TracePlayer] Failed to initialize jsonPlayers.");
    ezjson_free(jsonPlayers);
    return;
  }

  new szServerName[64];
  get_cvar_string("hostname", szServerName, charsmax(szServerName));
  ezjson_object_set_string(jsonData, "server", szServerName);

  new iPos = 0, szKey[64], szData[128];

  for (new iCurrent = 1; iCurrent <= iCount; iCurrent++)
  {
    iPos = nvault_util_read(vault_util_new, iPos, szKey, charsmax(szKey), szData, charsmax(szData));
    if (iPos <= 0)
    {
      break;
    }

    new szSteamID[32], szIP[32], szName[32];
    if (parse(szData, szSteamID, charsmax(szSteamID), szIP, charsmax(szIP), szName, charsmax(szName)) < 3)
    {
      continue;
    }

    new EzJSON:jsonPlayer = ezjson_init_object();
    if (jsonPlayer == EzInvalid_JSON)
    {
      log_amx("[TracePlayer] Не удалось создать JSON объект для игрока: %s", szSteamID);
      continue;
    }

    ezjson_object_set_string(jsonPlayer, "steamId", szSteamID);
    ezjson_object_set_string(jsonPlayer, "ip", szIP);
    ezjson_object_set_string(jsonPlayer, "name", szName);

    ezjson_array_append_value(jsonPlayers, jsonPlayer);

    nvault_set(vault_existing, szKey, szData);
  }

  ezjson_object_set_value(jsonData, "players", jsonPlayers);

  new EzHttpOptions:options = ezhttp_create_options();
  ezhttp_option_set_body_from_json(options, jsonData);
  ezhttp_option_set_header(options, "Content-Type", "application/json");
  ezhttp_option_set_header(options, "X-API-Key", ApiKey);

  ezhttp_post(fmt("%s/api/player", ApiServer), "APIPlayersUpload", options);

  ezjson_free(jsonPlayers);
  ezjson_free(jsonData);

  nvault_close(vault_existing);
  nvault_util_close(vault_util_new);
}

public APIPlayersUpload(EzHttpRequest:request_id)
{
  new code = ezhttp_get_http_code(request_id);

  if (code != 200)
  {
    log_amx("[TracePlayer] Upload failed. Status: %d", code);
    return;
  }

  vault_new = nvault_open(VAULT_NEW);
  nvault_prune(vault_new, 0, get_systime());
  log_amx("[TracePlayer] Upload success. Status: %d", code);
}