import { useState, useEffect } from 'react'
import { useAuth0 } from '@auth0/auth0-react'

const BACKGROUND_QUESTIONS = {
    videosPerMonth: {
        label: 'Wie viele Kurzvideos veröffentlichst du pro Monat durchschnittlich?',
        options: ['0–2', '3–5', '6–10', 'Mehr als 10']
    },
    mainPlatform: {
        label: 'Für welche Plattform erstellst du hauptsächlich Content?',
        options: ['TikTok', 'Instagram', 'Beide gleich häufig', 'Andere']
    },
    experience: {
        label: 'Wie viel Erfahrung hast du mit der Optimierung von Titeln, Hooks und Hashtags?',
        options: ['Keine', 'Wenig', 'Mittel', 'Viel']
    }
}

const SCORE_STEPS = [
    {
        title: 'Qualität der Vorschläge',
        icon: '📝',
        questions: [
            { key: 'titleScore', label: 'Der generierte Titel ist inhaltlich passend zum Video.' },
            { key: 'hookScore', label: 'Der generierte Hook ist verständlich formuliert.' },
            { key: 'hashtagScore', label: 'Die vorgeschlagenen Hashtags sind thematisch relevant.' },
            { key: 'practicalScore', label: 'Die generierten Vorschläge wären in der Praxis verwendbar.' },
            { key: 'overallQualityScore', label: 'Die Qualität der Vorschläge ist insgesamt hoch.' },
        ]
    },
    {
        title: 'Plattformoptimierung',
        icon: '🎯',
        questions: [
            { key: 'platformFitScore', label: 'Die Vorschläge wirken auf die gewählte Plattform zugeschnitten.' },
            { key: 'platformInfluenceScore', label: 'Die Plattformauswahl hat einen erkennbaren Einfluss auf das Ergebnis.' },
            { key: 'hashtagPlatformScore', label: 'Die Hashtags wirken plattformgerecht.' },
            { key: 'platformOptimizationScore', label: 'Die Plattformoptimierung ist insgesamt überzeugend.' },
        ]
    },
    {
        title: 'Benutzerfreundlichkeit',
        icon: '💻',
        questions: [
            { key: 'usabilityScore', label: 'Die Bedienung der Web-Applikation war einfach.' },
            { key: 'layoutScore', label: 'Die Benutzeroberfläche war übersichtlich gestaltet.' },
            { key: 'presentationScore', label: 'Die Ergebnisse wurden klar und verständlich dargestellt.' },
        ]
    },
    {
        title: 'Zeiteffizienz & Unterstützung',
        icon: '⏱️',
        questions: [
            { key: 'timeSavingScore', label: 'Die Web-Applikation spart mir Zeit bei der Content-Erstellung.' },
            { key: 'generationSpeedScore', label: 'Die Generierung der Vorschläge erfolgt ausreichend schnell.' },
            { key: 'supportScore', label: 'Die Web-Applikation unterstützt mich sinnvoll im Content-Erstellungsprozess.' },
            { key: 'efficiencyComparisonScore', label: 'Ich komme schneller zu verwertbaren Ergebnissen als ohne Hilfsmittel.' },
        ]
    },
    {
        title: 'Gesamtbewertung',
        icon: '⭐',
        questions: [
            { key: 'overallSatisfactionScore', label: 'Die Web-Applikation hat mir sehr gut gefallen.' },
            { key: 'reusabilityScore', label: 'Ich würde die Web-Applikation wieder nutzen.' },
            { key: 'recommendationScore', label: 'Ich würde die Web-Applikation weiterempfehlen.' },
        ]
    }
]

const TOTAL_STEPS = 7

const EMPTY_SCORES = {
    titleScore: 0, hookScore: 0, hashtagScore: 0, practicalScore: 0, overallQualityScore: 0,
    platformFitScore: 0, platformInfluenceScore: 0, hashtagPlatformScore: 0, platformOptimizationScore: 0,
    usabilityScore: 0, layoutScore: 0, presentationScore: 0,
    timeSavingScore: 0, generationSpeedScore: 0, supportScore: 0, efficiencyComparisonScore: 0,
    overallSatisfactionScore: 0, reusabilityScore: 0, recommendationScore: 0
}

function StarButton({ active, onClick, disabled }) {
    const [hovered, setHovered] = useState(false)
    const isActive = active || hovered
    return (
        <button
            type="button"
            className="btn"
            onClick={onClick}
            onMouseEnter={() => !disabled && setHovered(true)}
            onMouseLeave={() => setHovered(false)}
            disabled={disabled}
            style={{
                width: '44px', height: '44px',
                borderRadius: '10px',
                background: isActive ? 'rgba(167,139,250,0.2)' : '#1e1e35',
                border: `1px solid ${isActive ? '#6366f1' : '#2a2a45'}`,
                color: isActive ? '#a78bfa' : '#475569',
                fontSize: '20px', padding: 0,
                transition: 'all 0.15s',
                cursor: disabled ? 'default' : 'pointer',
                flexShrink: 0
            }}
        >⭐</button>
    )
}

function StarRow({ value, onChange, disabled }) {
    return (
        <div className="d-flex align-items-center gap-2">
            {[1, 2, 3, 4, 5].map(s => (
                <StarButton
                    key={s}
                    active={s <= value}
                    onClick={() => !disabled && onChange(s)}
                    disabled={disabled}
                />
            ))}
            {value > 0 && (
                <span style={{ fontSize: '11px', color: '#a78bfa', marginLeft: '2px' }}>
                    {value}/5
                </span>
            )}
        </div>
    )
}

function ProgressBar({ currentStep }) {
    const progress = Math.round((currentStep / (TOTAL_STEPS - 1)) * 100)
    return (
        <div className="mb-4">
            <div className="d-flex justify-content-between align-items-center mb-2">
                <span style={{ fontSize: '11px', color: '#475569' }}>
                    Schritt {currentStep + 1} von {TOTAL_STEPS}
                </span>
                <span style={{ fontSize: '11px', color: '#6366f1' }}>
                    {progress}%
                </span>
            </div>
            <div style={{ height: '4px', background: '#1e1e35', borderRadius: '2px' }}>
                <div style={{
                    height: '100%', borderRadius: '2px',
                    background: 'linear-gradient(90deg, #6366f1, #a78bfa)',
                    width: `${progress}%`,
                    transition: 'width 0.35s ease'
                }} />
            </div>
        </div>
    )
}

function SummaryBox({ summary }) {
    const items = [
        { label: 'Qualität', value: summary.qualityAverage },
        { label: 'Plattform', value: summary.platformAverage },
        { label: 'Usability', value: summary.usabilityAverage },
        { label: 'Zeiteffizienz', value: summary.timeEfficiencyAverage },
        { label: 'Gesamt', value: summary.overallAverage },
    ]
    return (
        <div style={{
            background: '#0a0a14', border: '1px solid #2a2a45',
            borderRadius: '12px', padding: '18px', marginBottom: '20px'
        }}>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <span style={{ fontSize: '13px', color: '#94a3b8', fontWeight: '600' }}>
                    📊 Aktuelle Auswertung
                </span>
                <span style={{
                    fontSize: '11px', color: '#4ade80',
                    background: 'rgba(74,222,128,0.1)',
                    border: '1px solid rgba(74,222,128,0.2)',
                    borderRadius: '6px', padding: '2px 10px'
                }}>
                    {summary.totalRatings} Bewertung{summary.totalRatings !== 1 ? 'en' : ''}
                </span>
            </div>
            <div className="row g-2 text-center">
                {items.map(item => (
                    <div key={item.label} className="col-6 col-sm-4 col-md">
                        <div style={{
                            background: '#13131f', border: '1px solid #1e1e35',
                            borderRadius: '10px', padding: '12px 8px'
                        }}>
                            <div style={{
                                fontSize: '22px', fontWeight: '700',
                                color: item.value > 0 ? '#a78bfa' : '#334155',
                                lineHeight: 1, marginBottom: '4px'
                            }}>
                                {item.value > 0 ? item.value.toFixed(1) : '–'}
                            </div>
                            <div style={{
                                fontSize: '10px', color: '#475569',
                                textTransform: 'uppercase', letterSpacing: '0.5px'
                            }}>
                                {item.label}
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default function RatingPage() {
    const { getAccessTokenSilently } = useAuth0()
    const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050'

    const [background, setBackground] = useState({ videosPerMonth: '', mainPlatform: '', experience: '' })
    const [scores, setScores] = useState(EMPTY_SCORES)
    const [comment, setComment] = useState('')
    const [currentStep, setCurrentStep] = useState(0)

    const [mode, setMode] = useState('loading')
    const [submitting, setSubmitting] = useState(false)
    const [rerating, setRerating] = useState(false)
    const [error, setError] = useState(null)
    const [summary, setSummary] = useState(null)

    useEffect(() => {
        const init = async () => {
            try {
                const token = await getAccessTokenSilently()
                const res = await fetch(`${apiUrl}/api/ratings/my`, {
                    headers: { Authorization: `Bearer ${token}` }
                })
                const data = await res.json()
                if (data.hasRated) {
                    const sRes = await fetch(`${apiUrl}/api/ratings/summary`, {
                        headers: { Authorization: `Bearer ${token}` }
                    })
                    setSummary(await sRes.json())
                    setMode('rated')
                } else {
                    setMode('form')
                }
            } catch (err) {
                console.error(err)
                setMode('form')
            }
        }
        init()
    }, [getAccessTokenSilently, apiUrl])

    async function handleReRate() {
        setRerating(true)
        setError(null)
        try {
            const token = await getAccessTokenSilently()
            await fetch(`${apiUrl}/api/ratings/my`, {
                method: 'DELETE',
                headers: { Authorization: `Bearer ${token}` }
            })
            setBackground({ videosPerMonth: '', mainPlatform: '', experience: '' })
            setScores(EMPTY_SCORES)
            setComment('')
            setCurrentStep(0)
            setMode('form')
        } catch {
            setError('Fehler beim Zurücksetzen der Bewertung.')
        } finally {
            setRerating(false)
        }
    }

    async function handleSubmit() {
        setSubmitting(true)
        setError(null)
        try {
            const token = await getAccessTokenSilently()
            const res = await fetch(`${apiUrl}/api/ratings`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify({ ...background, ...scores, comment })
            })
            if (!res.ok) throw new Error(await res.text() || `HTTP ${res.status}`)
            const sRes = await fetch(`${apiUrl}/api/ratings/summary`, {
                headers: { Authorization: `Bearer ${token}` }
            })
            setSummary(await sRes.json())
            setMode('submitted')
        } catch (err) {
            setError(err.message ?? 'Fehler beim Speichern.')
        } finally {
            setSubmitting(false)
        }
    }

    function isStepValid() {
        if (currentStep === 0) {
            return Object.values(background).every(v => v !== '')
        }
        if (currentStep >= 1 && currentStep <= 5) {
            const step = SCORE_STEPS[currentStep - 1]
            return step.questions.every(q => scores[q.key] > 0)
        }
        return true
    }

    function handleNext() {
        if (!isStepValid()) {
            setError('Bitte alle Fragen in diesem Schritt beantworten.')
            return
        }
        setError(null)
        setCurrentStep(s => s + 1)
    }

    function handleBack() {
        setError(null)
        setCurrentStep(s => s - 1)
    }

    if (mode === 'loading') return (
        <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '200px' }}>
            <div className="spinner-border" style={{ color: '#6366f1' }} />
        </div>
    )

    return (
        <div className="container-fluid px-3 px-md-4 py-3">
            <div className="row justify-content-center">
                <div className="col-12 col-sm-10 col-md-8 col-lg-6 col-xl-5">

                    <h2 style={{ margin: '0 0 4px', fontSize: '20px', fontWeight: '700', color: '#fff' }}>
                        Web-Applikation bewerten
                    </h2>
                    <p style={{ margin: '0 0 20px', color: '#475569', fontSize: '13px' }}>
                        Dein Feedback ist Teil der wissenschaftlichen Auswertung dieser Bachelorarbeit.
                    </p>

                    {(mode === 'rated' || mode === 'submitted') && (
                        <>
                            <div className="d-flex justify-content-between align-items-center mb-4" style={{
                                background: 'rgba(74,222,128,0.08)',
                                border: '1px solid rgba(74,222,128,0.2)',
                                borderRadius: '10px', padding: '12px 16px'
                            }}>
                                <span style={{ fontSize: '13px', color: '#4ade80', fontWeight: '500' }}>
                                    {mode === 'submitted'
                                        ? '🎉 Vielen Dank für deine Bewertung!'
                                        : '✅ Du hast bereits eine Bewertung abgegeben.'
                                    }
                                </span>
                                <button
                                    className="btn btn-sm"
                                    onClick={handleReRate}
                                    disabled={rerating}
                                    style={{
                                        background: '#1e1e35', border: '1px solid #2a2a45',
                                        color: '#a78bfa', fontSize: '11px',
                                        borderRadius: '7px', padding: '4px 12px',
                                        whiteSpace: 'nowrap'
                                    }}
                                >
                                    {rerating
                                        ? <span className="spinner-border spinner-border-sm" style={{ color: '#a78bfa' }} />
                                        : '🔄 Neu bewerten'
                                    }
                                </button>
                            </div>
                            {summary && <SummaryBox summary={summary} />}
                        </>
                    )}

                    {mode === 'form' && (
                        <>
                            <ProgressBar currentStep={currentStep} />

                            {currentStep === 0 && (
                                <div style={{
                                    background: '#0f0f1a', border: '1px solid #2a2a45',
                                    borderRadius: '12px', padding: '20px'
                                }}>
                                    <div style={{
                                        fontSize: '10px', color: '#6366f1',
                                        textTransform: 'uppercase', letterSpacing: '1px',
                                        fontWeight: '600', marginBottom: '4px'
                                    }}>
                                        Schritt 1 von {TOTAL_STEPS}
                                    </div>
                                    <div style={{
                                        fontSize: '16px', fontWeight: '700',
                                        color: '#fff', marginBottom: '18px'
                                    }}>
                                        👤 Angaben zur Person
                                    </div>
                                    {Object.entries(BACKGROUND_QUESTIONS).map(([key, { label, options }]) => (
                                        <div key={key} className="mb-4">
                                            <div style={{
                                                fontSize: '13px', color: '#94a3b8',
                                                marginBottom: '10px', lineHeight: '1.5'
                                            }}>
                                                {label}
                                            </div>
                                            <div className="d-flex flex-wrap gap-2">
                                                {options.map(opt => (
                                                    <button
                                                        key={opt}
                                                        type="button"
                                                        className="btn btn-sm"
                                                        onClick={() => setBackground(prev => ({ ...prev, [key]: opt }))}
                                                        style={{
                                                            background: background[key] === opt
                                                                ? 'rgba(167,139,250,0.2)' : '#1e1e35',
                                                            border: `1px solid ${background[key] === opt ? '#6366f1' : '#2a2a45'}`,
                                                            color: background[key] === opt ? '#a78bfa' : '#64748b',
                                                            fontSize: '12px', borderRadius: '8px',
                                                            padding: '6px 14px', transition: 'all 0.15s'
                                                        }}
                                                    >
                                                        {opt}
                                                    </button>
                                                ))}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}

                            {currentStep >= 1 && currentStep <= 5 && (() => {
                                const cat = SCORE_STEPS[currentStep - 1]
                                return (
                                    <div style={{
                                        background: '#0f0f1a', border: '1px solid #2a2a45',
                                        borderRadius: '12px', padding: '20px'
                                    }}>
                                        <div style={{
                                            fontSize: '10px', color: '#6366f1',
                                            textTransform: 'uppercase', letterSpacing: '1px',
                                            fontWeight: '600', marginBottom: '4px'
                                        }}>
                                            Schritt {currentStep + 1} von {TOTAL_STEPS}
                                        </div>
                                        <div style={{
                                            fontSize: '16px', fontWeight: '700',
                                            color: '#fff', marginBottom: '18px'
                                        }}>
                                            {cat.icon} {cat.title}
                                        </div>
                                        <div style={{
                                            fontSize: '11px', color: '#475569',
                                            marginBottom: '16px'
                                        }}>
                                            Skala: 1 = stimme überhaupt nicht zu &nbsp;·&nbsp; 5 = stimme voll zu
                                        </div>
                                        {cat.questions.map((q, idx) => (
                                            <div key={q.key} style={{
                                                background: '#0a0a14', border: '1px solid #1e1e35',
                                                borderRadius: '10px', padding: '14px',
                                                marginBottom: idx < cat.questions.length - 1 ? '10px' : 0
                                            }}>
                                                <div style={{
                                                    fontSize: '13px', color: '#94a3b8',
                                                    marginBottom: '12px', lineHeight: '1.5'
                                                }}>
                                                    {q.label}
                                                </div>
                                                <StarRow
                                                    value={scores[q.key]}
                                                    onChange={val => setScores(prev => ({ ...prev, [q.key]: val }))}
                                                    disabled={false}
                                                />
                                            </div>
                                        ))}
                                    </div>
                                )
                            })()}

                            {currentStep === 6 && (
                                <div style={{
                                    background: '#0f0f1a', border: '1px solid #2a2a45',
                                    borderRadius: '12px', padding: '20px'
                                }}>
                                    <div style={{
                                        fontSize: '10px', color: '#6366f1',
                                        textTransform: 'uppercase', letterSpacing: '1px',
                                        fontWeight: '600', marginBottom: '4px'
                                    }}>
                                        Schritt 7 von {TOTAL_STEPS} — Letzter Schritt
                                    </div>
                                    <div style={{
                                        fontSize: '16px', fontWeight: '700',
                                        color: '#fff', marginBottom: '18px'
                                    }}>
                                        💬 Offene Frage
                                    </div>
                                    <div style={{
                                        fontSize: '13px', color: '#94a3b8', marginBottom: '12px'
                                    }}>
                                        Was sollte an der Web-Applikation verbessert werden?{' '}
                                        <span style={{ color: '#334155' }}>(optional)</span>
                                    </div>
                                    <textarea
                                        className="form-control"
                                        placeholder="Dein Feedback..."
                                        value={comment}
                                        onChange={e => setComment(e.target.value)}
                                        maxLength={1000}
                                        rows={4}
                                        style={{
                                            background: '#0a0a14', border: '1px solid #2a2a45',
                                            color: '#e2e8f0', fontSize: '13px',
                                            borderRadius: '10px', resize: 'vertical'
                                        }}
                                    />
                                </div>
                            )}

                            {error && (
                                <div className="mt-3" style={{
                                    background: 'rgba(239,68,68,0.1)',
                                    border: '1px solid rgba(239,68,68,0.3)',
                                    borderRadius: '10px', padding: '12px 16px',
                                    color: '#ef4444', fontSize: '13px'
                                }}>
                                    {error}
                                </div>
                            )}

                            <div className="d-flex gap-2 mt-3 mb-4">
                                {currentStep > 0 && (
                                    <button
                                        className="btn"
                                        onClick={handleBack}
                                        disabled={submitting}
                                        style={{
                                            background: '#1e1e35', border: '1px solid #2a2a45',
                                            color: '#64748b', fontSize: '13px',
                                            borderRadius: '10px', padding: '11px 20px',
                                            flex: '0 0 auto'
                                        }}
                                    >
                                        ← Zurück
                                    </button>
                                )}

                                {currentStep < TOTAL_STEPS - 1 ? (
                                    <button
                                        className="btn flex-grow-1"
                                        onClick={handleNext}
                                        style={{
                                            background: isStepValid()
                                                ? 'linear-gradient(135deg, #6366f1, #a78bfa)'
                                                : '#1e1e35',
                                            border: 'none',
                                            color: isStepValid() ? '#fff' : '#475569',
                                            fontSize: '14px', fontWeight: '700',
                                            borderRadius: '10px', padding: '11px',
                                            transition: 'all 0.2s'
                                        }}
                                    >
                                        Weiter →
                                    </button>
                                ) : (
                                    <button
                                        className="btn flex-grow-1"
                                        onClick={handleSubmit}
                                        disabled={submitting}
                                        style={{
                                            background: 'linear-gradient(135deg, #6366f1, #a78bfa)',
                                            border: 'none', color: '#fff',
                                            fontSize: '14px', fontWeight: '700',
                                            borderRadius: '10px', padding: '11px',
                                        }}
                                    >
                                        {submitting
                                            ? <><span className="spinner-border spinner-border-sm me-2" />Wird gespeichert...</>
                                            : '⭐ Bewertung abschicken'
                                        }
                                    </button>
                                )}
                            </div>
                        </>
                    )}

                </div>
            </div>
        </div>
    )
}
