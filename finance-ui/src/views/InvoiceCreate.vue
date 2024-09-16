<template>
  <div>
    <h1>Fatura Oluştur</h1>

    <div v-if="loading">Fatura oluşturuluyor, lütfen bekleyin...</div>

    <form @submit.prevent="createInvoice" v-if="!loading">
      <div>
        <label for="customer">Müşteri Adı:</label>
        <input type="text" id="customer" v-model="invoice.customer" required />

        <p v-if="!invoice.customer && formSubmitted" class="error">
          Müşteri adı zorunludur.
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
        v-for="(item, index) in invoice.items"
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
import { ref } from "vue";
import axios from "axios";
import { useRouter } from "vue-router";

export default {
  setup() {
    const invoice = ref({
      customer: "",
      status: "Taslak",
      items: [
        {
          name: "",
          quantity: 1,
          price: 0,
        },
      ],
    });

    const loading = ref(false);
    const formSubmitted = ref(false);
    const router = useRouter();

    const addItem = () => {
      invoice.value.items.push({
        name: "",
        quantity: 1,
        price: 0,
      });
    };

    const removeItem = (index) => {
      invoice.value.items.splice(index, 1);
    };

    const createInvoice = async () => {
      formSubmitted.value = true;

      if (!invoice.value.customer) {
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
