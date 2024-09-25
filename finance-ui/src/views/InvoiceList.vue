<template>
  <div class="invoice-list-container">
    <h1>Fatura Listesi</h1>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Müşteri</th>
          <th>Toplam</th>
          <th>Durum</th>
          <th>İşlemler</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="invoice in invoices" :key="invoice.id">
          <td>{{ invoice.id }}</td>
          <td>{{ invoice.customer.name }}</td>
          <td>{{ invoice.totalAmount }}</td>
          <td>{{ invoice.status }}</td>
          <td>
            <button @click="viewInvoice(invoice.id)" class="btn btn-info">
              Detay
            </button>
          </td>
        </tr>
      </tbody>
    </table>
    <p v-if="invoices.length === 0">Listelenecek fatura bulunamadı.</p>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { computed, onMounted } from "vue";

export default {
  setup() {
    const store = useStore();
    const invoices = computed(() => store.getters.getInvoices);

    onMounted(() => {
      store.dispatch("fetchInvoices");
    });

    return {
      invoices,
    };
  },
};
</script>

<style scoped>
.invoice-list-container {
  padding: 20px;
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
  background-color: #f4f4f4;
}

td {
  color: #ff4500;
}

h1 {
  color: #ff69b4;
  font-weight: bold;
  margin-bottom: 1rem;
}

.status-approved {
  color: green;
  font-weight: bold;
}

.status-draft {
  color: #ffd700;
  font-weight: bold;
}

.status-canceled {
  color: red;
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

.btn:hover {
  opacity: 0.9;
}
</style>
