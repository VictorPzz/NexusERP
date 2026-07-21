export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string | null;
  errors: string[];
  traceId: string | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: {
    id: number;
    username: string;
    email: string;
    roles: string[];
  };
}

export interface DashboardData {
  totalSales: number;
  totalPurchases: number;
  totalRevenue: number;
  totalClients: number;
  totalProducts: number;
  lowStockProducts: number;
  pendingOrders: number;
  pendingPurchases: number;
  topProducts: { name: string; quantity: number; total: number }[];
  dailyMetrics: { date: string; sales: number; purchases: number }[];
}

export interface PersonInfo {
  id: number;
  documentType: string;
  documentNumber: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
  secondLastName: string | null;
  email: string | null;
  phone: string | null;
  mobile: string | null;
  dateOfBirth: string | null;
  gender: string | null;
}

export interface Client {
  id: number;
  clientCode: string;
  person: PersonInfo;
  creditLimit: number;
  currentBalance: number;
  status: string;
  notes: string | null;
}

export interface Supplier {
  id: number;
  supplierCode: string;
  person: PersonInfo;
  companyName: string | null;
  taxId: string | null;
  website: string | null;
  paymentTerms: string | null;
  rating: number | null;
  status: string;
  notes: string | null;
}

export interface Product {
  id: number;
  code: string;
  name: string;
  description: string | null;
  categoryId: number;
  categoryName: string | null;
  unitOfMeasure: string;
  costPrice: number;
  sellingPrice: number;
  taxRate: number;
  minStock: number;
  maxStock: number | null;
  imageUrl: string | null;
  barcode: string | null;
  isActive: boolean;
  isTaxable: boolean;
}

export interface Category {
  id: number;
  name: string;
  description: string | null;
  parentId: number | null;
  parentName: string | null;
  imageUrl: string | null;
  sortOrder: number;
  isActive: boolean;
  productCount: number;
}

export interface Warehouse {
  id: number;
  name: string;
  code: string;
  address: string | null;
  managerId: number | null;
  isActive: boolean;
  isDefault: boolean;
}

export interface Order {
  id: number;
  orderNumber: string;
  orderType: string;
  clientId: number | null;
  clientName: string | null;
  supplierId: number | null;
  supplierName: string | null;
  orderDate: string;
  expectedDate: string | null;
  status: string;
  subtotal: number;
  taxAmount: number;
  total: number;
  notes: string | null;
  itemCount: number;
}

export interface Sale {
  id: number;
  saleNumber: string;
  clientId: number;
  clientName: string;
  saleDate: string;
  status: string;
  paymentStatus: string;
  subtotal: number;
  taxAmount: number;
  discountAmount: number;
  total: number;
  notes: string | null;
  itemCount: number;
}

export interface Purchase {
  id: number;
  purchaseNumber: string;
  supplierId: number;
  supplierName: string;
  purchaseOrderId: number | null;
  purchaseDate: string;
  status: string;
  paymentStatus: string;
  subtotal: number;
  taxAmount: number;
  total: number;
  notes: string | null;
  itemCount: number;
}

export interface PurchaseOrder {
  id: number;
  orderNumber: string;
  supplierId: number;
  supplierName: string;
  orderDate: string;
  expectedDate: string | null;
  status: string;
  subtotal: number;
  taxAmount: number;
  total: number;
  notes: string | null;
  itemCount: number;
}

export interface Invoice {
  id: number;
  invoiceNumber: string;
  invoiceType: string;
  saleId: number | null;
  purchaseId: number | null;
  clientId: number | null;
  clientName: string | null;
  supplierId: number | null;
  supplierName: string | null;
  invoiceDate: string;
  dueDate: string | null;
  status: string;
  subtotal: number;
  taxAmount: number;
  total: number;
  amountPaid: number;
  balanceDue: number;
  notes: string | null;
  itemCount: number;
  paymentCount: number;
}

export interface Payment {
  id: number;
  paymentNumber: string;
  invoiceId: number;
  invoiceNumber: string | null;
  amount: number;
  paymentMethod: string;
  paymentDate: string;
  reference: string | null;
  notes: string | null;
  createdBy: number;
}

export interface SalesReport {
  totalSales: number;
  totalTransactions: number;
  averageSale: number;
  totalTax: number;
  groups: { period: string; transactionCount: number; total: number; tax: number }[];
  topClients: { clientId: number; clientName: string; purchaseCount: number; totalSpent: number }[];
}

export interface InventoryReport {
  totalProducts: number;
  activeProducts: number;
  lowStockProducts: number;
  totalInventoryValue: number;
  byCategory: { categoryId: number; categoryName: string; productCount: number; totalValue: number }[];
  lowStockItems: { productId: number; productName: string; productCode: string; minStock: number; costPrice: number }[];
}
