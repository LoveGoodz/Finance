import { createRouter, createWebHistory } from "vue-router";
import InvoiceList from "../views/InvoiceList.vue";
import InvoiceDetails from "../views/InvoiceDetails.vue";
import InvoiceCreate from "../views/InvoiceCreate.vue";
import InvoiceEdit from "../views/InvoiceEdit.vue";
import CustomerCreate from "../views/CustomerCreate.vue";
import CustomerList from "../views/CustomerList.vue";
import CustomerEdit from "../views/CustomerEdit.vue";
import CustomerDetails from "../views/CustomerDetails.vue";
import LoginPage from "../views/LoginPage.vue";
import RegisterPage from "../views/RegisterPage.vue";
import CompanyList from "../views/CompanyList.vue";
import CompanyCreate from "../views/CompanyCreate.vue";
import CompanyEdit from "../views/CompanyEdit.vue";
import CompanyDetails from "../views/CompanyDetails.vue";
import StockList from "../views/StockList.vue";
import StockCreate from "../views/StockCreate.vue";
import StockEdit from "../views/StockEdit.vue";
import StockDetails from "../views/StockDetails.vue";
import store from "../store";

const routes = [
  {
    path: "/",
    name: "login-page",
    component: LoginPage,
    meta: { requiresAuth: false },
  },
  {
    path: "/register",
    name: "register-page",
    component: RegisterPage,
    meta: { requiresAuth: false },
  },
  {
    path: "/invoice",
    name: "invoice-list",
    component: InvoiceList,
    meta: { requiresAuth: true },
  },
  {
    path: "/invoice-details/:id", // Rota yolu düzeltildi
    name: "invoice-details",
    component: InvoiceDetails,
    meta: { requiresAuth: true },
  },
  {
    path: "/invoice/create",
    name: "invoice-create",
    component: InvoiceCreate,
    meta: { requiresAuth: true },
  },
  {
    path: "/invoice/edit/:id",
    name: "invoice-edit",
    component: InvoiceEdit,
    meta: { requiresAuth: true },
  },
  {
    path: "/customer",
    name: "customer-list",
    component: CustomerList,
    meta: { requiresAuth: true },
  },
  {
    path: "/customer/create",
    name: "customer-create",
    component: CustomerCreate,
    meta: { requiresAuth: true },
  },
  {
    path: "/customer/edit/:id",
    name: "customer-edit",
    component: CustomerEdit,
    meta: { requiresAuth: true },
  },
  {
    path: "/customer/:id",
    name: "customer-details",
    component: CustomerDetails,
    meta: { requiresAuth: true },
  },
  {
    path: "/company",
    name: "company-list",
    component: CompanyList,
    meta: { requiresAuth: true },
  },
  {
    path: "/company/create",
    name: "company-create",
    component: CompanyCreate,
    meta: { requiresAuth: true },
  },
  {
    path: "/company/edit/:id",
    name: "company-edit",
    component: CompanyEdit,
    meta: { requiresAuth: true },
  },
  {
    path: "/company/:id",
    name: "company-details",
    component: CompanyDetails,
    meta: { requiresAuth: true },
  },
  {
    path: "/stock",
    name: "stock-list",
    component: StockList,
    meta: { requiresAuth: true },
  },
  {
    path: "/stock/create",
    name: "stock-create",
    component: StockCreate,
    meta: { requiresAuth: true },
  },
  {
    path: "/stock/edit/:id",
    name: "stock-edit",
    component: StockEdit,
    meta: { requiresAuth: true },
  },
  {
    path: "/stock/:id",
    name: "stock-details",
    component: StockDetails,
    meta: { requiresAuth: true },
  },
  {
    path: "/login",
    name: "login-page-alias",
    component: LoginPage,
    meta: { requiresAuth: false },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const authRequired = to.matched.some((record) => record.meta.requiresAuth);
  const isAuthenticated = store.getters.isAuthenticated;

  if (authRequired && !isAuthenticated) {
    next({ name: "login-page" });
  } else {
    next();
  }
});

export default router;
