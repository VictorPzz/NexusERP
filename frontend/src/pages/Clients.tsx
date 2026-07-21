import { useEffect, useState } from 'react';
import api from '../services/api';
import DataTable from '../components/DataTable';
import Modal from '../components/Modal';
import type { Client } from '../types';

const emptyForm = {
  documentType: 'CC', documentNumber: '', firstName: '', lastName: '',
  email: '', phone: '', clientCode: '', creditLimit: 0, notes: '',
};

export default function Clients() {
  const [clients, setClients] = useState<Client[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Client | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const load = () => {
    setLoading(true);
    api.get('/Clients', { params: { page, pageSize: 10, search: search || undefined } })
      .then((res) => { setClients(res.data.data.items || []); setTotalPages(res.data.data.totalPages); })
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [page, search]);

  const openCreate = () => { setEditing(null); setForm(emptyForm); setError(''); setModalOpen(true); };
  const openEdit = (c: Client) => {
    setEditing(c);
    setForm({
      documentType: c.person.documentType, documentNumber: c.person.documentNumber,
      firstName: c.person.firstName, lastName: c.person.lastName,
      email: c.person.email || '', phone: c.person.phone || '',
      clientCode: c.clientCode, creditLimit: c.creditLimit, notes: c.notes || '',
    });
    setError(''); setModalOpen(true);
  };

  const handleSave = async () => {
    setSaving(true); setError('');
    try {
      if (editing) {
        await api.put(`/Clients/${editing.id}`, {
          firstName: form.firstName, lastName: form.lastName,
          email: form.email || null, phone: form.phone || null,
          creditLimit: form.creditLimit, notes: form.notes || null,
        });
      } else {
        await api.post('/Clients', {
          documentType: form.documentType, documentNumber: form.documentNumber,
          firstName: form.firstName, lastName: form.lastName,
          email: form.email || null, phone: form.phone || null,
          clientCode: form.clientCode, creditLimit: form.creditLimit, notes: form.notes || null,
        });
      }
      setModalOpen(false); load();
    } catch (e: unknown) {
      const msg = (e as { response?: { data?: { message?: string } } }).response?.data?.message;
      setError(msg || 'Error al guardar');
    } finally { setSaving(false); }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('¿Eliminar este cliente?')) return;
    try { await api.delete(`/Clients/${id}`); load(); } catch { /* ignore */ }
  };

  const set = (key: string, value: string | number) => setForm((f) => ({ ...f, [key]: value }));

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'clientCode', label: 'Código' },
    { key: 'name', label: 'Nombre', render: (c: Client) => `${c.person.firstName} ${c.person.lastName}` },
    { key: 'doc', label: 'Documento', render: (c: Client) => `${c.person.documentType}: ${c.person.documentNumber}` },
    { key: 'creditLimit', label: 'Crédito', render: (c: Client) => `$${c.creditLimit.toLocaleString()}` },
    { key: 'status', label: 'Estado', render: (c: Client) => (
      <span className={`px-2 py-1 rounded-full text-xs font-medium ${c.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-600'}`}>{c.status}</span>
    )},
    { key: 'actions', label: '', render: (c: Client) => (
      <div className="flex gap-2">
        <button onClick={() => openEdit(c)} className="text-blue-600 hover:text-blue-800 text-xs">Editar</button>
        <button onClick={() => handleDelete(c.id)} className="text-red-600 hover:text-red-800 text-xs">Eliminar</button>
      </div>
    )},
  ];

  const input = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500";
  const select = `${input} bg-white`;
  const label = "block text-sm font-medium text-gray-700 mb-1";

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Clientes</h1>
        <div className="flex gap-3 items-center">
          <input value={search} onChange={(e) => { setSearch(e.target.value); setPage(1); }} placeholder="Buscar..." className={`${input} w-48`} />
          <button onClick={openCreate} className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium">+ Nuevo</button>
        </div>
      </div>
      <DataTable columns={columns} data={clients} loading={loading} />
      {totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-4">
          <button disabled={page <= 1} onClick={() => setPage((p) => p - 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Anterior</button>
          <span className="px-3 py-1 text-sm text-gray-600">{page} / {totalPages}</span>
          <button disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Siguiente</button>
        </div>
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Editar Cliente' : 'Nuevo Cliente'}>
        <div className="space-y-4">
          {!editing && (
            <div className="grid grid-cols-2 gap-4">
              <div><label className={label}>Código</label><input className={input} value={form.clientCode} onChange={(e) => set('clientCode', e.target.value)} /></div>
              <div>
                <label className={label}>Tipo Doc.</label>
                <select className={select} value={form.documentType} onChange={(e) => set('documentType', e.target.value)}>
                  <option value="CC">CC</option><option value="CE">CE</option><option value="NIT">NIT</option><option value="PASSPORT">Pasaporte</option>
                </select>
              </div>
              <div><label className={label}>No. Documento</label><input className={input} value={form.documentNumber} onChange={(e) => set('documentNumber', e.target.value)} /></div>
            </div>
          )}
          <div className="grid grid-cols-2 gap-4">
            <div><label className={label}>Nombre</label><input className={input} value={form.firstName} onChange={(e) => set('firstName', e.target.value)} /></div>
            <div><label className={label}>Apellido</label><input className={input} value={form.lastName} onChange={(e) => set('lastName', e.target.value)} /></div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div><label className={label}>Email</label><input type="email" className={input} value={form.email} onChange={(e) => set('email', e.target.value)} /></div>
            <div><label className={label}>Teléfono</label><input className={input} value={form.phone} onChange={(e) => set('phone', e.target.value)} /></div>
          </div>
          <div><label className={label}>Límite de Crédito</label><input type="number" className={input} value={form.creditLimit} onChange={(e) => set('creditLimit', Number(e.target.value))} /></div>
          <div><label className={label}>Notas</label><textarea className={input} rows={2} value={form.notes} onChange={(e) => set('notes', e.target.value)} /></div>
          {error && <p className="text-red-600 text-sm">{error}</p>}
          <div className="flex justify-end gap-3 pt-2">
            <button onClick={() => setModalOpen(false)} className="px-4 py-2 border rounded-lg text-sm">Cancelar</button>
            <button onClick={handleSave} disabled={saving} className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm disabled:opacity-50">{saving ? 'Guardando...' : 'Guardar'}</button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
