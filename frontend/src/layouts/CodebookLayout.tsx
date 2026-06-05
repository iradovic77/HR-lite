import { ReactNode } from 'react'
import { Typography, theme as antTheme } from 'antd'

const { Title } = Typography

interface CodebookLayoutProps {
  title: string
  extra?: ReactNode
  pagination?: ReactNode
  children: ReactNode
}

export default function CodebookLayout({ title, extra, pagination, children }: CodebookLayoutProps) {
  const { token } = antTheme.useToken()

  return (
    <div
      style={{
        flex: 1,
        display: 'flex',
        flexDirection: 'column',
        background: token.colorBgContainer,
        borderRadius: token.borderRadiusLG,
        padding: '16px 24px 16px',
        overflow: 'hidden',
        minHeight: 0,
      }}
    >
      {/* Toolbar */}
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

      {/* Tablica — AG Grid ili Ant Table */}
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden', minHeight: 0 }}>
        {children}
      </div>

      {/* Paginacija — uvijek fiksirana na dnu */}
      {pagination && (
        <div
          style={{
            flexShrink: 0,
            display: 'flex',
            justifyContent: 'flex-end',
            paddingTop: 12,
            borderTop: `1px solid ${token.colorBorderSecondary}`,
          }}
        >
          {pagination}
        </div>
      )}
    </div>
  )
}
