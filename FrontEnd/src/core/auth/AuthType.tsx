export type AuthState = {
  accessToken: string | null;
  user?: { id: string; name: string; roles?: string[] };
  isAuthenticated: boolean;
  isInitializing: boolean;
};

export type LoginPayload = {
  accessToken: string;
  user?: AuthState["user"];
};

export type AuthAction =
| { type: "LOGIN"; payload: LoginPayload }
| { type: "LOGOUT" }
| { type: "SET_ACCESS_TOKEN"; payload: LoginPayload }
| { type: "FINISH_INIT" };

export type AuthContextValue = {
    state: AuthState;
    login: (payload: LoginPayload) => void;
    logout: () => void;
    setAccessToken: (payload: LoginPayload) => void;
};

export const UserRole = {
  ADMIN: 'ADMIN',
  MANAGER: 'MANAGER',
  USER: 'USER'
} as const

export type UserRoleValue = typeof UserRole[keyof typeof UserRole];