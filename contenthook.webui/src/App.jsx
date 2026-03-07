import { useState } from 'react'
import { AuthGuard } from './components/AuthGuard'
import { LogoutButton } from './components/LogoutButton'
import VideoUpload from './components/VideoUpload'
import HistoryPage from './components/HistoryPage'
import RatingPage from './components/RatingPage'

const TABS = [
    { key: 'upload', label: 'Neues Video', icon: '📤' },
    { key: 'history', label: 'Verlauf', icon: '🎬' },
    { key: 'rating', label: 'Bewerten', icon: '⭐' },
]

function App() {
    const [activeTab, setActiveTab] = useState('upload')

    return (
        <AuthGuard>
            <div style={{ minHeight: '100vh', background: '#080810' }}>

                {/* Top Bar */}
                <div style={{
                    background: '#0a0a14',
                    borderBottom: '1px solid #1e1e35',
                    padding: '0 16px',
                    position: 'sticky', top: 0, zIndex: 100
                }}>
                    <div className="container-fluid px-0" style={{ maxWidth: '1100px', margin: '0 auto' }}>
                        <div className="d-flex align-items-center justify-content-between" style={{ height: '56px' }}>

                            {/* Logo */}
                            <span style={{
                                fontWeight: '800', fontSize: '18px',
                                background: 'linear-gradient(135deg, #6366f1, #a78bfa)',
                                WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent'
                            }}>
                                ContentHook
                            </span>

                            {/* Desktop Tabs */}
                            <div className="d-none d-md-flex gap-1">
                                {TABS.map(tab => (
                                    <button
                                        key={tab.key}
                                        className="btn btn-sm"
                                        onClick={() => setActiveTab(tab.key)}
                                        style={{
                                            background: activeTab === tab.key
                                                ? 'rgba(99,102,241,0.15)' : 'transparent',
                                            border: `1px solid ${activeTab === tab.key ? '#6366f1' : 'transparent'}`,
                                            color: activeTab === tab.key ? '#a78bfa' : '#64748b',
                                            borderRadius: '8px', padding: '6px 16px',
                                            fontSize: '13px', fontWeight: activeTab === tab.key ? '600' : '400',
                                            transition: 'all 0.15s'
                                        }}
                                    >
                                        {tab.icon} {tab.label}
                                    </button>
                                ))}
                            </div>

                            <LogoutButton />
                        </div>

                        {/* Mobile Tabs */}
                        <div className="d-flex d-md-none gap-1 pb-2">
                            {TABS.map(tab => (
                                <button
                                    key={tab.key}
                                    className="btn btn-sm flex-grow-1"
                                    onClick={() => setActiveTab(tab.key)}
                                    style={{
                                        background: activeTab === tab.key
                                            ? 'rgba(99,102,241,0.15)' : 'transparent',
                                        border: `1px solid ${activeTab === tab.key ? '#6366f1' : '#1e1e35'}`,
                                        color: activeTab === tab.key ? '#a78bfa' : '#475569',
                                        borderRadius: '8px', padding: '6px 4px',
                                        fontSize: '11px', fontWeight: activeTab === tab.key ? '600' : '400',
                                    }}
                                >
                                    <div>{tab.icon}</div>
                                    <div>{tab.label}</div>
                                </button>
                            ))}
                        </div>
                    </div>
                </div>

                {/* Content */}
                <div className="container-fluid px-3 px-md-4" style={{ maxWidth: '1100px', margin: '0 auto', paddingTop: '24px' }}>
                    {activeTab === 'upload' && <VideoUpload />}
                    {activeTab === 'history' && <HistoryPage />}
                    {activeTab === 'rating' && <RatingPage />}
                </div>

            </div>
        </AuthGuard>
    )
}

export default App