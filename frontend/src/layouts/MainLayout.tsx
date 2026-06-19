import { useMemo, useState } from 'react'
import { Outlet, useNavigate, useLocation } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import {
  Layout,
  Menu,
  Select,
  Switch,
  Avatar,
  Space,
  Typography,
  Tooltip,
  Input,
  theme as antTheme,
} from 'antd'
import {
  MoonOutlined,
  SunOutlined,
  LeftOutlined,
  SearchOutlined,
} from '@ant-design/icons'
import AppBreadcrumbs from '@/components/AppBreadcrumbs'
import { menuConfig, buildMenuTree, type MenuItemConfig } from '@/config/menuConfig'

const { Sider, Header, Content } = Layout
const { Text } = Typography

interface MainLayoutProps {
  isDark: boolean
  onThemeToggle: (dark: boolean) => void
}

const SIDER_WIDTH            = 264
const SIDER_COLLAPSED_WIDTH  = 80
const TOGGLE_BTN_SIZE        = 18
const FILTER_BAR_HEIGHT      = 48

function getAncestorIds(route: string): string[] {
  const item = menuConfig.find(i => i.route === route)
  if (!item) return []
  const ancestors: string[] = []
  let parentId = item.parentId
  while (parentId !== null) {
    ancestors.push(parentId)
    const parent = menuConfig.find(i => i.id === parentId)
    parentId = parent?.parentId ?? null
  }
  return ancestors
}

function filterItems(items: MenuItemConfig[], term: string, t: (k: string) => string): MenuItemConfig[] {
  if (!term.trim()) return items
  const normalized = term.trim().toLowerCase()
  const matchingIds = new Set<string>()
  items.forEach(item => {
    if (t(item.labelKey).toLowerCase().includes(normalized)) {
      matchingIds.add(item.id)
      let parentId = item.parentId
      while (parentId !== null) {
        matchingIds.add(parentId)
        const parent = items.find(i => i.id === parentId)
        parentId = parent?.parentId ?? null
      }
    }
  })
  return items.filter(item => matchingIds.has(item.id))
}

export default function MainLayout({ isDark, onThemeToggle }: MainLayoutProps) {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const location = useLocation()
  const [collapsed, setCollapsed] = useState(false)
  const [btnHovered, setBtnHovered] = useState(false)
  const [openKeys, setOpenKeys] = useState<string[]>(() =>
    getAncestorIds(location.pathname)
  )
  const [filterTerm, setFilterTerm] = useState('')
  const { token } = antTheme.useToken()

  const currentItem = menuConfig.find(i => i.route === location.pathname)
  const pageTitle = currentItem ? t(currentItem.labelKey) : t('app.name')

  const activeItems = useMemo(
    () => filterItems(menuConfig, filterTerm, t),
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [filterTerm, i18n.language]
  )

  const menuItems = useMemo(
    () => buildMenuTree(activeItems, t),
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [activeItems, i18n.language]
  )

  // Kad filter aktiviran — expandaj sve matchane grupe automatski
  const activeOpenKeys = filterTerm.trim()
    ? activeItems.filter(item => activeItems.some(c => c.parentId === item.id)).map(i => i.id)
    : openKeys

  const languageOptions = [
    { value: 'hr', label: '🇭🇷 HR' },
    { value: 'en', label: '🇬🇧 EN' },
  ]

  const siderWidth = collapsed ? SIDER_COLLAPSED_WIDTH : SIDER_WIDTH

  return (
    <Layout style={{ height: '100vh', position: 'relative' }}>
      {/* ── Sidebar ─────────────────────────────────────── */}
      <Sider
        trigger={null}
        collapsible
        collapsed={collapsed}
        width={SIDER_WIDTH}
        collapsedWidth={SIDER_COLLAPSED_WIDTH}
        style={{ background: token.colorBgContainer, overflow: 'hidden' }}
      >
        {/* Logo / naziv aplikacije — fiksirano na vrhu */}
        <div style={{
          height: 64,
          display: 'flex',
          alignItems: 'center',
          justifyContent: collapsed ? 'center' : 'flex-start',
          padding: collapsed ? 0 : '0 16px',
          borderBottom: `1px solid ${token.colorBorderSecondary}`,
        }}>
          <Text strong style={{ fontSize: collapsed ? 14 : 18, color: token.colorPrimary }}>
            {collapsed ? t('app.name_short') : t('app.name')}
          </Text>
        </div>

        {/* Menu — scroll između logoa i filter inputa */}
        <div
          className="sidebar-scroll"
          style={{
            position: 'absolute',
            top: 64,
            left: 0,
            right: 0,
            bottom: collapsed ? 0 : FILTER_BAR_HEIGHT,
            overflowY: 'auto',
            overflowX: 'hidden',
          }}
        >
          <Menu
            mode="inline"
            inlineCollapsed={collapsed}
            selectedKeys={[location.pathname]}
            openKeys={collapsed ? [] : activeOpenKeys}
            onOpenChange={keys => { if (!filterTerm.trim()) setOpenKeys(keys) }}
            items={menuItems}
            onClick={({ key }) => { if (key.startsWith('/')) navigate(key) }}
            style={{ borderRight: 0, marginTop: 8 }}
          />
        </div>

        {/* Filter input — fiksirano na dnu, sakriven kad je collapsed */}
        {!collapsed && (
          <div style={{
            position: 'absolute',
            bottom: 0,
            left: 0,
            right: 0,
            height: FILTER_BAR_HEIGHT,
            display: 'flex',
            alignItems: 'center',
            padding: '0 12px',
            borderTop: `1px solid ${token.colorBorderSecondary}`,
            background: token.colorBgContainer,
          }}>
            <Input
              size="small"
              placeholder={t('menu.filterPlaceholder')}
              prefix={<SearchOutlined style={{ color: token.colorTextTertiary }} />}
              allowClear
              value={filterTerm}
              onChange={e => setFilterTerm(e.target.value)}
            />
          </div>
        )}
      </Sider>

      {/* ── Collapse toggle button ────────────────────────── */}
      <button
        onClick={() => setCollapsed(c => !c)}
        onMouseEnter={() => setBtnHovered(true)}
        onMouseLeave={() => setBtnHovered(false)}
        style={{
          position: 'absolute',
          left: siderWidth - TOGGLE_BTN_SIZE / 2,
          top: '50%',
          transform: 'translateY(-50%)',
          width: TOGGLE_BTN_SIZE,
          height: TOGGLE_BTN_SIZE,
          borderRadius: '50%',
          border: `1px solid ${btnHovered ? token.colorPrimary : token.colorBorderSecondary}`,
          background: btnHovered ? token.colorPrimaryBg : token.colorBgContainer,
          boxShadow: btnHovered
            ? `0 0 0 2px ${token.colorPrimaryBg}`
            : '0 1px 4px rgba(0,0,0,0.12)',
          cursor: 'pointer',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          padding: 0,
          zIndex: 100,
          color: btnHovered ? token.colorPrimary : token.colorTextTertiary,
          transition: 'left 0.2s, border-color 0.2s, background 0.2s, color 0.2s, box-shadow 0.2s',
          outline: 'none',
        }}
      >
        <LeftOutlined style={{
          fontSize: 9,
          transform: collapsed ? 'rotate(180deg)' : 'rotate(0deg)',
          transition: 'transform 0.2s',
          display: 'block',
          lineHeight: 1,
        }} />
      </button>

      <Layout>
        {/* ── Topbar ──────────────────────────────────────── */}
        <Header style={{
          background: token.colorBgContainer,
          borderBottom: `1px solid ${token.colorBorderSecondary}`,
          padding: '0 24px',
          height: 'auto',
          lineHeight: 'normal',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
        }}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 2, padding: '10px 0' }}>
            <Text strong style={{ fontSize: 16, lineHeight: '22px' }}>{pageTitle}</Text>
            <AppBreadcrumbs />
          </div>

          <Space size="middle">
            <Select
              value={i18n.language}
              onChange={(lang) => i18n.changeLanguage(lang)}
              options={languageOptions}
              style={{ width: 90 }}
              size="small"
            />

            <Tooltip title={isDark ? t('topbar.theme_light') : t('topbar.theme_dark')}>
              <Switch
                checked={isDark}
                onChange={onThemeToggle}
                checkedChildren={<MoonOutlined />}
                unCheckedChildren={<SunOutlined />}
              />
            </Tooltip>

            <Tooltip title={t('topbar.user_role')}>
              <Avatar
                style={{ backgroundColor: token.colorPrimary, cursor: 'pointer' }}
                size="small"
              >
                AD
              </Avatar>
            </Tooltip>
          </Space>
        </Header>

        {/* ── Glavni sadržaj ──────────────────────────────── */}
        <Content style={{ margin: 24, overflow: 'hidden', display: 'flex', flexDirection: 'column', minHeight: 0 }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  )
}
