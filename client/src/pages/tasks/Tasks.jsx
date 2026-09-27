import { useEffect, useState } from "react";
import "./Tasks.css";

import {
  getTasks,
  createTask,
  updateTask,
  deleteTask,
} from "../../services/taskService";

import { getEmployees } from "../../services/employeeService";

function Tasks() {
  const [tasks, setTasks] = useState([]);
  const [employees, setEmployees] = useState([]);

  const [showForm, setShowForm] = useState(false);
  const [editingTask, setEditingTask] = useState(null);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [formData, setFormData] = useState({
    title: "",
    description: "",
    assignedToEmployeeId: "",
    status: "Pending",
    priority: "Medium",
    dueDate: "",
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError("");

      const [tasksData, employeesData] = await Promise.all([
        getTasks(),
        getEmployees(),
      ]);

      setTasks(tasksData);
      setEmployees(employeesData);
    } catch (error) {
      console.error(error);
      setError("Unable to load tasks.");
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (event) => {
    const { name, value } = event.target;

    setFormData((current) => ({
      ...current,
      [name]: value,
    }));
  };

  const resetForm = () => {
    setFormData({
      title: "",
      description: "",
      assignedToEmployeeId: "",
      status: "Pending",
      priority: "Medium",
      dueDate: "",
    });

    setEditingTask(null);
    setShowForm(false);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      setSaving(true);
      setError("");
      setSuccess("");

      const data = {
        title: formData.title,
        description: formData.description || null,
        assignedToEmployeeId:
          formData.assignedToEmployeeId || null,
        status: formData.status,
        priority: formData.priority,
        dueDate: formData.dueDate
          ? new Date(formData.dueDate).toISOString()
          : null,
      };

      if (editingTask) {
        await updateTask(editingTask.id, data);
        setSuccess("Task updated successfully.");
      } else {
        await createTask(data);
        setSuccess("Task created successfully.");
      }

      resetForm();
      await loadData();
    } catch (error) {
      console.error(error);

      const message =
        error?.response?.data?.message ||
        error?.response?.data?.title ||
        "Unable to save task.";

      setError(message);
    } finally {
      setSaving(false);
    }
  };

  const handleEdit = (task) => {
    setEditingTask(task);

    setFormData({
      title: task.title || "",
      description: task.description || "",
      assignedToEmployeeId: task.assignedToEmployeeId || "",
      status: task.status || "Pending",
      priority: task.priority || "Medium",
      dueDate: task.dueDate
        ? task.dueDate.substring(0, 10)
        : "",
    });

    setShowForm(true);
    setError("");
    setSuccess("");
  };

  const handleDelete = async (id) => {
    const confirmed = window.confirm(
      "Are you sure you want to delete this task?"
    );

    if (!confirmed) {
      return;
    }

    try {
      setError("");
      setSuccess("");

      await deleteTask(id);

      setSuccess("Task deleted successfully.");

      await loadData();
    } catch (error) {
      console.error(error);

      const message =
        error?.response?.data?.message ||
        error?.response?.data?.title ||
        "Unable to delete task.";

      setError(message);
    }
  };

  const getEmployeeName = (employeeId) => {
    if (!employeeId) {
      return "Unassigned";
    }

    const employee = employees.find(
      (item) => item.id === employeeId
    );

    if (!employee) {
      return "Unknown";
    }

    return `${employee.firstName} ${employee.lastName}`;
  };

  const formatDate = (date) => {
    if (!date) {
      return "—";
    }

    return new Date(date).toLocaleDateString("en-US");
  };

  if (loading) {
    return (
      <div className="tasks-page">
        <div className="loading-state">
          Loading tasks...
        </div>
      </div>
    );
  }

  return (
    <div className="tasks-page">
      <header className="page-header">
        <div>
          <h1>Tasks</h1>
          <p>Manage and track your company's tasks</p>
        </div>

        {!showForm && (
          <button
            type="button"
            className="primary-button"
            onClick={() => {
              setShowForm(true);
              setError("");
              setSuccess("");
            }}
          >
            Add Task
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
        <section className="form-card">
          <div className="form-header">
            <div>
              <h2>
                {editingTask ? "Edit Task" : "Add Task"}
              </h2>

              <p>
                Enter the task information below.
              </p>
            </div>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="form-grid">
              <div className="form-group">
                <label htmlFor="title">
                  Title
                </label>

                <input
                  id="title"
                  name="title"
                  type="text"
                  value={formData.title}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="assignedToEmployeeId">
                  Assigned Employee
                </label>

                <select
                  id="assignedToEmployeeId"
                  name="assignedToEmployeeId"
                  value={formData.assignedToEmployeeId}
                  onChange={handleChange}
                >
                  <option value="">
                    Unassigned
                  </option>

                  {employees.map((employee) => (
                    <option
                      key={employee.id}
                      value={employee.id}
                    >
                      {employee.firstName}{" "}
                      {employee.lastName}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group full-width">
                <label htmlFor="description">
                  Description
                </label>

                <textarea
                  id="description"
                  name="description"
                  value={formData.description}
                  onChange={handleChange}
                  rows="4"
                />
              </div>

              <div className="form-group">
                <label htmlFor="status">
                  Status
                </label>

                <select
                  id="status"
                  name="status"
                  value={formData.status}
                  onChange={handleChange}
                >
                  <option value="Pending">
                    Pending
                  </option>

                  <option value="In Progress">
                    In Progress
                  </option>

                  <option value="Completed">
                    Completed
                  </option>
                </select>
              </div>

              <div className="form-group">
                <label htmlFor="priority">
                  Priority
                </label>

                <select
                  id="priority"
                  name="priority"
                  value={formData.priority}
                  onChange={handleChange}
                >
                  <option value="Low">
                    Low
                  </option>

                  <option value="Medium">
                    Medium
                  </option>

                  <option value="High">
                    High
                  </option>
                </select>
              </div>

              <div className="form-group">
                <label htmlFor="dueDate">
                  Due Date
                </label>

                <input
                  id="dueDate"
                  name="dueDate"
                  type="date"
                  value={formData.dueDate}
                  onChange={handleChange}
                />
              </div>
            </div>

            <div className="form-actions">
              <button
                type="button"
                className="secondary-button"
                onClick={resetForm}
                disabled={saving}
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
                  : editingTask
                    ? "Update Task"
                    : "Create Task"}
              </button>
            </div>
          </form>
        </section>
      )}

      <section className="tasks-card">
        <div className="tasks-card-header">
          <div>
            <h2>Task List</h2>
            <p>
              {tasks.length} task
              {tasks.length !== 1 ? "s" : ""}
            </p>
          </div>
        </div>

        {tasks.length === 0 ? (
          <div className="empty-state">
            No tasks found.
          </div>
        ) : (
          <div className="table-wrapper">
            <table className="tasks-table">
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Assigned To</th>
                  <th>Status</th>
                  <th>Priority</th>
                  <th>Due Date</th>
                  <th>Completed</th>
                  <th>Actions</th>
                </tr>
              </thead>

              <tbody>
                {tasks.map((task) => (
                  <tr key={task.id}>
                    <td>
                      <div className="task-title">
                        {task.title}
                      </div>

                      {task.description && (
                        <div className="task-description">
                          {task.description}
                        </div>
                      )}
                    </td>

                    <td>
                      {getEmployeeName(
                        task.assignedToEmployeeId
                      )}
                    </td>

                    <td>
                      <span
                        className={`status-badge status-${task.status
                          .toLowerCase()
                          .replace(/\s+/g, "-")}`}
                      >
                        {task.status}
                      </span>
                    </td>

                    <td>
                      <span
                        className={`priority-badge priority-${task.priority.toLowerCase()}`}
                      >
                        {task.priority}
                      </span>
                    </td>

                    <td>
                      {formatDate(task.dueDate)}
                    </td>

                    <td>
                      {task.isCompleted ? "Yes" : "No"}
                    </td>

                    <td>
                      <div className="action-buttons">
                        <button
                          type="button"
                          className="edit-button"
                          onClick={() => handleEdit(task)}
                        >
                          Edit
                        </button>

                        <button
                          type="button"
                          className="delete-button"
                          onClick={() =>
                            handleDelete(task.id)
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
    </div>
  );
}

export default Tasks;
