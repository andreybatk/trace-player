export interface TokenResponse {
  accessToken: string,
  accessTokenExpirationTime: Date | null,
  refreshToken: string
}

export interface JwtPayload {
  role?: string | string[];
  [key: string]: any;
}