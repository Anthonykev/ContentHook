import { useState } from 'react';
import { useJobStatus } from '../hooks/useJobStatus';
import { useAuth0 } from '@auth0/auth0-react';

export default function VideoUpload() {
  const [jobId, setJobId] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [uploadError, setUploadError] = useState(null);
  const { status, payload, error } = useJobStatus(jobId);
  const { getAccessTokenSilently } = useAuth0();

  async function handleUpload(e) {
    const file = e.target.files[0];
    if (!file) return;

    setUploading(true);
    setJobId(null);
    setUploadError(null);

    const formData = new FormData();
    formData.append('file', file);
    const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050';

    try {
      const token = await getAccessTokenSilently({
        authorizationParams: {
          audience: import.meta.env.VITE_AUTH0_AUDIENCE
        }
      });

      const response = await fetch(
        `${apiUrl}/api/Videos?platform=tiktok`,
        {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${token}`
          },
          body: formData
        }
      );

      if (!response.ok) {
        const text = await response.text();
        throw new Error(text || `HTTP ${response.status}`);
      }

      const data = await response.json();
      setJobId(data.jobId);

    } catch (err) {
      console.error('Upload failed:', err);
      setUploadError(err.message ?? 'Upload failed');
    } finally {
      setUploading(false);
    }
  }

  return (
    <div style={{ padding: '2rem', fontFamily: 'sans-serif' }}>
      <h2>ContentHook — Video Upload</h2>
      <input
        type="file"
        accept="video/mp4,video/quicktime,.mp4,.mov"
        onChange={handleUpload}
        disabled={uploading}
      />
      {uploading && <p>⏳ Uploading...</p>}
      {uploadError && (
        <p style={{ color: 'red' }}>❌ {uploadError}</p>
      )}
      {status === 'transcribing' && <p>🎙 Transkription läuft...</p>}
      {status === 'generating'   && <p>✨ KI generiert Titel, Hook & Hashtags...</p>}
      {status === 'done' && (
        <div style={{ marginTop: '1rem', padding: '1rem',
          border: '1px solid green', borderRadius: '8px' }}>
          <h3>✅ Fertig!</h3>
          <pre>{JSON.stringify(payload, null, 2)}</pre>
        </div>
      )}
      {(status === 'failed' || error) && (
        <div style={{ marginTop: '1rem', padding: '1rem',
          border: '1px solid red', borderRadius: '8px' }}>
          <h3>❌ Fehler</h3>
          <p>{error}</p>
        </div>
      )}
      {jobId && (
        <p style={{ color: 'gray', fontSize: '12px' }}>
          JobId: {jobId} | Status: {status ?? 'waiting...'}
        </p>
      )}
    </div>
  );
}