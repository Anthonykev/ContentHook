import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import { AppAuth0Provider } from './auth/auth0-provider.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <AppAuth0Provider>
      <App />
    </AppAuth0Provider>
  </StrictMode>
)