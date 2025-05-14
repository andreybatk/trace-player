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

export interface FullSteamPlayerInfo {
  playerInfo: SteamPlayerInfo | null;
  banInfo: SteamPlayerBanInfo | null;
}

export interface PlayerResponse {
  steamId: string;
  ips: PlayerIp[];
  names: PlayerName[];
  fullSteamPlayerInfo: FullSteamPlayerInfo | null;
}
