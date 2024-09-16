import { createRouter, createWebHistory } from "vue-router";
import InvoiceList from "../views/InvoiceList.vue";
import InvoiceDetails from "../views/InvoiceDetails.vue";
import InvoiceCreate from "../views/InvoiceCreate.vue";
import LoginPage from "../views/LoginPage.vue";

const routes = [
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
    path: "/login",
    name: "login-page",
    component: LoginPage,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const publicPages = ["/login"];
  const authRequired = !publicPages.includes(to.path);
  const token = localStorage.getItem("token");

  if (authRequired && !token) {
    return next("/login");
  }
  next();
});

export default router;
