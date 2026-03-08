import { useAuth0 } from '@auth0/auth0-react'

export function LogoutButton() {
    const { logout } = useAuth0()
    return (
        <button
            className="btn btn-sm"
            onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}
            style={{
                background: 'transparent',
                border: '1px solid #2a2a45',
                color: '#475569',
                fontSize: '12px', borderRadius: '7px',
                padding: '5px 12px',
                transition: 'all 0.15s',
            }}
            onMouseEnter={e => {
                e.currentTarget.style.borderColor = '#ef4444'
                e.currentTarget.style.color = '#ef4444'
            }}
            onMouseLeave={e => {
                e.currentTarget.style.borderColor = '#2a2a45'
                e.currentTarget.style.color = '#475569'
            }}
        >
            Logout
        </button>
    )
}