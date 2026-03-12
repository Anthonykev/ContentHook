import { useState, useEffect, useCallback, useRef, useMemo } from 'react'
import { useAuth0 } from '@auth0/auth0-react'
import { useJobStatus } from '../hooks/useJobStatus'
import { TONALITY_OPTIONS } from '../constants/tonalityOptions'

// CopyButton 
function CopyButton({ text }) {
    const [copied, setCopied] = useState(false)

    async function handleCopy() {
        try {
            await navigator.clipboard.writeText(text)
        } catch {
            const el = document.createElement('textarea')
            el.value = text
            document.body.appendChild(el)
            el.select()
            document.execCommand('copy')
            document.body.removeChild(el)
        }
        setCopied(true)
        setTimeout(() => setCopied(false), 1500)
    }

    return (
        <button
            className="btn btn-sm"
            onClick={handleCopy}
            style={{
                background: copied ? 'rgba(74,222,128,0.15)' : 'rgba(167,139,250,0.1)',
                border: `1px solid ${copied ? '#4ade80' : '#3d3d6b'}`,
                color: copied ? '#4ade80' : '#a78bfa',
                fontSize: '13px',
                whiteSpace: 'nowrap',
                transition: 'all 0.2s',
                padding: '4px 12px',
                borderRadius: '6px',
                flexShrink: 0,
            }}
        >
            {copied ? '✓ Kopiert' : '📋'}
        </button>
    )
}

// GenerationBlock
function GenerationBlock({ gen, index, total, isNew }) {
    const hashtagsText = Array.isArray(gen.hashtags)
        ? gen.hashtags.map(t => (t.startsWith('#') ? t : `#${t}`)).join(' ')
        : gen.hashtags ?? ''

    return (
        <div
            style={{
                background: '#0a0a14',
                border: `1px solid ${isNew ? '#6366f1' : '#2a2a45'}`,
                borderRadius: '10px',
                padding: '16px',
                marginBottom: index < total - 1 ? '10px' : 0,
                boxShadow: isNew ? '0 0 0 1px rgba(99,102,241,0.3)' : 'none',
                transition: 'border-color 1.5s, box-shadow 1.5s',
            }}
        >
            <div className="d-flex justify-content-between align-items-center mb-3">
                <span style={{ fontSize: '14px', color: '#8f95ff', fontWeight: '700' }}>
                    Generation {index + 1}
                </span>
                {gen.tonality && (
                    <span
                        className="badge"
                        style={{
                            background: '#1e1e35',
                            color: '#94a3b8',
                            fontSize: '12px',
                            fontWeight: '500',
                            padding: '4px 8px',
                        }}
                    >
                        {gen.tonality}
                    </span>
                )}
            </div>

            {[
                ['Titel', gen.title ?? '–'],
                ['Hook', gen.hook ?? '–'],
                ['Hashtags', hashtagsText || '–'],
            ].map(([label, value]) => (
                <div
                    key={label}
                    className="d-flex justify-content-between align-items-start"
                    style={{
                        background: '#13131f',
                        borderRadius: '8px',
                        padding: '10px 14px',
                        marginBottom: '6px',
                        gap: '10px',
                    }}
                >
                    <div style={{ flex: 1, minWidth: 0 }}>
                        <div
                            style={{
                                fontSize: '12px',
                                color: '#9fb0c8',
                                textTransform: 'uppercase',
                                letterSpacing: '0.8px',
                                marginBottom: '4px',
                                fontWeight: '600',
                            }}
                        >
                            {label}
                        </div>
                        <div
                            style={{
                                fontSize: '15px',
                                color: '#f3f6fb',
                                lineHeight: '1.5',
                                wordBreak: 'break-word',
                            }}
                        >
                            {value}
                        </div>
                    </div>
                    <div style={{ paddingTop: '2px' }}>
                        <CopyButton text={value} />
                    </div>
                </div>
            ))}
        </div>
    )
}
function RegenForm({ tonality, setTonality, error, signalrError, isFailed, onCancel, onGenerate }) {
    return (
        <div
            style={{
                background: '#0a0a14',
                border: '1px solid #2a2a45',
                borderRadius: '10px',
                padding: '14px',
            }}
        >
            <div
                style={{
                    fontSize: '14px',
                    color: '#d2dcf0',
                    marginBottom: '10px',
                    fontWeight: '600',
                }}
            >
                Tonalität wählen:
            </div>

            <div style={{ position: 'relative', marginBottom: '8px' }}>
                <select
                    className="form-select form-select-sm"
                    value={tonality}
                    onChange={e => setTonality(e.target.value)}
                    style={{
                        background: '#13131f',
                        border: '1px solid #2a2a45',
                        color: '#e2e8f0',
                        fontSize: '14px',
                        padding: '8px 40px 8px 12px',
                        appearance: 'none',
                        WebkitAppearance: 'none',
                        MozAppearance: 'none',
                        cursor: 'pointer',
                        outline: 'none',        
                        boxShadow: 'none', 
                    }}
                >
                    {TONALITY_OPTIONS.map(option => (
                        <option
                            key={option.value}
                            value={option.value}
                            style={{ background: '#13131f' }}
                        >
                            {option.label}
                        </option>
                    ))}
                </select>

                <span
                    style={{
                        position: 'absolute',
                        right: '14px',
                        bottom: '50%',
                        transform: 'translateY(50%)',
                        color: '#a78bfa',
                        fontSize: '12px',
                        pointerEvents: 'none',
                    }}
                >
                    ▼
                </span>
            </div>

            <div
                style={{
                    fontSize: '12px',
                    color: '#7f8ea3',
                    marginBottom: '10px',
                }}
            >
                Wähl eine Tonalität für die neue Generierung aus.
            </div>

            {(error || signalrError || isFailed) && (
                <div
                    style={{
                        fontSize: '14px',
                        color: '#f87171',
                        marginBottom: '10px',
                        padding: '8px 10px',
                        background: 'rgba(239,68,68,0.08)',
                        borderRadius: '6px',
                    }}
                >
                    ⚠️ {error ?? signalrError ?? 'Fehlgeschlagen'}
                </div>
            )}

            <div className="d-flex gap-2">
                <button
                    className="btn btn-sm flex-fill"
                    onClick={onCancel}
                    style={{
                        background: '#1e1e35',
                        border: 'none',
                        color: '#94a3b8',
                        fontSize: '13px',
                        padding: '8px',
                    }}
                >
                    Abbrechen
                </button>
                <button
                    className="btn btn-sm flex-fill"
                    onClick={onGenerate}
                    style={{
                        background: 'linear-gradient(135deg, #6366f1, #a78bfa)',
                        border: 'none',
                        color: '#fff',
                        fontSize: '15px',
                        fontWeight: '700',
                        padding: '8px',
                    }}
                >
                    Generieren →
                </button>
            </div>
        </div>
    )
}
// PlatformCard 
function PlatformCard({ platform, generations, jobId, onRegenDone, onRegenStarted }) {
    const { getAccessTokenSilently } = useAuth0()
    const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050'

    const [showRegenForm, setShowRegenForm] = useState(false)
    const [tonality, setTonality] = useState('Auto')
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)
    const [activeJobId, setActiveJobId] = useState(null)
    const [signalrDone, setSignalrDone] = useState(false)
    const [newestGenId, setNewestGenId] = useState(null)

    const { status, error: signalrError } = useJobStatus(activeJobId)

    useEffect(() => {
        if (status === 'done' && !signalrDone) {
            setSignalrDone(true)
            setActiveJobId(null)
            setShowRegenForm(false)
            onRegenDone?.()
        }
    }, [status, signalrDone, onRegenDone])

    const newestGenerationIdFromData = useMemo(() => {
        if (!generations || generations.length === 0) return null

        const sorted = [...generations].sort(
            (a, b) => new Date(b.createdAt) - new Date(a.createdAt)
        )

        return sorted[0]?.id ?? null
    }, [generations])

    useEffect(() => {
        if (!newestGenerationIdFromData) return

        setNewestGenId(newestGenerationIdFromData)

        const timer = setTimeout(() => setNewestGenId(null), 3000)
        return () => clearTimeout(timer)
    }, [newestGenerationIdFromData])

    const icon = platform === 'tiktok' ? '🎵' : '📸'
    const label = platform === 'tiktok' ? 'TikTok' : 'Instagram'
    const hasData = generations && generations.length > 0
    const maxReached = generations && generations.length >= 3
    const isGenerating = loading || status === 'generating'
    const isFailed = status === 'failed'

    async function handleGenerate() {
        setLoading(true)
        setError(null)
        setSignalrDone(false)

        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE },
            })

            const previousCount = generations?.length ?? 0

            const res = await fetch(`${apiUrl}/api/jobs/${jobId}/generate`, {
                method: 'POST',
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ tonality, platform }),
            })

            if (!res.ok) throw new Error(await res.text() || `HTTP ${res.status}`)

            setActiveJobId(jobId)
            setShowRegenForm(false)

            onRegenStarted?.({
                platform,
                previousCount,
            })
        } catch (err) {
            setError(err.message ?? 'Generierung fehlgeschlagen')
        } finally {
            setLoading(false)
        }
    }

    return (
        <div className="col-12 col-md-6">
            <div
                style={{
                    background: '#0f0f1a',
                    border: `1px solid ${isGenerating ? '#6366f1' : hasData ? '#2a2a45' : '#1e1e2e'}`,
                    borderRadius: '12px',
                    padding: '20px',
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    transition: 'border-color 0.3s',
                }}
            >
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <div className="d-flex align-items-center gap-2">
                        <span style={{ fontSize: '18px' }}>{icon}</span>
                        <span style={{ fontWeight: '700', fontSize: '16px', color: '#fff' }}>{label}</span>
                    </div>

                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        {isGenerating && (
                            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                                <div
                                    style={{
                                        width: '8px',
                                        height: '8px',
                                        borderRadius: '50%',
                                        background: '#6366f1',
                                        animation: 'pulse 1s infinite',
                                    }}
                                />
                                <span style={{ fontSize: '12px', color: '#a78bfa' }}>Generiert…</span>
                            </div>
                        )}

                        <div
                            style={{
                                width: '28px',
                                height: '28px',
                                borderRadius: '50%',
                                background: hasData ? 'rgba(74,222,128,0.15)' : 'rgba(239,68,68,0.15)',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontSize: '13px',
                                color: hasData ? '#4ade80' : '#ef4444',
                            }}
                        >
                            {isGenerating ? '⏳' : hasData ? '✓' : '✕'}
                        </div>
                    </div>
                </div>

                {hasData ? (
                    <div style={{ flex: 1 }}>
                        {[...generations]
                            .sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt))
                            .map((gen, i) => (
                                <GenerationBlock
                                    key={gen.id}
                                    gen={gen}
                                    index={i}
                                    total={generations.length}
                                    isNew={gen.id === newestGenId}
                                />
                            ))}

                        <div style={{ marginTop: '12px' }}>
                            {maxReached ? (
                                <div
                                    className="text-center"
                                    style={{
                                        padding: '10px',
                                        fontSize: '13px',
                                        color: '#64748b',
                                        background: '#0a0a14',
                                        borderRadius: '8px',
                                    }}
                                >
                                    Maximum 3 Generierungen erreicht
                                </div>
                            ) : isGenerating ? (
                                <div
                                    style={{
                                        padding: '14px 16px',
                                        fontSize: '14px',
                                        color: '#a78bfa',
                                        background: '#0a0a14',
                                        borderRadius: '8px',
                                        border: '1px solid #2a2a45',
                                        display: 'flex',
                                        alignItems: 'center',
                                        gap: '10px',
                                    }}
                                >
                                    <div
                                        style={{
                                            width: '16px',
                                            height: '16px',
                                            borderRadius: '50%',
                                            border: '2px solid #2a2a45',
                                            borderTop: '2px solid #6366f1',
                                            animation: 'spin 0.8s linear infinite',
                                            flexShrink: 0,
                                        }}
                                    />
                                    KI generiert Content…
                                </div>
                            ) : !showRegenForm ? (
                                <button
                                    className="btn w-100"
                                    onClick={() => setShowRegenForm(true)}
                                    style={{
                                        background: '#1e1e35',
                                        border: '1px dashed #3d3d6b',
                                        color: '#a78bfa',
                                        fontSize: '14px',
                                        borderRadius: '8px',
                                        padding: '10px',
                                    }}
                                >
                                    + Nochmal generieren
                                </button>
                            ) : (
                                <RegenForm
                                    tonality={tonality}
                                    setTonality={setTonality}
                                    error={error}
                                    signalrError={signalrError}
                                    isFailed={isFailed}
                                    onCancel={() => {
                                        setShowRegenForm(false)
                                        setError(null)
                                    }}
                                    onGenerate={handleGenerate}
                                />
                            )}
                        </div>
                    </div>
                ) : (
                    <div className="d-flex flex-column align-items-center justify-content-center flex-grow-1 py-4 gap-3">
                        {isGenerating ? (
                            <>
                                <div
                                    style={{
                                        width: '40px',
                                        height: '40px',
                                        borderRadius: '50%',
                                        border: '3px solid #1e1e35',
                                        borderTop: '3px solid #6366f1',
                                        animation: 'spin 0.8s linear infinite',
                                    }}
                                />
                                <div style={{ fontSize: '14px', color: '#a78bfa' }}>
                                    KI generiert Content…
                                </div>
                            </>
                        ) : (
                                    <> <div style={{ width: '100%', marginBottom: '12px' }}>
                                        <div style={{ fontSize: '13px', color: '#9fb0c8', marginBottom: '6px', fontWeight: '600' }}>
                                            Tonalität wählen:
                                        </div>
                                        <select
                                            className="form-select form-select-sm"
                                            value={tonality}
                                            onChange={e => setTonality(e.target.value)}
                                            style={{
                                                background: '#13131f',
                                                border: '1px solid #2a2a45',
                                                color: '#e2e8f0',
                                                fontSize: '14px',
                                            }}
                                        >
                                            {TONALITY_OPTIONS.map(option => (
                                                <option key={option.value} value={option.value} style={{ background: '#13131f' }}>
                                                    {option.label}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                        
                                <button
                                    className="btn w-100"
                                    onClick={handleGenerate}
                                    style={{
                                        background: 'linear-gradient(135deg, #6366f1, #a78bfa)',
                                        border: 'none',
                                        color: '#fff',
                                        fontSize: '15px',
                                        fontWeight: '700',
                                        borderRadius: '8px',
                                        padding: '12px',
                                    }}
                                >
                                    Für {label} generieren →
                                </button>
                            </>
                        )}

                        {(error || isFailed) && (
                            <div
                                style={{
                                    fontSize: '14px',
                                    color: '#f87171',
                                    padding: '8px 12px',
                                    background: 'rgba(239,68,68,0.08)',
                                    borderRadius: '6px',
                                    width: '100%',
                                }}
                            >
                                ⚠️ {error ?? 'Fehlgeschlagen'}
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    )
}

// Haupt-Komponente
export default function HistoryPage({ selectedJobId, onRefreshHistory }) {
    const { getAccessTokenSilently } = useAuth0()
    const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050'

    const [job, setJob] = useState(null)
    const [loading, setLoading] = useState(false)
    const [refreshing, setRefreshing] = useState(false)
    const [error, setError] = useState(null)
    const [pendingRefresh, setPendingRefresh] = useState(null)
    const [isEditingTranscript, setIsEditingTranscript] = useState(false)
    const [editedTranscript, setEditedTranscript] = useState('')
    const [savingTranscript, setSavingTranscript] = useState(false)
    const [transcriptSaveError, setTranscriptSaveError] = useState(null)
    const currentJobIdRef = useRef(null)

    const fetchJobDetail = useCallback(async (silent = false) => {
        if (!selectedJobId) return

        if (silent) {
            setRefreshing(true)
        } else {
            setLoading(true)
            setError(null)
        }

        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE },
            })

            const res = await fetch(`${apiUrl}/api/history/${selectedJobId}`, {
                headers: { Authorization: `Bearer ${token}` },
            })

            if (!res.ok) throw new Error(`HTTP ${res.status}`)
            const data = await res.json()

            if (currentJobIdRef.current === selectedJobId) {
                setJob(data)
            }
        } catch (err) {
            if (!silent) setError(err.message)
        } finally {
            setLoading(false)
            setRefreshing(false)
        }
    }, [selectedJobId, getAccessTokenSilently, apiUrl])

    function getGenerationCountForPlatform(jobData, platformKey) {
        const gens = jobData?.generations ?? []
        return gens.filter(g => g.platform === platformKey).length
    }

    useEffect(() => {
        currentJobIdRef.current = selectedJobId
        setJob(null)
        setError(null)
        setPendingRefresh(null)
        fetchJobDetail(false)
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [selectedJobId]) 

    useEffect(() => {
        setEditedTranscript(job?.transcriptText ?? '')
        setTranscriptSaveError(null)
        setIsEditingTranscript(false)
    }, [job?.transcriptText, selectedJobId])

    function handleRegenDone() {
        fetchJobDetail(true)
        onRefreshHistory?.()
    }

    function handleRegenStarted({ platform, previousCount }) {
        setPendingRefresh({
            platform,
            previousCount,
            startedAt: Date.now(),
        })

        fetchJobDetail(true)
        onRefreshHistory?.()
    }

    useEffect(() => {
        if (!pendingRefresh) return

        const interval = setInterval(() => {
            fetchJobDetail(true)
        }, 1500)

        const timeout = setTimeout(() => {
            setPendingRefresh(null)
        }, 20000)

        return () => {
            clearInterval(interval)
            clearTimeout(timeout)
        }
    }, [pendingRefresh, fetchJobDetail])

    useEffect(() => {
        if (!pendingRefresh || !job) return

        const currentCount = getGenerationCountForPlatform(job, pendingRefresh.platform)

        if (currentCount > pendingRefresh.previousCount) {
            setPendingRefresh(null)
            onRefreshHistory?.()
        }
    }, [job, pendingRefresh, onRefreshHistory])

    async function handleSaveTranscript() {
        if (!job?.transcriptId) return

        setSavingTranscript(true)
        setTranscriptSaveError(null)

        try {
            const token = await getAccessTokenSilently({
                authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE },
            })

            const res = await fetch(`${apiUrl}/api/transcripts/${job.transcriptId}`, {
                method: 'PUT',
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ text: editedTranscript }),
            })

            if (!res.ok) throw new Error(await res.text() || `HTTP ${res.status}`)

            setJob(prev => (prev ? { ...prev, transcriptText: editedTranscript } : prev))
            setIsEditingTranscript(false)
        } catch (err) {
            setTranscriptSaveError(err.message ?? 'Transcript konnte nicht gespeichert werden')
        } finally {
            setSavingTranscript(false)
        }
    }

    if (!selectedJobId) {
        return (
            <div
                className="d-flex flex-column align-items-center justify-content-center h-100"
                style={{ color: '#475569', minHeight: '300px' }}
            >
                <div style={{ fontSize: '48px', marginBottom: '16px' }}>🎬</div>
                <p style={{ fontSize: '15px', margin: 0, color: '#64748b' }}>
                    Wähle ein Video aus dem Verlauf
                </p>
            </div>
        )
    }

    if (loading) {
        return (
            <div className="d-flex align-items-center justify-content-center" style={{ minHeight: '200px' }}>
                <div style={{ textAlign: 'center' }}>
                    <div className="spinner-border spinner-border-sm mb-2" style={{ color: '#6366f1' }} />
                    <div style={{ color: '#64748b', fontSize: '14px' }}>Lade Details…</div>
                </div>
            </div>
        )
    }

    if (error) {
        return (
            <div
                style={{
                    background: 'rgba(239,68,68,0.05)',
                    border: '1px solid rgba(239,68,68,0.3)',
                    borderRadius: '12px',
                    padding: '20px',
                }}
            >
                <div style={{ color: '#f87171', fontWeight: '600', marginBottom: '6px', fontSize: '15px' }}>
                    ❌ Fehler beim Laden
                </div>
                <div style={{ color: '#94a3b8', fontSize: '14px' }}>{error}</div>
            </div>
        )
    }

    if (!job) return null

    const generations = job.generations ?? []
    const tiktokGens = generations.filter(g => g.platform === 'tiktok')
    const instagramGens = generations.filter(g => g.platform === 'instagram')

    return (
        <div>
            <style>{`
        @keyframes spin  { to { transform: rotate(360deg) } }
        @keyframes pulse { 0%,100% { opacity: 1 } 50% { opacity: 0.4 } }
      `}</style>

            <div className="d-flex align-items-center justify-content-between mb-4" style={{ flexWrap: 'wrap', gap: '8px' }}>
                <h2 style={{ margin: 0, fontSize: '24px', fontWeight: '800', color: '#fff' }}>
                    🎬 {job.originalFileName?.replace(/\.(mp4|mov)$/i, '') ?? 'Video'}
                </h2>

                <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                    {refreshing && (
                        <span
                            style={{
                                fontSize: '12px',
                                color: '#6366f1',
                                display: 'flex',
                                alignItems: 'center',
                                gap: '6px',
                            }}
                        >
                            <div
                                className="spinner-border spinner-border-sm"
                                style={{ color: '#6366f1', width: '12px', height: '12px', borderWidth: '2px' }}
                            />
                            Aktualisiert…
                        </span>
                    )}

                    <span style={{ fontSize: '14px', color: '#8ea1ba' }}>
                        {new Date(job.createdAt).toLocaleDateString('de-AT', {
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric',
                        })}
                    </span>
                </div>
            </div>

            {job.transcriptText && (
                <div
                    style={{
                        background: '#0a0a14',
                        border: '1px solid #1e1e35',
                        borderRadius: '10px',
                        padding: '18px',
                        marginBottom: '20px',
                    }}
                >
                    <div className="d-flex justify-content-between align-items-center mb-3" style={{ gap: '10px', flexWrap: 'wrap' }}>
                        <div
                            style={{
                                fontSize: '14px',
                                color: '#dbe5f2',
                                fontWeight: '700',
                            }}
                        >
                            📝 Transcript
                        </div>

                        {job.transcriptId && (
                            <div className="d-flex gap-2">
                                {!isEditingTranscript ? (
                                    <button
                                        className="btn btn-sm"
                                        onClick={() => setIsEditingTranscript(true)}
                                        style={{
                                            background: '#1e1e35',
                                            border: '1px solid #2a2a45',
                                            color: '#cfc2ff',
                                            fontSize: '13px',
                                            padding: '6px 12px',
                                        }}
                                    >
                                        Bearbeiten
                                    </button>
                                ) : (
                                    <>
                                        <button
                                            className="btn btn-sm"
                                            onClick={() => {
                                                setEditedTranscript(job.transcriptText ?? '')
                                                setIsEditingTranscript(false)
                                                setTranscriptSaveError(null)
                                            }}
                                            style={{
                                                background: '#1e1e35',
                                                border: '1px solid #2a2a45',
                                                color: '#94a3b8',
                                                fontSize: '13px',
                                                padding: '6px 12px',
                                            }}
                                        >
                                            Abbrechen
                                        </button>

                                        <button
                                            className="btn btn-sm"
                                            onClick={handleSaveTranscript}
                                            disabled={savingTranscript || !editedTranscript.trim()}
                                            style={{
                                                background: 'linear-gradient(135deg, #6366f1, #a78bfa)',
                                                border: 'none',
                                                color: '#fff',
                                                fontSize: '13px',
                                                fontWeight: '700',
                                                padding: '6px 12px',
                                                opacity: savingTranscript ? 0.6 : 1,
                                            }}
                                        >
                                            {savingTranscript ? 'Speichert…' : 'Speichern'}
                                        </button>
                                    </>
                                )}
                            </div>
                        )}
                    </div>

                    {isEditingTranscript ? (
                        <textarea
                            rows={8}
                            value={editedTranscript}
                            onChange={e => setEditedTranscript(e.target.value)}
                            style={{
                                width: '100%',
                                background: '#080810',
                                border: '1px solid #2a2a45',
                                color: '#eef3fb',
                                borderRadius: '10px',
                                padding: '14px',
                                fontSize: '15px',
                                lineHeight: '1.75',
                                resize: 'vertical',
                                outline: 'none',
                            }}
                        />
                    ) : (
                        <div
                            style={{
                                margin: 0,
                                fontSize: '15px',
                                color: '#a8b6c8',
                                lineHeight: '1.75',
                                whiteSpace: 'pre-wrap',
                                wordBreak: 'break-word',
                            }}
                        >
                            {job.transcriptText}
                        </div>
                    )}

                    {transcriptSaveError && (
                        <div
                            style={{
                                marginTop: '12px',
                                padding: '10px 12px',
                                background: 'rgba(239,68,68,0.08)',
                                border: '1px solid rgba(239,68,68,0.25)',
                                borderRadius: '8px',
                                color: '#f87171',
                                fontSize: '14px',
                            }}
                        >
                            ⚠️ {transcriptSaveError}
                        </div>
                    )}
                </div>
            )}

            <div className="row g-3">
                <PlatformCard
                    platform="tiktok"
                    generations={tiktokGens}
                    jobId={job.jobId}
                    onRegenDone={handleRegenDone}
                    onRegenStarted={handleRegenStarted}
                />
                <PlatformCard
                    platform="instagram"
                    generations={instagramGens}
                    jobId={job.jobId}
                    onRegenDone={handleRegenDone}
                    onRegenStarted={handleRegenStarted}
                />
            </div>
        </div>
    )
}