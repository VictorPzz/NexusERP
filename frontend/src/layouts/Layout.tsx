import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';

const menuItems = [
  { path: '/dashboard', label: 'Dashboard', icon: '📊' },
  { path: '/clients', label: 'Clientes', icon: '👥' },
  { path: '/suppliers', label: 'Proveedores', icon: '🏭' },
  { path: '/categories', label: 'Categorías', icon: '📁' },
  { path: '/warehouses', label: 'Bodegas', icon: '🏗️' },
  { path: '/products', label: 'Productos', icon: '📦' },
  { path: '/orders', label: 'Órdenes', icon: '📋' },
  { path: '/sales', label: 'Ventas', icon: '💰' },
  { path: '/purchase-orders', label: 'Órdenes de Compra', icon: '🛒' },
  { path: '/purchases', label: 'Compras', icon: '🛍️' },
  { path: '/invoices', label: 'Facturas', icon: '📄' },
  { path: '/payments', label: 'Pagos', icon: '💳' },
  { path: '/reports', label: 'Reportes', icon: '📈' },
];

export default function Layout() {
  const { user, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login');
    }
  }, [isAuthenticated, navigate]);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (!isAuthenticated) return null;

  return (
    <div className="flex h-screen bg-gray-100">
      <aside className="w-64 bg-gray-900 text-white flex flex-col">
        <div className="p-4 border-b border-gray-700">
          <h1 className="text-xl font-bold text-blue-400">NexusERP</h1>
          <p className="text-xs text-gray-400 mt-1">Sistema ERP para PyMEs</p>
        </div>
        <nav className="flex-1 overflow-y-auto py-2">
          {menuItems.map((item) => (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) =>
                `flex items-center gap-3 px-4 py-2.5 text-sm transition-colors ${
                  isActive
                    ? 'bg-blue-600 text-white'
                    : 'text-gray-300 hover:bg-gray-800 hover:text-white'
                }`
              }
            >
              <span>{item.icon}</span>
              <span>{item.label}</span>
            </NavLink>
          ))}
        </nav>
        <div className="p-4 border-t border-gray-700">
          <div className="text-sm text-gray-400 mb-2">{user?.username || user?.email}</div>
          <button
            onClick={handleLogout}
            className="w-full bg-red-600 hover:bg-red-700 text-white text-sm py-2 rounded transition-colors"
          >
            Cerrar Sesión
          </button>
        </div>
      </aside>
      <main className="flex-1 overflow-y-auto">
        <div className="p-6">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
