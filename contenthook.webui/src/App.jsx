import { useState, useEffect, useCallback } from 'react'
import { useAuth0 } from '@auth0/auth0-react'
import { AuthGuard } from './components/AuthGuard'
import VideoUpload from './components/VideoUpload'
import HistoryPage from './components/HistoryPage'
import RatingPage from './components/RatingPage'

const NAV_ITEMS = [
    { key: 'upload', icon: '📤', label: 'Neues Video' },
    { key: 'rating', icon: '⭐', label: 'Bewerten' },
    { key: 'impressum', icon: 'ℹ️', label: 'Impressum' },
]

function DeleteModal({ onConfirm, onCancel }) {
    return (
        <>
            <div className="app-modal-backdrop" onClick={onCancel} />

            <div className="app-modal-card" role="dialog" aria-modal="true" aria-labelledby="delete-modal-title">
                <div className="app-modal-icon">🗑️</div>
                <h3 id="delete-modal-title" className="app-modal-title">
                    Job löschen?
                </h3>
                <p className="app-modal-text">
                    Dieser Vorgang kann nicht rückgängig gemacht werden.
                </p>
                <div className="d-flex gap-2">
                    <button onClick={onCancel} className="btn app-btn app-btn-secondary flex-fill">
                        Abbrechen
                    </button>
                    <button onClick={onConfirm} className="btn app-btn app-btn-danger flex-fill">
                        Löschen
                    </button>
                </div>
            </div>
        </>
    )
}

function ImpressumPage() {
    return (
        <div className="row justify-content-center">
            <div className="col-12 col-lg-8 col-xl-6">
                <div className="page-section-card">
                    <h2 className="page-title mb-3">Impressum</h2>
                    <div className="page-copy">
                        <p className="mb-2 text-white fw-semibold">ContentHook</p>
                        <p className="mb-0">Bachelorarbeit — FH Technikum Wien</p>
                        <p className="mb-0">Kodzo-Kevin-Anthony</p>
                        <p className="mt-3 mb-0 text-secondary-custom">
                            Diese Anwendung wurde im Rahmen einer wissenschaftlichen Bachelorarbeit entwickelt.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    )
}

function SidebarContent({
    history,
    selectedJobId,
    activeTab,
    hoveredDelete,
    onSelectJob,
    onNewVideo,
    onHoverDelete,
    onDeleteRequest,
}) {
    return (
        <div className="sidebar-content">
            <div className="sidebar-create-wrap">
                <button onClick={onNewVideo} className="btn w-100 app-btn app-btn-primary">
                    + Neues Video
                </button>
            </div>

            <div className="sidebar-scroll">
                <div className="sidebar-section-title">Verlauf</div>

                {history.length === 0 && <div className="sidebar-empty">Noch keine Videos</div>}

                {history.map(item => {
                    const isSelected = selectedJobId === item.jobId && activeTab === 'history'

                    return (
                        <div
                            key={item.jobId}
                            className={`history-item ${isSelected ? 'active' : ''}`}
                            onClick={() => onSelectJob(item.jobId)}
                        >
                            <div className="history-item-main">
                                <div className="history-item-title">
                                    🎬 {item.originalFileName?.replace(/\.(mp4|mov)$/i, '') ?? 'Video'}
                                </div>
                            </div>

                            <button
                                onClick={e => {
                                    e.stopPropagation()
                                    onDeleteRequest(item.jobId)
                                }}
                                onMouseEnter={() => onHoverDelete(item.jobId)}
                                onMouseLeave={() => onHoverDelete(null)}
                                className={`history-delete-btn ${hoveredDelete === item.jobId ? 'hovered' : ''}`}
                                aria-label="Löschen"
                            >
                                🗑️
                            </button>
                        </div>
                    )
                })}
            </div>
        </div>
    )
}

function AppInner() {
    const { getAccessTokenSilently, logout, user } = useAuth0()
    const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050'

    const [activeTab, setActiveTab] = useState('upload')
    const [sidebarOpen, setSidebarOpen] = useState(() => window.innerWidth >= 768)
    const [selectedJobId, setSelectedJobId] = useState(null)
    const [history, setHistory] = useState([])
    const [hoveredDelete, setHoveredDelete] = useState(null)
    const [deleteTarget, setDeleteTarget] = useState(null)
    const [isMobile, setIsMobile] = useState(() => window.innerWidth < 768)

    useEffect(() => {
        const handler = () => setIsMobile(window.innerWidth < 768)
        window.addEventListener('resize', handler)
        return () => window.removeEventListener('resize', handler)
    }, [])

    useEffect(() => {
        setSidebarOpen(!isMobile)
    }, [isMobile])

    useEffect(() => {
        let active = true

        async function load() {
            try {
                const token = await getAccessTokenSilently({
                    authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE },
                })
                const res = await fetch(`${apiUrl}/api/history`, {
                    headers: { Authorization: `Bearer ${token}` },
                })
                if (!res.ok || !active) return
                setHistory(await res.json())
            } catch (err) {
                console.error('History fetch error:', err)
            }
        }

        load()
        return () => {
            active = false
        }
    }, [getAccessTokenSilently, apiUrl])

    const fetchHistory = useCallback(async () => {
        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE },
            })
            const res = await fetch(`${apiUrl}/api/history`, {
                headers: { Authorization: `Bearer ${token}` },
            })
            if (!res.ok) return
            setHistory(await res.json())
        } catch (err) {
            console.error('History refresh error:', err)
        }
    }, [getAccessTokenSilently, apiUrl])

    const handleJobDone = useCallback(
        (jobId) => {
            fetchHistory()
            setSelectedJobId(jobId)
            setActiveTab('history')
        },
        [fetchHistory]
    )

    function handleSelectJob(jobId) {
        setSelectedJobId(jobId)
        setActiveTab('history')
        if (isMobile) setSidebarOpen(false)
    }

    function handleNewVideo() {
        setActiveTab('upload')
        if (isMobile) setSidebarOpen(false)
    }

    function handleDeleteRequest(jobId) {
        setDeleteTarget(jobId)
    }

    async function handleDeleteConfirm() {
        const jobId = deleteTarget
        setDeleteTarget(null)

        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE },
            })
            const res = await fetch(`${apiUrl}/api/jobs/${jobId}`, {
                method: 'DELETE',
                headers: { Authorization: `Bearer ${token}` },
            })
            if (!res.ok) throw new Error(`HTTP ${res.status}`)
            setHistory(prev => prev.filter(h => h.jobId !== jobId))
            if (selectedJobId === jobId) setSelectedJobId(null)
        } catch (err) {
            console.error('Delete failed:', err)
        }
    }

    const initials = user?.name
        ? user.name
            .split(' ')
            .map(w => w[0])
            .join('')
            .slice(0, 2)
            .toUpperCase()
        : (user?.email?.[0]?.toUpperCase() ?? 'K')

    return (
        <div className="app-shell">
            <div className="app-glow" />

            {deleteTarget && <DeleteModal onConfirm={handleDeleteConfirm} onCancel={() => setDeleteTarget(null)} />}

            <nav className="app-topbar">
                <div className="app-topbar-left">
                    <button
                        onClick={() => setSidebarOpen(v => !v)}
                        className="app-icon-btn"
                        aria-label="Sidebar umschalten"
                    >
                        ☰
                    </button>

                    <button type="button" className="app-brand-btn" onClick={handleNewVideo} aria-label="Zur Upload-Seite wechseln">
                        <span className="app-brand">
                            <span className="app-brand-accent">Content</span>
                            <span className="text-white">Hook</span>
                        </span>
                    </button>
                </div>

                <div className="d-none d-md-flex gap-2">
                    {NAV_ITEMS.map(item => (
                        <button
                            key={item.key}
                            onClick={() => setActiveTab(item.key)}
                            className={`app-nav-btn ${activeTab === item.key ? 'active' : ''}`}
                        >
                            <span>{item.icon}</span>
                            <span>{item.label}</span>
                        </button>
                    ))}
                </div>

                <div className="app-user-box">
                    <div className="app-avatar">{initials}</div>
                    <button
                        onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}
                        className="app-logout-btn"
                    >
                        Logout
                    </button>
                </div>
            </nav>

            <div className="app-body">
                {sidebarOpen && (
                    <aside className="app-sidebar-desktop d-none d-md-flex flex-column">
                        <SidebarContent
                            history={history}
                            selectedJobId={selectedJobId}
                            activeTab={activeTab}
                            hoveredDelete={hoveredDelete}
                            onSelectJob={handleSelectJob}
                            onNewVideo={handleNewVideo}
                            onHoverDelete={setHoveredDelete}
                            onDeleteRequest={handleDeleteRequest}
                        />

                        <div className="app-sidebar-footer">{user?.email ?? ''}</div>
                    </aside>
                )}

                {sidebarOpen && isMobile && <div className="app-sidebar-backdrop" onClick={() => setSidebarOpen(false)} />}

                <aside className={`app-sidebar-mobile d-md-none ${sidebarOpen ? 'open' : ''}`}>
                    <div className="app-sidebar-mobile-header">
                        <button type="button" className="app-brand-btn" onClick={handleNewVideo} aria-label="Zur Upload-Seite wechseln">
                            <span className="app-brand app-brand-mobile">
                                <span className="app-brand-accent">Content</span>
                                <span className="text-white">Hook</span>
                            </span>
                        </button>
                        <button onClick={() => setSidebarOpen(false)} className="app-icon-btn" aria-label="Sidebar schließen">
                            ✕
                        </button>
                    </div>

                    <div className="app-sidebar-mobile-body">
                        <SidebarContent
                            history={history}
                            selectedJobId={selectedJobId}
                            activeTab={activeTab}
                            hoveredDelete={hoveredDelete}
                            onSelectJob={handleSelectJob}
                            onNewVideo={handleNewVideo}
                            onHoverDelete={setHoveredDelete}
                            onDeleteRequest={handleDeleteRequest}
                        />
                    </div>

                    <div className="app-sidebar-footer">{user?.email ?? ''}</div>
                </aside>

                <main className="app-main">
                    <div className="app-mobile-tabs d-flex d-md-none">
                        {NAV_ITEMS.map(item => (
                            <button
                                key={item.key}
                                onClick={() => setActiveTab(item.key)}
                                className={`app-mobile-tab-btn ${activeTab === item.key ? 'active' : ''}`}
                            >
                                <span className="app-mobile-tab-icon">{item.icon}</span>
                                <span>{item.label}</span>
                            </button>
                        ))}
                    </div>

                    <div className="app-page-scroll">
                        <div className="app-page-container">
                            {activeTab === 'upload' && <VideoUpload onJobDone={handleJobDone} />}
                            {activeTab === 'history' && (
                                <HistoryPage
                                    selectedJobId={selectedJobId}
                                    setSelectedJobId={setSelectedJobId}
                                    onRefreshHistory={fetchHistory}
                                />
                            )}
                            {activeTab === 'rating' && <RatingPage />}
                            {activeTab === 'impressum' && <ImpressumPage />}
                        </div>
                    </div>
                </main>
            </div>
        </div>
    )
}

export default function App() {
    return (
        <AuthGuard>
            <AppInner />
        </AuthGuard>
    )
}