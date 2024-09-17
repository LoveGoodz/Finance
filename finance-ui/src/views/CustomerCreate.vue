<template>
  <div class="customer-create-container">
    <h1>Yeni Müşteri Ekle</h1>

    <form @submit.prevent="createCustomer">
      <div class="form-group">
        <label for="name">Müşteri Adı:</label>
        <input type="text" id="name" v-model="customer.name" required />
      </div>

      <div class="form-group">
        <label for="companyId">Şirket:</label>
        <select id="companyId" v-model="customer.companyId" required>
          <option disabled value="">Şirket Seçin</option>
          <option
            v-for="company in companies"
            :key="company.id"
            :value="company.id"
          >
            {{ company.name }}
          </option>
        </select>
        <p v-if="!customer.companyId && formSubmitted" class="error">
          Şirket seçimi zorunludur.
        </p>
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
import { ref, onMounted } from "vue";

export default {
  setup() {
    const customer = ref({
      name: "",
      companyId: null,
      address: "",
      phoneNumber: "",
      email: "",
    });
    const companies = ref([]);
    const errorMessage = ref("");
    const successMessage = ref("");
    const formSubmitted = ref(false);

    onMounted(async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await axios.get("https://localhost:7093/api/Company", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        companies.value = response.data;
      } catch (error) {
        console.error("Şirketler yüklenirken hata oluştu:", error);
        errorMessage.value = "Şirketler yüklenemedi.";
      }
    });

    const createCustomer = async () => {
      formSubmitted.value = true;
      try {
        const token = localStorage.getItem("token");

        if (!token) {
          errorMessage.value =
            "Yetkilendirme hatası: Lütfen tekrar giriş yapın.";
          return;
        }

        const response = await axios.post(
          "https://localhost:7093/api/Customer",
          customer.value,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        successMessage.value = `Müşteri başarıyla eklendi! Müşteri ID: ${response.data.id}`;
        errorMessage.value = "";
        resetForm();
      } catch (error) {
        console.error("Müşteri eklenirken hata oluştu:", error);
        errorMessage.value = "Müşteri eklenirken hata oluştu: " + error.message;
      }
    };

    const resetForm = () => {
      customer.value = {
        name: "",
        companyId: null,
        address: "",
        phoneNumber: "",
        email: "",
      };
      formSubmitted.value = false;
    };

    return {
      customer,
      companies,
      errorMessage,
      successMessage,
      formSubmitted,
      createCustomer,
    };
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

input,
select {
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
