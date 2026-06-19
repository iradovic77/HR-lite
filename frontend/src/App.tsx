import { useState, type ReactElement } from 'react'
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { App as AntApp, ConfigProvider, theme } from 'antd'
import hrHR from 'antd/locale/hr_HR'
import enUS from 'antd/locale/en_US'
import { useTranslation } from 'react-i18next'
import MainLayout from '@/layouts/MainLayout'
import { ThemeContext } from '@/context/ThemeContext'
import { menuConfig } from '@/config/menuConfig'
import ComingSoon from '@/pages/ComingSoon'
import GenderPage from '@/pages/codebook/GenderPage'
import CountryPage from '@/pages/codebook/CountryPage'
import CountyPage from '@/pages/codebook/CountyPage'
import MunicipalityPage from '@/pages/codebook/MunicipalityPage'
import SettlementPage from '@/pages/codebook/SettlementPage'

const STORAGE_KEY = 'hr-lite-theme'

const IMPLEMENTED_PAGES: Record<string, ReactElement> = {
  '/sifarnici/spolovi':         <GenderPage />,
  '/sifarnici/adrese/drzave':   <CountryPage />,
  '/sifarnici/adrese/zupanije': <CountyPage />,
  '/sifarnici/adrese/opcine':   <MunicipalityPage />,
  '/sifarnici/adrese/naselja':  <SettlementPage />,
}

export default function App() {
  const { i18n } = useTranslation()

  const [isDark, setIsDark] = useState<boolean>(
    () => localStorage.getItem(STORAGE_KEY) === 'dark'
  )

  const handleThemeToggle = (dark: boolean) => {
    setIsDark(dark)
    localStorage.setItem(STORAGE_KEY, dark ? 'dark' : 'light')
  }

  const antLocale = i18n.language === 'hr' ? hrHR : enUS

  return (
    <ThemeContext.Provider value={{ isDark, onToggle: handleThemeToggle }}>
    <ConfigProvider
      locale={antLocale}
      theme={{
        algorithm: isDark ? theme.darkAlgorithm : theme.defaultAlgorithm,
        token: {
          colorPrimary: '#1677ff',
          borderRadius: 6,
        },
      }}
    >
      <AntApp>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Navigate to="/sifarnici/spolovi" replace />} />

          <Route
            path="/"
            element={
              <MainLayout isDark={isDark} onThemeToggle={handleThemeToggle} />
            }
          >
            {menuConfig
              .filter(item => item.route)
              .map(item => (
                <Route
                  key={item.id}
                  path={item.route!}
                  element={
                    IMPLEMENTED_PAGES[item.route!] ??
                    <ComingSoon labelKey={item.labelKey} />
                  }
                />
              ))
            }
          </Route>

          <Route path="*" element={<Navigate to="/sifarnici/spolovi" replace />} />
        </Routes>
      </BrowserRouter>
      </AntApp>
    </ConfigProvider>
    </ThemeContext.Provider>
  )
}
