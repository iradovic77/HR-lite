import api from './axios'

// ── Tipovi ───────────────────────────────────────────────────────────

export interface GenderResponse {
  id: string
  code: string
  nameHr: string
  nameEn: string | null
  ordinal: number
  isActive: boolean
}

export interface CreateGenderRequest {
  code: string
  nameHr: string
  nameEn?: string
  ordinal: number
  isActive: boolean
}

export interface UpdateGenderRequest {
  code: string
  nameHr: string
  nameEn?: string | null
  ordinal: number
  isActive: boolean
}

// ── API pozivi ───────────────────────────────────────────────────────

const BASE = '/codebook/gender'

export const genderApi = {
  getAll: (includeInactive = false) =>
    api.get<GenderResponse[]>(BASE, { params: { includeInactive } }),

  getById: (id: string) =>
    api.get<GenderResponse>(`${BASE}/${id}`),

  create: (data: CreateGenderRequest) =>
    api.post<GenderResponse>(BASE, data),

  update: (id: string, data: UpdateGenderRequest) =>
    api.put<GenderResponse>(`${BASE}/${id}`, data),

  toggleActive: (id: string) =>
    api.patch<GenderResponse>(`${BASE}/${id}/toggle-active`),

  deleteById: (id: string) =>
    api.delete(`${BASE}/${id}`),
}
