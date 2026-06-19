import { createContext, useContext } from 'react'

interface ThemeContextValue {
  isDark: boolean
  onToggle: (dark: boolean) => void
}

export const ThemeContext = createContext<ThemeContextValue>({
  isDark: false,
  onToggle: () => {},
})

export const useTheme = () => useContext(ThemeContext)
