import { useEffect, useState } from "react";
import "./Dashboard.css";
import { getDashboard } from "../../services/dashboardService";

function Dashboard() {
  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadDashboard = async () => {
      try {
        const data = await getDashboard();

        setDashboard(data);
      } catch (error) {
        console.error(error);

        setError("Unable to load dashboard data.");
      } finally {
        setLoading(false);
      }
    };

    loadDashboard();
  }, []);

  if (loading) {
    return (
      <div className="dashboard-page">
        <div className="loading-state">
          Loading dashboard...
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="dashboard-page">
        <div className="error-state">
          {error}
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-page">
      <header className="page-header">
        <div>
          <h1>Dashboard</h1>
          <p>
            Welcome to BusinessOps SaaS
          </p>
        </div>
      </header>

      <section className="stats-grid">
        <div className="stat-card">
          <span>Total Employees</span>
          <strong>{dashboard.totalEmployees}</strong>
          <small>
            {dashboard.activeEmployees} active
          </small>
        </div>

        <div className="stat-card">
          <span>Total Tasks</span>
          <strong>{dashboard.totalTasks}</strong>
          <small>
            {dashboard.pendingTasks} pending
          </small>
        </div>

        <div className="stat-card">
          <span>Total Products</span>
          <strong>{dashboard.totalProducts}</strong>
          <small>
            {dashboard.lowStockProducts} low stock
          </small>
        </div>

        <div className="stat-card">
          <span>Balance</span>
          <strong>
            ${Number(dashboard.balance).toLocaleString(
              "en-US",
              {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
              }
            )}
          </strong>
          <small>Current balance</small>
        </div>
      </section>

      <section className="dashboard-grid">
        <div className="dashboard-card">
          <h2>Tasks Overview</h2>

          <div className="overview-row">
            <span>Total Tasks</span>
            <strong>{dashboard.totalTasks}</strong>
          </div>

          <div className="overview-row">
            <span>Pending</span>
            <strong>{dashboard.pendingTasks}</strong>
          </div>

          <div className="overview-row">
            <span>Completed</span>
            <strong>{dashboard.completedTasks}</strong>
          </div>
        </div>

        <div className="dashboard-card">
          <h2>Financial Overview</h2>

          <div className="overview-row">
            <span>Total Income</span>
            <strong>
              ${Number(dashboard.totalIncome).toLocaleString(
                "en-US",
                {
                  minimumFractionDigits: 2,
                }
              )}
            </strong>
          </div>

          <div className="overview-row">
            <span>Total Expenses</span>
            <strong>
              ${Number(dashboard.totalExpenses).toLocaleString(
                "en-US",
                {
                  minimumFractionDigits: 2,
                }
              )}
            </strong>
          </div>

          <div className="overview-rowbalance-row">
            <span>Balance</span>
            <strong>
              ${Number(dashboard.balance).toLocaleString(
                "en-US",
                {
                  minimumFractionDigits: 2,
                }
              )}
            </strong>
          </div>
        </div>
      </section>

      <section className="dashboard-card">
        <h2>Business Overview</h2>

        <div className="business-overview">
          <div>
            <span>Employees</span>
            <strong>{dashboard.totalEmployees}</strong>
          </div>

          <div>
            <span>Products</span>
            <strong>{dashboard.totalProducts}</strong>
          </div>

          <div>
            <span>Completed Tasks</span>
            <strong>{dashboard.completedTasks}</strong>
          </div>

          <div>
            <span>Low Stock</span>
            <strong>{dashboard.lowStockProducts}</strong>
          </div>
        </div>
      </section>
    </div>
  );
}

export default Dashboard;
