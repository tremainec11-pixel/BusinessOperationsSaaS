import api from "./api";

export const getSubscriptionPlans = async () => {
  const response = await api.get("/Subscriptions/plans");
  return response.data;
};

export const getCurrentSubscription = async () => {
  const response = await api.get("/Subscriptions/current");
  return response.data;
};

export const createCheckoutSession = async (subscriptionPlanId) => {
  const response = await api.post("/Subscriptions/checkout", {
    subscriptionPlanId,
    trialDays: 0,
  });

  return response.data;
};
