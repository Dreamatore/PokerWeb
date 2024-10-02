<template>
  <nav>
    <router-link @click="goHome" to="/">Home</router-link>
    |
    <router-link to="/about">About</router-link>
  </nav>
  <router-view />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "App",
  setup() {
    const router = useRouter();

    const goHome = () => {
      router.push("/").then(() => {
        window.location.reload(); // Принудительно перезагрузить страницу
      });
    };

    return {
      goHome,
    };
  },
});
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

nav {
  padding: 10px;
}

nav a {
  font-weight: bold;
  color: #2c3e50;
}
</style>
