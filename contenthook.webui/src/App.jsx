import { AuthGuard } from './components/AuthGuard'
import { LogoutButton } from './components/LogoutButton'
import VideoUpload from './components/VideoUpload'

function App() {
  return (
    <AuthGuard>
      <div>
        <LogoutButton />
        <VideoUpload />
      </div>
    </AuthGuard>
  )
}

export default App