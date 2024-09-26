import { createStore } from "vuex";
import axios from "axios";

axios.defaults.baseURL = "https://localhost:7093";

const store = createStore({
  state: {
    user: null,
    companies: [],
    customers: [],
    invoices: [],
    stocks: [],
    filteredStocks: [],
    selectedStock: {},
    token: localStorage.getItem("token") || "",
    error: null,
    loading: false,
  },
  mutations: {
    SET_USER(state, user) {
      state.user = user;
    },
    SET_COMPANIES(state, companies) {
      state.companies = companies;
    },
    SET_CUSTOMERS(state, customers) {
      state.customers = customers;
    },
    SET_INVOICES(state, invoices) {
      state.invoices = invoices;
    },
    SET_STOCKS(state, stocks) {
      state.stocks = stocks;
    },
    SET_FILTERED_STOCKS(state, stocks) {
      state.filteredStocks = stocks;
    },
    SET_SELECTED_STOCK(state, stock) {
      state.selectedStock = stock;
    },
    SET_ERROR(state, error) {
      state.error = error;
    },
    SET_LOADING(state, status) {
      state.loading = status;
    },
    LOGOUT(state) {
      state.token = "";
      state.user = null;
      localStorage.removeItem("token");
    },
  },
  actions: {
    async login({ commit }, credentials) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.post("/api/Auth/login", credentials);
        const token = response.data.Token || response.data.token;
        if (token) {
          localStorage.setItem("token", token);
          commit("SET_USER", response.data.user);
        } else {
          throw new Error("Token alınamadı.");
        }
      } catch (error) {
        commit("SET_ERROR", "Giriş hatası: " + error.message);
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async fetchCompanies({ commit }) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.get("/api/company", {
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        });
        commit("SET_COMPANIES", response.data);
      } catch (error) {
        commit("SET_ERROR", "Şirket verileri alınamadı.");
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async fetchCustomers({ commit }, companyId = "") {
      commit("SET_LOADING", true);
      try {
        let url = "/api/customer";
        if (companyId) {
          url += `?companyId=${companyId}`;
        }
        const response = await axios.get(url, {
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        });
        commit("SET_CUSTOMERS", response.data);
      } catch (error) {
        if (error.response && error.response.status === 404) {
          commit("SET_CUSTOMERS", []);
          commit("SET_ERROR", "Listelenecek müşteri bulunamadı.");
        } else {
          commit("SET_ERROR", "Müşteri verileri alınamadı.");
        }
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async fetchInvoices({ commit }) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.get("/api/invoice", {
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        });
        commit("SET_INVOICES", response.data);
      } catch (error) {
        commit("SET_ERROR", "Fatura verileri alınamadı.");
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async fetchStocks({ commit }, companyId = "") {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null); // Hata mesajını sıfırla
      try {
        let url = "/api/stock";
        if (companyId) {
          url += `?companyId=${companyId}`;
        }
        const response = await axios.get(url, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        });

        if (companyId) {
          commit("SET_FILTERED_STOCKS", response.data || []);
        } else {
          commit("SET_STOCKS", response.data || []);
          commit("SET_FILTERED_STOCKS", []);
        }
      } catch (error) {
        if (error.response && error.response.status === 404) {
          commit("SET_FILTERED_STOCKS", []); // Şirkete ait stok yoksa boş array
          commit("SET_ERROR", "Listelenecek stok bulunamadı."); // Hata mesajı
        } else {
          commit("SET_ERROR", "Stok verileri alınamadı."); // Genel hata mesajı
        }
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async fetchStockById({ commit }, stockId) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.get(`/api/stock/${stockId}`, {
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        });
        commit("SET_SELECTED_STOCK", response.data);
      } catch (error) {
        commit("SET_ERROR", "Stok bilgisi alınamadı.");
        throw error;
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async updateStock({ commit }, stock) {
      commit("SET_LOADING", true);
      try {
        await axios.put(`/api/stock/${stock.id}`, stock, {
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        });
      } catch (error) {
        commit("SET_ERROR", "Stok güncellenemedi.");
        throw error;
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async addStock({ commit }, stock) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.post("/api/stock", stock, {
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        });
        commit("SET_STOCKS", [...this.state.stocks, response.data]);
      } catch (error) {
        commit("SET_ERROR", "Stok kaydı eklenemedi.");
      } finally {
        commit("SET_LOADING", false);
      }
    },
    logout({ commit }) {
      commit("LOGOUT");
    },
  },
  getters: {
    getUser: (state) => state.user,
    getCompanies: (state) => state.companies,
    getCustomers: (state) => state.customers,
    getInvoices: (state) => state.invoices,
    getStocks: (state) => state.stocks,
    getFilteredStocks: (state) =>
      state.filteredStocks.length > 0 || state.error
        ? state.filteredStocks
        : state.stocks,
    getSelectedStock: (state) => state.selectedStock,
    getError: (state) => state.error,
    isAuthenticated: (state) => !!state.token,
    isLoading: (state) => state.loading,
  },
});

export default store;
