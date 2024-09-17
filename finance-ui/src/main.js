import { createApp } from "vue";
import App from "./App.vue";
import PrimeVue from "primevue/config";
import "primevue/resources/themes/saga-blue/theme.css";
import "primevue/resources/primevue.min.css";
import "primeicons/primeicons.css";
import router from "./router";
import axios from "axios";

axios.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");

  if (token) {
    console.log("Gönderilen Token:", token);
    config.headers.Authorization = `Bearer ${token}`;
  } else {
    console.log("Token bulunamadı.");
  }

  return config;
});

const app = createApp(App);
app.use(PrimeVue);
app.use(router);
app.mount("#app");
