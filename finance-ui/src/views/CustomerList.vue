<template>
  <div class="customer-list-container">
    <h1>Müşteri Listesi</h1>

    <!-- Şirketleri Listeleme Dropdown -->
    <div>
      <label for="companySelect">Şirket Seç:</label>
      <select
        id="companySelect"
        v-model="selectedCompany"
        @change="fetchCustomers"
      >
        <option value="">Tüm Müşteriler</option>
        <!-- Tüm müşteriler için boş seçenek -->
        <option
          v-for="company in companies"
          :key="company.id"
          :value="company.id"
        >
          {{ company.name }}
        </option>
      </select>
    </div>

    <!-- Müşteri Tablosu -->
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

    <!-- Müşteri yoksa gösterilecek mesaj -->
    <p v-if="customers.length === 0">{{ noCustomersMessage }}</p>
  </div>
</template>

<script>
import axios from "axios";
import { ref, onMounted } from "vue";

export default {
  setup() {
    const customers = ref([]);
    const companies = ref([]);
    const selectedCompany = ref(null);
    const noCustomersMessage = ref("");

    // Şirketleri listeleme
    const fetchCompanies = async () => {
      const token = localStorage.getItem("token");
      const response = await axios.get("https://localhost:7093/api/company", {
        headers: { Authorization: `Bearer ${token}` },
      });
      companies.value = response.data;
    };

    // Tüm müşterileri veya seçilen şirkete ait müşterileri getirme
    const fetchCustomers = async () => {
      const token = localStorage.getItem("token");
      try {
        let response;
        if (selectedCompany.value) {
          response = await axios.get(
            `https://localhost:7093/api/customer?companyId=${selectedCompany.value}`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          );
        } else {
          response = await axios.get("https://localhost:7093/api/customer", {
            headers: { Authorization: `Bearer ${token}` },
          });
        }
        customers.value = response.data;
        noCustomersMessage.value = ""; // Hata mesajını sıfırla
      } catch (error) {
        if (error.response && error.response.status === 404) {
          customers.value = [];
          noCustomersMessage.value = "Listelenecek müşteri bulunamadı."; // Hata mesajı ayarla
        } else {
          console.error("Müşteri listesi yüklenirken hata oluştu:", error);
        }
      }
    };

    onMounted(async () => {
      await fetchCompanies();
      await fetchCustomers();
    });

    const viewCustomer = (id) => {
      window.location.href = `/customer/${id}`;
    };

    const deleteCustomer = async (id) => {
      const token = localStorage.getItem("token");
      await axios.delete(`https://localhost:7093/api/customer/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      customers.value = customers.value.filter((c) => c.id !== id);
    };

    const editCustomer = (id) => {
      window.location.href = `/customer/edit/${id}`;
    };

    return {
      customers,
      companies,
      selectedCompany,
      viewCustomer,
      deleteCustomer,
      editCustomer,
      fetchCustomers,
      noCustomersMessage,
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
