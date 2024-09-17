<template>
  <div>
    <h1>Fatura Oluştur</h1>

    <form @submit.prevent="createInvoice">
      <div>
        <label for="customerID">Müşteri ID:</label>
        <input
          type="number"
          id="customerID"
          v-model="invoice.customerID"
          required
        />
      </div>

      <div>
        <label for="companyID">Şirket ID:</label>
        <input
          type="number"
          id="companyID"
          v-model="invoice.companyID"
          required
        />
      </div>

      <div>
        <label for="invoiceDate">Fatura Tarihi:</label>
        <input
          type="date"
          id="invoiceDate"
          v-model="invoice.invoiceDate"
          required
        />
      </div>

      <div>
        <label for="totalAmount">Toplam Tutar:</label>
        <input
          type="number"
          id="totalAmount"
          v-model="invoice.totalAmount"
          required
        />
      </div>

      <button type="submit">Fatura Oluştur</button>

      <p v-if="successMessage" class="success">{{ successMessage }}</p>
      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    </form>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      invoice: {
        customerID: null,
        companyID: 1, // Geçici Şirket ID
        invoiceDate: new Date().toISOString().slice(0, 10), // Bugünün tarihi
        totalAmount: 0,
      },
      successMessage: "",
      errorMessage: "",
    };
  },
  methods: {
    async createInvoice() {
      try {
        const token = localStorage.getItem("token");

        if (!token) {
          this.errorMessage = "Yetkilendirme hatası.";
          return;
        }

        // response'u kullanarak işlem yapalım
        const response = await axios.post(
          "https://localhost:7093/api/Invoice",
          this.invoice,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        // Gelen response içindeki bilgileri kullan
        this.successMessage = `Fatura başarıyla oluşturuldu! Fatura ID: ${response.data.id}, Tutar: ${response.data.totalAmount}`;
        this.errorMessage = ""; // Başarı olduğunda hata mesajını temizle
      } catch (error) {
        console.error("Fatura oluşturulurken hata oluştu:", error);
        this.errorMessage =
          "Fatura oluşturulurken hata oluştu: " + error.message;
        this.successMessage = "";
      }
    },
  },
};
</script>

<style scoped>
h1 {
  color: #ff69b4;
  font-family: "Montserrat", sans-serif;
}

form {
  background-color: #f0f0f0;
  padding: 20px;
  border-radius: 8px;
}

.error {
  color: red;
  font-size: 0.9rem;
}

div {
  margin-bottom: 1rem;
}

input,
select {
  padding: 10px;
  margin: 5px 0;
}

button {
  background-color: #ff4500;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #ff6347;
}
</style>
