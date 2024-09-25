<template>
  <div class="register-container">
    <h1>Kayıt Ol</h1>
    <form @submit.prevent="register">
      <div class="form-group">
        <label for="username">Kullanıcı Adı:</label>
        <input type="text" id="username" v-model="username" required />
      </div>

      <div class="form-group">
        <label for="email">E-posta:</label>
        <input type="email" id="email" v-model="email" required />
      </div>

      <div class="form-group">
        <label for="password">Şifre:</label>
        <input type="password" id="password" v-model="password" required />
      </div>

      <button type="submit">Kayıt Ol</button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </div>
</template>

<script>
import axios from "axios";
import { useRouter } from "vue-router";
import { ref } from "vue";

export default {
  setup() {
    const username = ref("");
    const email = ref("");
    const password = ref("");
    const errorMessage = ref("");
    const router = useRouter();

    const register = async () => {
      try {
        await axios.post("https://localhost:7093/api/Auth/register", {
          username: username.value,
          email: email.value,
          password: password.value,
        });

        router.push("/");
      } catch (error) {
        errorMessage.value =
          "Kayıt başarısız. Lütfen bilgilerinizi kontrol edin.";
      }
    };

    return {
      username,
      email,
      password,
      errorMessage,
      register,
    };
  },
};
</script>

<style scoped>
.register-container {
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
</style>
