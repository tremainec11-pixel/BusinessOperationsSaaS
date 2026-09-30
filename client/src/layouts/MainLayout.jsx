import { NavLink, Outlet } from "react-router-dom";
import {
  LayoutDashboard,
  Users,
  CheckSquare,
  Package,
  DollarSign,
  CreditCard,
} from "lucide-react";

function MainLayout() {
  const menuItems = [
    {
      label: "Dashboard",
      path: "/dashboard",
      icon: LayoutDashboard,
    },
    {
      label: "Employees",
      path: "/employees",
      icon: Users,
    },
    {
      label: "Tasks",
      path: "/tasks",
      icon: CheckSquare,
    },
    {
      label: "Products",
      path: "/products",
      icon: Package,
    },
    {
      label: "Financials",
      path: "/financials",
      icon: DollarSign,
    },
    {
      label: "Subscription",
      path: "/subscriptions",
      icon: CreditCard,
    },
  ];

  return (
    <div className="app-layout">
      <aside className="sidebar">
        <div className="sidebar-header">
          <h2>BusinessOps</h2>
          <span>SaaS</span>
        </div>

        <nav className="sidebar-nav">
          {menuItems.map((item) => {
            const Icon = item.icon;

            return (
              <NavLink
                key={item.path}
                to={item.path}
                className={({ isActive }) =>
                  isActive ? "nav-linkactive" : "nav-link"
                }
              >
                <Icon size={18} strokeWidth={2} />
                <span>{item.label}</span>
              </NavLink>
            );
          })}
        </nav>
      </aside>

      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}

export default MainLayout;
