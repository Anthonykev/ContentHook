export default function ImpressumPage() {
    return (
        <div className="row justify-content-center">
            <div className="col-12 col-lg-10 col-xl-9">
                <div className="page-section-card">
                    <h2 className="page-title mb-4">Impressum</h2>
                    <p className="text-secondary-custom mb-4" style={{ fontSize: '14px' }}>
                        Angaben gemäß § 5 ECG und § 25 MedienG
                    </p>
                    <div className="page-copy">
                        <p className="text-white fw-bold mb-1">Medieninhaber & Verantwortlicher</p>
                        <p className="mb-0">Kodzo Kevin Anthony</p>
                        <p className="mb-0">c/o FH Technikum Wien</p>
                        <p className="mb-0">Höchstädtplatz 6, 1200 Wien</p>
                        <p className="mb-4">Österreich</p>

                        <p className="text-white fw-bold mb-1">Kontakt</p>
                        <p className="mb-4">
                            E-Mail:{' '}
                            <a href="mailto:kevstillkev1@gmail.com" style={{ color: '#a78bfa' }}>
                                kevstillkev1@gmail.com
                            </a>
                        </p>

                        <p className="text-white fw-bold mb-1">Zweck der Website</p>
                        <p className="mb-4">
                            Betrieb und Bereitstellung der Web-Applikation „ContentHook" im Rahmen einer
                            Bachelorarbeit an der FH Technikum Wien zur Unterstützung von Content Creatorinnen
                            und Content Creatorn bei der Generierung von Titeln, Hooks und Hashtags für
                            Kurzvideo-Plattformen (TikTok, Instagram Reels).
                        </p>

                        <p className="text-white fw-bold mb-1">Grundlegende Richtung</p>
                        <p className="mb-4">
                            Information über und Bereitstellung der Web-Applikation „ContentHook" zu Test-,
                            Demonstrations- und Evaluierungszwecken im Rahmen einer wissenschaftlichen
                            Bachelorarbeit. Die Anwendung befindet sich im Prototyp-Stadium.
                        </p>

                        <p className="text-white fw-bold mb-1">Haftungsausschluss</p>
                        <p className="mb-4">
                            Die Inhalte dieser Website wurden mit größtmöglicher Sorgfalt erstellt.
                            Es wird jedoch keine Gewähr für die Richtigkeit, Vollständigkeit und
                            Aktualität der bereitgestellten Inhalte übernommen. Die durch den Betreiber
                            erstellten Inhalte und Werke unterliegen dem österreichischen Urheberrecht.
                        </p>

                        <p className="text-white fw-bold mb-1">Akademische Betreuung</p>
                        <p className="mb-0">Betreuer: Ing. Dr. techn. Dominik Dolezal, MSc</p>
                        <p className="mb-0">FH Technikum Wien — Degree Program Computer Science Dual</p>
                    </div>
                </div>
            </div>
        </div>
    )
}