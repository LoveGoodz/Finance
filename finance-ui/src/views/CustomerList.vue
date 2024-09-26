<template>
  <div class="customer-list-container">
    <h1>Müşteri Listesi</h1>
    <div>
      <label for="companySelect">Şirket Seç:</label>
      <select
        id="companySelect"
        v-model="selectedCompany"
        @change="fetchCustomers"
      >
        <option value="">Tüm Müşteriler</option>
        <option
          v-for="company in companies"
          :key="company.id"
          :value="company.id"
        >
          {{ company.name }}
        </option>
      </select>
    </div>

    <!-- Eğer hata mesajı varsa göster, müşteri listesi yoksa hata gösteriyoruz -->
    <p v-if="customers.length === 0 && errorMessage" class="error">
      {{ errorMessage }}
    </p>

    <!-- Eğer müşteri listesi varsa tabloyu göster -->
    <table v-if="customers.length > 0">
      <thead>
        <tr>
          <th>ID</th>
          <th>Müşteri Adı</th>
          <th>E-posta</th>
          <th>Telefon Numarası</th>
          <th>Adres</th>
          <th>İşlemler</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="customer in customers" :key="customer.id">
          <td class="customer-data">{{ customer.id }}</td>
          <td class="customer-data">{{ customer.name }}</td>
          <td class="customer-data">{{ customer.email }}</td>
          <td class="customer-data">{{ customer.phoneNumber }}</td>
          <td class="customer-data">{{ customer.address }}</td>
          <td>
            <button @click="viewCustomer(customer.id)" class="btn btn-info">
              Detay
            </button>
            <button @click="deleteCustomer(customer.id)" class="btn btn-danger">
              Sil
            </button>
            <button @click="editCustomer(customer.id)" class="btn btn-success">
              Düzenle
            </button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Eğer müşteri listesi boşsa ve bir hata mesajı yoksa gösterilecek mesaj -->
    <p v-if="customers.length === 0 && !errorMessage">
      Listelenecek müşteri bulunamadı.
    </p>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { ref, computed, onMounted } from "vue";

export default {
  setup() {
    const store = useStore();
    const customers = computed(() => store.getters.getCustomers);
    const companies = computed(() => store.getters.getCompanies);
    const selectedCompany = ref("");
    const errorMessage = computed(() => store.getters.getError);

    onMounted(() => {
      store.dispatch("fetchCompanies");
      fetchCustomers();
    });

    const fetchCustomers = () => {
      store.dispatch("fetchCustomers", selectedCompany.value);
    };

    const viewCustomer = (id) => {
      window.location.href = `/customer/${id}`;
    };

    const deleteCustomer = async (id) => {
      await store.dispatch("deleteCustomer", id);
    };

    const editCustomer = (id) => {
      window.location.href = `/customer/edit/${id}`;
    };

    return {
      customers,
      companies,
      selectedCompany,
      fetchCustomers,
      viewCustomer,
      deleteCustomer,
      editCustomer,
      errorMessage,
    };
  },
};
</script>

<style scoped>
.customer-list-container {
  padding: 20px;
}
h1 {
  color: #ff69b4;
  font-weight: bold;
  margin-bottom: 1rem;
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
.customer-data {
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
</style>
