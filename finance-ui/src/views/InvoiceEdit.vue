<template>
  <div>
    <h1>Fatura Düzenle</h1>

    <form @submit.prevent="updateInvoice">
      <div class="form-group">
        <label for="customer">Müşteri:</label>
        <select v-model="invoice.customerId" required>
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
        <label for="totalAmount">Toplam Tutar:</label>
        <input type="number" v-model="invoice.totalAmount" required />
      </div>

      <div class="form-group">
        <label for="status">Durum:</label>
        <select v-model="invoice.status" required>
          <option value="Taslak">Taslak</option>
          <option value="Onaylandı">Onaylandı</option>
          <option value="İptal Edildi">İptal Edildi</option>
        </select>
      </div>

      <button type="submit" class="btn btn-success">Faturayı Güncelle</button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </div>
</template>

<script>
import axios from "axios";
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";

export default {
  setup() {
    const invoice = ref({});
    const customers = ref([]);
    const errorMessage = ref("");
    const route = useRoute();

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

    return { invoice, customers, updateInvoice, errorMessage };
  },
};
</script>

<style scoped>
.error {
  color: red;
  margin-top: 10px;
}
</style>
