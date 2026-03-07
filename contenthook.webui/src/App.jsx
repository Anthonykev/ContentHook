import { AuthGuard } from './components/AuthGuard'
import { LogoutButton } from './components/LogoutButton'
import VideoUpload from './components/VideoUpload'
import HistoryPage from './components/HistoryPage'

function App() {
    return (
        <AuthGuard>
            <div className="container py-4">
                <LogoutButton />
                <VideoUpload />
                <hr />
                <HistoryPage />
            </div>
        </AuthGuard>
    )
}


export default App