<template>
  <div class="invoice-create-container">
    <h1>Fatura Oluştur</h1>

    <form @submit.prevent="createInvoice">
      <div class="form-group">
        <label for="customerID">Müşteri Seç:</label>
        <select
          v-model="invoice.customerID"
          @change="setCompanyIDForCustomer"
          required
        >
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
        <input
          type="date"
          id="invoiceDate"
          v-model="invoice.invoiceDate"
          required
        />
      </div>

      <div class="form-group">
        <label for="series">Fatura Serisi:</label>
        <input type="text" id="series" v-model="invoice.series" />
      </div>

      <h2>Fatura Detayları</h2>
      <div
        v-for="(detail, index) in invoice.invoiceDetails"
        :key="index"
        class="invoice-detail"
      >
        <h3>Ürün {{ index + 1 }}</h3>

        <div class="form-group">
          <label for="stockID">Ürün (Stok) ID:</label>
          <input type="number" v-model="detail.stockID" required />
        </div>

        <div class="form-group">
          <label for="quantity">Miktar:</label>
          <input type="number" v-model="detail.quantity" required />
        </div>

        <div class="form-group">
          <label for="unitPrice">Birim Fiyat:</label>
          <input type="number" v-model="detail.unitPrice" required />
        </div>

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

      <div class="form-group">
        <label for="totalAmount">Toplam Tutar:</label>
        <input
          type="number"
          id="totalAmount"
          v-model="invoice.totalAmount"
          required
        />
      </div>

      <button type="submit" class="btn btn-primary">Fatura Oluştur</button>
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
        companyID: null,
        invoiceDate: new Date().toISOString().slice(0, 10),
        series: "",
        status: "Taslak",
        totalAmount: 0,
        invoiceDetails: [
          {
            stockID: null,
            quantity: null,
            unitPrice: null,
          },
        ],
      },
      customers: [],
      successMessage: "",
      errorMessage: "",
    };
  },
  methods: {
    addDetail() {
      this.invoice.invoiceDetails.push({
        stockID: null,
        quantity: null,
        unitPrice: null,
      });
    },
    removeDetail(index) {
      this.invoice.invoiceDetails.splice(index, 1);
    },

    setCompanyIDForCustomer() {
      const selectedCustomer = this.customers.find(
        (customer) => customer.id === this.invoice.customerID
      );
      if (selectedCustomer) {
        this.invoice.companyID = selectedCustomer.companyID;
      }
    },
    async createInvoice() {
      try {
        const token = localStorage.getItem("token");
        if (!token) {
          this.errorMessage = "Yetkilendirme hatası.";
          return;
        }

        const response = await axios.post(
          "https://localhost:7093/api/Invoice",
          this.invoice,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        this.successMessage = `Fatura başarıyla oluşturuldu! Fatura ID: ${response.data.id}, Tutar: ${response.data.totalAmount}`;
        this.errorMessage = "";
      } catch (error) {
        console.error("Fatura oluşturulurken hata oluştu:", error);
        this.errorMessage =
          "Fatura oluşturulurken hata oluştu: " + error.message;
        this.successMessage = "";
      }
    },
    async fetchCustomers() {
      const token = localStorage.getItem("token");
      const response = await axios.get("https://localhost:7093/api/Customer", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      this.customers = response.data;
    },
  },
  mounted() {
    this.fetchCustomers();
  },
};
</script>

<style scoped>
.invoice-create-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  background-color: #f4f4f4;
  border-radius: 10px;
}

h1 {
  color: #ff69b4;
  font-family: "Montserrat", sans-serif;
}

form {
  background-color: #fff;
  padding: 20px;
  border-radius: 8px;
}

.form-group {
  margin-bottom: 1rem;
}

input,
select {
  padding: 10px;
  width: 100%;
  margin-top: 5px;
  border: 1px solid #ccc;
  border-radius: 5px;
}

button {
  background-color: #ff4500;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  margin-top: 10px;
}

button:hover {
  background-color: #ff6347;
}

.create-invoice-btn {
  display: block;
  margin-left: 0;
}

.error {
  color: red;
  font-size: 0.9rem;
}

.success {
  color: green;
  font-size: 0.9rem;
}
</style>
