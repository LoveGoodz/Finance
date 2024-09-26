<template>
  <div>
    <h1>Stok Detayları</h1>
    <div v-if="stock">
      <p><strong>ID:</strong> {{ stock.id }}</p>
      <p><strong>Stok Adı:</strong> {{ stock.name }}</p>
      <p><strong>Birim Fiyat:</strong> {{ stock.unitPrice }} TL</p>
      <p><strong>Oluşturulma Tarihi:</strong> {{ formattedCreatedAt }}</p>
      <p><strong>Güncellenme Tarihi:</strong> {{ formattedUpdatedAt }}</p>
      <p><strong>Miktar:</strong> {{ stock.quantity }}</p>
    </div>
    <p v-else>Stok bilgisi yüklenemedi.</p>
  </div>
</template>

<script>
import { ref, onMounted, computed } from "vue";
import axios from "axios";
import { useRoute } from "vue-router";

export default {
  setup() {
    const stock = ref(null);
    const route = useRoute();

    onMounted(async () => {
      const token = localStorage.getItem("token");
      const stockId = route.params.id;
      try {
        const response = await axios.get(
          `https://localhost:7093/api/stock/${stockId}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        stock.value = response.data;
      } catch (error) {
        console.error("Stok detayları yüklenirken hata oluştu:", error);
      }
    });

    const formattedCreatedAt = computed(() => {
      return new Date(stock.value.createdAt).toLocaleDateString("tr-TR", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    });

    const formattedUpdatedAt = computed(() => {
      return new Date(stock.value.updatedAt).toLocaleDateString("tr-TR", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    });

    return { stock, formattedCreatedAt, formattedUpdatedAt };
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
