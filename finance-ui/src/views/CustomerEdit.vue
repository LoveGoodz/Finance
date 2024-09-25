<template>
  <div class="customer-edit-container">
    <h1>Müşteri Bilgilerini Düzenle</h1>

    <form @submit.prevent="updateCustomer">
      <div class="form-group">
        <label for="name">Müşteri Adı:</label>
        <input type="text" id="name" v-model="customer.name" required />
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
    const customer = ref({
      name: "",
      address: "",
      phoneNumber: "",
      email: "",
    });
    const errorMessage = ref("");
    const successMessage = ref("");
    const router = useRouter();
    const route = useRoute();
    const customerId = route.params.id;

    // Müşteri bilgilerini yükleme
    onMounted(async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await axios.get(
          `https://localhost:7093/api/customer/${customerId}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        customer.value = response.data;
      } catch (error) {
        errorMessage.value = "Müşteri bilgileri yüklenirken hata oluştu.";
      }
    });

    // Müşteriyi güncelleme
    const updateCustomer = async () => {
      try {
        const token = localStorage.getItem("token");
        await axios.put(
          `https://localhost:7093/api/customer/${customerId}`,
          customer.value,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        successMessage.value = "Müşteri başarıyla güncellendi!";
        errorMessage.value = "";
      } catch (error) {
        errorMessage.value = "Müşteri güncellenirken hata oluştu.";
      }
    };

    const cancelEdit = () => {
      router.push("/customer");
    };

    return {
      customer,
      errorMessage,
      successMessage,
      updateCustomer,
      cancelEdit,
    };
  },
};
</script>

<style scoped>
.customer-edit-container {
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
