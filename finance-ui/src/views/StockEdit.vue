<template>
  <div class="form-container">
    <h1 class="title">Stok Düzenle</h1>
    <form @submit.prevent="submitForm" class="stock-form">
      <label for="name">Stok Adı:</label>
      <input
        type="text"
        v-model="stock.name"
        id="name"
        class="form-input"
        required
      />

      <label for="quantity">Miktar:</label>
      <input
        type="number"
        v-model="stock.quantity"
        id="quantity"
        class="form-input"
        required
      />

      <label for="unitPrice">Birim Fiyat:</label>
      <input
        type="number"
        step="0.01"
        v-model="stock.unitPrice"
        id="unitPrice"
        class="form-input"
        required
      />

      <button type="submit" class="btn-primary">Kaydet</button>
    </form>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRoute, useRouter } from "vue-router";

export default {
  setup() {
    const stock = ref({
      name: "",
      quantity: 0,
      unitPrice: 0,
    });

    const route = useRoute();
    const router = useRouter();

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
        console.error("Stok bilgileri alınamadı:", error);
      }
    });

    const submitForm = async () => {
      const token = localStorage.getItem("token");
      try {
        await axios.put(`/api/stock/${route.params.id}`, stock.value, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        router.push("/stock");
      } catch (error) {
        console.error("Stok kaydı güncellenemedi:", error);
      }
    };

    return { stock, submitForm };
  },
};
</script>

<style scoped>
.form-container {
  padding: 20px;
  background-color: #f9f9f9;
  border-radius: 8px;
  max-width: 600px;
  margin: 0 auto;
}

.title {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 20px;
  color: #333;
}

.stock-form {
  display: flex;
  flex-direction: column;
}

.form-input {
  padding: 10px;
  margin-bottom: 20px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

.btn-primary {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
}

.btn-primary:hover {
  background-color: #0056b3;
}
</style>
