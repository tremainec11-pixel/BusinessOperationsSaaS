import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../../services/authService";
import { useAuth } from "../../context/AuthContext";
import "./Login.css";

function Login() {
  const navigate = useNavigate();
  const { loginUser } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();

    setError("");
    setLoading(true);

    try {
      const data = await login(email, password);

      loginUser(
        {
          id: data.userId,
          firstName: data.firstName,
          lastName: data.lastName,
          email: data.email,
          role: data.role,
        },
        data.token
      );

      navigate("/dashboard");
    } catch (error) {
      console.error(error);

      setError(
        error.response?.data?.message ||
          "Invalid email or password."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-brand">
          <div className="login-logo">BO</div>

          <div>
            <h1>Business Operations</h1>
            <p>SaaS Management Platform</p>
          </div>
        </div>

        <div className="login-header">
          <h2>Welcome back</h2>
          <p>Sign in to manage your business operations.</p>
        </div>

        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label htmlFor="email">Email</label>

            <input
              id="email"
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="Enter your email"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>

            <input
              id="password"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Enter your password"
              required
            />
          </div>

          {error && (
            <div className="login-error">
              {error}
            </div>
          )}

          <button
            type="submit"
            className="login-button"
            disabled={loading}
          >
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>

        <div className="login-footer">
          Business Operations SaaS
        </div>
      </div>
    </div>
  );
}

export default Login;
