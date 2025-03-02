<template>
  <div>
    <h1>Fatura Detayları</h1>
    <div v-if="invoice">
      <h2>Müşteri: {{ invoice.customerName }}</h2>
      <h3>Şirket: {{ invoice.companyName }}</h3>
      <p>
        Fatura Durumu:
        <span :class="statusClass(invoice.status)">
          {{ invoice.status }}
        </span>
      </p>
      <p>Toplam Tutar: {{ invoice.totalAmount }} TL</p>
      <p>Fatura Tarihi: {{ formattedDate(invoice.invoiceDate) }}</p>

      <h3>Ürünler</h3>
      <DataTable :value="invoice.invoiceDetails" class="custom-table">
        <Column
          field="stockName"
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
          field="unitPrice"
          header="Birim Fiyat"
          :headerStyle="{
            fontWeight: 'bold',
            backgroundColor: '#a9a9a9',
            color: '#2c2c2c',
          }"
        ></Column>
        <Column
          field="totalPrice"
          header="Toplam Fiyat"
          :headerStyle="{
            fontWeight: 'bold',
            backgroundColor: '#a9a9a9',
            color: '#2c2c2c',
          }"
        ></Column>
      </DataTable>

      <div class="actions">
        <button
          @click="updateInvoiceStatus('Onaylandı')"
          class="btn btn-success"
        >
          Onayla
        </button>
        <button
          @click="updateInvoiceStatus('Reddedildi')"
          class="btn btn-danger"
        >
          Reddet
        </button>
      </div>
    </div>

    <p v-else>Fatura bilgisi yükleniyor...</p>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
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
    const router = useRouter();
    const invoiceId = route.params.id;
    const invoice = ref(null);

    const loadInvoice = async () => {
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
    };

    onMounted(() => {
      loadInvoice();
    });

    const statusClass = (status) => {
      if (status === "Onaylandı") return "approved-status";
      if (status === "Taslak") return "draft-status";
      return "cancel-status";
    };

    const formattedDate = (dateString) => {
      const date = new Date(dateString);
      const day = String(date.getDate()).padStart(2, "0");
      const month = String(date.getMonth() + 1).padStart(2, "0");
      const year = date.getFullYear();
      const hours = String(date.getHours()).padStart(2, "0");
      const minutes = String(date.getMinutes()).padStart(2, "0");
      const seconds = String(date.getSeconds()).padStart(2, "0");
      return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
    };

    const updateInvoiceStatus = async (status) => {
      const token = localStorage.getItem("token");
      try {
        await axios.put(
          `https://localhost:7093/api/Invoice/${invoiceId}/status`,
          { status },
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        alert(`Fatura durumu başarıyla ${status} olarak güncellendi.`);

        router.push({ name: "invoice-list" });
      } catch (error) {
        console.error(`Fatura durumu güncellenirken hata oluştu:`, error);
        alert(`Fatura durumu güncellenirken bir hata oluştu.`);
      }
    };

    return { invoice, statusClass, formattedDate, updateInvoiceStatus };
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

.actions {
  margin-top: 20px;
}

.btn {
  padding: 10px 20px;
  margin: 5px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.btn-success {
  background-color: #28a745;
  color: white;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn:hover {
  opacity: 0.9;
}
</style>
