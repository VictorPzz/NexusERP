import { useEffect, useState } from 'react';
import api from '../services/api';
import DataTable from '../components/DataTable';
import Modal from '../components/Modal';
import type { Product, Category } from '../types';

const emptyForm = {
  code: '', name: '', description: '', categoryId: 0, unitOfMeasure: 'unit',
  costPrice: 0, sellingPrice: 0, taxRate: 19, minStock: 0, maxStock: '', barcode: '', isTaxable: true,
};

export default function Products() {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Product | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const load = () => {
    setLoading(true);
    api.get('/Products', { params: { page, pageSize: 10, search: search || undefined } })
      .then((res) => { setProducts(res.data.data.items || []); setTotalPages(res.data.data.totalPages); })
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [page, search]);
  useEffect(() => { api.get('/Categories').then((res) => setCategories(res.data.data || [])); }, []);

  const openCreate = () => { setEditing(null); setForm(emptyForm); setError(''); setModalOpen(true); };
  const openEdit = (p: Product) => {
    setEditing(p);
    setForm({
      code: p.code, name: p.name, description: p.description || '', categoryId: p.categoryId,
      unitOfMeasure: p.unitOfMeasure, costPrice: p.costPrice, sellingPrice: p.sellingPrice,
      taxRate: p.taxRate, minStock: p.minStock, maxStock: p.maxStock?.toString() || '',
      barcode: p.barcode || '', isTaxable: p.isTaxable,
    });
    setError(''); setModalOpen(true);
  };

  const handleSave = async () => {
    setSaving(true); setError('');
    try {
      const body = {
        ...form, maxStock: form.maxStock ? Number(form.maxStock) : null,
        barcode: form.barcode || null, description: form.description || null,
      };
      if (editing) {
        await api.put(`/Products/${editing.id}`, body);
      } else {
        await api.post('/Products', body);
      }
      setModalOpen(false); load();
    } catch (e: unknown) {
      const msg = (e as { response?: { data?: { message?: string } } }).response?.data?.message;
      setError(msg || 'Error al guardar');
    } finally { setSaving(false); }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('¿Eliminar este producto?')) return;
    try { await api.delete(`/Products/${id}`); load(); } catch { /* ignore */ }
  };

  const set = (key: string, value: string | number | boolean) => setForm((f) => ({ ...f, [key]: value }));
  const input = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500";
  const select = `${input} bg-white`;
  const label = "block text-sm font-medium text-gray-700 mb-1";

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'code', label: 'Código' },
    { key: 'name', label: 'Nombre' },
    { key: 'categoryName', label: 'Categoría' },
    { key: 'costPrice', label: 'Costo', render: (p: Product) => `$${p.costPrice.toLocaleString()}` },
    { key: 'sellingPrice', label: 'Venta', render: (p: Product) => `$${p.sellingPrice.toLocaleString()}` },
    { key: 'taxRate', label: 'IVA', render: (p: Product) => `${p.taxRate}%` },
    { key: 'minStock', label: 'Stock Mín.' },
    { key: 'actions', label: '', render: (p: Product) => (
      <div className="flex gap-2">
        <button onClick={() => openEdit(p)} className="text-blue-600 hover:text-blue-800 text-xs">Editar</button>
        <button onClick={() => handleDelete(p.id)} className="text-red-600 hover:text-red-800 text-xs">Eliminar</button>
      </div>
    )},
  ];

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Productos</h1>
        <div className="flex gap-3 items-center">
          <input value={search} onChange={(e) => { setSearch(e.target.value); setPage(1); }} placeholder="Buscar..." className={`${input} w-48`} />
          <button onClick={openCreate} className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium">+ Nuevo</button>
        </div>
      </div>
      <DataTable columns={columns} data={products} loading={loading} />
      {totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-4">
          <button disabled={page <= 1} onClick={() => setPage((p) => p - 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Anterior</button>
          <span className="px-3 py-1 text-sm text-gray-600">{page} / {totalPages}</span>
          <button disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)} className="px-3 py-1 rounded border text-sm disabled:opacity-50">Siguiente</button>
        </div>
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Editar Producto' : 'Nuevo Producto'} wide>
        <div className="space-y-4">
          {!editing && <div className="grid grid-cols-2 gap-4">
            <div><label className={label}>Código</label><input className={input} value={form.code} onChange={(e) => set('code', e.target.value)} /></div>
            <div><label className={label}>Barras</label><input className={input} value={form.barcode} onChange={(e) => set('barcode', e.target.value)} /></div>
          </div>}
          <div><label className={label}>Nombre</label><input className={input} value={form.name} onChange={(e) => set('name', e.target.value)} /></div>
          <div><label className={label}>Descripción</label><textarea className={input} rows={2} value={form.description} onChange={(e) => set('description', e.target.value)} /></div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className={label}>Categoría</label>
              <select className={select} value={form.categoryId} onChange={(e) => set('categoryId', Number(e.target.value))}>
                <option value={0}>Seleccionar...</option>
                {categories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <div>
              <label className={label}>Unidad</label>
              <select className={select} value={form.unitOfMeasure} onChange={(e) => set('unitOfMeasure', e.target.value)}>
                <option value="unit">Unidad</option><option value="kg">Kg</option><option value="liter">Litro</option>
                <option value="meter">Metro</option><option value="box">Caja</option><option value="pack">Paquete</option>
              </select>
            </div>
          </div>
          <div className="grid grid-cols-3 gap-4">
            <div><label className={label}>Precio Costo</label><input type="number" step="0.01" className={input} value={form.costPrice} onChange={(e) => set('costPrice', Number(e.target.value))} /></div>
            <div><label className={label}>Precio Venta</label><input type="number" step="0.01" className={input} value={form.sellingPrice} onChange={(e) => set('sellingPrice', Number(e.target.value))} /></div>
            <div><label className={label}>IVA %</label><input type="number" step="0.01" className={input} value={form.taxRate} onChange={(e) => set('taxRate', Number(e.target.value))} /></div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div><label className={label}>Stock Mínimo</label><input type="number" className={input} value={form.minStock} onChange={(e) => set('minStock', Number(e.target.value))} /></div>
            <div><label className={label}>Stock Máximo</label><input type="number" className={input} value={form.maxStock} onChange={(e) => set('maxStock', e.target.value)} /></div>
          </div>
          <div className="flex items-center gap-2">
            <input type="checkbox" checked={form.isTaxable} onChange={(e) => set('isTaxable', e.target.checked)} className="rounded" />
            <label className="text-sm text-gray-700">Gravado con IVA</label>
          </div>
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
