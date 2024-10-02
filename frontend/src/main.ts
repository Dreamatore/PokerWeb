import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

const app = createApp(App);

app.use(store).use(router);

router.isReady().then(() => {
  const urlParams = new URLSearchParams(window.location.search);
  const token = urlParams.get("token");
  const sessionId = router.currentRoute.value.params.sessionId as string;

  if (token && sessionId) {
    store.dispatch("game/joinGame", { sessionId, token });
  }

  app.mount("#app");
});
