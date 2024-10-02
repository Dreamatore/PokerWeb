<template>
  <div class="home">
    <h1>Poker Bot Game</h1>
    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="!isInGame" class="join-game">
      <input v-model="token" placeholder="Enter token" :disabled="isLoading" />
      <input
        v-model="sessionId"
        placeholder="Enter session ID"
        :disabled="isLoading"
      />
      <button @click="joinGame" :disabled="isLoading">
        {{ isLoading ? "Joining..." : "JOIN GAME" }}
      </button>
      <p v-if="error" class="error">{{ error }}</p>
    </div>
    <div v-else>
      <GameTable />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, computed } from "vue";
import { useStore } from "vuex";
import GameTable from "@/components/GameTable.vue";

export default defineComponent({
  name: "HomeView",
  components: {
    GameTable,
  },
  setup() {
    const store = useStore();
    const token = ref("");
    const sessionId = ref("");
    const betAmount = ref(0);
    const error = computed(() => store.state.game.error);
    const isLoading = ref(false);

    const isInGame = computed(() => store.getters["game/isInGame"]);

    const joinGame = async () => {
      if (token.value && sessionId.value) {
        isLoading.value = true;
        try {
          await store.dispatch("game/joinGame", {
            sessionId: sessionId.value,
            token: token.value,
          });
        } catch (err) {
          console.error(err);
        } finally {
          isLoading.value = false;
        }
      } else {
        store.commit("game/setError", "Unexpected error. (Check backend)");
      }
    };

    const makeMove = (action: string, amount?: number) => {
      store.dispatch("game/makeMove", { action, amount });
    };

    return {
      token,
      sessionId,
      betAmount,
      isInGame,
      joinGame,
      makeMove,
      error,
      isLoading,
    };
  },
});
</script>

<style scoped>
.home {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 20px;
}

.join-game {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 20px;
}

input,
button {
  padding: 5px 10px;
  font-size: 16px;
}

button {
  cursor: pointer;
  background-color: #3498db;
  color: white;
  border: none;
  border-radius: 5px;
}

button:hover {
  background-color: #2980b9;
}

button:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}

.error {
  color: #e74c3c;
  font-weight: bold;
}
</style>
