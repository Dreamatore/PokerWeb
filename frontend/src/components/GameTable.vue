<template>
  <div v-if="sessionNotFound">
    <SessionNotFound />
  </div>
  <div v-else class="game-container">
    <div class="game-table">
      <div class="table-container">
        <div class="pot">Pot: {{ gameBank }}</div>
        <div class="community-cards">
          <div class="community-cards">
            <div
              v-for="(card, index) in communityCards"
              :key="index"
              class="card"
            >
              <div class="card" v-html="formatCard(card)"></div>
            </div>
          </div>
        </div>
        <div class="players">
          <div
            v-for="player in otherPlayers"
            :key="player.id"
            class="player"
            :class="{
              active: player.id === currentActiveId,
              folded: player.isFolded,
              dealer: isDealer(player.id),
            }"
            :style="
              playerPositionStyles(
                otherPlayers.indexOf(player),
                otherPlayers.length
              )
            "
          >
            <div class="player-name">{{ player.name }}</div>
            <div class="player-chips">Chips: {{ player.playerBank }}</div>
            <div class="player-bet">Bet: {{ player.currentBet }}</div>
          </div>
        </div>
        <div class="action-timer" v-if="actionTimeLeft > 0">
          Time left: {{ actionTimeLeft }}s
        </div>
      </div>
      <div class="current-player-area">
        <div
          class="current-player player"
          :class="{ active: isCurrentPlayer, dealer: isDealer(userId) }"
        >
          <div class="player-name">{{ playerName }}</div>
          <div class="player-cards">
            <div v-for="(card, index) in currentHand" :key="index" class="card">
              <div class="card" v-html="formatCard(card)"></div>
            </div>
          </div>
          <div class="player-chips">Chips: {{ playerBank }}</div>
          <div class="player-combo" v-if="playerComboName">
            {{ playerComboName }}
          </div>
        </div>
        <div v-if="isCurrentPlayer" class="actions">
          <button @click.prevent="makeMove('fold')">Fold</button>
          <button @click.prevent="makeMove('call')">Call</button>
          <button @click.prevent="makeMove('raise')">Raise</button>
          <button @click.prevent="makeMove('allin')">All-in</button>
        </div>
      </div>
    </div>
    <div class="game-log">
      <h3>Game Log</h3>
      <ul>
        <li v-for="(action, index) in reversedActionHistory" :key="index">
          {{ action }}
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, computed } from "vue";
import { useStore } from "vuex";
import { Card } from "@/services/api";

export default defineComponent({
  name: "GameTable",
  setup() {
    const store = useStore();

    const communityCards = computed(() => store.getters["game/communityCards"]);
    const currentHand = computed(() => store.getters["game/currentHand"]);
    const playerBank = computed(() => store.getters["game/playerBank"]);
    const gameBank = computed(() => store.getters["game/gameBank"]);
    const otherPlayers = computed(() => store.getters["game/otherPlayers"]);
    const isCurrentPlayer = computed(
      () => store.getters["game/isCurrentPlayer"]
    );
    const playerName = computed(() => store.state.game.gameState?.name || "");
    const currentActiveId = computed(
      () => store.state.game.gameState?.currentActiveId
    );
    const dealerId = computed(() => store.getters["game/dealerId"]);
    const actionHistory = computed(() => store.getters["game/actionHistory"]);
    const reversedActionHistory = computed(() =>
      [...actionHistory.value].reverse()
    );
    const actionTimeLeft = computed(() => store.getters["game/actionTimeLeft"]);
    const playerCombo = computed(() => store.getters["game/playerCombo"]);
    const playerComboName = computed(
      () => store.getters["game/playerComboName"]
    );
    const userId = computed(() => store.state.game.userId);
    const sessionNotFound = computed(() => store.state.game.sessionNotFound);

    const formatCard = (card: Card): string => {
      const suits = ["♥️", "♠️", "♦️", "♣️"];
      const ranks = [
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "J",
        "Q",
        "K",
        "A",
      ];

      const suit = suits[card.suit];
      const rank = ranks[card.rank - 2];
      const suitColor = suit === "♥️" || suit === "♦️" ? "red" : "black";

      return `<span style="color: black;">${rank}</span><span style="color: ${suitColor};">${suit}</span>`;
    };

    const makeMove = (action: string) => {
      store.dispatch("game/makeMove", {
        action,
        amount: action === "allin" ? playerBank.value : undefined,
      });
    };

    const playerPositionStyles = (index: number, totalPlayers: number) => {
      const angle = (index / totalPlayers) * 2 * Math.PI;
      const x = 50 + 40 * Math.cos(angle);
      const y = 50 + 40 * Math.sin(angle);
      return {
        top: `${y}%`,
        left: `${x}%`,
        transform: `translate(-50%, -50%)`,
      };
    };

    const isDealer = (playerId: number | null) => playerId === dealerId.value;

    return {
      communityCards,
      currentHand,
      playerBank,
      gameBank,
      otherPlayers,
      isCurrentPlayer,
      playerName,
      currentActiveId,
      dealerId,
      reversedActionHistory,
      actionTimeLeft,
      playerCombo,
      playerComboName,
      userId,
      sessionNotFound,
      formatCard,
      makeMove,
      playerPositionStyles,
      isDealer,
    };
  },
});
</script>

<style scoped>
.game-container {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  gap: 20px;
  padding: 5px;
}

.game-table {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: #2c3e50;
  color: white;
  border-radius: 10px;
  padding: 20px;
  width: 990px;
}

.table-container {
  width: 900px;
  height: 450px;
  background-color: #27ae60;
  border-radius: 225px;
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
}

.pot {
  position: absolute;
  top: 10px;
  left: 50%;
  transform: translateX(-50%);
  font-size: 24px;
  font-weight: bold;
}

.community-cards {
  display: flex;
  gap: 10px;
}

.card {
  width: 60px;
  height: 90px;
  background-color: white;
  color: black;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 18px;
  border-radius: 5px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.players {
  position: absolute;
  width: 100%;
  height: 100%;
}

.player {
  position: absolute;
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: rgba(0, 0, 0, 0.5);
  padding: 10px;
  border-radius: 5px;
}

.current-player-area {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 10px 0;
}

.current-player {
  position: static;
  transform: none;
  margin-bottom: 10px;
}

.player.active {
  box-shadow: 0 0 10px 5px rgba(255, 255, 255, 0.5);
}

.player.folded {
  opacity: 0.5;
}

.player.dealer::after {
  content: "D";
  position: absolute;
  top: -10px;
  right: -10px;
  background-color: #f39c12;
  color: white;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  display: flex;
  justify-content: center;
  align-items: center;
  font-weight: bold;
}

.player-name {
  font-weight: bold;
  margin-bottom: 5px;
}

.player-cards {
  display: flex;
  gap: 5px;
  margin-bottom: 5px;
}

.player-chips,
.player-bet {
  font-size: 14px;
}

.player-combo {
  margin-top: 5px;
  font-weight: bold;
  color: #f39c12;
}

.action-timer {
  position: absolute;
  top: 50px;
  left: 50%;
  transform: translateX(-50%);
  font-size: 18px;
  font-weight: bold;
  color: #f39c12;
}

.actions {
  display: flex;
  gap: 10px;
  justify-content: center;
}

button {
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
}

.game-log {
  width: 300px;
  height: 570px; /* Matches the total height of game-table (450px table + 80px current player area + 40px padding) */
  overflow-y: auto;
  background-color: #2c3e50;
  padding: 10px;
  border-radius: 5px;
  color: white;
}

.game-log h3 {
  margin-top: 0;
  margin-bottom: 10px;
}

.game-log ul {
  list-style-type: none;
  padding: 0;
  margin: 0;
}

.game-log li {
  margin-bottom: 5px;
}
</style>
