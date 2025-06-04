export interface PlayerItem {
  id: number;
  steamId: string;
  name: string;
  countryCode: string | null;
}

export interface PaginatedPlayersResponse {
  players: PlayerItem[];
  totalCount: number;
}

export interface PlayerIp {
  countryCode: string | null;
  addedAt: Date;
}

export interface PlayerName {
  name: string;
  server: string,
  addedAt: Date;
}

export interface SteamPlayerInfo {
  personaname: string;
  avatarfull: string;
  profileurl: string;
}

export interface SteamPlayerBanInfo {
  communityBanned: boolean;
  numberOfVACBans: number;
  numberOfGameBans: number;
}

export interface SteamPlayerGameInfo {
  playtime_forever: number;
  playtime_2weeks: number;
}

export interface FullSteamPlayerInfo {
  playerInfo: SteamPlayerInfo | null;
  banInfo: SteamPlayerBanInfo | null;
  gameInfo: SteamPlayerGameInfo | null;
}

export interface FungunPlayer {
  lastSuccess: FungunPlayerResult | null;
  lastWarning: FungunPlayerResult | null;
  lastDanger: FungunPlayerResult | null;
  lastReport: FungunPlayerResult | null;
}

export interface FungunPlayerResult {
  report_id: number;
  result_status: string;
}

export interface PlayerResponse {
  steamId: string;
  countryCode: string;
  ips: PlayerIp[];
  names: PlayerName[];
  fullSteamPlayerInfo: FullSteamPlayerInfo | null;
  fungunPlayer: FungunPlayer | null;
}
