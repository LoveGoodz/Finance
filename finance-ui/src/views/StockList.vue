<template>
  <div class="stock-container">
    <h1 class="title">Stok Listesi</h1>
    <Table :data="stocks" />
    <Pagination
      :currentPage="pageNumber"
      :totalItems="totalRecords"
      @pageChange="fetchStocks"
    />
    <router-link to="/stock/create">
      <button class="btn-primary">Yeni Stok Ekle</button>
    </router-link>
  </div>
</template>

<script>

import axios from "axios";

export default {
  data() {
    return {
      stocks: [],
      pageNumber: 1,
      pageSize: 10,
      totalRecords: 0,
    };
  },
  methods: {
    fetchStocks(pageNumber = 1) {
      axios
        .get(`/api/stock?pageNumber=${pageNumber}&pageSize=${this.pageSize}`)
        .then((response) => {
          this.stocks = response.data.Data;
          this.totalRecords = response.data.TotalRecords;
        })
        .catch((error) => {
          console.error("Stok verileri alınamadı:", error);
        });
    },
  },
  created() {
    this.fetchStocks();
  },
};
</script>

<style scoped>
.stock-container {
  padding: 20px;
  background-color: #f9f9f9;
}

.title {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 20px;
  color: #333;
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

.table-container {
  margin-top: 20px;
}
</style>
