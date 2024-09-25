<template>
  <div class="stock-details">
    <h1>Stok Detayları</h1>
    <p>Stok Adı: {{ stock.name }}</p>
    <p>Stok Kodu: {{ stock.code }}</p>
    <p>Oluşturulma Tarihi: {{ formattedCreatedAt }}</p>
    <p>Güncellenme Tarihi: {{ formattedUpdatedAt }}</p>
    <p>Miktar: {{ stock.quantity }}</p>
  </div>
</template>

<script>
export default {
  data() {
    return {
      stock: {
        name: "Örnek Stok",
        code: "STK123",
        createdAt: "2023-09-15T12:34:56Z",
        updatedAt: "2023-09-17T08:21:43Z",
        quantity: 100,
      },
    };
  },
  computed: {
    formattedCreatedAt() {
      return new Date(this.stock.createdAt).toLocaleDateString("tr-TR", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    },
    formattedUpdatedAt() {
      return new Date(this.stock.updatedAt).toLocaleDateString("tr-TR", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    },
  },
};
</script>

<style scoped>
.stock-details {
  background-color: #f4f4f4;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

h1 {
  font-size: 2rem;
  color: #2c3e50;
}

p {
  font-size: 1.2rem;
  color: #333;
}
</style>
