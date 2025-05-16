/*
  Trace Player API
  nvault_util https://forums.alliedmods.net/showthread.php?t=139584
  easy_http https://github.com/Next21Team/AmxxEasyHttp/releases/tag/1.4.0
*/

#include <amxmodx>
#include <nvault_util>
#include <easy_http>

#define PLUGIN "Trace Player API"
#define VERSION "1.4"
#define AUTHOR "yarmak"

new vault_existing;
new vault_new;
new vault_util_new;

new prune_time;

new const VAULT_EXISTING[] = "api_player_cache";
new const VAULT_NEW[] = "api_player_new";

new const ApiServer[] = "http://localhost:5000";
new const ApiKey[] = "ApiKey";

new CvarIsDebug;

public plugin_init()
{
  register_plugin(PLUGIN, VERSION, AUTHOR);

  CvarIsDebug = register_cvar("traceplayer_debug", "0", FCVAR_ARCHIVE|FCVAR_SERVER)

  vault_existing = nvault_open(VAULT_EXISTING);
  vault_new = nvault_open(VAULT_NEW);
  vault_util_new = nvault_util_open(VAULT_NEW);

  if (vault_existing == INVALID_HANDLE || vault_new == INVALID_HANDLE || vault_util_new == INVALID_HANDLE)
  {
    set_fail_state("[ERROR] Failed to open one of the nvaults.");
  }

  prune_time = get_systime();

  if (get_pcvar_num(CvarIsDebug) == 1)
    log_amx("[TracePlayer DEBUG] DEBUG mode is ON");

  UploadPlayers();
}

public client_putinserver(id)
{
  if (is_user_bot(id) || is_user_hltv(id)) return;

  new steamId[32]; get_user_authid(id, steamId, charsmax(steamId));
  new ip[32]; get_user_ip(id, ip, charsmax(ip), 1);
  new name[32]; get_user_name(id, name, charsmax(name));

  new key[128];
  formatex(key, charsmax(key), "^"%s^" ^"%s^"", ip, name);

  new szData[32];
  formatex(szData, charsmax(szData), "%s", steamId);

  new szExistingData[128];
  if (nvault_get(vault_existing, key, szExistingData, charsmax(szExistingData)))
  {
    return;
  }

  if (get_pcvar_num(CvarIsDebug) == 1)
    log_amx("[TracePlayer] Данные игрока записаны в vault_new %s", steamId);

  nvault_set(vault_new, key, szData);
}

public UploadPlayers()
{
  new iCount = nvault_util_count(vault_util_new);

  if(iCount <= 0)
  {
    if (get_pcvar_num(CvarIsDebug) == 1)
      log_amx("[TracePlayer DEBUG] Count vault_util_new is 0");

    return;
  }

  new EzJSON:jsonData = ezjson_init_object();
  if (jsonData == EzInvalid_JSON)
  {
    log_amx("[TracePlayer] Failed to initialize jsonData");
    ezjson_free(jsonData);
    return;
  }

  new EzJSON:jsonPlayers = ezjson_init_array();
  if (jsonPlayers == EzInvalid_JSON)
  {
    log_amx("[TracePlayer] Failed to initialize jsonPlayers");
    ezjson_free(jsonPlayers);
    return;
  }

  new serverName[64];
  get_cvar_string("hostname", serverName, charsmax(serverName));
  if(ezjson_object_set_string(jsonData, "server", serverName))
  {
    if (get_pcvar_num(CvarIsDebug) == 1)
      log_amx("[TracePlayer DEBUG] Server successfully written to jsonData: %s", serverName);
  }

  new iPos = 0, key[128], steamId[32];

  for (new iCurrent = 1; iCurrent <= iCount; iCurrent++)
  {
    if (get_pcvar_num(CvarIsDebug) == 1)
      log_amx("[TracePlayer DEBUG] Let's start reading the data");

    iPos = nvault_util_read(vault_util_new, iPos, key, charsmax(key), steamId, charsmax(steamId));
    if (iPos <= 0)
    {
      if (get_pcvar_num(CvarIsDebug) == 1)
        log_amx("[TracePlayer DEBUG] iPos <= 0");

      break;
    }

    new ip[32], name[32];
    if (parse(key, ip, charsmax(ip), name, charsmax(name)) < 2)
    {
      if (get_pcvar_num(CvarIsDebug) == 1)
        log_amx("[TracePlayer DEBUG] Failed to parse data: %s", key);

      continue;
    }

    if (strlen(steamId) == 0 || strlen(ip) == 0 || strlen(name) == 0)
    {
      if (get_pcvar_num(CvarIsDebug) == 1)
        log_amx("[TracePlayer DEBUG] Incorrect data after parsing: '%s'", key);

      continue;
    }

    if (get_pcvar_num(CvarIsDebug) == 1)
      log_amx("[TracePlayer DEBUG] Data processing: %s %s %s", steamId, ip, name);

    new EzJSON:jsonPlayer = ezjson_init_object();
    if (jsonPlayer == EzInvalid_JSON)
    {
      log_amx("[TracePlayer] Failed to create JSON object for player: %s", steamId);
      continue;
    }

    if(ezjson_object_set_string(jsonPlayer, "steamId", steamId))
    {
      if (get_pcvar_num(CvarIsDebug) == 1)
        log_amx("[TracePlayer DEBUG] SteamId successfully written to jsonPlayer: %s", steamId);
    }

    if(ezjson_object_set_string(jsonPlayer, "ip", ip))
    {
      if (get_pcvar_num(CvarIsDebug) == 1)
        log_amx("[TracePlayer DEBUG] The ip was successfully recorded in jsonPlayer: %s", ip);
    }

    if(ezjson_object_set_string(jsonPlayer, "name", name))
    {
      if (get_pcvar_num(CvarIsDebug) == 1)
        log_amx("[TracePlayer DEBUG] Name was successfully written to jsonPlayer: %s", name);
    }

    if(ezjson_array_append_value(jsonPlayers, jsonPlayer))
    {
      if (get_pcvar_num(CvarIsDebug) == 1)
        log_amx("[TracePlayer DEBUG] jsonPlayers contains jsonPlayer");
    }

    nvault_set(vault_existing, key, steamId);

    if (get_pcvar_num(CvarIsDebug) == 1)
      log_amx("[TracePlayer DEBUG] The vault_existing contains data for: %s", key);
  }

  ezjson_object_set_value(jsonData, "players", jsonPlayers);

  new EzHttpOptions:options = ezhttp_create_options();
  if(ezhttp_option_set_body_from_json(options, jsonData))
  {
    if (get_pcvar_num(CvarIsDebug) == 1)
      log_amx("[TracePlayer DEBUG] TThe JSON object is set in the request body");
  }

  ezhttp_option_set_header(options, "Content-Type", "application/json");
  ezhttp_option_set_header(options, "X-API-Key", ApiKey);

  ezhttp_post(fmt("%s/api/player", ApiServer), "APIPlayersUpload", options);

  ezjson_free(jsonPlayers);
  ezjson_free(jsonData);
  
  nvault_util_close(vault_util_new);
}

public APIPlayersUpload(EzHttpRequest:request_id)
{
  if (ezhttp_get_error_code(request_id) != EZH_OK)
  {
    new error[64]
    ezhttp_get_error_message(request_id, error, charsmax(error))
    log_amx("[TracePlayer] Upload failed. Error: %s", error);
    return;
  }

  nvault_prune(vault_new, 0, prune_time);
  log_amx("[TracePlayer] Upload success.");
}

public plugin_end()
{
  nvault_close(vault_new);
  nvault_close(vault_existing);
  nvault_util_close(vault_util_new);
}