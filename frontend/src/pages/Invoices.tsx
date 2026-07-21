import { useEffect, useState } from 'react';
import api from '../services/api';
import DataTable from '../components/DataTable';
import Modal from '../components/Modal';
import type { Invoice, Client, Supplier, Product } from '../types';

const statusColors: Record<string, string> = {
  issued: 'bg-yellow-100 text-yellow-800', paid: 'bg-green-100 text-green-800',
  overdue: 'bg-red-100 text-red-800', cancelled: 'bg-gray-100 text-gray-800',
};

const emptyDetail = { productId: 0, description: '', quantity: 1, unitPrice: 0, taxRate: 19 };

export default function Invoices() {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [clients, setClients] = useState<Client[]>([]);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [filter, setFilter] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [statusModal, setStatusModal] = useState<Invoice | null>(null);
  const [newStatus, setNewStatus] = useState('paid');
  const [form, setForm] = useState({ invoiceType: 'sale' as 'sale' | 'purchase', clientId: 0, supplierId: 0, notes: '', dueDate: '' });
  const [details, setDetails] = useState([{ ...emptyDetail }]);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const load = () => {
    setLoading(true);
    const params: Record<string, string | number> = { page, pageSize: 10 };
    if (filter) params.invoiceType = filter;
    api.get('/Invoices', { params })
      .then((res) => { setInvoices(res.data.data.items || []); setTotalPages(res.data.data.totalPages); })
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [page, filter]);
  useEffect(() => {
    api.get('/Clients', { params: { pageSize: 100 } }).then((r) => setClients(r.data.data.items || []));
    api.get('/Suppliers', { params: { pageSize: 100 } }).then((r) => setSuppliers(r.data.data.items || []));
    api.get('/Products', { params: { pageSize: 100 } }).then((r) => setProducts(r.data.data.items || []));
  }, []);

  const openCreate = () => { setForm({ invoiceType: 'sale', clientId: 0, supplierId: 0, notes: '', dueDate: '' }); setDetails([{ ...emptyDetail }]); setError(''); setModalOpen(true); };
  const addDetail = () => setDetails((d) => [...d, { ...emptyDetail }]);
  const removeDetail = (i: number) => setDetails((d) => d.filter((_, idx) => idx !== i));
  const updateDetail = (i: number, key: string, value: string | number) => setDetails((d) => d.map((item, idx) => idx === i ? { ...item, [key]: value } : item));

  const handleSave = async () => {
    setSaving(true); setError('');
    try {
      const body: Record<string, unknown> = {
        invoiceType: form.invoiceType,
        notes: form.notes || undefined,
        dueDate: form.dueDate || undefined,
        details: details.filter((d) => d.productId > 0).map((d) => ({ ...d, description: d.description || undefined })),
      };
      if (form.invoiceType === 'sale') body.clientId = form.clientId;
      else body.supplierId = form.supplierId;
      await api.post('/Invoices', body);
      setModalOpen(false); load();
    } catch (e: unknown) {
      setError((e as { response?: { data?: { message?: string } } }).response?.data?.message || 'Error al guardar');
    } finally { setSaving(false); }
  };

  const handleStatus = async () => {
    if (!statusModal) return;
    try { await api.put(`/Invoices/${statusModal.id}/status`, { status: newStatus }); setStatusModal(null); load(); } catch { /* ignore */ }
  };

  const set = (key: string, value: string | number) => setForm((f) => ({ ...f, [key]: value }));
  const input = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500";
  const select = `${input} bg-white`;
  const label = "block text-sm font-medium text-gray-700 mb-1";

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'invoiceNumber', label: 'Número' },
    { key: 'invoiceType', label: 'Tipo', render: (i: Invoice) => i.invoiceType === 'sale' ? 'Venta' : 'Compra' },
    { key: 'party', label: 'Cliente/Proveedor', render: (i: Invoice) => i.clientName || i.supplierName || '-' },
    { key: 'total', label: 'Total', render: (i: Invoice) => `$${i.total.toLocaleString()}` },
    { key: 'balanceDue', label: 'Saldo', render: (i: Invoice) => `$${i.balanceDue.toLocaleString()}` },
    { key: 'status', label: 'Estado', render: (i: Invoice) => <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[i.status] || 'bg-gray-100'}`}>{i.status}</span> },
    { key: 'actions', label: '', render: (i: Invoice) => <button onClick={() => { setStatusModal(i); setNewStatus('paid'); }} className="text-blue-600 hover:text-blue-800 text-xs">Estado</button> },
  ];

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Facturas</h1>
        <div className="flex gap-3 items-center">
          <select value={filter} onChange={(e) => { setFilter(e.target.value); setPage(1); }} className={`${select} w-36`}>
            <option value="">Todas</option><option value="sale">Venta</option><option value="purchase">Compra</option>
          </select>
          <button onClick={openCreate} className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium">+ Nueva</button>
        </div>
      </div>
      <DataTable columns={columns} data={invoices} loading={loading} />
      {totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-4">
          <button disabled={page <= 1} onClick={() => setPage((p) => p - 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Anterior</button>
          <span className="px-3 py-1 text-sm text-gray-600">{page} / {totalPages}</span>
          <button disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Siguiente</button>
        </div>
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title="Nueva Factura" wide>
        <div className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className={label}>Tipo</label>
              <select className={select} value={form.invoiceType} onChange={(e) => set('invoiceType', e.target.value)}>
                <option value="sale">Venta</option><option value="purchase">Compra</option>
              </select>
            </div>
            {form.invoiceType === 'sale' ? (
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
          <div><label className={label}>Fecha Vencimiento</label><input type="date" className={input} value={form.dueDate} onChange={(e) => set('dueDate', e.target.value)} /></div>
          <div><label className={label}>Notas</label><textarea className={input} rows={2} value={form.notes} onChange={(e) => set('notes', e.target.value)} /></div>
          <div>
            <div className="flex justify-between items-center mb-2">
              <label className={label + ' mb-0'}>Artículos</label>
              <button type="button" onClick={addDetail} className="text-blue-600 text-sm hover:underline">+ Agregar</button>
            </div>
            {details.map((d, i) => (
              <div key={i} className="grid grid-cols-6 gap-2 mb-2 items-end">
                <div className="col-span-2">
                  <select className={select} value={d.productId} onChange={(e) => updateDetail(i, 'productId', Number(e.target.value))}>
                    <option value={0}>Producto...</option>
                    {products.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
                  </select>
                </div>
                <input type="number" className={input} placeholder="Cant." value={d.quantity} onChange={(e) => updateDetail(i, 'quantity', Number(e.target.value))} />
                <input type="number" step="0.01" className={input} placeholder="Precio" value={d.unitPrice} onChange={(e) => updateDetail(i, 'unitPrice', Number(e.target.value))} />
                <input type="number" step="0.01" className={input} placeholder="IVA" value={d.taxRate} onChange={(e) => updateDetail(i, 'taxRate', Number(e.target.value))} />
                <div className="flex gap-1">
                  <input className={input} placeholder="Desc." value={d.description} onChange={(e) => updateDetail(i, 'description', e.target.value)} />
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
          <p className="text-sm text-gray-600">Factura: <strong>{statusModal?.invoiceNumber}</strong></p>
          <div>
            <label className={label}>Nuevo Estado</label>
            <select className={select} value={newStatus} onChange={(e) => setNewStatus(e.target.value)}>
              <option value="issued">Emitida</option><option value="paid">Pagada</option>
              <option value="overdue">Vencida</option><option value="cancelled">Cancelada</option>
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
