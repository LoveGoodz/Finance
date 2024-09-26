<template>
  <div class="login-container">
    <h1>Giriş Yap</h1>
    <form @submit.prevent="login">
      <div class="form-group">
        <label for="username">Kullanıcı Adı:</label>
        <input type="text" id="username" v-model="username" required />
      </div>

      <div class="form-group">
        <label for="password">Şifre:</label>
        <input type="password" id="password" v-model="password" required />
      </div>

      <button type="submit" :disabled="loading">
        <span v-if="loading">Giriş Yapılıyor...</span>
        <span v-else>Giriş Yap</span>
      </button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

    <div class="register-container">
      <p>Henüz bir hesabınız yok mu?</p>
      <router-link to="/register" class="register-link">Kayıt Ol</router-link>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import { useRouter } from "vue-router";
import store from "../store";

export default {
  setup() {
    const username = ref("");
    const password = ref("");
    const errorMessage = ref("");
    const loading = ref(false);
    const router = useRouter();

    const login = async () => {
      loading.value = true;
      try {
        await store.dispatch("login", {
          username: username.value,
          password: password.value,
        });

        if (store.getters.isAuthenticated) {
          router.push("/invoice");
        } else {
          errorMessage.value = "Geçersiz kullanıcı adı veya şifre.";
        }
      } catch (error) {
        errorMessage.value = "Giriş sırasında bir hata oluştu.";
      } finally {
        loading.value = false;
      }
    };

    return {
      username,
      password,
      errorMessage,
      loading,
      login,
    };
  },
};
</script>

<style scoped>
.login-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
}

h1 {
  color: red;
  font-family: "Montserrat", sans-serif;
  margin-bottom: 1.5rem;
}

.form-group {
  margin-bottom: 1rem;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  width: 100%;
  max-width: 300px;
}

label {
  margin-bottom: 0.5rem;
  color: orange;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 10px;
  border-radius: 4px;
  border: 1px solid #ccc;
  background-color: #c0c0c0;
  font-size: 1rem;
}

.error {
  color: red;
  margin-top: 1rem;
}

button {
  width: 100%;
  background-color: #b87333;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  margin-top: 1rem;
}

button:hover {
  background-color: #a25f29;
}

button:disabled {
  background-color: #e0e0e0;
  cursor: not-allowed;
}

.register-container {
  margin-top: 1rem;
  text-align: center;
}

.register-link {
  color: #ff4500;
  text-decoration: none;
  font-weight: bold;
}

.register-link:hover {
  color: #e63900;
}
</style>
