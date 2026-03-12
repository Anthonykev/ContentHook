import { useState, useRef, useCallback, useEffect } from 'react'
import { useAuth0 } from '@auth0/auth0-react'
import { useJobStatus } from '../hooks/useJobStatus'
import { TONALITY_OPTIONS } from '../constants/tonalityOptions'


const STEP_ORDER = ['queued', 'transcribing', 'transcribed', 'generating', 'done']
const STEPS = [
    { key: 'queued', label: 'Empfangen' },
    { key: 'transcribing', label: 'Transkription' },
    { key: 'transcribed', label: 'Bereit' },
    { key: 'generating', label: 'KI generiert' },
    { key: 'done', label: 'Fertig' },
]

const S = {
    card: {
        background: '#0a0a14',
        border: '1px solid #1e1e35',
        borderRadius: '16px',
        padding: '24px',
        marginBottom: '18px',
        boxShadow: '0 18px 40px rgba(0,0,0,0.18)',
    },
    stepLabel: {
        fontSize: '13px',
        color: '#9fb0c8',
        fontWeight: '700',
        textTransform: 'uppercase',
        letterSpacing: '1px',
        marginBottom: '12px',
        display: 'block',
    },
    helper: {
        fontSize: '15px',
        color: '#b8c7d9',
        margin: '0 0 14px',
        lineHeight: '1.6',
    },
    textarea: {
        background: '#080810',
        border: '1px solid #2a2a45',
        color: '#e2e8f0',
        borderRadius: '10px',
        padding: '15px',
        fontSize: '15px',
        lineHeight: '1.7',
        width: '100%',
        resize: 'vertical',
        outline: 'none',
        fontFamily: "'DM Sans','Segoe UI',sans-serif",
        boxSizing: 'border-box',
    },
    select: {
        background: '#080810',
        border: '1px solid #2a2a45',
        color: '#e2e8f0',
        borderRadius: '10px',
        padding: '12px 14px',
        fontSize: '15px',
        width: '100%',
        outline: 'none',
        cursor: 'pointer',
    },
    btnPrimary: {
        background: 'linear-gradient(135deg, #6366f1, #a78bfa)',
        border: 'none',
        color: '#fff',
        fontWeight: '700',
        fontSize: '15px',
        borderRadius: '10px',
        padding: '14px 28px',
        cursor: 'pointer',
        transition: 'opacity 0.15s',
        width: '100%',
    },
    btnDisabled: {
        opacity: 0.45,
        cursor: 'not-allowed',
        pointerEvents: 'none',
    },
}

function StatusStepper({ status }) {
    const currentIdx = STEP_ORDER.indexOf(status)
    return (
        <>
            <style>{`@keyframes ch-spin { to { transform: rotate(360deg) } }`}</style>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px', flexWrap: 'wrap' }}>
                {STEPS.map((step, i) => {
                    const idx = STEP_ORDER.indexOf(step.key)
                    const isDone = idx < currentIdx
                    const isActive = step.key === status
                    return (
                        <div key={step.key} style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                            <div style={{
                                display: 'flex', alignItems: 'center', gap: '6px',
                                padding: '6px 12px', borderRadius: '20px',
                                fontSize: '12px', fontWeight: '700',
                                background: isDone
                                    ? 'rgba(74,222,128,0.12)'
                                    : isActive ? 'rgba(99,102,241,0.2)' : '#0d0d1a',
                                border: `1px solid ${isDone
                                    ? 'rgba(74,222,128,0.3)'
                                    : isActive ? '#6366f1' : '#1e1e35'}`,
                                color: isDone ? '#4ade80' : isActive ? '#d8ccff' : '#8ea1ba',
                                transition: 'all 0.3s',
                            }}>
                                {isActive && (
                                    <span style={{
                                        width: '8px', height: '8px', borderRadius: '50%',
                                        border: '2px solid #a78bfa', borderTopColor: 'transparent',
                                        display: 'inline-block',
                                        animation: 'ch-spin 0.8s linear infinite',
                                        flexShrink: 0,
                                    }} />
                                )}
                                {isDone && <span>✓</span>}
                                {step.label}
                            </div>
                            {i < STEPS.length - 1 && (
                                <span style={{ color: '#2d3550', fontSize: '12px' }}>›</span>
                            )}
                        </div>
                    )
                })}
            </div>
        </>
    )
}

function PlatformSelector({ value, onChange }) {
    const platforms = [
        { key: 'tiktok', label: 'TikTok', emoji: '🎵' },
        { key: 'instagram', label: 'Instagram Reels', emoji: '📸' },
    ]
    return (
        <div className="row g-3">
            {platforms.map(p => (
                <div key={p.key} className="col-12 col-sm-6">
                    <button
                        type="button"
                        onClick={() => onChange(p.key)}
                        style={{
                            width: '100%', padding: '20px 14px', borderRadius: '12px', cursor: 'pointer',
                            border: `2px solid ${value === p.key ? '#6366f1' : '#1e1e35'}`,
                            background: value === p.key ? 'rgba(99,102,241,0.1)' : '#080810',
                            color: value === p.key ? '#e9ddff' : '#c1cee0',
                            fontWeight: value === p.key ? '700' : '500',
                            fontSize: '16px', transition: 'all 0.15s',
                            display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px',
                        }}
                    >
                        <span style={{ fontSize: '28px' }}>{p.emoji}</span>
                        <span>{p.label}</span>
                    </button>
                </div>
            ))}
        </div>
    )
}

function DropZone({ onFile, disabled }) {
    const inputRef = useRef(null)
    const [drag, setDrag] = useState(false)

    function handleDrop(e) {
        e.preventDefault()
        setDrag(false)
        if (disabled) return
        const file = e.dataTransfer.files?.[0]
        if (file) onFile(file)
    }

    function handleChange(e) {
        const file = e.target.files?.[0]
        if (file) onFile(file)
        e.target.value = ''
    }

    return (
        <div
            onDragOver={e => { e.preventDefault(); if (!disabled) setDrag(true) }}
            onDragLeave={() => setDrag(false)}
            onDrop={handleDrop}
            onClick={() => !disabled && inputRef.current?.click()}
            style={{
                border: `2px dashed ${drag ? '#6366f1' : '#2a2a45'}`,
                borderRadius: '14px', padding: '44px 20px', textAlign: 'center',
                cursor: disabled ? 'not-allowed' : 'pointer',
                background: drag ? 'rgba(99,102,241,0.06)' : 'transparent',
                transition: 'all 0.2s', opacity: disabled ? 0.55 : 1,
            }}
        >
            <div style={{ fontSize: '38px', marginBottom: '14px' }}>🎬</div>
            <div style={{ color: '#e2e8f0', fontSize: '17px', fontWeight: '600', marginBottom: '8px' }}>
                Video hierher ziehen oder{' '}
                <span style={{ color: '#cfc2ff' }}>auswählen</span>
            </div>
            <div style={{ color: '#8ea1ba', fontSize: '14px' }}>MP4 oder MOV · max. 200 MB</div>
            <input
                ref={inputRef}
                id="video-upload-input"
                name="videoFile"
                type="file"
                accept="video/mp4,video/quicktime,.mp4,.mov"
                style={{ display: 'none' }}
                onChange={handleChange}
                disabled={disabled}
            />
        </div>
    )
}

export default function VideoUpload({ onJobDone }) {
    const { getAccessTokenSilently } = useAuth0()
    const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050'

    const [platform, setPlatform] = useState('tiktok')
    const [selectedFile, setSelectedFile] = useState(null)
    const [jobId, setJobId] = useState(null)
    const [uploading, setUploading] = useState(false)
    const [uploadError, setUploadError] = useState(null)
    const [transcriptText, setTranscriptText] = useState('')
    const [transcriptId, setTranscriptId] = useState(null)
    const [tonality, setTonality] = useState('Auto')
    const [generating, setGenerating] = useState(false)
    const [generateError, setGenerateError] = useState(null)
    const [redirected, setRedirected] = useState(false)

    const { status, payload, error: signalrError } = useJobStatus(jobId)

    useEffect(() => {
        if (status === 'transcribed' && payload?.transcriptText && !transcriptId) {
            setTranscriptText(payload.transcriptText)
            setTranscriptId(payload.transcriptId)
        }
    }, [status, payload, transcriptId])

    useEffect(() => {
        if (status === 'done' && !redirected && jobId) {
            setRedirected(true)
            setTimeout(() => { onJobDone?.(jobId) }, 1200)
        }
    }, [status, redirected, jobId, onJobDone])

    function handleFileSelect(file) {
        setSelectedFile(file)
        setUploadError(null)
    }

    const handleUpload = useCallback(async () => {
        if (!selectedFile) return
        setUploading(true)
        setJobId(null)
        setUploadError(null)
        setTranscriptText('')
        setTranscriptId(null)
        setGenerateError(null)
        setRedirected(false)

        const formData = new FormData()
        formData.append('file', selectedFile)

        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE },
            })
            const res = await fetch(`${apiUrl}/api/Videos?platform=${platform}`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            })
            if (!res.ok) throw new Error(await res.text() || `HTTP ${res.status}`)
            const data = await res.json()
            setJobId(data.jobId)
        } catch (err) {
            setUploadError(err.message ?? 'Upload fehlgeschlagen')
        } finally {
            setUploading(false)
        }
    }, [selectedFile, platform, getAccessTokenSilently, apiUrl])

    async function handleGenerate() {
        if (!jobId || !transcriptId) return
        setGenerating(true)
        setGenerateError(null)
        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE },
            })
            await fetch(`${apiUrl}/api/transcripts/${transcriptId}`, {
                method: 'PUT',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify({ text: transcriptText }),
            })
            const res = await fetch(`${apiUrl}/api/jobs/${jobId}/generate`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify({ tonality }),
            })
            if (!res.ok) throw new Error(await res.text() || `HTTP ${res.status}`)
        } catch (err) {
            setGenerateError(err.message ?? 'Generierung fehlgeschlagen')
        } finally {
            setGenerating(false)
        }
    }

    function handleReset() {
        setSelectedFile(null); setJobId(null); setUploadError(null)
        setTranscriptText(''); setTranscriptId(null); setGenerateError(null)
        setRedirected(false); setPlatform('tiktok'); setTonality('Auto')
    }

    const isFailed = status === 'failed'
    const isDone = status === 'done'

    return (
        <div className="row justify-content-center">
            <div className="col-12 col-lg-10 col-xl-9">

                <h2 style={{ fontSize: '28px', fontWeight: '800', color: '#fff', margin: '0 0 6px' }}>
                    Neues Video
                </h2>
                <p style={{ fontSize: '15px', color: '#b8c7d9', margin: '0 0 20px' }}>
                    Lade ein Kurzvideo hoch, prüfe das Transkript und lasse dir passende Titel, Hooks und Hashtags generieren.
                </p>

                {!jobId && (
                    <div style={S.card}>
                        <span style={S.stepLabel}>1. Zielplattform wählen</span>
                        <PlatformSelector value={platform} onChange={setPlatform} />
                    </div>
                )}

                {!jobId && (
                    <div style={S.card}>
                        <span style={S.stepLabel}>2. Video auswählen</span>
                        <p style={S.helper}>Wähle dein Video per Drag & Drop oder über den Dateidialog aus.</p>

                        <DropZone onFile={handleFileSelect} disabled={uploading} />
                        <p style={{ fontSize: '12px', color: '#64748b', marginTop: '8px' }}>
                            Bitte laden Sie nur Inhalte hoch, zu deren Verwendung Sie berechtigt sind.
                        </p>

                        {selectedFile && (
                            <div style={{
                                marginTop: '14px', padding: '12px 14px', borderRadius: '10px',
                                background: 'rgba(99,102,241,0.07)', border: '1px solid #2a2a45',
                                fontSize: '15px', color: '#d2dcf0',
                                display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '10px',
                                flexWrap: 'wrap',
                            }}>
                                <span>📎 {selectedFile.name}</span>
                                <span style={{ color: '#8ea1ba' }}>
                                    {(selectedFile.size / 1024 / 1024).toFixed(1)} MB
                                </span>
                               
                            </div>
                        )}

                        {uploadError && (
                            <div style={{
                                marginTop: '12px', padding: '11px 14px', borderRadius: '10px',
                                background: 'rgba(239,68,68,0.08)', border: '1px solid rgba(239,68,68,0.3)',
                                fontSize: '14px', color: '#f87171',
                            }}>
                                ⚠️ {uploadError}
                            </div>
                        )}

                        <div style={{ marginTop: '16px' }}>
                            <button
                                onClick={handleUpload}
                                disabled={!selectedFile || uploading}
                                style={{
                                    ...S.btnPrimary,
                                    ...(!selectedFile || uploading ? S.btnDisabled : {}),
                                }}
                            >
                                {uploading ? '⏳ Wird hochgeladen…' : '🚀 Upload starten'}
                            </button>
                        </div>
                    </div>
                )}

                {jobId && status && !isFailed && (
                    <div style={S.card}>
                        <span style={S.stepLabel}>Fortschritt</span>
                        <StatusStepper status={status} />
                    </div>
                )}

                {(isFailed || signalrError) && (
                    <div style={{
                        ...S.card,
                        border: '1px solid rgba(239,68,68,0.3)',
                        background: 'rgba(239,68,68,0.05)',
                    }}>
                        <div style={{ color: '#f87171', fontWeight: '700', marginBottom: '8px', fontSize: '16px' }}>
                            ❌ Verarbeitung fehlgeschlagen
                        </div>
                        <div style={{ color: '#c2d0e0', fontSize: '15px', marginBottom: '16px' }}>
                            {signalrError ?? 'Ein Fehler ist aufgetreten. Bitte erneut versuchen.'}
                        </div>
                        <button onClick={handleReset} style={S.btnPrimary}>Neues Video</button>
                    </div>
                )}

                {status === 'transcribed' && transcriptText && (
                    <div style={S.card}>
                        <span style={S.stepLabel}>3. Transkript prüfen & bearbeiten</span>
                        <p style={S.helper}>Korrigiere den Text, bevor er an die KI gesendet wird.</p>

                        <textarea
                            rows={8}
                            value={transcriptText}
                            onChange={e => setTranscriptText(e.target.value)}
                            style={S.textarea}
                        />

                        <div style={{ marginTop: '18px' }}>
                            <span style={S.stepLabel}>4. Tonalität wählen</span>
                            <select
                                value={tonality}
                                onChange={e => setTonality(e.target.value)}
                                style={S.select}
                            >
                                {TONALITY_OPTIONS.map(opt => (
                                    <option key={opt.value} value={opt.value}>{opt.label}</option>
                                ))}
                            </select>
                        </div>

                        {generateError && (
                            <div style={{
                                marginTop: '12px', padding: '11px 14px', borderRadius: '10px',
                                background: 'rgba(239,68,68,0.08)', border: '1px solid rgba(239,68,68,0.3)',
                                fontSize: '14px', color: '#f87171',
                            }}>
                                ⚠️ {generateError}
                            </div>
                        )}

                        <div style={{ marginTop: '16px' }}>
                            <button
                                onClick={handleGenerate}
                                disabled={generating || !transcriptText.trim()}
                                style={{
                                    ...S.btnPrimary,
                                    ...(generating || !transcriptText.trim() ? S.btnDisabled : {}),
                                }}
                            >
                                {generating ? '⏳ KI generiert…' : '✨ Titel, Hook & Hashtags generieren'}
                            </button>
                        </div>
                    </div>
                )}

                {status === 'generating' && (
                    <div style={{ ...S.card, textAlign: 'center', padding: '40px 20px' }}>
                        <div style={{ fontSize: '34px', marginBottom: '12px' }}>✨</div>
                        <div style={{ color: '#d2dcf0', fontWeight: '700', fontSize: '16px' }}>
                            KI generiert Titel, Hook & Hashtags…
                        </div>
                        <div style={{ color: '#8ea1ba', fontSize: '15px', marginTop: '8px' }}>
                            Das dauert nur wenige Sekunden.
                        </div>
                    </div>
                )}

                {isDone && (
                    <div style={{
                        ...S.card,
                        border: '1px solid rgba(74,222,128,0.3)',
                        background: 'rgba(74,222,128,0.04)',
                        textAlign: 'center', padding: '40px 20px',
                    }}>
                        <div style={{ fontSize: '36px', marginBottom: '10px' }}>🎉</div>
                        <div style={{ color: '#4ade80', fontWeight: '800', fontSize: '17px', marginBottom: '6px' }}>
                            Fertig!
                        </div>
                        <div style={{ color: '#c2d0e0', fontSize: '15px', marginBottom: '18px' }}>
                            Du wirst zum Verlauf weitergeleitet…
                        </div>
                        <button
                            onClick={handleReset}
                            style={{ ...S.btnPrimary, width: 'auto', padding: '10px 22px', fontSize: '14px' }}
                        >
                            Weiteres Video hochladen
                        </button>
                    </div>
                )}
            </div>
        </div>
    )
}
