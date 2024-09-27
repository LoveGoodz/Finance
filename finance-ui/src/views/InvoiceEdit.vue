<template>
  <div class="invoice-edit-container">
    <h1>Fatura Düzenle</h1>

    <form @submit.prevent="updateInvoice">
      <div class="form-group">
        <label for="customer">Müşteri:</label>
        <select v-model="invoice.customerID" required>
          <option
            v-for="customer in customers"
            :key="customer.id"
            :value="customer.id"
          >
            {{ customer.name }}
          </option>
        </select>
      </div>

      <div class="form-group">
        <label for="invoiceDate">Fatura Tarihi:</label>
        <input type="datetime-local" v-model="formattedInvoiceDate" required />
      </div>

      <div class="form-group">
        <label for="series">Fatura Seri:</label>
        <input type="text" v-model="invoice.series" required />
      </div>

      <h2>Fatura Detayları</h2>
      <div
        v-for="(detail, index) in invoice.invoiceDetails"
        :key="index"
        class="form-group"
      >
        <label>Ürün ID:</label>
        <input type="number" v-model="detail.stockID" required />

        <label>Miktar:</label>
        <input type="number" v-model="detail.quantity" required />

        <label>Birim Fiyat:</label>
        <input type="number" v-model="detail.unitPrice" required />

        <button
          type="button"
          class="btn btn-danger"
          @click="removeDetail(index)"
        >
          Ürünü Kaldır
        </button>
      </div>

      <button type="button" class="btn btn-secondary" @click="addDetail">
        Yeni Ürün Ekle
      </button>

      <button type="submit" class="btn btn-success">Faturayı Güncelle</button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </div>
</template>

<script>
import axios from "axios";
import { ref, onMounted, computed } from "vue";
import { useRoute } from "vue-router";

export default {
  setup() {
    const invoice = ref({
      invoiceDetails: [],
    });
    const customers = ref([]);
    const errorMessage = ref("");
    const route = useRoute();

    const formattedInvoiceDate = computed({
      get() {
        if (!invoice.value.invoiceDate) return "";
        const date = new Date(invoice.value.invoiceDate);
        return new Date(date.getTime() - date.getTimezoneOffset() * 60000)
          .toISOString()
          .slice(0, 16);
      },
      set(value) {
        invoice.value.invoiceDate = new Date(value).toISOString();
      },
    });

    onMounted(async () => {
      const token = localStorage.getItem("token");
      const id = route.params.id;
      try {
        const invoiceResponse = await axios.get(
          `https://localhost:7093/api/Invoice/${id}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        invoice.value = invoiceResponse.data;

        const customerResponse = await axios.get(
          "https://localhost:7093/api/Customer",
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        customers.value = customerResponse.data;
      } catch (error) {
        console.error("Hata:", error);
        errorMessage.value = "Fatura yüklenemedi.";
      }
    });

    const updateInvoice = async () => {
      const token = localStorage.getItem("token");
      try {
        await axios.put(
          `https://localhost:7093/api/Invoice/${invoice.value.id}`,
          invoice.value,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        alert("Fatura başarıyla güncellendi.");
      } catch (error) {
        console.error("Güncelleme hatası:", error);
        errorMessage.value = "Fatura güncellenirken bir hata oluştu.";
      }
    };

    const addDetail = () => {
      invoice.value.invoiceDetails.push({
        stockID: null,
        quantity: null,
        unitPrice: null,
      });
    };

    const removeDetail = (index) => {
      invoice.value.invoiceDetails.splice(index, 1);
    };

    return {
      invoice,
      customers,
      formattedInvoiceDate,
      updateInvoice,
      addDetail,
      removeDetail,
      errorMessage,
    };
  },
};
</script>

<style scoped>
.invoice-edit-container {
  padding: 20px;
  max-width: 600px;
  margin: 0 auto;
  background-color: #f4f4f4;
  border-radius: 10px;
}

h1 {
  color: #ff69b4;
  font-weight: bold;
  margin-bottom: 20px;
}

h2 {
  margin-top: 20px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

input,
select {
  width: 100%;
  padding: 10px;
  border-radius: 5px;
  border: 1px solid #ccc;
  margin-bottom: 10px;
}

button {
  padding: 10px 20px;
  margin-right: 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.btn-success {
  background-color: #28a745;
  color: white;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
  margin-top: 10px;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
  margin-top: 10px;
}

.error {
  color: red;
  margin-top: 10px;
}
</style>
