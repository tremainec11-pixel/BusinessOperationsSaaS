import { useEffect, useState } from "react";
import "./Employees.css";
import {
  createEmployee,
  deleteEmployee,
  getEmployees,
  updateEmployee,
} from "../../services/employeeService";

const emptyForm = {
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
  position: "",
  department: "",
  hireDate: "",
  isActive: true,
};

function Employees() {
  const [employees, setEmployees] = useState([]);
  const [search, setSearch] = useState("");

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState(null);

  const [form, setForm] = useState(emptyForm);

  const loadEmployees = async () => {
    try {
      setError("");

      console.log("Loading employees...");

      const data = await getEmployees();

      console.log("Employees response:", data);

      setEmployees(data);
    } catch (error) {
      console.error("Employees error:", error);
      console.error("Status:", error.response?.status);
      console.error("Response:", error.response?.data);

      setError(
        error.response?.data?.message ||
          "Unable to load employees."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadEmployees();
  }, []);

  const handleChange = (event) => {
    const { name, value, type, checked } = event.target;

    setForm((current) => ({
      ...current,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleAdd = () => {
    setEditingId(null);
    setForm(emptyForm);
    setError("");
    setSuccess("");
    setShowForm(true);
  };

  const handleEdit = (employee) => {
    setEditingId(employee.id);

    setForm({
      firstName: employee.firstName || "",
      lastName: employee.lastName || "",
      email: employee.email || "",
      phone: employee.phone || "",
      position: employee.position || "",
      department: employee.department || "",
      hireDate: employee.hireDate
        ? employee.hireDate.substring(0, 10)
        : "",
      isActive: employee.isActive,
    });

    setError("");
    setSuccess("");
    setShowForm(true);
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingId(null);
    setForm(emptyForm);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    setSaving(true);
    setError("");
    setSuccess("");

    try {
      const payload = {
        ...form,
        hireDate: new Date(
          `${form.hireDate}T00:00:00`
        ).toISOString(),
      };

      if (editingId) {
        await updateEmployee(editingId, payload);
        setSuccess("Employee updated successfully.");
      } else {
        await createEmployee(payload);
        setSuccess("Employee created successfully.");
      }

      setShowForm(false);
      setEditingId(null);
      setForm(emptyForm);

      await loadEmployees();
    } catch (error) {
      console.error(error);

      setError(
        error.response?.data?.message ||
          "Unable to save employee."
      );
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (employee) => {
    const confirmed = window.confirm(
      `Are you sure you want to delete ${employee.firstName} ${employee.lastName}?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setError("");
      setSuccess("");

      await deleteEmployee(employee.id);

      setSuccess("Employee deleted successfully.");

      await loadEmployees();
    } catch (error) {
      console.error(error);

      setError(
        error.response?.data?.message ||
          "Unable to delete employee."
      );
    }
  };

  const filteredEmployees = employees.filter((employee) => {
    const searchValue = search.toLowerCase().trim();

    if (!searchValue) {
      return true;
    }

    const fullName =
      `${employee.firstName} ${employee.lastName}`.toLowerCase();

    return (
      fullName.includes(searchValue) ||
      employee.email
        ?.toLowerCase()
        .includes(searchValue) ||
      employee.position
        ?.toLowerCase()
        .includes(searchValue) ||
      employee.department
        ?.toLowerCase()
        .includes(searchValue)
    );
  });

  return (
    <div className="employees-page">
      <header className="page-header">
        <div>
          <h1>Employees</h1>
          <p>Manage your company's employees</p>
        </div>

        {!showForm && (
          <button
            type="button"
            className="primary-button"
            onClick={handleAdd}
          >
            + Add Employee
          </button>
        )}
      </header>

      {success && (
        <div className="success-state">
          {success}
        </div>
      )}

      {error && (
        <div className="error-state">
          {error}
        </div>
      )}

      {showForm && (
        <section className="employee-form-card">
          <div className="form-header">
            <div>
              <h2>
                {editingId
                  ? "Edit Employee"
                  : "Add Employee"}
              </h2>
              <p>
                Enter the employee information below.
              </p>
            </div>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="form-grid">
              <div className="form-group">
                <label htmlFor="firstName">
                  First Name
                </label>

                <input
                  id="firstName"
                  name="firstName"
                  type="text"
                  value={form.firstName}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="lastName">
                  Last Name
                </label>

                <input
                  id="lastName"
                  name="lastName"
                  type="text"
                  value={form.lastName}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="email">
                  Email
                </label>

                <input
                  id="email"
                  name="email"
                  type="email"
                  value={form.email}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="phone">
                  Phone
                </label>

                <input
                  id="phone"
                  name="phone"
                  type="text"
                  value={form.phone}
                  onChange={handleChange}
                />
              </div>

              <div className="form-group">
                <label htmlFor="position">
                  Position
                </label>

                <input
                  id="position"
                  name="position"
                  type="text"
                  value={form.position}
                  onChange={handleChange}
                />
              </div>

              <div className="form-group">
                <label htmlFor="department">
                  Department
                </label>

                <input
                  id="department"
                  name="department"
                  type="text"
                  value={form.department}
                  onChange={handleChange}
                />
              </div>

              <div className="form-group">
                <label htmlFor="hireDate">
                  Hire Date
                </label>

                <input
                  id="hireDate"
                  name="hireDate"
                  type="date"
                  value={form.hireDate}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group checkbox-group">
                <label>
                  <input
                    name="isActive"
                    type="checkbox"
                    checked={form.isActive}
                    onChange={handleChange}
                  />

                  <span>Active Employee</span>
                </label>
              </div>
            </div>

            <div className="form-actions">
              <button
                type="button"
                className="secondary-button"
                onClick={handleCancel}
              >
                Cancel
              </button>

              <button
                type="submit"
                className="primary-button"
                disabled={saving}
              >
                {saving
                  ? "Saving..."
                  : editingId
                    ? "Update Employee"
                    : "Create Employee"}
              </button>
            </div>
          </form>
        </section>
      )}

      {!showForm && (
        <>
          <section className="employee-toolbar">
            <input
              type="text"
              placeholder="Search employees..."
              value={search}
              onChange={(event) =>
                setSearch(event.target.value)
              }
            />
          </section>

          <section className="employee-table-card">
            {loading ? (
              <div className="loading-state">
                Loading employees...
              </div>
            ) : filteredEmployees.length === 0 ? (
              <div className="empty-state">
                <h2>No employees found</h2>
                <p>
                  Add your first employee to get started.
                </p>
              </div>
            ) : (
              <div className="table-wrapper">
                <table className="employees-table">
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Email</th>
                      <th>Position</th>
                      <th>Department</th>
                      <th>Hire Date</th>
                      <th>Status</th>
                      <th>Actions</th>
                    </tr>
                  </thead>

                  <tbody>
                    {filteredEmployees.map((employee) => (
                      <tr key={employee.id}>
                        <td>
                          <strong>
                            {employee.firstName}{" "}
                            {employee.lastName}
                          </strong>
                        </td>

                        <td>{employee.email}</td>

                        <td>
                          {employee.position || "—"}
                        </td>

                        <td>
                          {employee.department || "—"}
                        </td>

                        <td>
                          {employee.hireDate
                            ? new Date(
                                employee.hireDate
                              ).toLocaleDateString()
                            : "—"}
                        </td>

                        <td>
                          <span
                            className={
                              employee.isActive
                                ? "status-badge active"
                                : "status-badge inactive"
                            }
                          >
                            {employee.isActive
                              ? "Active"
                              : "Inactive"}
                          </span>
                        </td>

                        <td>
                          <div className="table-actions">
                            <button
                              type="button"
                              className="action-button edit"
                              onClick={() =>
                                handleEdit(employee)
                              }
                            >
                              Edit
                            </button>

                            <button
                              type="button"
                              className="action-button delete"
                              onClick={() =>
                                handleDelete(employee)
                              }
                            >
                              Delete
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </section>
        </>
      )}
    </div>
  );
}

export default Employees;

