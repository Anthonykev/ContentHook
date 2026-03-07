import { useState } from 'react'
import { useJobStatus } from '../hooks/useJobStatus'
import { useAuth0 } from '@auth0/auth0-react'

const TONALITY_OPTIONS = [
    { value: 'Auto', label: '🤖 Auto (KI entscheidet)' },
    { value: 'Emotional', label: '❤️ Emotional' },
    { value: 'Sachlich', label: '📋 Sachlich' },
    { value: 'Neugierig', label: '🤔 Neugierig' },
    { value: 'Humorvoll', label: '😄 Humorvoll' },
]

const STEPS = [
    { key: 'queued', label: '📥 Video empfangen' },
    { key: 'transcribing', label: '🎙 Transkription läuft' },
    { key: 'transcribed', label: '✅ Transkript bereit' },
    { key: 'generating', label: '✨ KI generiert' },
    { key: 'done', label: '🎉 Fertig!' },
]

const STEP_ORDER = ['queued', 'transcribing', 'transcribed', 'generating', 'done']

function StatusStepper({ status }) {
    const currentIndex = STEP_ORDER.indexOf(status)
    return (
        <div className="d-flex align-items-center gap-2 flex-wrap my-3">
            {STEPS.map((step, i) => {
                const stepIndex = STEP_ORDER.indexOf(step.key)
                const isDone = stepIndex < currentIndex
                const isActive = step.key === status
                return (
                    <div key={step.key} className="d-flex align-items-center gap-1">
                        <span className={`badge ${isDone ? 'bg-success' :
                                isActive ? 'bg-primary' :
                                    'bg-secondary opacity-50'
                            }`}>
                            {isActive && (
                                <span className="spinner-border spinner-border-sm me-1"
                                    style={{ width: '10px', height: '10px' }} />
                            )}
                            {step.label}
                        </span>
                        {i < STEPS.length - 1 && (
                            <span className="text-muted">→</span>
                        )}
                    </div>
                )
            })}
        </div>
    )
}

export default function VideoUpload() {
    const [jobId, setJobId] = useState(null)
    const [uploading, setUploading] = useState(false)
    const [uploadError, setUploadError] = useState(null)
    const [transcriptText, setTranscriptText] = useState('')
    const [transcriptId, setTranscriptId] = useState(null)
    const [tonality, setTonality] = useState('Auto')
    const [generating, setGenerating] = useState(false)
    const [generateError, setGenerateError] = useState(null)

    const { status, payload, error } = useJobStatus(jobId)
    const { getAccessTokenSilently } = useAuth0()
    const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050'

    // SignalR transcribed → Transcript-Text übernehmen
    if (status === 'transcribed' && payload?.transcriptText && !transcriptId) {
        setTranscriptText(payload.transcriptText)
        setTranscriptId(payload.transcriptId)
    }

    async function handleUpload(e) {
        const file = e.target.files[0]
        if (!file) return
        setUploading(true)
        setJobId(null)
        setUploadError(null)
        setTranscriptText('')
        setTranscriptId(null)
        setGenerateError(null)

        const formData = new FormData()
        formData.append('file', file)

        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE }
            })
            const response = await fetch(`${apiUrl}/api/Videos?platform=tiktok`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}` },
                body: formData
            })
            if (!response.ok) throw new Error(await response.text() || `HTTP ${response.status}`)
            const data = await response.json()
            setJobId(data.jobId)
        } catch (err) {
            setUploadError(err.message ?? 'Upload fehlgeschlagen')
        } finally {
            setUploading(false)
        }
    }

    async function handleGenerate() {
        if (!jobId || !transcriptId) return
        setGenerating(true)
        setGenerateError(null)
        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE }
            })
            // Transcript speichern falls bearbeitet
            await fetch(`${apiUrl}/api/transcripts/${transcriptId}`, {
                method: 'PUT',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify({ text: transcriptText })
            })
            // GPT starten
            const res = await fetch(`${apiUrl}/api/jobs/${jobId}/generate`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify({ tonality })
            })
            if (!res.ok) throw new Error(await res.text() || `HTTP ${res.status}`)
        } catch (err) {
            setGenerateError(err.message ?? 'Generierung fehlgeschlagen')
        } finally {
            setGenerating(false)
        }
    }

    return (
        <div className="container py-4">
            <h2 className="mb-4">ContentHook — Video Upload</h2>

            {/* Upload */}
            <div className="mb-3">
                <input
                    type="file"
                    className="form-control"
                    accept="video/mp4,video/quicktime,.mp4,.mov"
                    onChange={handleUpload}
                    disabled={uploading}
                />
            </div>

            {uploading && (
                <div className="d-flex align-items-center gap-2 text-muted mb-2">
                    <div className="spinner-border spinner-border-sm" />
                    <span>Video wird hochgeladen...</span>
                </div>
            )}

            {uploadError && <div className="alert alert-danger">{uploadError}</div>}

            {/* Live Status Stepper */}
            {status && status !== 'failed' && <StatusStepper status={status} />}

            {/* Transcript Review */}
            {status === 'transcribed' && transcriptText && (
                <div className="card mb-4">
                    <div className="card-header">
                        <strong>📝 Transkript prüfen & bearbeiten</strong>
                        <span className="text-muted ms-2 small">
                            Korrigiere den Text bevor er an die KI gesendet wird.
                        </span>
                    </div>
                    <div className="card-body">
                        <textarea
                            className="form-control mb-3"
                            rows={8}
                            value={transcriptText}
                            onChange={e => setTranscriptText(e.target.value)}
                        />
                        <div className="mb-3">
                            <label className="form-label fw-semibold">Tonalität</label>
                            <select
                                className="form-select"
                                value={tonality}
                                onChange={e => setTonality(e.target.value)}
                            >
                                {TONALITY_OPTIONS.map(opt => (
                                    <option key={opt.value} value={opt.value}>{opt.label}</option>
                                ))}
                            </select>
                            <div className="form-text">
                                Bei "Auto" wählt die KI selbst die passende Tonalität.
                            </div>
                        </div>

                        {generateError && <div className="alert alert-danger">{generateError}</div>}

                        <button
                            className="btn btn-primary"
                            onClick={handleGenerate}
                            disabled={generating || !transcriptText.trim()}
                        >
                            {generating
                                ? <><span className="spinner-border spinner-border-sm me-2" />Generiert...</>
                                : '✨ Jetzt generieren'
                            }
                        </button>
                    </div>
                </div>
            )}

            {/* Ergebnis */}
            {status === 'done' && payload && (
                <div className="card border-success mb-4">
                    <div className="card-header bg-success text-white fw-semibold">
                        🎉 Generierung abgeschlossen
                    </div>
                    <div className="card-body">
                        <p><strong>Titel:</strong> {payload.title}</p>
                        <p><strong>Hook:</strong> {payload.hook}</p>
                        <p><strong>Hashtags:</strong> {payload.hashtags}</p>
                        <p>
                            <strong>Tonalität:</strong>{' '}
                            <span className="badge bg-secondary">{payload.tonality}</span>
                        </p>
                    </div>
                </div>
            )}

            {/* Fehler */}
            {(status === 'failed' || error) && (
                <div className="alert alert-danger">
                    <strong>Fehler:</strong> {error ?? 'Unbekannter Fehler'}
                </div>
            )}

            {jobId && (
                <p className="text-muted small mt-2">
                    JobId: {jobId} | Status: {status ?? 'waiting...'}
                </p>
            )}
        </div>
    )
}
