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
import { ref } from "vue";
import DataTable from "primevue/datatable";
import Column from "primevue/column";

export default {
  components: {
    DataTable,
    Column,
  },
  setup() {
    const invoice = ref(null);

    // Test amaçlı fatura detayları
    const testInvoices = [
      {
        id: 1,
        customer: "Müşteri A",
        status: "Onaylandı",
        totalAmount: 1500,
        items: [
          { name: "Ürün 1", quantity: 2, price: 300, total: 600 },
          { name: "Ürün 2", quantity: 1, price: 900, total: 900 },
        ],
      },
      {
        id: 2,
        customer: "Müşteri B",
        status: "Taslak",
        totalAmount: 2300,
        items: [{ name: "Ürün 3", quantity: 1, price: 2300, total: 2300 }],
      },
    ];

    // Manuel olarak test verilerini kullan
    const loadInvoice = (id) => {
      const foundInvoice = testInvoices.find((inv) => inv.id === id);
      invoice.value = foundInvoice || null;
    };

    // Test amacıyla ID 1 olan faturayı yükle
    loadInvoice(1);

    // Durum sınıfını belirlemek için fonksiyon
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
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

h2 {
  font-family: "Montserrat", sans-serif;
  font-size: 1.8rem;
  margin-top: 1rem;
  color: #2c3e50;
}

.approved-status {
  color: green;
  font-weight: bold;
}

.draft-status {
  color: yellow;
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
  font-family: "Montserrat", sans-serif;
  font-size: 1.2rem;
}

.p-datatable-tbody > tr > td {
  background-color: #dcdcdc !important;
  border: 1px solid #b0b0b0;
}

.p-datatable.custom-table {
  border-radius: 10px;
  padding: 20px;
}

p {
  font-family: "Roboto", sans-serif;
  font-size: 1.2rem;
  color: #2c2c2c;
}
</style>
