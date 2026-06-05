import { Breadcrumb } from 'antd'
import { useLocation } from 'react-router-dom'
import { useTranslation } from 'react-i18next'

interface RouteEntry {
  labelKey: string
  parent?: string
}

/**
 * Centralna mapa ruta → breadcrumb konfiguracija.
 * Za svaku novu stranicu dodaj ovdje entry s odgovarajućim i18n ključem i parent rutom.
 * Primjer za dublje rute: { labelKey: 'breadcrumbs.employee_edit', parent: '/employees' }
 */
export const ROUTE_CONFIG: Record<string, RouteEntry> = {
  '/codebook':        { labelKey: 'breadcrumbs.codebooks' },
  '/codebook/gender': { labelKey: 'breadcrumbs.gender', parent: '/codebook' },
}

function buildCrumbs(path: string): string[] {
  const config = ROUTE_CONFIG[path]
  if (!config) return []
  if (config.parent) return [...buildCrumbs(config.parent), path]
  return [path]
}

export default function AppBreadcrumbs() {
  const { pathname } = useLocation()
  const { t } = useTranslation()

  const crumbs = buildCrumbs(pathname)
  if (crumbs.length === 0) return null

  return (
    <Breadcrumb
      separator="›"
      items={crumbs.map(path => ({
        title: t(ROUTE_CONFIG[path].labelKey),
      }))}
      style={{ fontSize: 12, lineHeight: '18px' }}
    />
  )
}
