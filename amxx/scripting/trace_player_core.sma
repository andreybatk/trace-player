/*
  Trace Player Core
*/

#include <amxmodx>
#include <amxmisc>

#define PLUGIN "Trace Player Core"
#define VERSION "1.4"
#define AUTHOR "yarmak"

new const Server[] = "http://localhost:8000";

public plugin_init()
{
  register_plugin(PLUGIN, VERSION, AUTHOR);

  register_clcmd("say", "SayHandle");
  register_clcmd("say /names", "MenuNames")
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
      new steamId[32];
      get_user_authid(id, steamId, charsmax(steamId));

      RequestPlayerInfo(id, steamId, id);
      return PLUGIN_HANDLED;
    }

    new targetId = find_player("bl", szTargetName);

    if (!targetId)
    {
      return PLUGIN_HANDLED;
    }

    new steamId[32];
    get_user_authid(targetId, steamId, charsmax(steamId));

    RequestPlayerInfo(id, steamId, targetId);

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

  new targetId = str_to_num(s_Data);

  if (!is_user_connected(targetId))
  {
    MenuNames(id);
    return;
  }

  new steamId[32];
  get_user_authid(targetId, steamId, charsmax(steamId));
  RequestPlayerInfo(id, steamId, targetId);
}

public RequestPlayerInfo(requesterId, targetSteamId[], targetId)
{
  new url[256]; new name[32]; new motd[MAX_MOTD_LENGTH];
  formatex(url, charsmax(url), "%s/player.php?steamId=%s", Server, targetSteamId);
  formatex(motd, sizeof(motd) - 1,\
    "<html><head><meta http-equiv=^"Refresh^" content=^"0;url=%s^"></head><body><p><center>LOADING...</center></p></body></html>",\
  url);

  get_user_name(targetId, name, charsmax(name));
  show_motd(requesterId, motd, name);
}
