import { Breadcrumb } from 'antd'
import { Link, useLocation } from 'react-router-dom'
import { useTranslation } from 'react-i18next'

interface RouteEntry {
  labelKey: string
  parent?: string
}

export const ROUTE_CONFIG: Record<string, RouteEntry> = {
  '/codebook':              { labelKey: 'breadcrumbs.codebooks' },
  '/codebook/gender':       { labelKey: 'breadcrumbs.gender',       parent: '/codebook' },
  '/codebook/country':      { labelKey: 'breadcrumbs.country',      parent: '/codebook' },
  '/codebook/county':       { labelKey: 'breadcrumbs.county',       parent: '/codebook' },
  '/codebook/municipality': { labelKey: 'breadcrumbs.municipality', parent: '/codebook' },
  '/codebook/city':         { labelKey: 'breadcrumbs.city',         parent: '/codebook' },
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

  const lastIndex = crumbs.length - 1

  return (
    <Breadcrumb
      separator="›"
      items={crumbs.map((path, index) => ({
        title: index === lastIndex
          ? t(ROUTE_CONFIG[path].labelKey)
          : <Link to={path}>{t(ROUTE_CONFIG[path].labelKey)}</Link>,
      }))}
      style={{ fontSize: 12, lineHeight: '18px' }}
    />
  )
}
