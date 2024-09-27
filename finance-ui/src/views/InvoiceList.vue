<template>
  <div class="invoice-list-container">
    <h1>Fatura Listesi</h1>

    <div>
      <label for="companySelect">Şirket Seç:</label>
      <select
        id="companySelect"
        v-model="selectedCompany"
        @change="filterInvoices"
      >
        <option value="">Tüm Şirketler</option>
        <option
          v-for="company in companies"
          :key="company.id"
          :value="company.id"
        >
          {{ company.name }}
        </option>
      </select>
    </div>

    <div>
      <label for="statusSelect">Durum Seç:</label>
      <select
        id="statusSelect"
        v-model="selectedStatus"
        @change="filterInvoices"
      >
        <option value="">Tüm Durumlar</option>
        <option value="Taslak">Taslak</option>
        <option value="Onaylandı">Onaylandı</option>
        <option value="Reddedildi">Reddedildi</option>
      </select>
    </div>

    <table v-if="filteredInvoices.length > 0">
      <thead>
        <tr>
          <th>ID</th>
          <th>Fatura Tarihi</th>
          <th>Fatura Serisi</th>
          <th>Müşteri</th>
          <th>Toplam</th>
          <th>Durum</th>
          <th>İşlemler</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="invoice in filteredInvoices" :key="invoice.id">
          <td>{{ invoice.id }}</td>
          <td>{{ formatDate(invoice.invoiceDate) }}</td>
          <td>{{ invoice.series }}</td>
          <td>{{ invoice.customerName || "Müşteri bilgisi yok" }}</td>
          <td>{{ invoice.totalAmount }}</td>
          <td :class="statusClass(invoice.status)">{{ invoice.status }}</td>
          <td>
            <button @click="viewInvoice(invoice.id)" class="btn btn-info">
              Detay
            </button>
            <button @click="deleteInvoice(invoice.id)" class="btn btn-danger">
              Sil
            </button>
            <button @click="editInvoice(invoice.id)" class="btn btn-success">
              Düzenle
            </button>
          </td>
        </tr>
      </tbody>
    </table>

    <p v-if="filteredInvoices.length === 0">Listelenecek fatura bulunamadı.</p>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { ref, computed, onMounted, watch } from "vue";
import { useRouter } from "vue-router";

export default {
  setup() {
    const store = useStore();
    const router = useRouter();
    const invoices = computed(() => store.getters.getInvoices);
    const companies = computed(() => store.getters.getCompanies);

    const selectedCompany = ref("");
    const selectedStatus = ref("");
    const filteredInvoices = ref([]);

    watch([selectedCompany, selectedStatus, invoices], filterInvoices, {
      immediate: true,
    });

    onMounted(() => {
      store.dispatch("fetchInvoices");
      store.dispatch("fetchCompanies");
    });

    const viewInvoice = (id) => {
      router.push({ name: "invoice-details", params: { id } });
    };

    const editInvoice = (id) => {
      router.push({ name: "invoice-edit", params: { id } });
    };

    const deleteInvoice = async (id) => {
      if (confirm("Faturayı silmek istediğinizden emin misiniz?")) {
        await store.dispatch("deleteInvoice", id);
        alert("Fatura başarıyla silindi.");
        filterInvoices();
      }
    };

    const formatDate = (dateString) => {
      const date = new Date(dateString);
      const day = String(date.getDate()).padStart(2, "0");
      const month = String(date.getMonth() + 1).padStart(2, "0");
      const year = date.getFullYear();
      const hours = String(date.getHours()).padStart(2, "0");
      const minutes = String(date.getMinutes()).padStart(2, "0");
      const seconds = String(date.getSeconds()).padStart(2, "0");
      return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
    };

    function filterInvoices() {
      filteredInvoices.value = invoices.value.filter((invoice) => {
        const matchesCompany = selectedCompany.value
          ? invoice.companyID === parseInt(selectedCompany.value)
          : true;
        const matchesStatus = selectedStatus.value
          ? invoice.status === selectedStatus.value
          : true;
        return matchesCompany && matchesStatus;
      });
    }

    const statusClass = (status) => {
      if (status === "Onaylandı") return "status-approved";
      if (status === "Taslak") return "status-draft";
      return "status-rejected";
    };

    return {
      invoices,
      companies,
      selectedCompany,
      selectedStatus,
      filteredInvoices,
      viewInvoice,
      editInvoice,
      deleteInvoice,
      formatDate,
      filterInvoices,
      statusClass,
    };
  },
};
</script>

<style scoped>
.invoice-list-container {
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
td {
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
.status-draft {
  color: #ffcc00;
}
.status-approved {
  color: green;
}
.status-rejected {
  color: red;
}
</style>
