<template>
  <div>
    <h1>Fatura Detayları</h1>

    <div v-if="invoice">
      <h2>Müşteri: {{ invoice.customer }}</h2>
      <p>
        Fatura Durumu:
        <span :class="statusClass(invoice.status)">
          {{ invoice.status }}
        </span>
      </p>
      <p>Toplam Tutar: {{ invoice.totalAmount }} TL</p>

      <h3>Ürünler</h3>
      <DataTable :value="invoice.items" class="custom-table">
        <Column
          field="name"
          header="Ürün Adı"
          :headerStyle="{
            fontWeight: 'bold',
            backgroundColor: '#a9a9a9',
            color: '#2c2c2c',
          }"
        ></Column>
        <Column
          field="quantity"
          header="Adet"
          :headerStyle="{
            fontWeight: 'bold',
            backgroundColor: '#a9a9a9',
            color: '#2c2c2c',
          }"
        ></Column>
        <Column
          field="price"
          header="Birim Fiyat"
          :headerStyle="{
            fontWeight: 'bold',
            backgroundColor: '#a9a9a9',
            color: '#2c2c2c',
          }"
        ></Column>
        <Column
          field="total"
          header="Toplam Fiyat"
          :headerStyle="{
            fontWeight: 'bold',
            backgroundColor: '#a9a9a9',
            color: '#2c2c2c',
          }"
        ></Column>
      </DataTable>
    </div>

    <p v-if="!invoice">Fatura bulunamadı.</p>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import axios from "axios";

export default {
  components: {
    DataTable,
    Column,
  },
  setup() {
    const route = useRoute();
    const invoiceId = route.params.id;
    const invoice = ref(null);

    onMounted(async () => {
      const token = localStorage.getItem("token");

      if (!token) {
        alert("Yetkilendirme hatası: Token bulunamadı.");
        return;
      }

      try {
        const response = await axios.get(
          `https://localhost:7093/api/Invoice/${invoiceId}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        invoice.value = response.data;
      } catch (error) {
        console.error("Fatura detayları yüklenirken hata oluştu:", error);
        alert("Fatura detayları yüklenirken bir hata oluştu.");
      }
    });

    const statusClass = (status) => {
      if (status === "Onaylandı") return "approved-status";
      if (status === "Taslak") return "draft-status";
      return "cancel-status";
    };

    return { invoice, statusClass };
  },
};
</script>

<style scoped>
h1 {
  color: #f40002;
  font-family: "Montserrat", sans-serif;
}

.approved-status {
  color: green;
  font-weight: bold;
}

.draft-status {
  color: #ffcc00;
  font-weight: bold;
}

.cancel-status {
  color: red;
  font-weight: bold;
}

.p-datatable-thead > tr > th {
  background-color: #a9a9a9 !important;
  color: #2c2c2c;
  font-weight: bold;
}

.p-datatable-tbody > tr > td {
  background-color: #dcdcdc !important;
  border: 1px solid #b0b0b0;
}
</style>
