<template>
  <div>
    <GameTable />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import GameTable from "@/components/GameTable.vue";

export default defineComponent({
  name: "GameView",
  components: {
    GameTable,
  },
  setup() {
    // Весь код, который был раньше в HomeView для работы с игрой
  },
});
</script>

<style scoped>
/* Стили для игры */
</style>
