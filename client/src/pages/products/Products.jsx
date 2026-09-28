import { useEffect, useState } from "react";
import "./Products.css";

import {
  getProducts,
  createProduct,
  updateProduct,
  deleteProduct,
} from "../../services/productService";

function Products() {
  const [products, setProducts] = useState([]);

  const [showForm, setShowForm] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [formData, setFormData] = useState({
    name: "",
    description: "",
    price: "",
    stock: "",
    sku: "",
    category: "",
  });

  useEffect(() => {
    loadProducts();
  }, []);

  const loadProducts = async () => {
    try {
      setLoading(true);
      setError("");

      const data = await getProducts();

      setProducts(data);
    } catch (error) {
      console.error(error);
      setError("Unable to load products.");
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
      name: "",
      description: "",
      price: "",
      stock: "",
      sku: "",
      category: "",
    });

    setEditingProduct(null);
    setShowForm(false);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      setSaving(true);
      setError("");
      setSuccess("");

      const data = {
        name: formData.name,
        description: formData.description || null,
        price: Number(formData.price),
        stock: Number(formData.stock),
        sku: formData.sku || null,
        category: formData.category || null,
      };

      if (editingProduct) {
        await updateProduct(editingProduct.id, data);
        setSuccess("Product updated successfully.");
      } else {
        await createProduct(data);
        setSuccess("Product created successfully.");
      }

      resetForm();
      await loadProducts();
    } catch (error) {
      console.error(error);

      const message =
        error?.response?.data?.message ||
        error?.response?.data?.title ||
        "Unable to save product.";

      setError(message);
    } finally {
      setSaving(false);
    }
  };

  const handleEdit = (product) => {
    setEditingProduct(product);

    setFormData({
      name: product.name || "",
      description: product.description || "",
      price: product.price ?? "",
      stock: product.stock ?? "",
      sku: product.sku || "",
      category: product.category || "",
    });

    setShowForm(true);
    setError("");
    setSuccess("");
  };

  const handleDelete = async (id) => {
    const confirmed = window.confirm(
      "Are you sure you want to delete this product?"
    );

    if (!confirmed) {
      return;
    }

    try {
      setError("");
      setSuccess("");

      await deleteProduct(id);

      setSuccess("Product deleted successfully.");

      await loadProducts();
    } catch (error) {
      console.error(error);

      const message =
        error?.response?.data?.message ||
        error?.response?.data?.title ||
        "Unable to delete product.";

      setError(message);
    }
  };

  const formatPrice = (price) => {
    return Number(price || 0).toLocaleString("en-US", {
      style: "currency",
      currency: "USD",
    });
  };

  if (loading) {
    return (
      <div className="products-page">
        <div className="loading-state">
          Loading products...
        </div>
      </div>
    );
  }

  return (
    <div className="products-page">
      <header className="page-header">
        <div>
          <h1>Products</h1>
          <p>Manage your company's products and inventory</p>
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
            Add Product
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
                {editingProduct ? "Edit Product" : "Add Product"}
              </h2>

              <p>
                Enter the product information below.
              </p>
            </div>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="form-grid">
              <div className="form-group">
                <label htmlFor="name">
                  Name
                </label>

                <input
                  id="name"
                  name="name"
                  type="text"
                  value={formData.name}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="sku">
                  SKU
                </label>

                <input
                  id="sku"
                  name="sku"
                  type="text"
                  value={formData.sku}
                  onChange={handleChange}
                />
              </div>

              <div className="form-group">
                <label htmlFor="price">
                  Price
                </label>

                <input
                  id="price"
                  name="price"
                  type="number"
                  min="0"
                  step="0.01"
                  value={formData.price}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="stock">
                  Stock
                </label>

                <input
                  id="stock"
                  name="stock"
                  type="number"
                  min="0"
                  step="1"
                  value={formData.stock}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="category">
                  Category
                </label>

                <input
                  id="category"
                  name="category"
                  type="text"
                  value={formData.category}
                  onChange={handleChange}
                />
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
                  : editingProduct
                    ? "Update Product"
                    : "Create Product"}
              </button>
            </div>
          </form>
        </section>
      )}

      <section className="products-card">
        <div className="products-card-header">
          <div>
            <h2>Product List</h2>
            <p>
              {products.length} product
              {products.length !== 1 ? "s" : ""}
            </p>
          </div>
        </div>

        {products.length === 0 ? (
          <div className="empty-state">
            No products found.
          </div>
        ) : (
          <div className="table-wrapper">
            <table className="products-table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>SKU</th>
                  <th>Category</th>
                  <th>Price</th>
                  <th>Stock</th>
                  <th>Actions</th>
                </tr>
              </thead>

              <tbody>
                {products.map((product) => (
                  <tr key={product.id}>
                    <td>
                      <div className="product-name">
                        {product.name}
                      </div>

                      {product.description && (
                        <div className="product-description">
                          {product.description}
                        </div>
                      )}
                    </td>

                    <td>
                      {product.sku || "—"}
                    </td>

                    <td>
                      {product.category || "—"}
                    </td>

                    <td>
                      {formatPrice(product.price)}
                    </td>

                    <td>
                      <span
                        className={
                          Number(product.stock) <= 5
                            ? "stock-badge low-stock"
                            : "stock-badge"
                        }
                      >
                        {product.stock}
                      </span>
                    </td>

                    <td>
                      <div className="action-buttons">
                        <button
                          type="button"
                          className="edit-button"
                          onClick={() => handleEdit(product)}
                        >
                          Edit
                        </button>

                        <button
                          type="button"
                          className="delete-button"
                          onClick={() =>
                            handleDelete(product.id)
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

export default Products;
