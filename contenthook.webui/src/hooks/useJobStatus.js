import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export function useJobStatus(jobId) {
  const [status, setStatus] = useState(null);
  const [payload, setPayload] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!jobId) return;

    const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5050';

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiUrl}/hubs/progress`)
      .withAutomaticReconnect()
      .build();

    connection.on('JobStatus', (data) => {
      console.log('SignalR JobStatus:', data);
      setStatus(data.status);
      setPayload(data.payload);
      if (data.status === 'failed') {
        setError(data.payload?.error ?? 'Unknown error');
      }
    });

    connection.start()
      .then(() => {
        console.log('SignalR connected, joining group:', jobId);
        return connection.invoke('JoinJobGroup', jobId);
      })
      .catch((err) => {
        console.error('SignalR connection failed:', err);
        setError('Connection failed');
      });

    return () => {
      connection.invoke('LeaveJobGroup', jobId)
        .catch(() => {})
        .finally(() => connection.stop());
    };
  }, [jobId]);

  return { status, payload, error };
}