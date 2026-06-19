import { Breadcrumb } from 'antd'
import { useLocation, useNavigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { menuConfig, type MenuItemConfig } from '@/config/menuConfig'

interface DropdownItem {
  key: string
  label: string
  children?: DropdownItem[]
  onClick?: () => void
}

function getAncestors(id: string): MenuItemConfig[] {
  const item = menuConfig.find(i => i.id === id)
  if (!item) return []
  if (!item.parentId) return [item]
  return [...getAncestors(item.parentId), item]
}

function getFirstLeaf(id: string): string | undefined {
  const children = menuConfig
    .filter(i => i.parentId === id)
    .sort((a, b) => a.ordinal - b.ordinal)
  for (const child of children) {
    if (child.route) return child.route
    const leaf = getFirstLeaf(child.id)
    if (leaf) return leaf
  }
  return undefined
}

function resolveRoute(item: MenuItemConfig): string | undefined {
  return item.route ?? getFirstLeaf(item.id)
}

function buildDropdownItems(
  parentId: string | null,
  t: (k: string) => string,
  navigate: (route: string) => void,
): DropdownItem[] {
  return menuConfig
    .filter(i => i.parentId === parentId)
    .sort((a, b) => a.ordinal - b.ordinal)
    .map(child => {
      const hasChildren = menuConfig.some(i => i.parentId === child.id)
      if (hasChildren) {
        return {
          key: child.id,
          label: t(child.labelKey),
          children: buildDropdownItems(child.id, t, navigate),
        }
      }
      return {
        key: child.id,
        label: t(child.labelKey),
        onClick: () => {
          const route = resolveRoute(child)
          if (route) navigate(route)
        },
      }
    })
}

export default function AppBreadcrumbs() {
  const { pathname } = useLocation()
  const { t } = useTranslation()
  const navigate = useNavigate()

  const current = menuConfig.find(item => item.route === pathname)
  if (!current) return null

  const crumbs = getAncestors(current.id)
  const lastIndex = crumbs.length - 1

  // Sintetički root "HR-lite" — dropdown s top-level stavkama (rekurzivno)
  const rootBreadcrumb = {
    title: (
      <span style={{ cursor: 'pointer' }} onClick={() => navigate('/sifarnici/spolovi')}>
        {t('app.name')}
      </span>
    ),
    menu: { items: buildDropdownItems(null, t, navigate) as any },
  }

  const ancestorBreadcrumbs = crumbs.map((item, index) => {
    const isLast = index === lastIndex

    // Dropdown prikazuje djecu tog čvora, rekurzivno za grupe
    const hasChildren = menuConfig.some(i => i.parentId === item.id)
    const dropdownItems = !isLast && hasChildren
      ? buildDropdownItems(item.id, t, navigate)
      : undefined

    const title = isLast
      ? t(item.labelKey)
      : (
        <span
          style={{ cursor: 'pointer' }}
          onClick={() => {
            const route = resolveRoute(item)
            if (route && route !== pathname) navigate(route)
          }}
        >
          {t(item.labelKey)}
        </span>
      )

    return {
      title,
      menu: dropdownItems ? { items: dropdownItems as any } : undefined,
    }
  })

  return (
    <Breadcrumb
      separator="›"
      items={[rootBreadcrumb, ...ancestorBreadcrumbs]}
      style={{ fontSize: 12, lineHeight: '18px' }}
    />
  )
}
