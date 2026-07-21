import { useEffect, useState } from 'react';
import api from '../services/api';
import DataTable from '../components/DataTable';
import Modal from '../components/Modal';
import type { Order, Client, Supplier, Product } from '../types';

const statusColors: Record<string, string> = {
  pending: 'bg-yellow-100 text-yellow-800', confirmed: 'bg-blue-100 text-blue-800',
  processing: 'bg-purple-100 text-purple-800', completed: 'bg-green-100 text-green-800',
  cancelled: 'bg-red-100 text-red-800',
};

const emptyDetail = { productId: 0, quantity: 1, unitPrice: 0, taxRate: 19 };

export default function Orders() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [clients, setClients] = useState<Client[]>([]);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [statusModal, setStatusModal] = useState<Order | null>(null);
  const [newStatus, setNewStatus] = useState('confirmed');
  const [form, setForm] = useState({ orderType: 'sale' as 'sale' | 'purchase', clientId: 0, supplierId: 0, notes: '' });
  const [details, setDetails] = useState([{ ...emptyDetail }]);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const load = () => {
    setLoading(true);
    api.get('/Orders', { params: { page, pageSize: 10, search: search || undefined } })
      .then((res) => { setOrders(res.data.data.items || []); setTotalPages(res.data.data.totalPages); })
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [page, search]);
  useEffect(() => {
    api.get('/Clients', { params: { pageSize: 100 } }).then((r) => setClients(r.data.data.items || []));
    api.get('/Suppliers', { params: { pageSize: 100 } }).then((r) => setSuppliers(r.data.data.items || []));
    api.get('/Products', { params: { pageSize: 100 } }).then((r) => setProducts(r.data.data.items || []));
  }, []);

  const openCreate = () => { setForm({ orderType: 'sale', clientId: 0, supplierId: 0, notes: '' }); setDetails([{ ...emptyDetail }]); setError(''); setModalOpen(true); };

  const addDetail = () => setDetails((d) => [...d, { ...emptyDetail }]);
  const removeDetail = (i: number) => setDetails((d) => d.filter((_, idx) => idx !== i));
  const updateDetail = (i: number, key: string, value: number) => setDetails((d) => d.map((item, idx) => idx === i ? { ...item, [key]: value } : item));

  const handleSave = async () => {
    setSaving(true); setError('');
    try {
      const body = {
        orderType: form.orderType,
        clientId: form.orderType === 'sale' ? form.clientId : undefined,
        supplierId: form.orderType === 'purchase' ? form.supplierId : undefined,
        notes: form.notes || undefined,
        details: details.filter((d) => d.productId > 0),
      };
      await api.post('/Orders', body);
      setModalOpen(false); load();
    } catch (e: unknown) {
      const msg = (e as { response?: { data?: { message?: string } } }).response?.data?.message;
      setError(msg || 'Error al guardar');
    } finally { setSaving(false); }
  };

  const handleStatus = async () => {
    if (!statusModal) return;
    try { await api.put(`/Orders/${statusModal.id}/status`, { status: newStatus }); setStatusModal(null); load(); } catch { /* ignore */ }
  };

  const set = (key: string, value: string | number) => setForm((f) => ({ ...f, [key]: value }));
  const input = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500";
  const select = `${input} bg-white`;
  const label = "block text-sm font-medium text-gray-700 mb-1";

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'orderNumber', label: 'Número' },
    { key: 'orderType', label: 'Tipo', render: (o: Order) => o.orderType === 'sale' ? 'Venta' : 'Compra' },
    { key: 'party', label: 'Cliente/Proveedor', render: (o: Order) => o.clientName || o.supplierName || '-' },
    { key: 'total', label: 'Total', render: (o: Order) => `$${o.total.toLocaleString()}` },
    { key: 'status', label: 'Estado', render: (o: Order) => <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[o.status] || 'bg-gray-100'}`}>{o.status}</span> },
    { key: 'actions', label: '', render: (o: Order) => (
      <button onClick={() => { setStatusModal(o); setNewStatus('confirmed'); }} className="text-blue-600 hover:text-blue-800 text-xs">Estado</button>
    )},
  ];

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Órdenes</h1>
        <div className="flex gap-3 items-center">
          <input value={search} onChange={(e) => { setSearch(e.target.value); setPage(1); }} placeholder="Buscar..." className={`${input} w-48`} />
          <button onClick={openCreate} className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium">+ Nueva</button>
        </div>
      </div>
      <DataTable columns={columns} data={orders} loading={loading} />
      {totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-4">
          <button disabled={page <= 1} onClick={() => setPage((p) => p - 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Anterior</button>
          <span className="px-3 py-1 text-sm text-gray-600">{page} / {totalPages}</span>
          <button disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Siguiente</button>
        </div>
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title="Nueva Orden" wide>
        <div className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className={label}>Tipo</label>
              <select className={select} value={form.orderType} onChange={(e) => set('orderType', e.target.value)}>
                <option value="sale">Venta</option><option value="purchase">Compra</option>
              </select>
            </div>
            {form.orderType === 'sale' ? (
              <div>
                <label className={label}>Cliente</label>
                <select className={select} value={form.clientId} onChange={(e) => set('clientId', Number(e.target.value))}>
                  <option value={0}>Seleccionar...</option>
                  {clients.map((c) => <option key={c.id} value={c.id}>{c.person.firstName} {c.person.lastName}</option>)}
                </select>
              </div>
            ) : (
              <div>
                <label className={label}>Proveedor</label>
                <select className={select} value={form.supplierId} onChange={(e) => set('supplierId', Number(e.target.value))}>
                  <option value={0}>Seleccionar...</option>
                  {suppliers.map((s) => <option key={s.id} value={s.id}>{s.person.firstName} {s.person.lastName}</option>)}
                </select>
              </div>
            )}
          </div>
          <div><label className={label}>Notas</label><textarea className={input} rows={2} value={form.notes} onChange={(e) => set('notes', e.target.value)} /></div>

          <div>
            <div className="flex justify-between items-center mb-2">
              <label className={label + ' mb-0'}>Artículos</label>
              <button type="button" onClick={addDetail} className="text-blue-600 text-sm hover:underline">+ Agregar</button>
            </div>
            {details.map((d, i) => (
              <div key={i} className="grid grid-cols-5 gap-2 mb-2 items-end">
                <div className="col-span-2">
                  <select className={select} value={d.productId} onChange={(e) => updateDetail(i, 'productId', Number(e.target.value))}>
                    <option value={0}>Producto...</option>
                    {products.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
                  </select>
                </div>
                <input type="number" className={input} placeholder="Cant." value={d.quantity} onChange={(e) => updateDetail(i, 'quantity', Number(e.target.value))} />
                <input type="number" step="0.01" className={input} placeholder="Precio" value={d.unitPrice} onChange={(e) => updateDetail(i, 'unitPrice', Number(e.target.value))} />
                <div className="flex gap-1">
                  <input type="number" step="0.01" className={input} placeholder="IVA" value={d.taxRate} onChange={(e) => updateDetail(i, 'taxRate', Number(e.target.value))} />
                  {details.length > 1 && <button type="button" onClick={() => removeDetail(i)} className="text-red-500 hover:text-red-700 px-1">&times;</button>}
                </div>
              </div>
            ))}
          </div>
          {error && <p className="text-red-600 text-sm">{error}</p>}
          <div className="flex justify-end gap-3 pt-2">
            <button onClick={() => setModalOpen(false)} className="px-4 py-2 border rounded-lg text-sm">Cancelar</button>
            <button onClick={handleSave} disabled={saving} className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm disabled:opacity-50">{saving ? 'Guardando...' : 'Crear'}</button>
          </div>
        </div>
      </Modal>

      <Modal open={!!statusModal} onClose={() => setStatusModal(null)} title="Cambiar Estado">
        <div className="space-y-4">
          <p className="text-sm text-gray-600">Orden: <strong>{statusModal?.orderNumber}</strong></p>
          <div>
            <label className={label}>Nuevo Estado</label>
            <select className={select} value={newStatus} onChange={(e) => setNewStatus(e.target.value)}>
              <option value="pending">Pendiente</option><option value="confirmed">Confirmada</option>
              <option value="processing">Procesando</option><option value="completed">Completada</option>
              <option value="cancelled">Cancelada</option>
            </select>
          </div>
          <div className="flex justify-end gap-3">
            <button onClick={() => setStatusModal(null)} className="px-4 py-2 border rounded-lg text-sm">Cancelar</button>
            <button onClick={handleStatus} className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm">Aplicar</button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
