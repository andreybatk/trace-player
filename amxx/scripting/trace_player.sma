/*
  Trace Player
  nvault_util https://forums.alliedmods.net/showthread.php?t=139584
*/

#include <amxmodx>
#include <nvault>
#include <nvault_util>
#include <grip>

#define PLUGIN "Trace Player"
#define VERSION "1.1"
#define AUTHOR "yarmak"

new vault_existing;
new vault_new;
new start_time;

new const VAULT_EXISTING[] = "api_player_cache";
new const VAULT_NEW[] = "api_player_new";

new const ApiServer[] = "http://0.0.0.0:8080";
new const ApiKey[] = "ApiKeyTracePlayer";

public plugin_init()
{
  register_plugin(PLUGIN, VERSION, AUTHOR);

  vault_existing = nvault_open(VAULT_EXISTING);
  vault_new = nvault_open(VAULT_NEW);
  start_time = get_systime();

  if (vault_existing == INVALID_HANDLE || vault_new == INVALID_HANDLE)
  {
    set_fail_state("Failed to open one of the nvaults");
  }
}

public client_connected(id)
{
  if (is_user_bot(id) || is_user_hltv(id)) return;

  new szSteamID[32]; get_user_authid(id, szSteamID, charsmax(szSteamID));
  new szIP[32]; get_user_ip(id, szIP, charsmax(szIP), 1);
  new szName[32]; get_user_name(id, szName, charsmax(szName));

  new szKey[64];
  formatex(szKey, charsmax(szKey), "player_%s", szSteamID);

  new szData[128];
  formatex(szData, charsmax(szData), "%s|%s|%s", szSteamID, szIP, szName);

  new szExistingData[128];
  if (nvault_get(vault_existing, szKey, szExistingData, charsmax(szExistingData))) {
    return;
  }

  nvault_set(vault_new, szKey, szData);
  //TODO: проверяет если полностью совпадает хеш, т.е может быть так что ip новый но name старый и все равно отправит на api (api конечно обработает но все же)
}

public plugin_end()
{
  if (vault_new == INVALID_HANDLE) return;

  new GripRequestOptions:hRequestOptions = grip_create_default_options(.timeout = -1.0);
  grip_options_add_header(hRequestOptions, "Content-Type", "application/json");
  grip_options_add_header(hRequestOptions, "X-API-Key", ApiKey);

  new GripJSONValue:hRoot = grip_json_init_object();
  new GripJSONValue:hPlayers = grip_json_init_array();

  new szKey[64], szData[128];
  for (new i = 0; i < nvault_util_count(vault_new); i++) {
    if (nvault_get(vault_new, szKey, szData, charsmax(szData))) {
      new szSteamID[32], szIP[32], szName[32];
      parse(szData, szSteamID, charsmax(szSteamID), szIP, charsmax(szIP), szName, charsmax(szName));

      new GripJSONValue:hPlayer = grip_json_init_object();
      grip_json_object_set_string(hPlayer, "steamId", szSteamID);
      grip_json_object_set_string(hPlayer, "ip", szIP);
      grip_json_object_set_string(hPlayer, "name", szName);
      grip_json_array_append_value(hPlayers, hPlayer);
      grip_destroy_json_value(hPlayer);

      nvault_set(vault_existing, szKey, szData);
    }
  }

  new end_time = get_systime();
  nvault_prune(vault_new, start_time, end_time);
  
  grip_json_object_set_value(hRoot, "players", hPlayers);

  new szServerName[64]; get_cvar_string("hostname", szServerName, charsmax(szServerName));

  grip_json_object_set_string(hRoot, "server", szServerName);

  new GripBody:hBody = grip_body_from_json(hRoot);

  grip_request(fmt("%s/api/player", ApiServer), hBody, GripRequestTypePost, "APIPlayersUpload", hRequestOptions);

  grip_destroy_body(hBody);
  grip_destroy_json_value(hRoot);
  grip_destroy_json_value(hPlayers);
  grip_destroy_options(hRequestOptions);

  nvault_close(vault_existing);
  nvault_close(vault_new);
}

public APIPlayersUpload()
{
  new GripResponseState:rState = grip_get_response_state();
  if (rState == GripResponseStateError) return;

  new GripHTTPStatus:rHTTPStatus = GripHTTPStatus:grip_get_response_status_code();
  if (rHTTPStatus != GripHTTPStatusOk) return;
}
