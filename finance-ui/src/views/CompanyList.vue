<template>
  <div>
    <h1>Şirket Listesi</h1>

    <p v-if="loading">Şirket listesi yükleniyor...</p>

    <DataTable
      :value="companies"
      v-if="companies && companies.length > 0 && !loading"
    >
      <Column field="id" header="ID"></Column>
      <Column field="name" header="Şirket Adı"></Column>
      <Column field="email" header="E-posta"></Column>
      <Column field="phoneNumber" header="Telefon Numarası"></Column>
      <Column field="address" header="Adres"></Column>
      <Column
        header="İşlemler"
        :headerStyle="{ textAlign: 'center' }"
        :bodyStyle="{ textAlign: 'left' }"
      >
        <template #body="slotProps">
          <div class="actions">
            <Button
              label="Detay"
              class="action-button"
              @click="viewCompany(slotProps.data.id)"
            />
            <Button
              label="Sil"
              class="action-button p-button-danger"
              @click="deleteCompany(slotProps.data.id)"
            />
          </div>
        </template>
      </Column>
    </DataTable>

    <p v-if="!loading && companies && companies.length === 0">
      Listelenecek şirket bulunamadı.
    </p>
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
    const companies = ref([]);
    const loading = ref(true);
    const router = useRouter();

    onMounted(async () => {
      const token = localStorage.getItem("token");

      if (!token) {
        alert("Token bulunamadı, lütfen giriş yapın.");
        loading.value = false;
        return;
      }

      try {
        const response = await axios.get("https://localhost:7093/api/company", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        companies.value = response.data;
        console.log("Şirketler yüklendi:", response.data);
      } catch (error) {
        console.error("Şirket listesi yüklenirken hata oluştu:", error);

        if (error.response && error.response.status === 401) {
          alert("Kullanıcı doğrulanamadı, lütfen tekrar giriş yapın.");
        } else {
          alert("Şirket listesi yüklenemedi!");
        }
      } finally {
        loading.value = false;
      }
    });

    const viewCompany = (id) => {
      router.push(`/company/${id}`);
    };

    const deleteCompany = async (id) => {
      if (confirm("Bu şirketi silmek istediğinize emin misiniz?")) {
        try {
          const token = localStorage.getItem("token");

          await axios.delete(`https://localhost:7093/api/company/${id}`, {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          });

          companies.value = companies.value.filter(
            (company) => company.id !== id
          );
          alert("Şirket başarıyla silindi.");
        } catch (error) {
          console.error("Şirket silinirken hata oluştu:", error);
          alert("Şirket silinemedi.");
        }
      }
    };

    return { companies, loading, viewCompany, deleteCompany };
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
  text-align: center;
}

.p-datatable-tbody > tr > td {
  background-color: #e0e0e0;
  text-align: left;
}

.p-button {
  background-color: #fffdd0;
  color: #b87333;
  border: none;
  font-weight: bold;
  width: 90px;
  text-align: center;
}

.p-button:hover {
  background-color: #ffebb5;
}

.p-button-danger {
  background-color: red;
  color: white;
}

.actions {
  display: flex;
  gap: 10px;
  align-items: center;
}
</style>
