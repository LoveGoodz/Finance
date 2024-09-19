<template>
  <div>
    <h1>Müşteri Listesi</h1>

    <DataTable :value="customers" v-if="customers.length > 0">
      <Column field="id" header="ID"></Column>
      <Column field="name" header="Müşteri Adı"></Column>
      <Column field="email" header="E-posta"></Column>
      <Column field="phoneNumber" header="Telefon Numarası"></Column>
      <Column field="address" header="Adres"></Column>
      <Column header="İşlemler">
        <template #body="slotProps">
          <Button label="Detay" @click="viewCustomer(slotProps.data.id)" />
        </template>
      </Column>
    </DataTable>

    <p v-if="customers.length === 0">Listelenecek müşteri bulunamadı.</p>
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
    const customers = ref([]);
    const router = useRouter();

    onMounted(async () => {
      const token = localStorage.getItem("token");
      try {
        const response = await axios.get(
          "https://localhost:7093/api/Customer",
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        customers.value = response.data.data;
      } catch (error) {
        console.error("Müşteri listesi yüklenirken hata oluştu:", error);
        alert("Müşteri listesi yüklenemedi!");
      }
    });

    const viewCustomer = (id) => {
      router.push(`/customer/${id}`);
    };

    return { customers, viewCustomer };
  },
};
</script>

<style scoped>
h1 {
  color: #ff69b4;
  font-weight: bold;
  margin-bottom: 1rem;
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
