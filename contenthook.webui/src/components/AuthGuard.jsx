import { useAuth0 } from '@auth0/auth0-react'

export function AuthGuard({ children }) {
    const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0()

    // Auth0 lädt noch
    if (isLoading) {
        return (
            <div style={{
                minHeight: '100vh', background: '#080810',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}>
                <div style={{
                    width: '36px', height: '36px',
                    border: '3px solid #1e1e35',
                    borderTop: '3px solid #6366f1',
                    borderRadius: '50%',
                    animation: 'spin 0.8s linear infinite',
                }} />
                <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
            </div>
        )
    }

    // ContentHook Login-Seite
    if (!isAuthenticated) {
        return (
            <div style={{
                minHeight: '100vh', background: '#080810',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                fontFamily: "'DM Sans','Segoe UI',sans-serif",
                padding: '20px',
            }}>
                <div style={{
                    background: '#0a0a14',
                    border: '1px solid #1e1e35',
                    borderRadius: '16px',
                    padding: '48px 40px',
                    width: '100%',
                    maxWidth: '420px',
                    textAlign: 'center',
                }}>

                    {/* Logo */}
                    <div style={{ marginBottom: '8px' }}>
                        <span style={{
                            fontWeight: '800', fontSize: '32px',
                        }}>
                            <span style={{ color: '#a78bfa' }}>Content</span>
                            <span style={{ color: '#fff' }}>Hook</span>
                        </span>
                    </div>

                    {/* Subtitle */}
                    <p style={{
                        color: '#475569', fontSize: '15px',
                        margin: '0 0 40px', lineHeight: '1.5',
                    }}>
                        KI-gestützte Titel, Hooks und Hashtags<br />
                        für TikTok & Instagram
                    </p>

                    {/* Divider */}
                    <div style={{
                        height: '1px', background: '#1e1e35',
                        marginBottom: '32px',
                    }} />

                    {/* Anmelden Button */}
                    <button
                        onClick={() => loginWithRedirect()}
                        style={{
                            width: '100%',
                            background: 'linear-gradient(135deg, #6366f1, #a78bfa)',
                            border: 'none', color: '#fff',
                            fontWeight: '700', fontSize: '16px',
                            borderRadius: '10px', padding: '14px',
                            cursor: 'pointer',
                            transition: 'opacity 0.15s',
                            fontFamily: "'DM Sans','Segoe UI',sans-serif",
                        }}
                        onMouseEnter={e => e.currentTarget.style.opacity = '0.85'}
                        onMouseLeave={e => e.currentTarget.style.opacity = '1'}
                    >
                        🔐 Jetzt anmelden
                    </button>

                    <p style={{
                        margin: '20px 0 0',
                        color: '#334155', fontSize: '12px',
                    }}>
                        Weiterleitung zu Auth0 · Sichere Anmeldung
                    </p>
                </div>
            </div>
        )
    }

    return children
}