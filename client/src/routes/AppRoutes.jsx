import { Navigate, Route, Routes } from "react-router-dom";
import Login from "../pages/auth/Login";
import Dashboard from "../pages/dashboard/Dashboard";
import MainLayout from "../layouts/MainLayout";
import { useAuth } from "../context/AuthContext";
import Employees from "../pages/employees/Employees";
import Tasks from "../pages/tasks/Tasks";
import Products from "../pages/products/Products";
import Financials from "../pages/financials/Financials";
import Subscriptions from "../pages/subscriptions/Subscriptions";

function ProtectedRoute({ children }) {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />

      <Route
        element={
          <ProtectedRoute>
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/employees" element={<Employees />} />
        <Route path="/tasks" element={<Tasks />} />
        <Route path="/products" element={<Products />} />
        <Route path="/financials" element={<Financials />} />
        <Route
          path="/subscriptions"
          element={<Subscriptions />}
        />
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


