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
  GlobalOutlined,
  ApartmentOutlined,
  EnvironmentOutlined,
  HomeOutlined,
  MoonOutlined,
  SunOutlined,
  LeftOutlined,
} from '@ant-design/icons'
import AppBreadcrumbs from '@/components/AppBreadcrumbs'

const { Sider, Header, Content } = Layout
const { Text } = Typography

interface MainLayoutProps {
  isDark: boolean
  onThemeToggle: (dark: boolean) => void
}

const PAGE_TITLES: Record<string, string> = {
  '/codebook/gender':       'codebook.gender.title',
  '/codebook/country':      'codebook.country.title',
  '/codebook/county':       'codebook.county.title',
  '/codebook/municipality': 'codebook.municipality.title',
  '/codebook/city':         'codebook.city.title',
}

const SIDER_WIDTH          = 200
const SIDER_COLLAPSED_WIDTH = 80
const TOGGLE_BTN_SIZE       = 18

export default function MainLayout({ isDark, onThemeToggle }: MainLayoutProps) {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const location = useLocation()
  const [collapsed, setCollapsed] = useState(false)
  const [btnHovered, setBtnHovered] = useState(false)
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
        { key: '/codebook/gender',       icon: <ManOutlined />,         label: t('nav.gender') },
        { key: '/codebook/country',      icon: <GlobalOutlined />,      label: t('nav.country') },
        { key: '/codebook/county',       icon: <ApartmentOutlined />,   label: t('nav.county') },
        { key: '/codebook/municipality', icon: <EnvironmentOutlined />, label: t('nav.municipality') },
        { key: '/codebook/city',         icon: <HomeOutlined />,        label: t('nav.city') },
      ],
    },
  ]

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
          {/* Lijeva strana: naslov + breadcrumbs */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 2, padding: '10px 0' }}>
            <Text strong style={{ fontSize: 16, lineHeight: '22px' }}>{pageTitle}</Text>
            <AppBreadcrumbs />
          </div>

          {/* Desni dio topbara */}
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
