import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import 'bootstrap/dist/css/bootstrap.min.css'
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