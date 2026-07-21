import { useEffect, useState } from 'react';
import api from '../services/api';
import DataTable from '../components/DataTable';
import Modal from '../components/Modal';
import type { Warehouse } from '../types';

export default function Warehouses() {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState({ name: '', code: '', address: '' });
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const load = () => {
    setLoading(true);
    api.get('/Warehouses').then((res) => setWarehouses(res.data.data || [])).finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const handleSave = async () => {
    setSaving(true); setError('');
    try {
      await api.post('/Warehouses', { ...form, address: form.address || null });
      setModalOpen(false); setForm({ name: '', code: '', address: '' }); load();
    } catch (e: unknown) {
      const msg = (e as { response?: { data?: { message?: string } } }).response?.data?.message;
      setError(msg || 'Error al guardar');
    } finally { setSaving(false); }
  };

  const input = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500";
  const label = "block text-sm font-medium text-gray-700 mb-1";

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'code', label: 'Código' },
    { key: 'name', label: 'Nombre' },
    { key: 'address', label: 'Dirección' },
    { key: 'isDefault', label: 'Principal', render: (w: Warehouse) => w.isDefault ? '✅' : '' },
  ];

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Bodegas</h1>
        <div className="flex gap-3 items-center">
          <span className="text-sm text-gray-500">{warehouses.length} registros</span>
          <button onClick={() => { setForm({ name: '', code: '', address: '' }); setError(''); setModalOpen(true); }} className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium">+ Nueva</button>
        </div>
      </div>
      <DataTable columns={columns} data={warehouses} loading={loading} />

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title="Nueva Bodega">
        <div className="space-y-4">
          <div><label className={label}>Código</label><input className={input} value={form.code} onChange={(e) => setForm((f) => ({ ...f, code: e.target.value }))} /></div>
          <div><label className={label}>Nombre</label><input className={input} value={form.name} onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))} /></div>
          <div><label className={label}>Dirección</label><input className={input} value={form.address} onChange={(e) => setForm((f) => ({ ...f, address: e.target.value }))} /></div>
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
