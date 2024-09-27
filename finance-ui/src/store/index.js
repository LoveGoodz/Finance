import { createStore } from "vuex";
import axios from "axios";

// Axios yapılandırması
axios.defaults.baseURL = "https://localhost:7093";

const token = localStorage.getItem("token");
if (token) {
  axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
}

// Vuex Store oluşturma
const store = createStore({
  state: {
    user: null,
    token: token || "",
    companies: [],
    customers: [],
    invoices: [],
    stocks: [],
    filteredStocks: [],
    selectedStock: {},
    customerBalances: [],
    companyBalances: [],
    error: null,
    loading: false,
  },
  mutations: {
    SET_USER(state, user) {
      state.user = user;
    },
    SET_TOKEN(state, token) {
      state.token = token;
      axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
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
    SET_CUSTOMER_BALANCES(state, balances) {
      state.customerBalances = balances;
    },
    SET_COMPANY_BALANCES(state, balances) {
      state.companyBalances = balances;
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
      delete axios.defaults.headers.common["Authorization"];
    },
    REMOVE_INVOICE(state, id) {
      state.invoices = state.invoices.filter((invoice) => invoice.id !== id);
    },
    ADD_INVOICE(state, invoice) {
      state.invoices.push(invoice);
    },
    UPDATE_INVOICE(state, updatedInvoice) {
      const index = state.invoices.findIndex(
        (invoice) => invoice.id === updatedInvoice.id
      );
      if (index !== -1) {
        state.invoices.splice(index, 1, updatedInvoice);
      }
    },
  },
  actions: {
    async login({ commit }, credentials) {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);
      try {
        const response = await axios.post("/api/Auth/login", credentials);
        const token = response.data.Token || response.data.token;
        if (token) {
          localStorage.setItem("token", token);
          commit("SET_TOKEN", token);
          commit("SET_USER", response.data.user);
          return true;
        } else {
          throw new Error("Token alınamadı.");
        }
      } catch (error) {
        commit("SET_ERROR", "Giriş hatası: " + error.message);
        return false;
      } finally {
        commit("SET_LOADING", false);
      }
    },

    // Şirket işlemleri
    async fetchCompanies({ commit }) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.get("/api/company");
        commit("SET_COMPANIES", response.data);
      } catch (error) {
        commit("SET_ERROR", "Şirket verileri alınamadı.");
      } finally {
        commit("SET_LOADING", false);
      }
    },

    // Müşteri işlemleri
    async fetchCustomers({ commit }, companyId = "") {
      commit("SET_LOADING", true);
      try {
        let url = "/api/customer";
        if (companyId) {
          url += `?companyId=${companyId}`;
        }
        const response = await axios.get(url);
        commit("SET_CUSTOMERS", response.data);
      } catch (error) {
        commit("SET_ERROR", "Müşteri verileri alınamadı.");
      } finally {
        commit("SET_LOADING", false);
      }
    },

    // Fatura işlemleri
    async fetchInvoices({ commit }) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.get("/api/invoice");
        commit("SET_INVOICES", response.data.data || []);
      } catch (error) {
        commit("SET_ERROR", "Fatura verileri alınamadı.");
      } finally {
        commit("SET_LOADING", false);
      }
    },

    async deleteInvoice({ commit }, id) {
      commit("SET_LOADING", true);
      try {
        await axios.delete(`/api/invoice/${id}`);
        commit("REMOVE_INVOICE", id);
      } catch (error) {
        commit("SET_ERROR", "Fatura silinemedi.");
      } finally {
        commit("SET_LOADING", false);
      }
    },

    async editInvoice({ commit }, updatedInvoice) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.put(
          `/api/invoice/${updatedInvoice.id}`,
          updatedInvoice
        );
        commit("UPDATE_INVOICE", response.data);
      } catch (error) {
        commit("SET_ERROR", "Fatura güncellenemedi.");
      } finally {
        commit("SET_LOADING", false);
      }
    },

    // Stok işlemleri
    async fetchStocks({ commit }, companyId = "") {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);
      try {
        let url = "/api/stock";
        if (companyId) {
          url += `?companyId=${companyId}`;
        }
        const response = await axios.get(url);
        commit("SET_STOCKS", response.data || []);
        commit("SET_FILTERED_STOCKS", companyId ? response.data : []);
      } catch (error) {
        commit("SET_ERROR", "Stok verileri alınamadı.");
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async fetchStockById({ commit }, stockId) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.get(`/api/stock/${stockId}`);
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
        await axios.put(`/api/stock/${stock.id}`, stock);
      } catch (error) {
        commit("SET_ERROR", "Stok güncellenemedi.");
        throw error;
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async addStock({ commit, state }, stock) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.post("/api/stock", stock);
        commit("SET_STOCKS", [...state.stocks, response.data]);
      } catch (error) {
        commit("SET_ERROR", "Stok kaydı eklenemedi.");
      } finally {
        commit("SET_LOADING", false);
      }
    },

    // Bakiye işlemleri
    async fetchCompanyBalances({ commit }) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.get("/api/balance?type=company");
        commit("SET_COMPANY_BALANCES", response.data.data || []);
      } catch (error) {
        commit("SET_ERROR", "Şirket bakiyesi verileri alınamadı.");
      } finally {
        commit("SET_LOADING", false);
      }
    },
    async fetchCustomerBalances({ commit }) {
      commit("SET_LOADING", true);
      try {
        const response = await axios.get("/api/balance?type=customer");
        commit("SET_CUSTOMER_BALANCES", response.data.data || []);
      } catch (error) {
        commit("SET_ERROR", "Müşteri bakiyesi verileri alınamadı.");
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
    getToken: (state) => state.token,
    getCompanies: (state) => state.companies,
    getCustomers: (state) => state.customers,
    getInvoices: (state) => state.invoices,
    getStocks: (state) => state.stocks,
    getFilteredStocks: (state) =>
      state.filteredStocks.length > 0 || state.error
        ? state.filteredStocks
        : state.stocks,
    getSelectedStock: (state) => state.selectedStock,
    getCustomerBalances: (state) => state.customerBalances,
    getCompanyBalances: (state) => state.companyBalances,
    getError: (state) => state.error,
    isAuthenticated: (state) => !!state.token,
    isLoading: (state) => state.loading,
  },
});

export default store;
