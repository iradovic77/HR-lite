import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 3000,
    proxy: {
      '/api/identity':  'http://localhost:5001',
      '/api/employees': 'http://localhost:5002',
      '/api/codebook':  'http://localhost:5006',
    },
  },
})
