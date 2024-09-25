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
  },
  {
    path: "/register",
    name: "register-page",
    component: RegisterPage,
  },
  {
    path: "/invoice",
    name: "invoice-list",
    component: InvoiceList,
  },
  {
    path: "/invoice/:id",
    name: "invoice-details",
    component: InvoiceDetails,
  },
  {
    path: "/invoice/create",
    name: "invoice-create",
    component: InvoiceCreate,
  },
  {
    path: "/invoice/edit/:id",
    name: "invoice-edit",
    component: InvoiceEdit,
  },
  {
    path: "/customer",
    name: "customer-list",
    component: CustomerList,
  },
  {
    path: "/customer/create",
    name: "customer-create",
    component: CustomerCreate,
  },
  {
    path: "/customer/edit/:id",
    name: "customer-edit",
    component: CustomerEdit,
  },
  {
    path: "/customer/:id",
    name: "customer-details",
    component: CustomerDetails,
  },
  {
    path: "/company",
    name: "company-list",
    component: CompanyList,
  },
  {
    path: "/company/create",
    name: "company-create",
    component: CompanyCreate,
  },
  {
    path: "/company/edit/:id",
    name: "company-edit",
    component: CompanyEdit,
  },
  {
    path: "/company/:id",
    name: "company-details",
    component: CompanyDetails,
  },
  {
    path: "/stock",
    name: "stock-list",
    component: StockList,
  },
  {
    path: "/stock/create",
    name: "stock-create",
    component: StockCreate,
  },
  {
    path: "/stock/edit/:id",
    name: "stock-edit",
    component: StockEdit,
  },
  {
    path: "/stock/:id",
    name: "stock-details",
    component: StockDetails,
  },
  {
    path: "/login",
    name: "login-page-alias",
    component: LoginPage,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const publicPages = ["/", "/login", "/register"];
  const authRequired = !publicPages.includes(to.path);
  const isAuthenticated = store.state.token;

  if (authRequired && !isAuthenticated) {
    return next("/login");
  }

  next();
});

export default router;
