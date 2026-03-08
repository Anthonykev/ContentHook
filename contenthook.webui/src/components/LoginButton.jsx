import { useAuth0 } from '@auth0/auth0-react'

export function LoginButton() {
    const { loginWithRedirect, isLoading } = useAuth0()
    return (
        <button
            className="btn"
            disabled={isLoading}
            onClick={() => loginWithRedirect()}
            style={{
                background: 'linear-gradient(135deg, #6366f1, #a78bfa)',
                border: 'none', color: '#fff',
                fontWeight: '600', fontSize: '14px',
                borderRadius: '10px', padding: '12px 32px',
                cursor: isLoading ? 'default' : 'pointer',
                opacity: isLoading ? 0.7 : 1,
            }}
        >
            {isLoading ? '⏳ Lädt…' : '🔐 Anmelden'}
        </button>
    )
}