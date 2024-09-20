<template>
  <div class="company-list-container">
    <h1>Şirket Listesi</h1>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Şirket Adı</th>
          <th>E-posta</th>
          <th>Telefon Numarası</th>
          <th>Adres</th>
          <th>İşlemler</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="company in companies" :key="company.id">
          <td class="company-data">{{ company.id }}</td>
          <td class="company-data">{{ company.name }}</td>
          <td class="company-data">{{ company.email }}</td>
          <td class="company-data">{{ company.phoneNumber }}</td>
          <td class="company-data">{{ company.address }}</td>
          <td>
            <button @click="viewDetails(company.id)" class="btn btn-info">
              Detay
            </button>
            <button @click="deleteCompany(company.id)" class="btn btn-danger">
              Sil
            </button>
            <button @click="editCompany(company.id)" class="btn btn-success">
              Düzenle
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from "axios";
import { ref, onMounted } from "vue";

export default {
  setup() {
    const companies = ref([]);

    onMounted(async () => {
      const token = localStorage.getItem("token");
      const response = await axios.get("https://localhost:7093/api/company", {
        headers: { Authorization: `Bearer ${token}` },
      });
      companies.value = response.data;
    });

    const viewDetails = (id) => {
      window.location.href = `/company/${id}`;
    };

    const deleteCompany = async (id) => {
      const token = localStorage.getItem("token");
      await axios.delete(`https://localhost:7093/api/company/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      companies.value = companies.value.filter((c) => c.id !== id);
    };

    const editCompany = (id) => {
      window.location.href = `/company/edit/${id}`;
    };

    return {
      companies,
      viewDetails,
      deleteCompany,
      editCompany,
    };
  },
};
</script>

<style scoped>
.company-list-container {
  padding: 20px;
}

h1 {
  color: #ff69b4; /* Başlık rengi */
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
  color: white; /* Başlıkların beyaz rengi */
}

.company-data {
  color: #cc5500; /* Koyu turuncu yazı rengi */
  font-weight: bold;
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
