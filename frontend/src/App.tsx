import { useState } from 'react'
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { ConfigProvider, theme } from 'antd'
import hrHR from 'antd/locale/hr_HR'
import enUS from 'antd/locale/en_US'
import { useTranslation } from 'react-i18next'
import MainLayout from '@/layouts/MainLayout'
import GenderPage from '@/pages/codebook/GenderPage'

const STORAGE_KEY = 'hr-lite-theme'

export default function App() {
  const { i18n } = useTranslation()

  // Light/dark mode — učitaj iz localStorage
  const [isDark, setIsDark] = useState<boolean>(
    () => localStorage.getItem(STORAGE_KEY) === 'dark'
  )

  const handleThemeToggle = (dark: boolean) => {
    setIsDark(dark)
    localStorage.setItem(STORAGE_KEY, dark ? 'dark' : 'light')
  }

  // Ant Design lokalizacija ovisi o odabranom jeziku
  const antLocale = i18n.language === 'hr' ? hrHR : enUS

  return (
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
      <BrowserRouter>
        <Routes>
          {/* Redirect s root-a na prvi ekran */}
          <Route path="/" element={<Navigate to="/codebook/gender" replace />} />

          {/* Zaštićene rute unutar MainLayout-a */}
          <Route
            path="/"
            element={
              <MainLayout isDark={isDark} onThemeToggle={handleThemeToggle} />
            }
          >
            <Route path="codebook/gender" element={<GenderPage />} />
          </Route>

          {/* Fallback */}
          <Route path="*" element={<Navigate to="/codebook/gender" replace />} />
        </Routes>
      </BrowserRouter>
    </ConfigProvider>
  )
}
