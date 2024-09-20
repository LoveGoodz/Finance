<template>
  <div class="company-edit-container">
    <h1>Şirket Bilgilerini Düzenle</h1>

    <form @submit.prevent="updateCompany">
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

      <button type="submit" class="btn btn-success">Kaydet</button>
      <button @click="cancelEdit" class="btn btn-secondary">İptal</button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p v-if="successMessage" class="success">{{ successMessage }}</p>
  </div>
</template>

<script>
import axios from "axios";
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";

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
    const router = useRouter();
    const route = useRoute();
    const companyId = route.params.id;

    // Şirket bilgilerini yükleme
    onMounted(async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await axios.get(
          `https://localhost:7093/api/company/${companyId}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        company.value = response.data;
      } catch (error) {
        errorMessage.value = "Şirket bilgileri yüklenirken hata oluştu.";
      }
    });

    // Şirketi güncelleme
    const updateCompany = async () => {
      try {
        const token = localStorage.getItem("token");
        await axios.put(
          `https://localhost:7093/api/company/${companyId}`,
          company.value,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        successMessage.value = "Şirket başarıyla güncellendi!";
        errorMessage.value = "";
      } catch (error) {
        errorMessage.value = "Şirket güncellenirken hata oluştu.";
      }
    };

    const cancelEdit = () => {
      router.push("/company");
    };

    return {
      company,
      errorMessage,
      successMessage,
      updateCompany,
      cancelEdit,
    };
  },
};
</script>

<style scoped>
.company-edit-container {
  padding: 20px;
  max-width: 600px;
  margin: 0 auto;
  background-color: #f4f4f4;
  border-radius: 10px;
}

h1 {
  color: #ff69b4;
  font-weight: bold;
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
  padding: 10px 20px;
  margin-right: 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.btn-success {
  background-color: #28a745;
  color: white;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
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
