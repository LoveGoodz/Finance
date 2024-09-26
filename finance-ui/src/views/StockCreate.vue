<template>
  <div class="form-container">
    <h1 class="title">Yeni Stok Ekle</h1>
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
        v-model="stock.unitPrice"
        id="unitPrice"
        class="form-input"
        required
      />

      <label for="companyId">Şirket ID:</label>
      <input
        type="number"
        v-model="stock.companyID"
        id="companyId"
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
        unitPrice: 0,
        companyID: 0,
      },
    };
  },
  methods: {
    submitForm() {
      axios
        .post("/api/stock", this.stock)
        .then(() => {
          this.$router.push("/stock");
        })
        .catch((error) => {
          console.error("Stok kaydı eklenemedi:", error);
        });
    },
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
