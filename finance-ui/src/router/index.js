import { createRouter, createWebHistory } from "vue-router";
import InvoiceList from "../views/InvoiceList.vue";
import InvoiceDetails from "../views/InvoiceDetails.vue";
import InvoiceCreate from "../views/InvoiceCreate.vue";
import CustomerCreate from "../views/CustomerCreate.vue";
import CustomerList from "../views/CustomerList.vue";
import LoginPage from "../views/LoginPage.vue";
import CompanyList from "../views/CompanyList.vue";
import CompanyCreate from "../views/CompanyCreate.vue";
import CompanyDetails from "../views/CompanyDetails.vue";

const routes = [
  {
    path: "/",
    name: "login-page",
    component: LoginPage,
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
    path: "/customer/create",
    name: "customer-create",
    component: CustomerCreate,
  },
  {
    path: "/customer",
    name: "customer-list",
    component: CustomerList,
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
    path: "/company/:id",
    name: "company-details",
    component: CompanyDetails,
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
  const publicPages = ["/", "/login"];
  const authRequired = !publicPages.includes(to.path);
  const token = localStorage.getItem("token");

  if (authRequired && !token) {
    return next("/login");
  }
  next();
});

export default router;
