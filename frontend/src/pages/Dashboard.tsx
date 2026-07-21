import { useEffect, useState } from 'react';
import api from '../services/api';
import type { DashboardData } from '../types';

export default function Dashboard() {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const today = new Date();
    const startDate = new Date(today.getFullYear(), 0, 1).toISOString().split('T')[0];
    const endDate = today.toISOString().split('T')[0];
    api.get(`/Dashboard?startDate=${startDate}&endDate=${endDate}`)
      .then((res) => setData(res.data.data))
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="text-center py-8 text-gray-500">Cargando...</div>;
  if (!data) return <div className="text-center py-8 text-gray-500">Error al cargar dashboard</div>;

  const metrics = [
    { label: 'Ventas Totales', value: `$${data.totalSales.toLocaleString()}`, color: 'bg-green-500', icon: '💰' },
    { label: 'Compras Totales', value: `$${data.totalPurchases.toLocaleString()}`, color: 'bg-blue-500', icon: '🛒' },
    { label: 'Ganancia Neta', value: `$${data.totalRevenue.toLocaleString()}`, color: data.totalRevenue >= 0 ? 'bg-emerald-500' : 'bg-red-500', icon: '📈' },
    { label: 'Clientes', value: data.totalClients.toString(), color: 'bg-purple-500', icon: '👥' },
    { label: 'Productos', value: data.totalProducts.toString(), color: 'bg-orange-500', icon: '📦' },
    { label: 'Stock Bajo', value: data.lowStockProducts.toString(), color: 'bg-yellow-500', icon: '⚠️' },
  ];

  const recentDays = data.dailyMetrics.filter(d => d.sales > 0 || d.purchases > 0).slice(-7);

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800 mb-6">Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
        {metrics.map((m) => (
          <div key={m.label} className="bg-white rounded-xl p-5 shadow-sm border border-gray-200">
            <div className="flex items-center gap-3">
              <span className="text-2xl">{m.icon}</span>
              <div>
                <p className="text-sm text-gray-500">{m.label}</p>
                <p className="text-2xl font-bold text-gray-800">{m.value}</p>
              </div>
            </div>
          </div>
        ))}
      </div>
      <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-200">
        <h2 className="text-lg font-semibold text-gray-800 mb-4">Actividad Reciente</h2>
        {recentDays.length === 0 ? (
          <p className="text-gray-400 text-sm">Sin actividad reciente</p>
        ) : (
          <div className="space-y-2">
            {recentDays.map((d) => (
              <div key={d.date} className="flex justify-between items-center py-2 border-b border-gray-100 last:border-0">
                <span className="text-sm text-gray-600">{new Date(d.date).toLocaleDateString('es-CO')}</span>
                <div className="flex gap-4 text-sm">
                  {d.sales > 0 && <span className="text-green-600">Ventas: ${d.sales.toLocaleString()}</span>}
                  {d.purchases > 0 && <span className="text-blue-600">Compras: ${d.purchases.toLocaleString()}</span>}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
