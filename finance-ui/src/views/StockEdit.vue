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

      <button type="submit" class="btn-primary">Kaydet</button>
    </form>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      stock: {
        name: "",
        quantity: 0,
      },
    };
  },
  methods: {
    fetchStock() {
      axios
        .get(`/api/stock/${this.$route.params.id}`)
        .then((response) => {
          this.stock = response.data.Data;
        })
        .catch((error) => {
          console.error("Stok bilgileri alınamadı:", error);
        });
    },
    submitForm() {
      axios
        .put(`/api/stock/${this.$route.params.id}`, this.stock)
        .then(() => {
          this.$router.push("/stock");
        })
        .catch((error) => {
          console.error("Stok kaydı güncellenemedi:", error);
        });
    },
  },
  created() {
    this.fetchStock();
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
