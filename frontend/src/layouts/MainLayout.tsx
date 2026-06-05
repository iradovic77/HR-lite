import { useState } from 'react'
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
  theme as antTheme,
} from 'antd'
import {
  BookOutlined,
  ManOutlined,
  MoonOutlined,
  SunOutlined,
} from '@ant-design/icons'
import AppBreadcrumbs from '@/components/AppBreadcrumbs'

const { Sider, Header, Content } = Layout
const { Text } = Typography

interface MainLayoutProps {
  isDark: boolean
  onThemeToggle: (dark: boolean) => void
}

// Putanja → i18n ključ naslova stranice
const PAGE_TITLES: Record<string, string> = {
  '/codebook/gender': 'codebook.gender.title',
}

export default function MainLayout({ isDark, onThemeToggle }: MainLayoutProps) {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const location = useLocation()
  const [collapsed, setCollapsed] = useState(false)
  const { token } = antTheme.useToken()

  const pageTitle = PAGE_TITLES[location.pathname]
    ? t(PAGE_TITLES[location.pathname])
    : t('app.name')

  const menuItems = [
    {
      key: 'codebooks',
      icon: <BookOutlined />,
      label: t('nav.codebooks'),
      children: [
        {
          key: '/codebook/gender',
          icon: <ManOutlined />,
          label: t('nav.gender'),
        },
      ],
    },
  ]

  const languageOptions = [
    { value: 'hr', label: '🇭🇷 HR' },
    { value: 'en', label: '🇬🇧 EN' },
  ]

  return (
    <Layout style={{ height: '100vh' }}>
      {/* ── Sidebar ─────────────────────────────────────── */}
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        style={{ background: token.colorBgContainer }}
      >
        {/* Logo / naziv aplikacije */}
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

        <Menu
          mode="inline"
          selectedKeys={[location.pathname]}
          defaultOpenKeys={['codebooks']}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
          style={{ borderRight: 0, marginTop: 8 }}
        />
      </Sider>

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
          {/* Lijeva strana: naslov + breadcrumbs */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 2, padding: '10px 0' }}>
            <Text strong style={{ fontSize: 16, lineHeight: '22px' }}>{pageTitle}</Text>
            <AppBreadcrumbs />
          </div>

          {/* Desni dio topbara */}
          <Space size="middle">
            {/* Dropdown za odabir jezika */}
            <Select
              value={i18n.language}
              onChange={(lang) => i18n.changeLanguage(lang)}
              options={languageOptions}
              style={{ width: 90 }}
              size="small"
            />

            {/* Toggle za light/dark mode */}
            <Tooltip title={isDark ? t('topbar.theme_light') : t('topbar.theme_dark')}>
              <Switch
                checked={isDark}
                onChange={onThemeToggle}
                checkedChildren={<MoonOutlined />}
                unCheckedChildren={<SunOutlined />}
              />
            </Tooltip>

            {/* Avatar prijavljenog korisnika */}
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
