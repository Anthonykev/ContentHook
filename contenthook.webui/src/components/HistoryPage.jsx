import { useEffect, useState } from 'react'
import { useAuth0 } from '@auth0/auth0-react'

export default function HistoryPage() {
    const { getAccessTokenSilently } = useAuth0()
    const [history, setHistory] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [deletingId, setDeletingId] = useState(null)

    useEffect(() => {
        const fetchHistory = async () => {
            try {
                const token = await getAccessTokenSilently()
                const res = await fetch(`${import.meta.env.VITE_API_URL}/api/history`, {
                    headers: { Authorization: `Bearer ${token}` }
                })
                if (!res.ok) throw new Error(`Fehler: ${res.status}`)
                const data = await res.json()
                setHistory(data)
            } catch (err) {
                setError(err.message)
            } finally {
                setLoading(false)
            }
        }
        fetchHistory()
    }, [getAccessTokenSilently])

    async function handleDelete(jobId) {
        if (!window.confirm('Job wirklich löschen?')) return
        setDeletingId(jobId)
        try {
            const token = await getAccessTokenSilently()
            const res = await fetch(`${import.meta.env.VITE_API_URL}/api/jobs/${jobId}`, {
                method: 'DELETE',
                headers: { Authorization: `Bearer ${token}` }
            })
            if (!res.ok) throw new Error(`Fehler: ${res.status}`)
            setHistory(prev => prev.filter(item => item.jobId !== jobId))
        } catch (err) {
            alert('Löschen fehlgeschlagen: ' + err.message)
        } finally {
            setDeletingId(null)
        }
    }

    const getStatusBadge = (status) => {
        const map = {
            Done: 'success',
            Failed: 'danger',
            Transcribing: 'warning',
            Transcribed: 'info',
            Generating: 'primary',
            Queued: 'secondary'
        }
        return map[status] ?? 'secondary'
    }

    if (loading) return (
        <div className="d-flex justify-content-center mt-5">
            <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Laden...</span>
            </div>
        </div>
    )

    if (error) return (
        <div className="alert alert-danger mt-4">{error}</div>
    )

    if (history.length === 0) return (
        <div className="alert alert-info mt-4">Noch keine Videos verarbeitet.</div>
    )

    return (
        <div className="mt-4">
            <h4 className="mb-3">Meine Videos</h4>
            <div className="d-flex flex-column gap-3">
                {history.map(item => (
                    <div key={item.jobId} className="card shadow-sm">
                        <div className="card-header d-flex justify-content-between align-items-center">
                            <div>
                                <strong>{item.originalFileName}</strong>
                                <span className="ms-2 text-muted small">{item.platform}</span>
                            </div>
                            <div className="d-flex align-items-center gap-2">
                                <span className={`badge bg-${getStatusBadge(item.status)}`}>
                                    {item.status}
                                </span>
                                <span className="text-muted small">
                                    {new Date(item.createdAt).toLocaleString('de-AT')}
                                </span>
                                <button
                                    className="btn btn-outline-danger btn-sm"
                                    onClick={() => handleDelete(item.jobId)}
                                    disabled={deletingId === item.jobId}
                                >
                                    {deletingId === item.jobId
                                        ? <span className="spinner-border spinner-border-sm" />
                                        : '🗑️'
                                    }
                                </button>
                            </div>
                        </div>

                        {item.generations.length > 0 && (
                            <div className="card-body">
                                <h6 className="card-subtitle mb-3 text-muted">
                                    Generierungen ({item.generations.length}/3)
                                </h6>
                                <div className="d-flex flex-column gap-2">
                                    {item.generations.map(g => (
                                        <div key={g.id} className="border rounded p-3 bg-light">
                                            <div className="d-flex justify-content-between mb-1">
                                                <span className="badge bg-secondary">{g.platform}</span>
                                                <span className="badge bg-outline text-muted border">
                                                    {g.tonality}
                                                </span>
                                            </div>
                                            <p className="mb-1"><strong>Titel:</strong> {g.title}</p>
                                            <p className="mb-1"><strong>Hook:</strong> {g.hook}</p>
                                            <p className="mb-0 text-muted small">{g.hashtags}</p>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}

                        {item.generations.length === 0 && (
                            <div className="card-body text-muted small">
                                Noch keine Generierungen vorhanden.
                            </div>
                        )}
                    </div>
                ))}
            </div>
        </div>
    )
}