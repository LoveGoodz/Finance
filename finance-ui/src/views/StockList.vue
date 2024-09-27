<template>
  <div class="stock-list-container">
    <h1>Stok Listesi</h1>

    <div>
      <label for="companySelect">Şirket Seç:</label>
      <select
        id="companySelect"
        v-model="selectedCompany"
        @change="fetchStocks"
      >
        <option value="">Tüm Stoklar</option>
        <option
          v-for="company in companies"
          :key="company.id"
          :value="company.id"
        >
          {{ company.name }}
        </option>
      </select>
    </div>

    <p v-if="displayedStocks.length === 0 && errorMessage" class="error">
      {{ errorMessage }}
    </p>

    <table v-if="displayedStocks.length > 0">
      <thead>
        <tr>
          <th>ID</th>
          <th>Stok Adı</th>
          <th>Miktar</th>
          <th>Birim Fiyat</th>
          <th>Şirket ID</th>
          <th>İşlemler</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="stock in displayedStocks" :key="stock.id">
          <td class="stock-data">{{ stock.id }}</td>
          <td class="stock-data">{{ stock.name }}</td>
          <td class="stock-data">{{ stock.quantity }}</td>
          <td class="stock-data">{{ stock.unitPrice }}</td>
          <td class="stock-data">{{ stock.companyID }}</td>
          <td>
            <button @click="viewStock(stock.id)" class="btn btn-info">
              Detay
            </button>
            <button @click="deleteStock(stock.id)" class="btn btn-danger">
              Sil
            </button>
            <button @click="editStock(stock.id)" class="btn btn-success">
              Düzenle
            </button>
          </td>
        </tr>
      </tbody>
    </table>

    <p v-if="displayedStocks.length === 0 && !errorMessage">
      Listelenecek stok bulunamadı.
    </p>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";

export default {
  setup() {
    const store = useStore();
    const router = useRouter();
    const stocks = computed(() => store.getters.getStocks || []);
    const filteredStocks = computed(
      () => store.getters.getFilteredStocks || []
    );
    const companies = computed(() => store.getters.getCompanies || []);
    const selectedCompany = ref("");
    const errorMessage = computed(() => store.getters.getError);

    const displayedStocks = computed(() => {
      return selectedCompany.value ? filteredStocks.value : stocks.value;
    });

    onMounted(() => {
      selectedCompany.value = "";
      store.dispatch("fetchCompanies");
      fetchStocks();
    });

    const fetchStocks = () => {
      store.dispatch("fetchStocks", selectedCompany.value);
    };

    const viewStock = (id) => {
      router.push({ name: "stock-details", params: { id } });
    };

    const editStock = (id) => {
      router.push({ name: "stock-edit", params: { id } });
    };

    const deleteStock = async (stockId) => {
      if (confirm("Stok kaydını silmek istediğinizden emin misiniz?")) {
        await store.dispatch("deleteStock", stockId);
        alert("Stok kaydı başarıyla silindi.");
        fetchStocks();
      }
    };

    return {
      displayedStocks,
      companies,
      selectedCompany,
      fetchStocks,
      errorMessage,
      viewStock,
      editStock,
      deleteStock,
    };
  },
};
</script>

<style scoped>
.stock-list-container {
  padding: 20px;
}
h1 {
  color: #ff69b4;
  font-weight: bold;
}
table {
  width: 100%;
  border-collapse: collapse;
}
th,
td {
  padding: 10px;
  border: 1px solid #ddd;
}
th {
  background-color: #333;
  color: white;
}
.stock-data {
  color: #cc5500;
  font-weight: bold;
}
select {
  margin-bottom: 20px;
  padding: 10px;
  font-size: 1rem;
}
.error {
  color: red;
  margin-top: 1rem;
}
.btn {
  padding: 8px 16px;
  margin: 4px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.btn-info {
  background-color: #17a2b8;
  color: white;
}
.btn-danger {
  background-color: #dc3545;
  color: white;
}
.btn-success {
  background-color: #28a745;
  color: white;
}
.btn:hover {
  opacity: 0.9;
}
p {
  color: red;
  font-weight: bold;
}
</style>
