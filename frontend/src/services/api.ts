import axios, { AxiosResponse, AxiosError } from "axios";

const apiClient = axios.create({
  baseURL: "http://localhost:5000/v1", // Replace with the actual URL
  headers: {
    "Content-Type": "application/json",
  },
});

export interface Card {
  suit: number;
  rank: number;
}

export interface Player {
  name: string;
  playerBank: number;
  currentBet: number;
  id: number;
  isFolded: boolean;
}

export interface GameState {
  dealerId: number;
  actionHistory: string[];
  actionTimeLeftSec: number;
  gameBank: number;
  name: string;
  combo: Card[];
  comboName: string;
  id: number;
  hand: Card[];
  table: Card[];
  playerBank: number;
  otherPlayers: Player[];
  currentActiveId: number;
}

export class SessionNotFoundError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "SessionNotFoundError";
  }
}

const handleSessionNotFoundError = (error: AxiosError) => {
  if (
    error.response &&
    error.response.status === 400 &&
    error.response.data === "Session not found"
  ) {
    throw new SessionNotFoundError("Session not found");
  }
  throw error;
};

export default {
  getGameState(
    sessionId: string,
    token: string
  ): Promise<AxiosResponse<GameState>> {
    return apiClient
      .get(`/session/${sessionId.trim()}/info/${token.trim()}`)
      .catch(handleSessionNotFoundError);
  },
  makeMove(
    sessionId: string,
    token: string,
    action: string,
    amount?: number
  ): Promise<AxiosResponse<GameState>> {
    return apiClient
      .post(
        `/session/action`,
        { amount },
        {
          params: { sessionId: sessionId.trim(), action, token: token.trim() },
        }
      )
      .catch(handleSessionNotFoundError);
  },
  leaveGame(sessionId: string, token: string): Promise<AxiosResponse<void>> {
    return apiClient
      .post(`/session/leave`, null, {
        params: { sessionId: sessionId.trim(), token: token.trim() },
      })
      .catch(handleSessionNotFoundError);
  },
};
