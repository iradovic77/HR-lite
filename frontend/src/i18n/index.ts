import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'
import hr from './locales/hr.json'
import en from './locales/en.json'

const STORAGE_KEY = 'hr-lite-lang'

i18n
  .use(initReactI18next)
  .init({
    resources: {
      hr: { translation: hr },
      en: { translation: en },
    },
    // Učitaj jezik iz localStorage, ili koristi 'hr' kao default
    lng: localStorage.getItem(STORAGE_KEY) ?? 'hr',
    fallbackLng: 'hr',
    interpolation: {
      escapeValue: false, // React već escapa XSS
    },
  })

// Svaki put kad se promijeni jezik, spremi u localStorage
i18n.on('languageChanged', (lang) => {
  localStorage.setItem(STORAGE_KEY, lang)
})

export default i18n
