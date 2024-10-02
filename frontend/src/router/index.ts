import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import HomeView from "../views/HomeView.vue";
import GameView from "../views/GameView.vue";
import AboutView from "../views/AboutView.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "home",
    component: HomeView, // Домашняя страница
  },
  {
    path: "/game/:sessionId",
    name: "game",
    component: GameView, // Страница игры
    props: (route) => ({
      sessionId: route.params.sessionId,
      token: route.query.token,
    }),
  },
  {
    path: "/about",
    name: "about",
    component: AboutView, // Указываем компонент для маршрута "About"
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
