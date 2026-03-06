import { useAuth0 } from '@auth0/auth0-react'

export function LogoutButton() {
  const { logout, user } = useAuth0()

  return (
    <div>
      <span>{user?.email}</span>
      <button onClick={() => logout({
        logoutParams: { returnTo: window.location.origin }
      })}>
        Logout
      </button>
    </div>
  )
}