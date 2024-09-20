<template>
  <div>
    <h1>Müşteri Düzenle</h1>

    <form @submit.prevent="updateCustomer">
      <div class="form-group">
        <label for="name">Müşteri Adı:</label>
        <input type="text" v-model="customer.name" required />
      </div>

      <div class="form-group">
        <label for="address">Adres:</label>
        <input type="text" v-model="customer.address" required />
      </div>

      <div class="form-group">
        <label for="phoneNumber">Telefon Numarası:</label>
        <input type="text" v-model="customer.phoneNumber" required />
      </div>

      <div class="form-group">
        <label for="email">E-posta:</label>
        <input type="email" v-model="customer.email" required />
      </div>

      <button type="submit" class="btn btn-success">Müşteriyi Güncelle</button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </div>
</template>

<script>
import axios from "axios";
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router"; // Vue Router'dan 'id' almak için ekledik

export default {
  setup() {
    const customer = ref({});
    const errorMessage = ref("");
    const route = useRoute(); // Vue Router'dan 'id' parametresini al

    onMounted(async () => {
      const token = localStorage.getItem("token");
      const id = route.params.id; // 'id' parametresini al
      try {
        const response = await axios.get(
          `https://localhost:7093/api/Customer/${id}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        customer.value = response.data;
      } catch (error) {
        console.error("Hata:", error);
        errorMessage.value = "Müşteri yüklenemedi.";
      }
    });

    const updateCustomer = async () => {
      const token = localStorage.getItem("token");
      try {
        await axios.put(
          `https://localhost:7093/api/Customer/${customer.value.id}`,
          customer.value,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        alert("Müşteri başarıyla güncellendi.");
      } catch (error) {
        console.error("Güncelleme hatası:", error);
        errorMessage.value = "Müşteri güncellenirken bir hata oluştu.";
      }
    };

    return { customer, updateCustomer, errorMessage };
  },
};
</script>

<style scoped>
.error {
  color: red;
  margin-top: 10px;
}
</style>
