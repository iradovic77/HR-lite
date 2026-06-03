import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    // Proxy API pozivi prema backend servisima u dev modu
    proxy: {
      '/api/identity':  'http://localhost:5001',
      '/api/employees': 'http://localhost:5002',
      '/api/codebook':  'http://localhost:5006',
    },
  },
})
