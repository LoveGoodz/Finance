<template>
  <div>
    <h1>Fatura Listesi</h1>

    <DataTable :value="invoices" v-if="invoices.length > 0">
      <Column field="id" header="ID"></Column>
      <Column field="customer.name" header="Müşteri"></Column>
      <Column field="totalAmount" header="Toplam"></Column>
      <Column header="Durum">
        <template #body="slotProps">
          <span
            :class="{
              'status-approved': slotProps.data.status === 'Onaylandı',
              'status-draft': slotProps.data.status === 'Taslak',
              'status-canceled':
                slotProps.data.status !== 'Onaylandı' &&
                slotProps.data.status !== 'Taslak',
            }"
          >
            {{ slotProps.data.status }}
          </span>
        </template>
      </Column>
      <Column header="İşlemler">
        <template #body="slotProps">
          <Button label="Detay" @click="viewInvoice(slotProps.data.id)" />
        </template>
      </Column>
    </DataTable>

    <p v-if="invoices.length === 0">Listelenecek fatura bulunamadı.</p>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Button from "primevue/button";
import axios from "axios";
import { useRouter } from "vue-router";

export default {
  components: {
    DataTable,
    Column,
    Button,
  },
  setup() {
    const invoices = ref([]);
    const router = useRouter();

    onMounted(async () => {
      const token = localStorage.getItem("token");
      try {
        const response = await axios.get("https://localhost:7093/api/Invoice", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        invoices.value = response.data.data;
      } catch (error) {
        console.error("Faturalar yüklenirken hata oluştu:", error);
        alert("Fatura yüklenemedi!");
      }
    });

    const viewInvoice = (id) => {
      router.push(`/invoice/${id}`);
    };

    return { invoices, viewInvoice };
  },
};
</script>

<style scoped>
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

.p-datatable-thead > tr > th {
  background-color: #c0c0c0;
  font-weight: bold;
  color: #333;
}

.p-datatable-tbody > tr > td {
  background-color: #e0e0e0;
}

.p-button {
  background-color: #fffdd0;
  color: #b87333;
  border: none;
  font-weight: bold;
}

.p-button:hover {
  background-color: #ffebb5;
}
</style>
