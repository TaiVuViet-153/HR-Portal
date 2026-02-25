const required = (value: string | undefined, name: string, fallback?: string) => {
	if (value && value.trim().length > 0) return value;
	if (fallback) {
		console.warn(`[config] ${name} missing; falling back to ${fallback}`);
		return fallback;
	}
	throw new Error(`[config] Missing required env: ${name}`);
};

export const AUTH_BASE_URL = required(import.meta.env.VITE_AUTH_BASE_URL as string | undefined, 'VITE_AUTH_BASE_URL');
export const REQUEST_BASE_URL = required(import.meta.env.VITE_REQUEST_BASE_URL as string | undefined, 'VITE_REQUEST_BASE_URL');
export const EMPLOYEE_BASE_URL = required(import.meta.env.VITE_EMPLOYEE_BASE_URL as string | undefined, 'VITE_EMPLOYEE_BASE_URL');


// Debug log (non-secret) to ensure correct scheme/port during local dev
try { console.info('[config] AUTH_BASE_URL:', AUTH_BASE_URL); } catch {}