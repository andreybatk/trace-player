/*
  Trace Player Core
  easy_http https://github.com/Next21Team/AmxxEasyHttp/releases/tag/1.4.0
*/

#include <amxmodx>
#include <amxmisc>
#include <easy_http>

#define PLUGIN "Trace Player Core"
#define VERSION "1.3"
#define AUTHOR "yarmak"

new const ApiServer[] = "http://localhost:5000";
new const ApiKey[] = "ApiKey";

new MotdTemplate[2048];
new FinalMotd[MAX_PLAYERS + 1][4096];

public plugin_init()
{
  register_plugin(PLUGIN, VERSION, AUTHOR);

  register_clcmd("say", "SayHandle");
  register_clcmd("say /names", "MenuNames")

  InitMotd();
}

public InitMotd()
{
  new filePath[128];
  get_datadir(filePath, charsmax(filePath));
  format(filePath, charsmax(filePath), "%s/player-motd-template.html", filePath);

  if (!file_exists(filePath))
  {
    set_fail_state("[ERROR] MOTD template not found");
  }

  read_file(filePath, 0, MotdTemplate, charsmax(MotdTemplate), _);
}

public SayHandle(id)
{
  new szArgs[64];
  read_args(szArgs, charsmax(szArgs));
  remove_quotes(szArgs);
  trim(szArgs);

  if (!szArgs[0] || szArgs[0] != '/')
    return PLUGIN_CONTINUE;

  new szCommand[32], szTargetName[32];
  parse(szArgs, szCommand, charsmax(szCommand), szTargetName, charsmax(szTargetName));

  if (equali(szCommand, "/name"))
  {
    trim(szTargetName);

    if (!szTargetName[0])
    {
      return PLUGIN_HANDLED;
    }

    new targetId = find_player("bl", szTargetName);

    if (!targetId)
    {
      return PLUGIN_HANDLED;
    }

    new steamId[32];
    get_user_authid(targetId, steamId, charsmax(steamId));

    RequestPlayerInfo(id, steamId);

    return PLUGIN_HANDLED;
  }

  return PLUGIN_CONTINUE;
}

public MenuNames(id)
{
	new menu = menu_create("Trace Player", "MenuNamesHandler");

	new iPlayers[MAX_PLAYERS], iNum, szPlayer[10], iPlayer;
	get_players(iPlayers, iNum, "ch");
	
	new szBuffer[256];
	for (new i; i < iNum; i++)
	{
		iPlayer = iPlayers[i];

		num_to_str(iPlayer, szPlayer, charsmax(szPlayer));
		add(szBuffer, charsmax(szBuffer), fmt("%n ", iPlayer));

		menu_additem(menu, szBuffer, szPlayer);
		
		szBuffer = "";
	}
	
	menu_display(id, menu, 0);
}
public MenuNamesHandler(id, menu, item)
{
  if (!is_user_connected(id))
  {
    menu_destroy(menu);
    return;
  }

  if (item == MENU_EXIT)
  {
    menu_destroy(menu);
    return;
  }

  new s_Data[6], s_Name[64], i_Access, i_Callback;
  menu_item_getinfo(menu, item, i_Access, s_Data, charsmax(s_Data), s_Name, charsmax(s_Name), i_Callback);

  menu_destroy(menu);

  new iPlayer = str_to_num(s_Data);

  if (!is_user_connected(iPlayer))
  {
    MenuNames(id);
    return;
  }

  new steamId[32];
  get_user_authid(iPlayer, steamId, charsmax(steamId));
  RequestPlayerInfo(id, steamId);
}

public RequestPlayerInfo(requesterId, targetSteamId[])
{
  new url[256];
  formatex(url, charsmax(url), "%s/api/player/bySteamId?steamId=%s", ApiServer, targetSteamId);

  new EzHttpOptions:options = ezhttp_create_options();
  ezhttp_option_set_header(options, "X-API-Key", ApiKey);
  ezhttp_option_set_header(options, "Content-Type", "application/json");

  new data[1]; data[0] = requesterId;
  ezhttp_option_set_user_data(options, data, sizeof(data));
  ezhttp_get(url, "OnMotdDataReceived", options);
}

public OnMotdDataReceived(EzHttpRequest:request_id)
{
  new data[1];
  ezhttp_get_user_data(request_id, data);
  new requesterId = data[0];

  if (!is_user_connected(requesterId))
  {
    return;
  }

  if (ezhttp_get_error_code(request_id) != EZH_OK)
  {
    new error[64]
    ezhttp_get_error_message(request_id, error, charsmax(error))
    log_amx("[TracePlayer] Get player failed. Error: %s", error);
    return;
  }

  new response[2048]
  ezhttp_get_data(request_id, response, charsmax(response))

  new EzJSON:json = ezjson_parse(response)
  if (json == EzInvalid_JSON)
  {
    log_amx("[TracePlayer] Incorrect JSON from API");
    return;
  }

  new name[64], avatar[128], steamId[32], vacBans[4], gameBans[4], communityBan[6];
  new namesRows[512], ipsRows[512];

  ezjson_object_get_string(json, "steamId", steamId, charsmax(steamId));
  ezjson_object_get_string(json, "namesRows", namesRows, charsmax(namesRows));
  ezjson_object_get_string(json, "ipsRows", ipsRows, charsmax(ipsRows));


  new EzJSON:fullInfo = ezjson_object_get_value(json, "fullSteamPlayerInfo");
  if (fullInfo == EzInvalid_JSON)
  {
    log_amx("[TracePlayer] Failed to initialize fullInfo");
    return;
  }

  new EzJSON:playerInfo = ezjson_object_get_value(fullInfo, "playerInfo");
  if (playerInfo == EzInvalid_JSON)
  {
    log_amx("[TracePlayer] Failed to initialize playerInfo");
    return;
  }

  ezjson_object_get_string(playerInfo, "personaname", name, charsmax(name));
  ezjson_object_get_string(playerInfo, "avatarfull", avatar, charsmax(avatar));

  new EzJSON:banInfo = ezjson_object_get_value(fullInfo, "banInfo");
  if (banInfo == EzInvalid_JSON)
  {
    log_amx("[TracePlayer] Failed to initialize banInfo");
    return;
  }

  new bool:bCommunityBanned = ezjson_object_get_bool(banInfo, "communityBanned");
  new vacInt = ezjson_object_get_number(banInfo, "numberOfVACBans");
  new gameInt = ezjson_object_get_number(banInfo, "numberOfGameBans");

  formatex(communityBan, charsmax(communityBan), bCommunityBanned ? "Community Ban" : "");
  formatex(vacBans, charsmax(vacBans), "%d", vacInt);
  formatex(gameBans, charsmax(gameBans), "%d", gameInt);

  ezjson_free(playerInfo);
  ezjson_free(banInfo);
  ezjson_free(fullInfo);
  ezjson_free(json);

  copy(FinalMotd[requesterId], charsmax(FinalMotd[]), MotdTemplate);

  replace_all(FinalMotd[requesterId], charsmax(FinalMotd[]), "{{personaname}}", name);
  replace_all(FinalMotd[requesterId], charsmax(FinalMotd[]), "{{avatarfull}}", avatar);
  replace_all(FinalMotd[requesterId], charsmax(FinalMotd[]), "{{steamId}}", steamId);
  replace_all(FinalMotd[requesterId], charsmax(FinalMotd[]), "{{vacBans}}", vacBans);
  replace_all(FinalMotd[requesterId], charsmax(FinalMotd[]), "{{gameBans}}", gameBans);
  replace_all(FinalMotd[requesterId], charsmax(FinalMotd[]), "{{communityBan}}", communityBan);
  replace_all(FinalMotd[requesterId], charsmax(FinalMotd[]), "{{namesRows}}", namesRows);
  replace_all(FinalMotd[requesterId], charsmax(FinalMotd[]), "{{ipsRows}}", ipsRows);

  show_motd(requesterId, FinalMotd[requesterId], "Trace Player");
}