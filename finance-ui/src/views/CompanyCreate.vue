<template>
  <div class="company-create-container">
    <h1>Yeni Şirket Ekle</h1>

    <form @submit.prevent="createCompany">
      <div class="form-group">
        <label for="name">Şirket Adı:</label>
        <input type="text" id="name" v-model="company.name" required />
      </div>

      <div class="form-group">
        <label for="address">Adres:</label>
        <input type="text" id="address" v-model="company.address" required />
      </div>

      <div class="form-group">
        <label for="phoneNumber">Telefon Numarası:</label>
        <input
          type="text"
          id="phoneNumber"
          v-model="company.phoneNumber"
          required
        />
      </div>

      <div class="form-group">
        <label for="email">E-posta:</label>
        <input type="email" id="email" v-model="company.email" required />
      </div>

      <button type="submit">Şirket Ekle</button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p v-if="successMessage" class="success">{{ successMessage }}</p>
  </div>
</template>

<script>
import axios from "axios";
import { ref } from "vue";

export default {
  setup() {
    const company = ref({
      name: "",
      address: "",
      phoneNumber: "",
      email: "",
    });
    const errorMessage = ref("");
    const successMessage = ref("");

    const createCompany = async () => {
      try {
        const token = localStorage.getItem("token");

        if (!token) {
          errorMessage.value = "Yetkilendirme hatası.";
          return;
        }

        const response = await axios.post(
          "https://localhost:7093/api/company",
          company.value,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        successMessage.value = `Şirket başarıyla eklendi! Şirket ID: ${response.data.id}`;
        errorMessage.value = "";
        resetForm();
      } catch (error) {
        console.error("Şirket eklenirken hata oluştu:", error);
        errorMessage.value = "Şirket eklenirken hata oluştu: " + error.message;
        successMessage.value = "";
      }
    };

    const resetForm = () => {
      company.value = {
        name: "",
        address: "",
        phoneNumber: "",
        email: "",
      };
    };

    return {
      company,
      errorMessage,
      successMessage,
      createCompany,
    };
  },
};
</script>

<style scoped>
.company-create-container {
  background-color: #f9f9f9;
  padding: 20px;
  border-radius: 10px;
  max-width: 500px;
  margin: 0 auto;
}

h1 {
  color: #2c3e50;
  margin-bottom: 20px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 10px;
  border-radius: 5px;
  border: 1px solid #ccc;
}

button {
  background-color: #28a745;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
}

button:hover {
  background-color: #218838;
}

.error {
  color: red;
  margin-top: 10px;
}

.success {
  color: green;
  margin-top: 10px;
}
</style>
