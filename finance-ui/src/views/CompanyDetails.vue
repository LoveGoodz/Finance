<template>
  <div>
    <h1>Şirket Detayları</h1>
    <div v-if="company">
      <p><strong>ID:</strong> {{ company.id }}</p>
      <p><strong>Şirket Adı:</strong> {{ company.name }}</p>
      <p><strong>Adres:</strong> {{ company.address }}</p>
      <p><strong>Telefon Numarası:</strong> {{ company.phoneNumber }}</p>
      <p><strong>E-posta:</strong> {{ company.email }}</p>
    </div>
    <p v-else>Şirket bilgisi yüklenemedi.</p>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRoute } from "vue-router";

export default {
  setup() {
    const company = ref(null);
    const route = useRoute();

    onMounted(async () => {
      const token = localStorage.getItem("token");
      const companyId = route.params.id;
      try {
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
        console.error("Şirket detayları yüklenirken hata oluştu:", error);
      }
    });

    return { company };
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
