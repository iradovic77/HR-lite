import { ReactNode } from 'react'
import { Typography, theme as antTheme } from 'antd'

const { Title } = Typography

/**
 * Vertikalni offset koji konzumiraju svi "chrome" elementi iznad tijela tablice:
 * MainLayout header(64) + Content margin(48) + CodebookLayout padding(40) +
 * toolbar(38) + tablica header(39) + paginacija(56) + buffer(15)
 */
export const TABLE_SCROLL_Y = 'calc(100vh - 300px)'

interface CodebookLayoutProps {
  title: string
  extra?: ReactNode
  children: ReactNode
}

export default function CodebookLayout({ title, extra, children }: CodebookLayoutProps) {
  const { token } = antTheme.useToken()

  return (
    <div
      style={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        background: token.colorBgContainer,
        borderRadius: token.borderRadiusLG,
        padding: '16px 24px 24px',
      }}
    >
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 16,
          flexShrink: 0,
        }}
      >
        <Title level={4} style={{ margin: 0 }}>
          {title}
        </Title>
        {extra}
      </div>

      {children}
    </div>
  )
}
