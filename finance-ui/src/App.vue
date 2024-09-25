<template>
  <div id="app">
    <nav>
      <router-link to="/invoice">Fatura Listesi</router-link>
      <router-link to="/invoice/create">Fatura Oluştur</router-link>
      <router-link to="/customer">Müşteri Listesi</router-link>
      <router-link to="/customer/create">Müşteri Ekle</router-link>
      <router-link to="/company">Şirket Listesi</router-link>
      <router-link to="/company/create">Şirket Ekle</router-link>
      <router-link to="/stock">Stok Listesi</router-link>
      <router-link to="/stock/create">Stok Ekle</router-link>
      <button @click="logout">Çıkış Yap</button>
    </nav>
    <router-view />
  </div>
</template>

<script>
import { useStore } from "vuex";
import { useRouter } from "vue-router";

export default {
  setup() {
    const store = useStore();
    const router = useRouter();

    const logout = () => {
      store.dispatch("logout");
      router.push("/login");
    };

    return {
      logout,
    };
  },
};
</script>

<style>
body {
  background-color: #87ceeb;
  margin: 0;
  padding: 0;
  font-family: "Roboto", sans-serif;
}

#app {
  font-family: "Montserrat", sans-serif;
  text-align: center;
  color: #2c3e50;
}

nav {
  padding: 20px;
  background-color: #ffffff;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

nav a {
  font-family: "Roboto", sans-serif;
  font-size: 1.2rem;
  color: #2c3e50;
  text-decoration: none;
  margin: 0 15px;
}

nav a.router-link-exact-active {
  color: #ff4500;
}

button {
  background-color: #ff4500;
  color: white;
  border: none;
  padding: 12px 24px;
  font-size: 1.1rem;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #e63900;
}
</style>
