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
  serverIp: string,
  addedAt: Date;
}

export interface SteamPlayerInfo {
  steamid: string;
  personaname: string;
  avatarfull: string;
  profileurl: string;
}

export interface PlayerResponse {
  steamId: string;
  ips: PlayerIp[];
  names: PlayerName[];
  steamPlayerInfo: SteamPlayerInfo | null;
}
