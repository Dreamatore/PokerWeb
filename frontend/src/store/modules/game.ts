import { Module } from "vuex";
import api, { GameState, SessionNotFoundError } from "@/services/api";
import axios from "axios";

export interface GameModule {
  sessionId: string | null;
  token: string | null;
  gameState: GameState | null;
  error: string | null;
  userId: number | null;
  sessionNotFound: boolean;
}

class RootState {}

const gameModule: Module<GameModule, RootState> = {
  namespaced: true,
  state: {
    sessionId: null,
    token: null,
    gameState: null,
    error: null,
    userId: null,
    sessionNotFound: false,
  },
  mutations: {
    setSessionId(state, sessionId: string) {
      state.sessionId = sessionId;
    },
    setToken(state, token: string) {
      state.token = token;
    },
    setGameState(state, gameState: GameState) {
      state.gameState = gameState;
    },
    setError(state, error: string) {
      state.error = error;
    },
    setUserId(state, userId: number) {
      state.userId = userId;
    },
    setSessionNotFound(state, value: boolean) {
      state.sessionNotFound = value;
    },
  },
  actions: {
    async joinGame({ commit, dispatch }, { sessionId, token }) {
      try {
        commit("setSessionId", sessionId);
        commit("setToken", token);
        await dispatch("fetchGameState");
        dispatch("startPolling");
      } catch (error: any) {
        let errorMessage = "An unknown error occurred.";
        if (error instanceof SessionNotFoundError) {
          commit("setSessionNotFound", true);
          errorMessage = "Session not found.";
        } else if (error.response && error.response.data) {
          errorMessage = `Error: ${error.response.data}`;
        } else if (error.message) {
          errorMessage = `Error: ${error.message}`;
        }
        commit("setError", errorMessage);
        throw error;
      }
    },
    async fetchGameState({ state, commit }) {
      if (!state.sessionId || !state.token) return;
      try {
        const response = await api.getGameState(state.sessionId, state.token);
        commit("setGameState", response.data);
        commit("setUserId", response.data.id);
        commit("setSessionNotFound", false);
      } catch (error: unknown) {
        let errorMessage =
          "An unknown error occurred while fetching game state.";
        if (axios.isAxiosError(error)) {
          if (error.response && error.response.status === 400) {
            errorMessage = "Request failed with status code 400: Bad Request.";
          } else if (error.response && error.response.data) {
            errorMessage = `Error: ${error.response.data}`;
          } else if (error.message) {
            errorMessage = `Error: ${error.message}`;
          }
        } else if (error instanceof Error) {
          errorMessage = `Error: ${error.message}`;
        }
        commit("setError", errorMessage);
        console.error("Error while fetching game state:", errorMessage);
        throw error;
      }
    },
    async makeMove({ state, commit }, { action, amount }) {
      if (!state.sessionId || !state.token) return;
      try {
        const response = await api.makeMove(
          state.sessionId,
          state.token,
          action,
          amount
        );
        commit("setGameState", response.data);
        commit("setSessionNotFound", false);
      } catch (error: any) {
        let errorMessage = "An unknown error occurred.";
        if (error instanceof SessionNotFoundError) {
          commit("setSessionNotFound", true);
          errorMessage = "Session not found.";
        } else if (error.response && error.response.data) {
          errorMessage = `Error: ${error.response.data}`;
        } else if (error.message) {
          errorMessage = `Error: ${error.message}`;
        }
        commit("setError", errorMessage);
        throw error;
      }
    },
    async leaveGame({ state, commit }) {
      if (!state.sessionId || !state.token) return;
      try {
        await api.leaveGame(state.sessionId, state.token);
        commit("setSessionId", null);
        commit("setToken", null);
        commit("setGameState", null);
        commit("setUserId", null);
        commit("setSessionNotFound", false);
      } catch (error) {
        if (error instanceof SessionNotFoundError) {
          commit("setSessionNotFound", true);
        } else {
          commit("setError", "Failed to leave game");
        }
        throw error;
      }
    },
    startPolling({ state, dispatch, commit }) {
      if (!state.sessionId || !state.token) return;
      const intervalId = setInterval(async () => {
        try {
          await dispatch("fetchGameState");
        } catch (error: any) {
          let errorMessage = "An unknown error occurred during polling.";
          if (error instanceof SessionNotFoundError) {
            commit("setSessionNotFound", true);
            errorMessage = "Session not found during polling.";
          } else if (axios.isAxiosError(error)) {
            if (error.response && error.response.data) {
              errorMessage = `Polling Error: ${error.response.data}`;
            } else if (error.message) {
              errorMessage = `Polling Error: ${error.message}`;
            }
          } else if (error instanceof Error) {
            errorMessage = `Polling Error: ${error.message}`;
          }
          commit("setError", errorMessage); // Сохранение сообщения об ошибке в состоянии
          console.error("Error while polling game state:", errorMessage);
        }
      }, 1000);

      return () => clearInterval(intervalId);
    },
  },
  getters: {
    isInGame: (state) => !!state.sessionId && !!state.token,
    isCurrentPlayer: (state) =>
      state.gameState?.currentActiveId === state.userId,
    currentHand: (state) => state.gameState?.hand || [],
    communityCards: (state) => state.gameState?.table || [],
    playerBank: (state) => state.gameState?.playerBank || 0,
    gameBank: (state) => state.gameState?.gameBank || 0,
    otherPlayers: (state) => state.gameState?.otherPlayers || [],
    dealerId: (state) => state.gameState?.dealerId,
    actionHistory: (state) => state.gameState?.actionHistory || [],
    actionTimeLeft: (state) => state.gameState?.actionTimeLeftSec || 0,
    playerCombo: (state) => state.gameState?.combo || [],
    playerComboName: (state) => state.gameState?.comboName || "",
  },
};

export default gameModule;
