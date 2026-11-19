// Todas as chamadas ao back-end ASP.NET passam por aqui.
const BASE = import.meta.env.VITE_API_URL || 'http://localhost:5273/api';

export async function api(path, method = 'GET', body) {
  const res = await fetch(`${BASE}/${path}`, {
    method,
    headers: body ? { 'Content-Type': 'application/json' } : {},
    body: body ? JSON.stringify(body) : undefined,
  });
  if (res.status === 204) return null;
  const data = await res.json().catch(() => null);
  if (!res.ok) {
    const msg = data?.mensagem || (data?.errors && Object.values(data.errors).flat().join(' '));
    throw new Error(msg || `Erro ${res.status}`);
  }
  return data;
}
