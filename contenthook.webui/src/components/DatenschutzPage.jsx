export default function DatenschutzPage() {
    return (
        <div className="row justify-content-center">
            <div className="col-12 col-lg-10 col-xl-9">
                <div className="page-section-card">
                    <h2 className="page-title mb-4">Datenschutzerklärung</h2>
                    <p className="text-secondary-custom mb-4" style={{ fontSize: '14px' }}>
                        Gemäß DSGVO und TKG 2021 — Stand: März 2026
                    </p>

                    <div className="page-copy">

                        <p className="text-white fw-bold mb-1">1. Verantwortlicher</p>
                        <p className="mb-4">
                            Kodzo Kevin Anthony<br />
                            c/o FH Technikum Wien, Höchstädtplatz 6, 1200 Wien<br />
                            E-Mail:{' '}
                            <a href="mailto:kevstillkev1@gmail.com" style={{ color: '#a78bfa' }}>
                                kevstillkev1@gmail.com
                            </a>
                        </p>

                        <p className="text-white fw-bold mb-1">2. Zweck der Datenverarbeitung</p>
                        <p className="mb-4">
                            „ContentHook" ist eine im Rahmen einer Bachelorarbeit entwickelte Web-Applikation.
                            Sie verarbeitet hochgeladene Kurzvideos, erstellt Transkripte und generiert mithilfe
                            eines KI-Dienstes Vorschläge für Titel, Hooks und Hashtags für TikTok und Instagram Reels.
                            Testnutzerinnen und Testnutzer können die Anwendung bewerten. Die Bewertungsdaten
                            dienen der wissenschaftlichen Auswertung im Rahmen der Bachelorarbeit.
                        </p>

                        <p className="text-white fw-bold mb-1">3. Verarbeitete Daten</p>
                        <p className="mb-1"><span className="text-white">a) Login- und Kontodaten (Auth0)</span></p>
                        <p className="mb-3">
                            Für die Anmeldung wird Auth0 verwendet. Verarbeitet werden Benutzerkennung,
                            Name, E-Mail-Adresse sowie Authentifizierungsmetadaten.{' '}
                            <span className="text-secondary-custom">Rechtsgrundlage: Art. 6 Abs. 1 lit. b DSGVO</span>
                        </p>

                        <p className="mb-1"><span className="text-white">b) Hochgeladene Videos</span></p>
                        <p className="mb-3">
                            Videodateien werden ausschließlich zur Transkription verarbeitet und
                            unmittelbar danach automatisch gelöscht. Eine dauerhafte Speicherung findet nicht statt.{' '}
                            <span className="text-secondary-custom">Rechtsgrundlage: Art. 6 Abs. 1 lit. b DSGVO</span>
                        </p>

                        <p className="mb-1"><span className="text-white">c) Transkripte & Generierungen</span></p>
                        <p className="mb-3">
                            Transkripttext, Plattform, Tonalität, generierter Titel, Hook und Hashtags
                            werden gespeichert und sind im Verlauf abrufbar. Nutzer können ihre Daten
                            jederzeit löschen.{' '}
                            <span className="text-secondary-custom">Rechtsgrundlage: Art. 6 Abs. 1 lit. b DSGVO</span>
                        </p>

                        <p className="mb-1"><span className="text-white">d) Bewertungsdaten</span></p>
                        <p className="mb-4">
                            Bewertungen und optionale Kommentare werden ausschließlich zur
                            wissenschaftlichen Auswertung der Bachelorarbeit verwendet.{' '}
                            <span className="text-secondary-custom">Rechtsgrundlage: Art. 6 Abs. 1 lit. a DSGVO (Einwilligung)</span>
                        </p>

                        <p className="text-white fw-bold mb-1">4. Eingesetzte Drittdienste</p>
                        <p className="mb-1"><span className="text-white">Auth0</span></p>
                        <p className="mb-3">
                            Authentifizierungsdienstleister.{' '}
                            <a href="https://auth0.com/privacy" target="_blank" rel="noreferrer" style={{ color: '#a78bfa' }}>
                                auth0.com/privacy
                            </a>
                        </p>
                        <p className="mb-1"><span className="text-white">OpenAI</span></p>
                        <p className="mb-3">
                            KI-Dienst zur Generierung. Transkriptinhalte werden zur Verarbeitung übermittelt.{' '}
                            <a href="https://openai.com/privacy" target="_blank" rel="noreferrer" style={{ color: '#a78bfa' }}>
                                openai.com/privacy
                            </a>
                        </p>
                        <p className="mb-1"><span className="text-white">Railway / Vercel</span></p>
                        <p className="mb-4">
                            Hosting-Anbieter für Backend und Frontend. Serverlogdaten werden maximal 7 Tage gespeichert.
                        </p>

                        <p className="text-white fw-bold mb-1">5. Speicherdauer</p>
                        <p className="mb-4">
                            Videos werden unmittelbar nach der Transkription gelöscht. Transkripte und
                            Generierungen bleiben bis zur aktiven Löschung durch den Nutzer gespeichert.
                            Bewertungsdaten werden bis zum Abschluss der Bachelorarbeit aufbewahrt
                            und danach anonymisiert oder gelöscht.
                        </p>

                        <p className="text-white fw-bold mb-1">6. Cookies</p>
                        <p className="mb-4">
                            Diese Website verwendet ausschließlich technisch notwendige Cookies
                            für Anmeldung und Sitzungsverwaltung (Auth0). Es werden keine
                            Analyse-, Tracking- oder Marketing-Cookies eingesetzt.
                        </p>

                        <p className="text-white fw-bold mb-1">7. Ihre Rechte</p>
                        <p className="mb-4">
                            Sie haben das Recht auf Auskunft, Berichtigung, Löschung und
                            Einschränkung der Verarbeitung sowie Widerruf einer Einwilligung.
                            Beschwerden können an die österreichische Datenschutzbehörde gerichtet werden:{' '}
                            <a href="https://www.dsb.gv.at" target="_blank" rel="noreferrer" style={{ color: '#a78bfa' }}>
                                dsb.gv.at
                            </a>
                        </p>

                        <p className="text-white fw-bold mb-1">8. Kontakt bei Datenschutzanfragen</p>
                        <p className="mb-0">
                            E-Mail:{' '}
                            <a href="mailto:kevstillkev1@gmail.com" style={{ color: '#a78bfa' }}>
                                kevstillkev1@gmail.com
                            </a>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    )
}