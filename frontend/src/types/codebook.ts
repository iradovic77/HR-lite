// Tipovi za codebook entitete

export interface GenderItem {
  id: string
  code: string
  nameHr: string
  nameEn: string
  ordinal: number
  isActive: boolean
}

// Generički tip za listu s paginacijom (prema CLAUDE.md API dizajn pravilima)
export interface PagedResponse<T> {
  data: T[]
  page: number
  pageSize: number
  totalCount: number
}
