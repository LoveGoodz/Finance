<template>
  <div>
    <h1>Müşteri Detayları</h1>
    <div v-if="customer">
      <p><strong>ID:</strong> {{ customer.id }}</p>
      <p><strong>Müşteri Adı:</strong> {{ customer.name }}</p>
      <p><strong>Adres:</strong> {{ customer.address }}</p>
      <p><strong>Telefon Numarası:</strong> {{ customer.phoneNumber }}</p>
      <p><strong>E-posta:</strong> {{ customer.email }}</p>
    </div>
    <p v-else>Müşteri bilgisi yüklenemedi.</p>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRoute } from "vue-router";

export default {
  setup() {
    const customer = ref(null);
    const route = useRoute();

    onMounted(async () => {
      const token = localStorage.getItem("token");
      const customerId = route.params.id;
      try {
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
        console.error("Müşteri detayları yüklenirken hata oluştu:", error);
      }
    });

    return { customer };
  },
};
</script>

<style scoped>
h1 {
  color: #ff4500;
  font-weight: bold;
  margin-bottom: 1rem;
}

p {
  font-size: 1.2rem;
  color: #333;
}

strong {
  color: #000;
}
</style>
