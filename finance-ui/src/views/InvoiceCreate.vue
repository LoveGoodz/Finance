<template>
  <div>
    <h1>Fatura Oluştur</h1>

    <div v-if="loading">Fatura oluşturuluyor, lütfen bekleyin...</div>

    <form @submit.prevent="createInvoice" v-if="!loading">
      <div>
        <label for="customerID">Müşteri Seçin:</label>
        <select id="customerID" v-model="invoice.customerID" required>
          <option
            v-for="customer in customers"
            :key="customer.id"
            :value="customer.id"
          >
            {{ customer.name }}
          </option>
        </select>

        <p v-if="!invoice.customerID && formSubmitted" class="error">
          Müşteri seçimi zorunludur.
        </p>
      </div>

      <div>
        <label for="status">Fatura Durumu:</label>
        <select id="status" v-model="invoice.status" required>
          <option value="Taslak">Taslak</option>
          <option value="Onaylandı">Onaylandı</option>
        </select>
      </div>

      <h3>Ürünler</h3>
      <div
        v-for="(item, index) in invoice.invoiceDetails"
        :key="index"
        class="product-item"
      >
        <input
          type="text"
          v-model="item.name"
          placeholder="Ürün Adı"
          required
        />
        <input
          type="number"
          v-model="item.quantity"
          placeholder="Adet"
          required
        />
        <input
          type="number"
          v-model="item.price"
          placeholder="Birim Fiyat"
          required
        />
        <button type="button" @click="removeItem(index)">Ürünü Kaldır</button>
      </div>

      <button type="button" @click="addItem">Ürün Ekle</button>
      <button type="submit">Fatura Oluştur</button>
    </form>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRouter } from "vue-router";

export default {
  setup() {
    const invoice = ref({
      customerID: null,
      status: "Taslak",
      invoiceDetails: [
        {
          name: "",
          quantity: 1,
          price: 0,
        },
      ],
    });

    const loading = ref(false);
    const formSubmitted = ref(false);
    const customers = ref([]);
    const router = useRouter();

    onMounted(async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await axios.get(
          "https://localhost:7093/api/Customer",
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        customers.value = response.data;
      } catch (error) {
        console.error("Müşteriler yüklenirken hata oluştu:", error);
      }
    });

    const addItem = () => {
      invoice.value.invoiceDetails.push({
        name: "",
        quantity: 1,
        price: 0,
      });
    };

    const removeItem = (index) => {
      invoice.value.invoiceDetails.splice(index, 1);
    };

    const createInvoice = async () => {
      formSubmitted.value = true;

      if (!invoice.value.customerID) {
        return;
      }

      loading.value = true;
      try {
        const token = localStorage.getItem("token");
        if (!token) {
          alert("Yetkilendirme hatası: Lütfen tekrar giriş yapın.");
          return;
        }

        await axios.post("https://localhost:7093/api/Invoice", invoice.value, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        alert("Fatura başarıyla oluşturuldu!");

        router.push("/invoice");
      } catch (error) {
        console.error("Fatura oluşturulurken hata oluştu:", error);
        alert("Fatura oluşturulamadı!");
      } finally {
        loading.value = false;
      }
    };

    return {
      invoice,
      loading,
      formSubmitted,
      addItem,
      removeItem,
      customers,
      createInvoice,
    };
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
