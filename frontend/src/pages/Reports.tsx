import { useState, useEffect } from 'react';
import api from '../services/api';
import type { SalesReport, InventoryReport } from '../types';

export default function Reports() {
  const [activeTab, setActiveTab] = useState<'sales' | 'inventory'>('sales');
  const [salesReport, setSalesReport] = useState<SalesReport | null>(null);
  const [inventoryReport, setInventoryReport] = useState<InventoryReport | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    if (activeTab === 'sales') {
      const today = new Date();
      const startDate = new Date(today.getFullYear(), today.getMonth(), 1).toISOString().split('T')[0];
      const endDate = today.toISOString().split('T')[0];
      api.get(`/Reports/sales?startDate=${startDate}&endDate=${endDate}&groupBy=day`)
        .then((res) => setSalesReport(res.data.data))
        .finally(() => setLoading(false));
    } else {
      api.get('/Reports/inventory')
        .then((res) => setInventoryReport(res.data.data))
        .finally(() => setLoading(false));
    }
  }, [activeTab]);

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800 mb-6">Reportes</h1>
      <div className="flex gap-2 mb-6">
        <button
          onClick={() => setActiveTab('sales')}
          className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${activeTab === 'sales' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-600 hover:bg-gray-300'}`}
        >
          Ventas
        </button>
        <button
          onClick={() => setActiveTab('inventory')}
          className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${activeTab === 'inventory' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-600 hover:bg-gray-300'}`}
        >
          Inventario
        </button>
      </div>

      {loading && <div className="text-center py-8 text-gray-500">Cargando...</div>}

      {activeTab === 'sales' && salesReport && !loading && (
        <div className="space-y-6">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200">
              <p className="text-sm text-gray-500">Total Ventas</p>
              <p className="text-xl font-bold text-gray-800">${salesReport.totalSales.toLocaleString()}</p>
            </div>
            <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200">
              <p className="text-sm text-gray-500">Transacciones</p>
              <p className="text-xl font-bold text-gray-800">{salesReport.totalTransactions}</p>
            </div>
            <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200">
              <p className="text-sm text-gray-500">Venta Promedio</p>
              <p className="text-xl font-bold text-gray-800">${salesReport.averageSale.toLocaleString()}</p>
            </div>
            <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200">
              <p className="text-sm text-gray-500">Impuestos</p>
              <p className="text-xl font-bold text-gray-800">${salesReport.totalTax.toLocaleString()}</p>
            </div>
          </div>
          {salesReport.topClients.length > 0 && (
            <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-200">
              <h3 className="font-semibold text-gray-800 mb-3">Top Clientes</h3>
              {salesReport.topClients.map((c) => (
                <div key={c.clientId} className="flex justify-between py-2 border-b border-gray-100 last:border-0">
                  <span className="text-sm text-gray-700">{c.clientName}</span>
                  <span className="text-sm text-gray-600">{c.purchaseCount} compras — ${c.totalSpent.toLocaleString()}</span>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {activeTab === 'inventory' && inventoryReport && !loading && (
        <div className="space-y-6">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200">
              <p className="text-sm text-gray-500">Productos</p>
              <p className="text-xl font-bold text-gray-800">{inventoryReport.totalProducts}</p>
            </div>
            <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200">
              <p className="text-sm text-gray-500">Valor Inventario</p>
              <p className="text-xl font-bold text-gray-800">${inventoryReport.totalInventoryValue.toLocaleString()}</p>
            </div>
            <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200">
              <p className="text-sm text-gray-500">Stock Bajo</p>
              <p className="text-xl font-bold text-yellow-600">{inventoryReport.lowStockProducts}</p>
            </div>
          </div>
          {inventoryReport.byCategory.length > 0 && (
            <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-200">
              <h3 className="font-semibold text-gray-800 mb-3">Por Categoría</h3>
              {inventoryReport.byCategory.map((cat) => (
                <div key={cat.categoryId} className="flex justify-between py-2 border-b border-gray-100 last:border-0">
                  <span className="text-sm text-gray-700">{cat.categoryName}</span>
                  <span className="text-sm text-gray-600">{cat.productCount} productos — ${cat.totalValue.toLocaleString()}</span>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
