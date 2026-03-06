import { useAuth0 } from '@auth0/auth0-react'
import { LoginButton } from './LoginButton'

export function AuthGuard({ children }) {
  const { isAuthenticated, isLoading } = useAuth0()

  if (isLoading) {
    return <div>Laden...</div>
  }

  if (!isAuthenticated) {
    return (
      <div>
        <h2>Bitte einloggen um ContentHook zu nutzen</h2>
        <LoginButton />
      </div>
    )
  }

  return children
}