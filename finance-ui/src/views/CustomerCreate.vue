<template>
  <div class="customer-create-container">
    <h1>Yeni Müşteri Ekle</h1>

    <form @submit.prevent="createCustomer">
      <div class="form-group">
        <label for="name">Müşteri Adı:</label>
        <input type="text" id="name" v-model="customer.name" required />
      </div>

      <div class="form-group">
        <label for="companyId">Şirket ID:</label>
        <input
          type="number"
          id="companyId"
          v-model="customer.companyId"
          required
        />
      </div>

      <div class="form-group">
        <label for="address">Adres:</label>
        <input type="text" id="address" v-model="customer.address" required />
      </div>

      <div class="form-group">
        <label for="phoneNumber">Telefon Numarası:</label>
        <input
          type="text"
          id="phoneNumber"
          v-model="customer.phoneNumber"
          required
        />
      </div>

      <div class="form-group">
        <label for="email">E-posta:</label>
        <input type="email" id="email" v-model="customer.email" required />
      </div>

      <button type="submit">Müşteri Ekle</button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p v-if="successMessage" class="success">{{ successMessage }}</p>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      customer: {
        name: "",
        companyId: null,
        address: "",
        phoneNumber: "",
        email: "",
      },
      errorMessage: "",
      successMessage: "",
    };
  },
  methods: {
    async createCustomer() {
      try {
        const token = localStorage.getItem("token");

        if (!token) {
          this.errorMessage =
            "Yetkilendirme hatası: Lütfen tekrar giriş yapın.";
          return;
        }

        const response = await axios.post(
          "https://localhost:7093/api/Customer",
          this.customer,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        this.successMessage = `Müşteri başarıyla eklendi! Müşteri ID: ${response.data.id}, Müşteri Adı: ${response.data.name}`;
        this.errorMessage = "";
        this.resetForm();
      } catch (error) {
        console.error("Müşteri eklenirken hata oluştu:", error);
        this.successMessage = "";
        this.errorMessage = "Müşteri eklenirken hata oluştu: " + error.message;
      }
    },
    resetForm() {
      this.customer = {
        name: "",
        companyId: null,
        address: "",
        phoneNumber: "",
        email: "",
      };
    },
  },
};
</script>

<style scoped>
.customer-create-container {
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
