<template>
  <div class="balance-list-container">
    <h1>Şirket Bakiyeleri</h1>
    <table v-if="balances.length > 0">
      <thead>
        <tr>
          <th>ID</th>
          <th>Şirket Adı</th>
          <th>Kredi (Credit)</th>
          <th>Toplam Stok</th>
          <th>Toplam Alacak</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="balance in balances" :key="balance.id">
          <td class="balance-data">{{ balance.companyID }}</td>
          <td class="balance-data">{{ balance.company.name }}</td>
          <td class="balance-data">{{ balance.totalCredit }}</td>
          <td class="balance-data">{{ balance.totalStock }}</td>
          <td class="balance-data">{{ balance.totalDebit }}</td>
        </tr>
      </tbody>
    </table>
    <p v-else>Şirket bakiyesi bulunamadı.</p>
  </div>
</template>

<script>
import { useStore } from "vuex";
import { computed, onMounted } from "vue";

export default {
  setup() {
    const store = useStore();
    const balances = computed(() => store.getters.getCompanyBalances);

    onMounted(() => {
      store.dispatch("fetchCompanyBalances");
    });

    return {
      balances,
    };
  },
};
</script>

<style scoped>
.balance-list-container {
  padding: 20px;
}
h1 {
  color: #ff69b4;
}
table {
  width: 100%;
  border-collapse: collapse;
}
th,
td {
  padding: 10px;
  border: 1px solid #ddd;
}
th {
  background-color: #333;
  color: white;
}
.balance-data {
  color: #cc5500;
  font-weight: bold;
}
</style>
