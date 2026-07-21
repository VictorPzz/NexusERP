import { useEffect, useState } from 'react';
import api from '../services/api';
import DataTable from '../components/DataTable';
import Modal from '../components/Modal';
import type { Payment, Invoice } from '../types';

export default function Payments() {
  const [payments, setPayments] = useState<Payment[]>([]);
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState({ invoiceId: 0, amount: 0, paymentMethod: 'cash', reference: '', notes: '' });
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const load = () => {
    setLoading(true);
    api.get('/Payments', { params: { page, pageSize: 10 } })
      .then((res) => { setPayments(res.data.data.items || []); setTotalPages(res.data.data.totalPages); })
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [page]);
  useEffect(() => {
    api.get('/Invoices', { params: { pageSize: 100 } }).then((r) => setInvoices(r.data.data.items || []));
  }, []);

  const openCreate = () => {
    setForm({ invoiceId: 0, amount: 0, paymentMethod: 'cash', reference: '', notes: '' });
    setError('');
    setModalOpen(true);
  };

  const handleSave = async () => {
    setSaving(true);
    setError('');
    try {
      await api.post('/Payments', {
        invoiceId: form.invoiceId,
        amount: form.amount,
        paymentMethod: form.paymentMethod,
        reference: form.reference || undefined,
        notes: form.notes || undefined,
      });
      setModalOpen(false);
      load();
    } catch (e: unknown) {
      setError((e as { response?: { data?: { message?: string } } }).response?.data?.message || 'Error al guardar');
    } finally {
      setSaving(false);
    }
  };

  const set = (key: string, value: string | number) => setForm((f) => ({ ...f, [key]: value }));
  const inputCls = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500";
  const selectCls = inputCls + " bg-white";
  const labelCls = "block text-sm font-medium text-gray-700 mb-1";

  const selectedInvoice = invoices.find((i) => i.id === form.invoiceId);

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'paymentNumber', label: 'Número' },
    { key: 'invoiceNumber', label: 'Factura' },
    { key: 'amount', label: 'Monto', render: (p: Payment) => '$' + p.amount.toLocaleString() },
    { key: 'paymentMethod', label: 'Método', render: (p: Payment) => {
      const m: Record<string, string> = { cash: 'Efectivo', card: 'Tarjeta', transfer: 'Transferencia', check: 'Cheque', other: 'Otro' };
      return m[p.paymentMethod] || p.paymentMethod;
    }},
    { key: 'reference', label: 'Referencia' },
    { key: 'paymentDate', label: 'Fecha', render: (p: Payment) => new Date(p.paymentDate).toLocaleDateString('es-CO') },
  ];

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Pagos</h1>
        <button onClick={openCreate} className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium">+ Nuevo</button>
      </div>
      <DataTable columns={columns} data={payments} loading={loading} />
      {totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-4">
          <button disabled={page <= 1} onClick={() => setPage((p) => p - 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Anterior</button>
          <span className="px-3 py-1 text-sm text-gray-600">{page} / {totalPages}</span>
          <button disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Siguiente</button>
        </div>
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title="Nuevo Pago">
        <div className="space-y-4">
          <div>
            <label className={labelCls}>Factura</label>
            <select className={selectCls} value={form.invoiceId} onChange={(e) => set('invoiceId', Number(e.target.value))}>
              <option value={0}>Seleccionar...</option>
              {invoices.filter((i) => i.balanceDue > 0 && i.status !== 'cancelled').map((i) => (
                <option key={i.id} value={i.id}>{i.invoiceNumber} - Saldo: ${i.balanceDue.toLocaleString()}</option>
              ))}
            </select>
          </div>
          {selectedInvoice && (
            <div className="bg-gray-50 rounded-lg p-3 text-sm space-y-1">
              <div className="flex justify-between"><span className="text-gray-500">Total:</span><span>${selectedInvoice.total.toLocaleString()}</span></div>
              <div className="flex justify-between"><span className="text-gray-500">Pagado:</span><span>${selectedInvoice.amountPaid.toLocaleString()}</span></div>
              <div className="flex justify-between font-medium"><span className="text-gray-700">Saldo:</span><span className="text-red-600">${selectedInvoice.balanceDue.toLocaleString()}</span></div>
            </div>
          )}
          <div><label className={labelCls}>Monto</label><input type="number" step="0.01" min="0.01" className={inputCls} value={form.amount || ''} onChange={(e) => set('amount', Number(e.target.value))} /></div>
          <div>
            <label className={labelCls}>Método de Pago</label>
            <select className={selectCls} value={form.paymentMethod} onChange={(e) => set('paymentMethod', e.target.value)}>
              <option value="cash">Efectivo</option><option value="card">Tarjeta</option>
              <option value="transfer">Transferencia</option><option value="check">Cheque</option>
              <option value="other">Otro</option>
            </select>
          </div>
          <div><label className={labelCls}>Referencia</label><input className={inputCls} value={form.reference} onChange={(e) => set('reference', e.target.value)} placeholder="Ej: TRF-123456" /></div>
          <div><label className={labelCls}>Notas</label><textarea className={inputCls} rows={2} value={form.notes} onChange={(e) => set('notes', e.target.value)} /></div>
          {error && <p className="text-red-600 text-sm">{error}</p>}
          <div className="flex justify-end gap-3 pt-2">
            <button onClick={() => setModalOpen(false)} className="px-4 py-2 border rounded-lg text-sm">Cancelar</button>
            <button onClick={handleSave} disabled={saving} className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm disabled:opacity-50">{saving ? 'Guardando...' : 'Registrar Pago'}</button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
