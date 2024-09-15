<template>
  <nav>
    <router-link to="/invoices">Fatura Listesi</router-link>
  </nav>
  <router-view />
</template>

<style>
body {
  background-color: #87ceeb;
  margin: 0;
  padding: 0;
  font-family: "Roboto", sans-serif;
}

/* Genel uygulama stili */
#app {
  font-family: "Montserrat", sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

/* Başlıklar için stil */
h1,
h2 {
  font-family: "Montserrat", sans-serif;
  font-size: 2.5rem;
  color: #ffffff;
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
  font-weight: bold;
  color: #2c3e50;
  text-decoration: none;
  margin: 0 15px;
  transition: color 0.3s ease;
}

nav a.router-link-exact-active {
  color: #ff4500;
}

nav a:hover {
  color: #42b983;
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

.status-green {
  color: green;
  font-weight: bold;
}

.status-yellow {
  color: yellow;
  font-weight: bold;
}

.status-red {
  color: red;
  font-weight: bold;
}
</style>
