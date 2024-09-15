import { createRouter, createWebHistory } from "vue-router";
import InvoicesList from "../views/InvoicesList.vue";
import InvoiceDetails from "../views/InvoiceDetails.vue";

const routes = [
  {
    path: "/",
    redirect: "/invoices",
  },
  {
    path: "/invoices",
    name: "InvoicesList",
    component: InvoicesList,
  },
  {
    path: "/invoice/:id",
    name: "InvoiceDetails",
    component: InvoiceDetails,
    props: true,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
