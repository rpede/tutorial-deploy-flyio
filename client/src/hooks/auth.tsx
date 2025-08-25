import type { AuthUserInfo, LoginRequest } from "../models/generated-client";

type AuthHook = {
  user: AuthUserInfo | null;
  login: (request: LoginRequest) => Promise<void>;
  logout: () => void;
};

export const useAuth = () => {
  // TODO add client-side session management logic here

  return {
    user: null,
    login: async () => {
      throw new Error("Not implemented");
    },
    logout: async () => {
      throw new Error("Not implemented");
    },
  } as AuthHook;
};
