import { Navigate, Route, Routes } from "react-router-dom";
import Dashboard from "../pages/dashboard/Dashboard";
import MainLayout from "../layouts/MainLayout";
import Employees from "../pages/employees/Employees";
import Tasks from "../pages/tasks/Tasks";
import Products from "../pages/products/Products";
import Financials from "../pages/financials/Financials";
import Subscriptions from "../pages/subscriptions/Subscriptions";

function AppRoutes() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/employees" element={<Employees />} />
        <Route path="/tasks" element={<Tasks />} />
        <Route path="/products" element={<Products />} />
        <Route path="/financials" element={<Financials />} />
        <Route path="/subscriptions" element={<Subscriptions />} />
      </Route>

      <Route
        path="/"
        element={<Navigate to="/dashboard" replace />}
      />

      <Route
        path="*"
        element={<Navigate to="/dashboard" replace />}
      />
    </Routes>
  );
}

export default AppRoutes;
