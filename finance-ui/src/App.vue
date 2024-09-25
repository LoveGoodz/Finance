<template>
  <div id="app">
    <nav>
      <router-link to="/invoice">Fatura Listesi</router-link>
      <router-link to="/invoice/create">Fatura Oluştur</router-link>
      <router-link to="/customer">Müşteri Listesi</router-link>
      <router-link to="/customer/create">Müşteri Ekle</router-link>
      <router-link to="/company">Şirket Listesi</router-link>
      <router-link to="/company/create">Şirket Ekle</router-link>
      <router-link to="/login">Çıkış Yap</router-link>
    </nav>
    <router-view />
  </div>
</template>

<style scoped>
body {
  background-color: #f0f8ff;
  margin: 0;
  padding: 0;
  font-family: "Roboto", sans-serif;
}

#app {
  font-family: "Montserrat", sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

h1,
h2 {
  font-family: "Montserrat", sans-serif;
  font-size: 2.5rem;
  color: #333;
}

nav {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 15px;
  background-color: #ffffff;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;
}

nav a {
  font-family: "Roboto", sans-serif;
  font-size: 1.2rem;
  font-weight: bold;
  color: #2c3e50;
  text-decoration: none;
  margin: 0 20px;
  padding: 8px 16px;
  border-radius: 5px;
  transition: all 0.3s ease;
}

nav a.router-link-exact-active {
  color: #ff4500;
  background-color: #f0f8ff;
}

nav a:hover {
  background-color: #42b983;
  color: white;
}

button {
  background-color: #ff4500;
  color: white;
  border: none;
  padding: 12px 24px;
  font-size: 1.1rem;
  font-family: "Montserrat", sans-serif;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #e63900;
}
</style>
