<template>
  <div class="about">
    <h1>About Poker Bot Game</h1>
    <p>
      This project is a Texas Hold'em Poker game.
    </p>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "AboutView",
});
</script>

<style scoped>
.about {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  text-align: center;
}

h1 {
  font-size: 32px;
  margin-bottom: 20px;
}

h2 {
  font-size: 24px;
  margin-top: 20px;
}

p {
  font-size: 18px;
  margin-bottom: 15px;
}

a {
  color: #3498db;
  text-decoration: none;
  font-weight: bold;
}

a:hover {
  text-decoration: underline;
}
</style>
