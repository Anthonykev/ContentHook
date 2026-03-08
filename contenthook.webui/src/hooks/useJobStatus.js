import { useEffect, useState } from 'react'
import { useAuth0 } from '@auth0/auth0-react'
import * as signalR from '@microsoft/signalr'

export function useJobStatus(jobId) {
    const { getAccessTokenSilently, isAuthenticated } = useAuth0()

    const [status, setStatus] = useState(null)
    const [payload, setPayload] = useState(null)
    const [error, setError] = useState(null)

    useEffect(() => {
        if (!jobId || !isAuthenticated) return

        let mounted = true
        const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050'

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${apiUrl}/hubs/progress`, {
                accessTokenFactory: async () =>
                    await getAccessTokenSilently({
                        authorizationParams: {
                            audience: import.meta.env.VITE_AUTH0_AUDIENCE,
                        },
                    }),
            })
            .withAutomaticReconnect()
            .build()

        connection.on('JobStatus', (data) => {
            if (!mounted || !data) return

            console.log('SignalR JobStatus:', data)

            setStatus(data.status ?? null)
            setPayload(data.payload ?? null)

            if (data.status === 'failed') {
                setError(data.payload?.error ?? 'Unknown error')
            } else {
                setError(null)
            }
        })

        connection.onreconnected(async () => {
            try {
                console.log('SignalR reconnected, rejoining group:', jobId)
                await connection.invoke('JoinJobGroup', jobId)
            } catch (err) {
                console.error('SignalR rejoin failed:', err)
            }
        })

        connection.start()
            .then(() => {
                console.log('SignalR connected, joining group:', jobId)
                return connection.invoke('JoinJobGroup', jobId)
            })
            .catch((err) => {
                console.error('SignalR connection failed:', err)
                if (mounted) setError('Connection failed')
            })

        return () => {
            mounted = false

            connection.invoke('LeaveJobGroup', jobId)
                .catch(() => { })
                .finally(() => connection.stop().catch(() => { }))
        }
    }, [jobId, isAuthenticated, getAccessTokenSilently])

    return { status, payload, error }
}