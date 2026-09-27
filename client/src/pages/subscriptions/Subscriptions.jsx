import { useEffect, useState } from "react";
import {
  getSubscriptionPlans,
  getCurrentSubscription,
  createCheckoutSession,
} from "../../services/subscriptionService";
import "./Subscriptions.css";

function Subscriptions() {
  const [plans, setPlans] = useState([]);
  const [currentSubscription, setCurrentSubscription] = useState(null);
  const [loading, setLoading] = useState(true);
  const [checkoutLoading, setCheckoutLoading] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadSubscriptionData = async () => {
      try {
        setLoading(true);
        setError("");

        const plansData = await getSubscriptionPlans();
        setPlans(plansData);

        try {
          const currentData = await getCurrentSubscription();
          setCurrentSubscription(currentData);
        } catch (subscriptionError) {
          if (subscriptionError.response?.status !== 404) {
            throw subscriptionError;
          }

          setCurrentSubscription(null);
        }
      } catch (err) {
        console.error(err);

        setError(
          err.response?.data?.message ||
            "Unable to load subscription information."
        );
      } finally {
        setLoading(false);
      }
    };

    loadSubscriptionData();
  }, []);

  const handleSubscribe = async (planId) => {
    try {
      setCheckoutLoading(planId);
      setError("");

      const data = await createCheckoutSession(planId);

      if (!data.checkoutUrl) {
        throw new Error("Stripe checkout URL was not returned.");
      }

      window.location.href = data.checkoutUrl;
    } catch (err) {
      console.error(err);

      setError(
        err.response?.data?.message ||
          err.message ||
          "Unable to start the checkout process."
      );

      setCheckoutLoading(null);
    }
  };

  const formatPrice = (price) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      minimumFractionDigits: 2,
    }).format(price);
  };

  if (loading) {
    return (
      <div className="subscriptions-page">
        <div className="subscriptions-loading">
          Loading subscription information...
        </div>
      </div>
    );
  }

  return (
    <div className="subscriptions-page">
      <div className="subscriptions-header">
        <div>
          <h1>Subscription</h1>
          <p>Manage your subscription and choose the plan that fits your business.</p>
        </div>
      </div>

      {error && (
        <div className="subscriptions-error">
          {error}
        </div>
      )}

      {currentSubscription && (
        <section className="current-subscription-card">
          <div className="current-subscription-header">
            <div>
              <span className="section-label">Current Plan</span>
              <h2>{currentSubscription.subscriptionPlan?.name}</h2>
            </div>

            <span className="subscription-status">
              {currentSubscription.status}
            </span>
          </div>

          <p className="current-subscription-description">
            {currentSubscription.subscriptionPlan?.description}
          </p>

          <div className="subscription-details">
            <div>
              <span>Monthly Price</span>
              <strong>
                {formatPrice(
                  currentSubscription.subscriptionPlan?.monthlyPrice || 0
                )}
              </strong>
            </div>

            <div>
              <span>Employees</span>
              <strong>
                {currentSubscription.subscriptionPlan?.maxEmployees === -1
                  ? "Unlimited"
                  : currentSubscription.subscriptionPlan?.maxEmployees}
              </strong>
            </div>

            <div>
              <span>Products</span>
              <strong>
                {currentSubscription.subscriptionPlan?.maxProducts === -1
                  ? "Unlimited"
                  : currentSubscription.subscriptionPlan?.maxProducts}
              </strong>
            </div>

            <div>
              <span>Tasks</span>
              <strong>
                {currentSubscription.subscriptionPlan?.maxTasks === -1
                  ? "Unlimited"
                  : currentSubscription.subscriptionPlan?.maxTasks}
              </strong>
            </div>
          </div>
        </section>
      )}

      <section className="plans-section">
        <div className="section-heading">
          <h2>Available Plans</h2>
          <p>Choose the plan that best matches your business needs.</p>
        </div>

        <div className="plans-grid">
          {plans.map((plan) => {
            const isCurrentPlan =
              currentSubscription?.subscriptionPlanId === plan.id;

            return (
              <div
                key={plan.id}
                className={`plan-card ${
                  isCurrentPlan ? "current-plan" : ""
                }`}
              >
                {isCurrentPlan && (
                  <div className="current-plan-badge">
                    Current Plan
                  </div>
                )}

                <div className="plan-card-header">
                  <h3>{plan.name}</h3>
                  <p>{plan.description}</p>
                </div>

                <div className="plan-price">
                  <strong>{formatPrice(plan.monthlyPrice)}</strong>
                  <span>/ month</span>
                </div>

                <div className="plan-features">
                  <div>
                    <span>Employees</span>
                    <strong>
                      {plan.maxEmployees === -1
                        ? "Unlimited"
                        : plan.maxEmployees}
                    </strong>
                  </div>

                  <div>
                    <span>Products</span>
                    <strong>
                      {plan.maxProducts === -1
                        ? "Unlimited"
                        : plan.maxProducts}
                    </strong>
                  </div>

                  <div>
                    <span>Tasks</span>
                    <strong>
                      {plan.maxTasks === -1
                        ? "Unlimited"
                        : plan.maxTasks}
                    </strong>
                  </div>
                </div>

                <button
                  className={
                    isCurrentPlan
                      ? "secondary-button plan-button"
                      : "primary-button plan-button"
                  }
                  disabled={
                    isCurrentPlan || checkoutLoading !== null
                  }
                  onClick={() => handleSubscribe(plan.id)}
                >
                  {checkoutLoading === plan.id
                    ? "Opening Checkout..."
                    : isCurrentPlan
                    ? "Current Plan"
                    : currentSubscription
                    ? "Change Plan"
                    : "Subscribe"}
                </button>
              </div>
            );
          })}
        </div>
      </section>
    </div>
  );
}

export default Subscriptions;
