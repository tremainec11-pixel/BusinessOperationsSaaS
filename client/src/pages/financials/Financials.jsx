import { useEffect, useMemo, useState } from "react";
import "./Financials.css";
import {
  getFinancialTransactions,
  createFinancialTransaction,
  updateFinancialTransaction,
  deleteFinancialTransaction,
} from "../../services/financialService";

const initialForm = {
  type: "Income",
  category: "",
  description: "",
  amount: "",
  transactionDate: new Date().toISOString().split("T")[0],
  isActive: true,
};

function Financials() {
  const [transactions, setTransactions] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [editingTransaction, setEditingTransaction] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [formData, setFormData] = useState(initialForm);

  const loadTransactions = async () => {
    try {
      setLoading(true);
      setError("");

      const data = await getFinancialTransactions();
      setTransactions(data);
    } catch (err) {
      console.error(err);
      setError(
        err.response?.data?.message ||
          "Unable to load financial transactions."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadTransactions();
  }, []);

  const totals = useMemo(() => {
    const activeTransactions = transactions.filter(
      (transaction) => transaction.isActive
    );

    const income = activeTransactions
      .filter((transaction) => transaction.type.toLowerCase() === "income")
      .reduce((sum, transaction) => sum + Number(transaction.amount), 0);

    const expenses = activeTransactions
      .filter((transaction) => transaction.type.toLowerCase() === "expense")
      .reduce((sum, transaction) => sum + Number(transaction.amount), 0);

    return {
      income,
      expenses,
      balance: income - expenses,
    };
  }, [transactions]);

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(amount);
  };

  const formatDate = (date) => {
    if (!date) {
      return "-";
    }

    return new Date(date).toLocaleDateString("en-US");
  };

  const handleChange = (event) => {
    const { name, value, type, checked } = event.target;

    setFormData((current) => ({
      ...current,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const resetForm = () => {
    setFormData(initialForm);
    setEditingTransaction(null);
    setShowForm(false);
  };

  const handleAdd = () => {
    setEditingTransaction(null);
    setFormData(initialForm);
    setError("");
    setSuccess("");
    setShowForm(true);
  };

  const handleEdit = (transaction) => {
    setEditingTransaction(transaction);

    setFormData({
      type: transaction.type || "Income",
      category: transaction.category || "",
      description: transaction.description || "",
      amount: transaction.amount ?? "",
      transactionDate: transaction.transactionDate
        ? new Date(transaction.transactionDate)
            .toISOString()
            .split("T")[0]
        : new Date().toISOString().split("T")[0],
      isActive: transaction.isActive,
    });

    setError("");
    setSuccess("");
    setShowForm(true);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      setSaving(true);
      setError("");
      setSuccess("");

      const data = {
        type: formData.type,
        category: formData.category,
        description: formData.description,
        amount: Number(formData.amount),
        transactionDate: formData.transactionDate,
      };

      if (editingTransaction) {
        await updateFinancialTransaction(editingTransaction.id, {
          ...data,
          isActive: formData.isActive,
        });

        setSuccess("Financial transaction updated successfully.");
      } else {
        await createFinancialTransaction(data);

        setSuccess("Financial transaction created successfully.");
      }

      await loadTransactions();
      resetForm();
    } catch (err) {
      console.error(err);

      setError(
        err.response?.data?.message ||
          "Unable to save financial transaction."
      );
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (transaction) => {
    const confirmed = window.confirm(
      `Are you sure you want to delete "${transaction.description}"?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setError("");
      setSuccess("");

      await deleteFinancialTransaction(transaction.id);

      setSuccess("Financial transaction deleted successfully.");

      await loadTransactions();
    } catch (err) {
      console.error(err);

      setError(
        err.response?.data?.message ||
          "Unable to delete financial transaction."
      );
    }
  };

  if (loading) {
    return (
      <div className="financials-page">
        <div className="financials-loading">
          Loading financials...
        </div>
      </div>
    );
  }

  return (
    <div className="financials-page">
      <div className="financials-header">
        <div>
          <h1>Financials</h1>
          <p>Manage your company's income and expenses</p>
        </div>

        {!showForm && (
          <button className="primary-button" onClick={handleAdd}>
            Add Transaction
          </button>
        )}
      </div>

      {error && <div className="financials-message error">{error}</div>}

      {success && (
        <div className="financials-message success">{success}</div>
      )}

      <div className="financial-summary">
        <div className="financial-summary-card">
          <span>Total Income</span>
          <strong>{formatCurrency(totals.income)}</strong>
        </div>

        <div className="financial-summary-card">
          <span>Total Expenses</span>
          <strong>{formatCurrency(totals.expenses)}</strong>
        </div>

        <div className="financial-summary-card">
          <span>Balance</span>
          <strong>{formatCurrency(totals.balance)}</strong>
        </div>
      </div>

      {showForm && (
        <div className="financial-form-card">
          <div className="financial-form-header">
            <div>
              <h2>
                {editingTransaction
                  ? "Edit Transaction"
                  : "Add Transaction"}
              </h2>
              <p>
                {editingTransaction
                  ? "Update the financial transaction"
                  : "Create a new financial transaction"}
              </p>
            </div>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="financial-form-grid">
              <div className="form-group">
                <label htmlFor="type">Type</label>
                <select
                  id="type"
                  name="type"
                  value={formData.type}
                  onChange={handleChange}
                  required
                >
                  <option value="Income">Income</option>
                  <option value="Expense">Expense</option>
                </select>
              </div>

              <div className="form-group">
                <label htmlFor="category">Category</label>
                <input
                  id="category"
                  name="category"
                  type="text"
                  value={formData.category}
                  onChange={handleChange}
                  placeholder="e.g. Sales, Payroll, Rent"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="amount">Amount</label>
                <input
                  id="amount"
                  name="amount"
                  type="number"
                  min="0"
                  step="0.01"
                  value={formData.amount}
                  onChange={handleChange}
                  placeholder="0.00"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="transactionDate">
                  Transaction Date
                </label>
                <input
                  id="transactionDate"
                  name="transactionDate"
                  type="date"
                  value={formData.transactionDate}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group full-width">
                <label htmlFor="description">Description</label>
                <textarea
                  id="description"
                  name="description"
                  value={formData.description}
                  onChange={handleChange}
                  placeholder="Enter transaction description"
                  rows="4"
                  required
                />
              </div>

              {editingTransaction && (
                <div className="form-group checkbox-group full-width">
                  <label>
                    <input
                      type="checkbox"
                      name="isActive"
                      checked={formData.isActive}
                      onChange={handleChange}
                    />
                    Active transaction
                  </label>
                </div>
              )}
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
                  : editingTransaction
                    ? "Update Transaction"
                    : "Save Transaction"}
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="financial-list-card">
        <div className="financial-list-header">
          <div>
            <h2>Transaction List</h2>
            <span>
              {transactions.length}{" "}
              {transactions.length === 1
                ? "transaction"
                : "transactions"}
            </span>
          </div>
        </div>

        {transactions.length === 0 ? (
          <div className="financial-empty">
            <h3>No financial transactions</h3>
            <p>
              Add your first income or expense transaction to get
              started.
            </p>
          </div>
        ) : (
          <div className="financial-table-wrapper">
            <table className="financial-table">
              <thead>
                <tr>
                  <th>Type</th>
                  <th>Category</th>
                  <th>Description</th>
                  <th>Amount</th>
                  <th>Date</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>

              <tbody>
                {transactions.map((transaction) => (
                  <tr key={transaction.id}>
                    <td>
                      <span
                        className={`transaction-type ${
                          transaction.type.toLowerCase() === "income"
                            ? "income"
                            : "expense"
                        }`}
                      >
                        {transaction.type}
                      </span>
                    </td>

                    <td>{transaction.category}</td>

                    <td>
                      <div className="transaction-description">
                        {transaction.description}
                      </div>
                    </td>

                    <td
                      className={
                        transaction.type.toLowerCase() === "income"
                          ? "amount-income"
                          : "amount-expense"
                      }
                    >
                      {transaction.type.toLowerCase() === "income"
                        ? "+"
                        : "-"}
                      {formatCurrency(transaction.amount)}
                    </td>

                    <td>{formatDate(transaction.transactionDate)}</td>

                    <td>
                      <span
                        className={
                          transaction.isActive
                            ? "status-badge active"
                            : "status-badge inactive"
                        }
                      >
                        {transaction.isActive
                          ? "Active"
                          : "Inactive"}
                      </span>
                    </td>

                    <td>
                      <div className="table-actions">
                        <button
                          className="edit-button"
                          onClick={() =>
                            handleEdit(transaction)
                          }
                        >
                          Edit
                        </button>

                        <button
                          className="delete-button"
                          onClick={() =>
                            handleDelete(transaction)
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
      </div>
    </div>
  );
}

export default Financials;
