import api from './axios'

// ── Gender ────────────────────────────────────────────────────────────

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

export const genderApi = {
  getAll: (includeInactive = false) =>
    api.get<GenderResponse[]>('/codebook/gender', { params: { includeInactive } }),
  getById: (id: string) =>
    api.get<GenderResponse>(`/codebook/gender/${id}`),
  create: (data: CreateGenderRequest) =>
    api.post<GenderResponse>('/codebook/gender', data),
  update: (id: string, data: UpdateGenderRequest) =>
    api.put<GenderResponse>(`/codebook/gender/${id}`, data),
  toggleActive: (id: string) =>
    api.patch<GenderResponse>(`/codebook/gender/${id}/toggle-active`),
  deleteById: (id: string) =>
    api.delete(`/codebook/gender/${id}`),
}

// ── Country ───────────────────────────────────────────────────────────

export interface CountryResponse {
  id: string
  code: string
  nameHr: string
  nameEn: string | null
  citizenshipHr: string | null
  citizenshipEn: string | null
  ordinal: number
  isActive: boolean
}

export interface CreateCountryRequest {
  code: string
  nameHr: string
  nameEn?: string
  citizenshipHr?: string
  citizenshipEn?: string
  ordinal: number
  isActive: boolean
}

export interface UpdateCountryRequest {
  code: string
  nameHr: string
  nameEn?: string | null
  citizenshipHr?: string | null
  citizenshipEn?: string | null
  ordinal: number
  isActive: boolean
}

export const countryApi = {
  getAll: (includeInactive = false) =>
    api.get<CountryResponse[]>('/codebook/country', { params: { includeInactive } }),
  getById: (id: string) =>
    api.get<CountryResponse>(`/codebook/country/${id}`),
  create: (data: CreateCountryRequest) =>
    api.post<CountryResponse>('/codebook/country', data),
  update: (id: string, data: UpdateCountryRequest) =>
    api.put<CountryResponse>(`/codebook/country/${id}`, data),
  toggleActive: (id: string) =>
    api.patch<CountryResponse>(`/codebook/country/${id}/toggle-active`),
  deleteById: (id: string) =>
    api.delete(`/codebook/country/${id}`),
}

// ── County ────────────────────────────────────────────────────────────

export interface CountyResponse {
  id: string
  code: string
  nameHr: string
  nameEn: string | null
  ordinal: number
  isActive: boolean
  countryId: string | null
  countryNameHr: string | null
}

export interface CreateCountyRequest {
  code: string
  nameHr: string
  nameEn?: string
  countryId?: string | null
  ordinal: number
  isActive: boolean
}

export interface UpdateCountyRequest {
  code: string
  nameHr: string
  nameEn?: string | null
  countryId?: string | null
  ordinal: number
  isActive: boolean
}

export const countyApi = {
  getAll: (includeInactive = false, countryId?: string | null) =>
    api.get<CountyResponse[]>('/codebook/county', {
      params: { includeInactive, ...(countryId ? { countryId } : {}) },
    }),
  getById: (id: string) =>
    api.get<CountyResponse>(`/codebook/county/${id}`),
  create: (data: CreateCountyRequest) =>
    api.post<CountyResponse>('/codebook/county', data),
  update: (id: string, data: UpdateCountyRequest) =>
    api.put<CountyResponse>(`/codebook/county/${id}`, data),
  toggleActive: (id: string) =>
    api.patch<CountyResponse>(`/codebook/county/${id}/toggle-active`),
  deleteById: (id: string) =>
    api.delete(`/codebook/county/${id}`),
}

// ── Municipality ──────────────────────────────────────────────────────

export interface MunicipalityResponse {
  id: string
  code: string
  nameHr: string
  nameEn: string | null
  ordinal: number
  isActive: boolean
  countyId: string | null
  countyNameHr: string | null
  countryNameHr: string | null
  joppdCode: string | null
}

export interface CreateMunicipalityRequest {
  code: string
  nameHr: string
  nameEn?: string
  countyId?: string | null
  ordinal: number
  isActive: boolean
  joppdCode?: string | null
}

export interface UpdateMunicipalityRequest {
  code: string
  nameHr: string
  nameEn?: string | null
  countyId?: string | null
  ordinal: number
  isActive: boolean
  joppdCode?: string | null
}

export const municipalityApi = {
  getAll: (includeInactive = false, countyId?: string | null) =>
    api.get<MunicipalityResponse[]>('/codebook/municipality', {
      params: { includeInactive, ...(countyId ? { countyId } : {}) },
    }),
  getById: (id: string) =>
    api.get<MunicipalityResponse>(`/codebook/municipality/${id}`),
  create: (data: CreateMunicipalityRequest) =>
    api.post<MunicipalityResponse>('/codebook/municipality', data),
  update: (id: string, data: UpdateMunicipalityRequest) =>
    api.put<MunicipalityResponse>(`/codebook/municipality/${id}`, data),
  toggleActive: (id: string) =>
    api.patch<MunicipalityResponse>(`/codebook/municipality/${id}/toggle-active`),
  deleteById: (id: string) =>
    api.delete(`/codebook/municipality/${id}`),
}

// ── Settlement (City) ─────────────────────────────────────────────────

export interface SettlementResponse {
  id: string
  code: string
  nameHr: string
  nameEn: string | null
  ordinal: number
  isActive: boolean
  municipalityId: string | null
  municipalityNameHr: string | null
  countyNameHr: string | null
  countryNameHr: string | null
}

export interface CreateSettlementRequest {
  code: string
  nameHr: string
  nameEn?: string
  municipalityId?: string | null
  ordinal: number
  isActive: boolean
}

export interface UpdateSettlementRequest {
  code: string
  nameHr: string
  nameEn?: string | null
  municipalityId?: string | null
  ordinal: number
  isActive: boolean
}

export const settlementApi = {
  getAll: (includeInactive = false, municipalityId?: string | null) =>
    api.get<SettlementResponse[]>('/codebook/city', {
      params: { includeInactive, ...(municipalityId ? { municipalityId } : {}) },
    }),
  getById: (id: string) =>
    api.get<SettlementResponse>(`/codebook/city/${id}`),
  create: (data: CreateSettlementRequest) =>
    api.post<SettlementResponse>('/codebook/city', data),
  update: (id: string, data: UpdateSettlementRequest) =>
    api.put<SettlementResponse>(`/codebook/city/${id}`, data),
  toggleActive: (id: string) =>
    api.patch<SettlementResponse>(`/codebook/city/${id}/toggle-active`),
  deleteById: (id: string) =>
    api.delete(`/codebook/city/${id}`),
}
