import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import './i18n'        // inicijalizira i18next prije rendera
import 'antd/dist/reset.css'
import { ModuleRegistry, AllCommunityModule } from 'ag-grid-community'
ModuleRegistry.registerModules([AllCommunityModule])

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
)
