# NexusERP - Diseño de Base de Datos Relacional

## Convenciones de Nomenclatura

- **Tablas**: snake_case en plural (ej: `users`, `product_suppliers`)
- **Columnas**: snake_case (ej: `created_at`, `first_name`)
- **Primary Keys**: `id` ( SERIAL / BIGSERIAL )
- **Foreign Keys**: `{tabla_referenciada}_id` (ej: `user_id`, `product_id`)
- **Índices**: `ix_{tabla}_{columna}`
- **Unique Constraints**: `uq_{tabla}_{columna}`
- **Check Constraints**: `ck_{tabla}_{condicion}`

---

## Módulo 1: Security

### Tabla: users
```sql
CREATE TABLE users (
    id              SERIAL PRIMARY KEY,
    username        VARCHAR(50) NOT NULL,
    email           VARCHAR(255) NOT NULL,
    password_hash   VARCHAR(255) NOT NULL,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    email_verified  BOOLEAN NOT NULL DEFAULT FALSE,
    last_login_at   TIMESTAMP WITH TIME ZONE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT uq_users_username UNIQUE (username),
    CONSTRAINT uq_users_email UNIQUE (email)
);

CREATE INDEX ix_users_username ON users(username);
CREATE INDEX ix_users_email ON users(email);
CREATE INDEX ix_users_is_active ON users(is_active);
```

### Tabla: roles
```sql
CREATE TABLE roles (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(50) NOT NULL,
    description     VARCHAR(255),
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_roles_name UNIQUE (name)
);
```

### Tabla: user_roles
```sql
CREATE TABLE user_roles (
    id              SERIAL PRIMARY KEY,
    user_id         INTEGER NOT NULL,
    role_id         INTEGER NOT NULL,
    assigned_at     TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    assigned_by     INTEGER,

    CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id)
        REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id)
        REFERENCES roles(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_assigned_by FOREIGN KEY (assigned_by)
        REFERENCES users(id) ON DELETE SET NULL,
    CONSTRAINT uq_user_roles UNIQUE (user_id, role_id)
);
```

### Tabla: refresh_tokens
```sql
CREATE TABLE refresh_tokens (
    id              SERIAL PRIMARY KEY,
    user_id         INTEGER NOT NULL,
    token           VARCHAR(500) NOT NULL,
    expires_at      TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    revoked_at      TIMESTAMP WITH TIME ZONE,
    replaced_by     INTEGER,
    is_revoked      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id)
        REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT uq_refresh_tokens_token UNIQUE (token)
);

CREATE INDEX ix_refresh_tokens_user_id ON refresh_tokens(user_id);
CREATE INDEX ix_refresh_tokens_expires_at ON refresh_tokens(expires_at);
CREATE INDEX ix_refresh_tokens_is_revoked ON refresh_tokens(is_revoked);
```

### Tabla: audit_logs
```sql
CREATE TABLE audit_logs (
    id              BIGSERIAL PRIMARY KEY,
    user_id         INTEGER,
    action          VARCHAR(50) NOT NULL,
    entity_name     VARCHAR(100) NOT NULL,
    entity_id       INTEGER,
    old_values      JSONB,
    new_values      JSONB,
    ip_address      VARCHAR(45),
    user_agent      VARCHAR(500),
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_audit_logs_user FOREIGN KEY (user_id)
        REFERENCES users(id) ON DELETE SET NULL
);

CREATE INDEX ix_audit_logs_user_id ON audit_logs(user_id);
CREATE INDEX ix_audit_logs_entity_name ON audit_logs(entity_name);
CREATE INDEX ix_audit_logs_entity_id ON audit_logs(entity_id);
CREATE INDEX ix_audit_logs_action ON audit_logs(action);
CREATE INDEX ix_audit_logs_created_at ON audit_logs(created_at);
```

### Tabla: system_configurations
```sql
CREATE TABLE system_configurations (
    id              SERIAL PRIMARY KEY,
    module          VARCHAR(50) NOT NULL,
    key             VARCHAR(100) NOT NULL,
    value           TEXT,
    description     VARCHAR(255),
    data_type       VARCHAR(20) NOT NULL DEFAULT 'string',
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_system_configurations_module_key UNIQUE (module, key)
);

CREATE INDEX ix_system_configurations_module ON system_configurations(module);
```

---

## Módulo 2: People

### Tabla: persons
```sql
CREATE TABLE persons (
    id              SERIAL PRIMARY KEY,
    document_type   VARCHAR(20) NOT NULL,
    document_number VARCHAR(30) NOT NULL,
    first_name      VARCHAR(100) NOT NULL,
    middle_name     VARCHAR(100),
    last_name       VARCHAR(100) NOT NULL,
    second_last_name VARCHAR(100),
    email           VARCHAR(255),
    phone           VARCHAR(30),
    mobile          VARCHAR(30),
    date_of_birth   DATE,
    gender          VARCHAR(10),
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT uq_persons_document UNIQUE (document_type, document_number),
    CONSTRAINT ck_persons_document_type CHECK (document_type IN ('CC', 'CE', 'NIT', 'PASSPORT'))
);

CREATE INDEX ix_persons_document ON persons(document_type, document_number);
CREATE INDEX ix_persons_email ON persons(email);
```

### Tabla: departments
```sql
CREATE TABLE departments (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(100) NOT NULL,
    description     VARCHAR(500),
    parent_id       INTEGER,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_departments_parent FOREIGN KEY (parent_id)
        REFERENCES departments(id) ON DELETE SET NULL,
    CONSTRAINT uq_departments_name UNIQUE (name)
);
```

### Tabla: job_positions
```sql
CREATE TABLE job_positions (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(100) NOT NULL,
    description     VARCHAR(500),
    department_id   INTEGER NOT NULL,
    min_salary      DECIMAL(12,2),
    max_salary      DECIMAL(12,2),
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_job_positions_department FOREIGN KEY (department_id)
        REFERENCES departments(id) ON DELETE RESTRICT,
    CONSTRAINT uq_job_positions_name UNIQUE (name, department_id),
    CONSTRAINT ck_job_positions_salary CHECK (min_salary <= max_salary)
);

CREATE INDEX ix_job_positions_department_id ON job_positions(department_id);
```

### Tabla: employees
```sql
CREATE TABLE employees (
    id              SERIAL PRIMARY KEY,
    person_id       INTEGER NOT NULL,
    employee_code   VARCHAR(20) NOT NULL,
    job_position_id INTEGER NOT NULL,
    hire_date       DATE NOT NULL,
    termination_date DATE,
    employment_type VARCHAR(20) NOT NULL DEFAULT 'full_time',
    status          VARCHAR(20) NOT NULL DEFAULT 'active',
    salary          DECIMAL(12,2),
    user_id         INTEGER,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_employees_person FOREIGN KEY (person_id)
        REFERENCES persons(id) ON DELETE RESTRICT,
    CONSTRAINT fk_employees_job_position FOREIGN KEY (job_position_id)
        REFERENCES job_positions(id) ON DELETE RESTRICT,
    CONSTRAINT fk_employees_user FOREIGN KEY (user_id)
        REFERENCES users(id) ON DELETE SET NULL,
    CONSTRAINT uq_employees_employee_code UNIQUE (employee_code),
    CONSTRAINT uq_employees_person UNIQUE (person_id),
    CONSTRAINT uq_employees_user UNIQUE (user_id),
    CONSTRAINT ck_employees_employment_type CHECK (employment_type IN ('full_time', 'part_time', 'contract', 'intern')),
    CONSTRAINT ck_employees_status CHECK (status IN ('active', 'inactive', 'terminated'))
);

CREATE INDEX ix_employees_job_position_id ON employees(job_position_id);
CREATE INDEX ix_employees_status ON employees(status);
```

### Tabla: employee_contacts
```sql
CREATE TABLE employee_contacts (
    id              SERIAL PRIMARY KEY,
    employee_id     INTEGER NOT NULL,
    contact_type    VARCHAR(30) NOT NULL,
    contact_value   VARCHAR(255) NOT NULL,
    is_primary      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_employee_contacts_employee FOREIGN KEY (employee_id)
        REFERENCES employees(id) ON DELETE CASCADE,
    CONSTRAINT uq_employee_contacts UNIQUE (employee_id, contact_type, contact_value)
);

CREATE INDEX ix_employee_contacts_employee_id ON employee_contacts(employee_id);
```

### Tabla: clients
```sql
CREATE TABLE clients (
    id              SERIAL PRIMARY KEY,
    person_id       INTEGER NOT NULL,
    client_code     VARCHAR(20) NOT NULL,
    credit_limit    DECIMAL(12,2) DEFAULT 0,
    current_balance DECIMAL(12,2) DEFAULT 0,
    status          VARCHAR(20) NOT NULL DEFAULT 'active',
    notes           TEXT,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_clients_person FOREIGN KEY (person_id)
        REFERENCES persons(id) ON DELETE RESTRICT,
    CONSTRAINT uq_clients_person UNIQUE (person_id),
    CONSTRAINT uq_clients_client_code UNIQUE (client_code),
    CONSTRAINT ck_clients_status CHECK (status IN ('active', 'inactive', 'suspended')),
    CONSTRAINT ck_clients_credit_limit CHECK (credit_limit >= 0),
    CONSTRAINT ck_clients_current_balance CHECK (current_balance >= 0)
);

CREATE INDEX ix_clients_client_code ON clients(client_code);
CREATE INDEX ix_clients_status ON clients(status);
```

### Tabla: client_addresses
```sql
CREATE TABLE client_addresses (
    id              SERIAL PRIMARY KEY,
    client_id       INTEGER NOT NULL,
    address_type    VARCHAR(30) NOT NULL DEFAULT 'billing',
    street          VARCHAR(255) NOT NULL,
    city            VARCHAR(100) NOT NULL,
    state           VARCHAR(100),
    postal_code     VARCHAR(20),
    country         VARCHAR(100) NOT NULL DEFAULT 'México',
    is_primary      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_client_addresses_client FOREIGN KEY (client_id)
        REFERENCES clients(id) ON DELETE CASCADE,
    CONSTRAINT ck_client_addresses_type CHECK (address_type IN ('billing', 'shipping', 'both'))
);

CREATE INDEX ix_client_addresses_client_id ON client_addresses(client_id);
```

### Tabla: suppliers
```sql
CREATE TABLE suppliers (
    id              SERIAL PRIMARY KEY,
    person_id       INTEGER NOT NULL,
    supplier_code   VARCHAR(20) NOT NULL,
    company_name    VARCHAR(200),
    tax_id          VARCHAR(30),
    website         VARCHAR(255),
    payment_terms   VARCHAR(100),
    rating          DECIMAL(3,2),
    status          VARCHAR(20) NOT NULL DEFAULT 'active',
    notes           TEXT,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_suppliers_person FOREIGN KEY (person_id)
        REFERENCES persons(id) ON DELETE RESTRICT,
    CONSTRAINT uq_suppliers_person UNIQUE (person_id),
    CONSTRAINT uq_suppliers_supplier_code UNIQUE (supplier_code),
    CONSTRAINT uq_suppliers_tax_id UNIQUE (tax_id),
    CONSTRAINT ck_suppliers_status CHECK (status IN ('active', 'inactive', 'suspended')),
    CONSTRAINT ck_suppliers_rating CHECK (rating >= 0 AND rating <= 5)
);

CREATE INDEX ix_suppliers_supplier_code ON suppliers(supplier_code);
CREATE INDEX ix_suppliers_status ON suppliers(status);
```

### Tabla: supplier_contacts
```sql
CREATE TABLE supplier_contacts (
    id              SERIAL PRIMARY KEY,
    supplier_id     INTEGER NOT NULL,
    contact_type    VARCHAR(30) NOT NULL,
    contact_value   VARCHAR(255) NOT NULL,
    contact_name    VARCHAR(200),
    is_primary      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_supplier_contacts_supplier FOREIGN KEY (supplier_id)
        REFERENCES suppliers(id) ON DELETE CASCADE,
    CONSTRAINT uq_supplier_contacts UNIQUE (supplier_id, contact_type, contact_value)
);

CREATE INDEX ix_supplier_contacts_supplier_id ON supplier_contacts(supplier_id);
```

---

## Módulo 3: Inventory

### Tabla: categories
```sql
CREATE TABLE categories (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(100) NOT NULL,
    description     VARCHAR(500),
    parent_id       INTEGER,
    image_url       VARCHAR(500),
    sort_order      INTEGER DEFAULT 0,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_categories_parent FOREIGN KEY (parent_id)
        REFERENCES categories(id) ON DELETE SET NULL,
    CONSTRAINT uq_categories_name UNIQUE (name)
);

CREATE INDEX ix_categories_parent_id ON categories(parent_id);
```

### Tabla: products
```sql
CREATE TABLE products (
    id              SERIAL PRIMARY KEY,
    code            VARCHAR(50) NOT NULL,
    name            VARCHAR(200) NOT NULL,
    description     TEXT,
    category_id     INTEGER NOT NULL,
    unit_of_measure VARCHAR(20) NOT NULL DEFAULT 'unit',
    cost_price      DECIMAL(12,2) NOT NULL DEFAULT 0,
    selling_price   DECIMAL(12,2) NOT NULL DEFAULT 0,
    tax_rate        DECIMAL(5,2) NOT NULL DEFAULT 0,
    min_stock       INTEGER NOT NULL DEFAULT 0,
    max_stock       INTEGER,
    image_url       VARCHAR(500),
    barcode         VARCHAR(100),
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    is_taxable      BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_products_category FOREIGN KEY (category_id)
        REFERENCES categories(id) ON DELETE RESTRICT,
    CONSTRAINT uq_products_code UNIQUE (code),
    CONSTRAINT uq_products_barcode UNIQUE (barcode),
    CONSTRAINT ck_products_cost_price CHECK (cost_price >= 0),
    CONSTRAINT ck_products_selling_price CHECK (selling_price >= 0),
    CONSTRAINT ck_products_tax_rate CHECK (tax_rate >= 0 AND tax_rate <= 100),
    CONSTRAINT ck_products_min_stock CHECK (min_stock >= 0),
    CONSTRAINT ck_products_max_stock CHECK (max_stock IS NULL OR max_stock >= min_stock),
    CONSTRAINT ck_products_unit_of_measure CHECK (unit_of_measure IN ('unit', 'kg', 'liter', 'meter', 'box', 'pack'))
);

CREATE INDEX ix_products_code ON products(code);
CREATE INDEX ix_products_category_id ON products(category_id);
CREATE INDEX ix_products_barcode ON products(barcode);
CREATE INDEX ix_products_is_active ON products(is_active);
```

### Tabla: warehouses
```sql
CREATE TABLE warehouses (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(100) NOT NULL,
    code            VARCHAR(20) NOT NULL,
    address         VARCHAR(500),
    manager_id      INTEGER,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    is_default      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_warehouses_manager FOREIGN KEY (manager_id)
        REFERENCES employees(id) ON DELETE SET NULL,
    CONSTRAINT uq_warehouses_name UNIQUE (name),
    CONSTRAINT uq_warehouses_code UNIQUE (code)
);

CREATE INDEX ix_warehouses_code ON warehouses(code);
```

### Tabla: product_suppliers
```sql
CREATE TABLE product_suppliers (
    id              SERIAL PRIMARY KEY,
    product_id      INTEGER NOT NULL,
    supplier_id     INTEGER NOT NULL,
    supplier_price  DECIMAL(12,2) NOT NULL,
    lead_time_days  INTEGER DEFAULT 0,
    min_order_qty   INTEGER DEFAULT 1,
    is_preferred    BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_product_suppliers_product FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE CASCADE,
    CONSTRAINT fk_product_suppliers_supplier FOREIGN KEY (supplier_id)
        REFERENCES suppliers(id) ON DELETE CASCADE,
    CONSTRAINT uq_product_suppliers UNIQUE (product_id, supplier_id),
    CONSTRAINT ck_product_suppliers_price CHECK (supplier_price >= 0),
    CONSTRAINT ck_product_suppliers_lead_time CHECK (lead_time_days >= 0),
    CONSTRAINT ck_product_suppliers_min_order CHECK (min_order_qty > 0)
);

CREATE INDEX ix_product_suppliers_product_id ON product_suppliers(product_id);
CREATE INDEX ix_product_suppliers_supplier_id ON product_suppliers(supplier_id);
```

### Tabla: inventory
```sql
CREATE TABLE inventory (
    id              SERIAL PRIMARY KEY,
    product_id      INTEGER NOT NULL,
    warehouse_id    INTEGER NOT NULL,
    quantity        INTEGER NOT NULL DEFAULT 0,
    reserved_qty    INTEGER NOT NULL DEFAULT 0,
    last_count_date DATE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_inventory_product FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE RESTRICT,
    CONSTRAINT fk_inventory_warehouse FOREIGN KEY (warehouse_id)
        REFERENCES warehouses(id) ON DELETE RESTRICT,
    CONSTRAINT uq_inventory_product_warehouse UNIQUE (product_id, warehouse_id),
    CONSTRAINT ck_inventory_quantity CHECK (quantity >= 0),
    CONSTRAINT ck_inventory_reserved_qty CHECK (reserved_qty >= 0),
    CONSTRAINT ck_inventory_reserved_lte_quantity CHECK (reserved_qty <= quantity)
);

CREATE INDEX ix_inventory_product_id ON inventory(product_id);
CREATE INDEX ix_inventory_warehouse_id ON inventory(warehouse_id);
```

### Tabla: inventory_movements
```sql
CREATE TABLE inventory_movements (
    id              BIGSERIAL PRIMARY KEY,
    product_id      INTEGER NOT NULL,
    warehouse_id    INTEGER NOT NULL,
    movement_type   VARCHAR(20) NOT NULL,
    quantity        INTEGER NOT NULL,
    unit_cost       DECIMAL(12,2),
    reference_type  VARCHAR(50),
    reference_id    INTEGER,
    notes           TEXT,
    performed_by    INTEGER NOT NULL,
    movement_date   TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_inventory_movements_product FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE RESTRICT,
    CONSTRAINT fk_inventory_movements_warehouse FOREIGN KEY (warehouse_id)
        REFERENCES warehouses(id) ON DELETE RESTRICT,
    CONSTRAINT fk_inventory_movements_performed_by FOREIGN KEY (performed_by)
        REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT ck_inventory_movements_type CHECK (movement_type IN ('entry', 'exit', 'adjustment', 'transfer_in', 'transfer_out')),
    CONSTRAINT ck_inventory_movements_quantity CHECK (quantity > 0)
);

CREATE INDEX ix_inventory_movements_product_id ON inventory_movements(product_id);
CREATE INDEX ix_inventory_movements_warehouse_id ON inventory_movements(warehouse_id);
CREATE INDEX ix_inventory_movements_movement_type ON inventory_movements(movement_type);
CREATE INDEX ix_inventory_movements_movement_date ON inventory_movements(movement_date);
CREATE INDEX ix_inventory_movements_reference ON inventory_movements(reference_type, reference_id);
```

---

## Módulo 4: Purchases

### Tabla: purchase_orders
```sql
CREATE TABLE purchase_orders (
    id              SERIAL PRIMARY KEY,
    order_number    VARCHAR(30) NOT NULL,
    supplier_id     INTEGER NOT NULL,
    order_date      DATE NOT NULL,
    expected_date   DATE,
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',
    subtotal        DECIMAL(12,2) NOT NULL DEFAULT 0,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL DEFAULT 0,
    notes           TEXT,
    approved_by     INTEGER,
    approved_at     TIMESTAMP WITH TIME ZONE,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER NOT NULL,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_purchase_orders_supplier FOREIGN KEY (supplier_id)
        REFERENCES suppliers(id) ON DELETE RESTRICT,
    CONSTRAINT fk_purchase_orders_approved_by FOREIGN KEY (approved_by)
        REFERENCES users(id) ON DELETE SET NULL,
    CONSTRAINT fk_purchase_orders_created_by FOREIGN KEY (created_by)
        REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT uq_purchase_orders_order_number UNIQUE (order_number),
    CONSTRAINT ck_purchase_orders_status CHECK (status IN ('pending', 'approved', 'received', 'cancelled')),
    CONSTRAINT ck_purchase_orders_subtotal CHECK (subtotal >= 0),
    CONSTRAINT ck_purchase_orders_tax CHECK (tax_amount >= 0),
    CONSTRAINT ck_purchase_orders_total CHECK (total >= 0)
);

CREATE INDEX ix_purchase_orders_supplier_id ON purchase_orders(supplier_id);
CREATE INDEX ix_purchase_orders_status ON purchase_orders(status);
CREATE INDEX ix_purchase_orders_order_date ON purchase_orders(order_date);
```

### Tabla: purchase_order_details
```sql
CREATE TABLE purchase_order_details (
    id              SERIAL PRIMARY KEY,
    purchase_order_id INTEGER NOT NULL,
    product_id      INTEGER NOT NULL,
    quantity        INTEGER NOT NULL,
    unit_price      DECIMAL(12,2) NOT NULL,
    tax_rate        DECIMAL(5,2) NOT NULL DEFAULT 0,
    subtotal        DECIMAL(12,2) NOT NULL,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL,
    received_qty    INTEGER NOT NULL DEFAULT 0,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_purchase_order_details_order FOREIGN KEY (purchase_order_id)
        REFERENCES purchase_orders(id) ON DELETE CASCADE,
    CONSTRAINT fk_purchase_order_details_product FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE RESTRICT,
    CONSTRAINT ck_purchase_order_details_qty CHECK (quantity > 0),
    CONSTRAINT ck_purchase_order_details_price CHECK (unit_price >= 0),
    CONSTRAINT ck_purchase_order_details_received CHECK (received_qty >= 0),
    CONSTRAINT ck_purchase_order_details_received_lte CHECK (received_qty <= quantity)
);

CREATE INDEX ix_purchase_order_details_order_id ON purchase_order_details(purchase_order_id);
CREATE INDEX ix_purchase_order_details_product_id ON purchase_order_details(product_id);
```

### Tabla: purchases
```sql
CREATE TABLE purchases (
    id              SERIAL PRIMARY KEY,
    purchase_number VARCHAR(30) NOT NULL,
    supplier_id     INTEGER NOT NULL,
    purchase_order_id INTEGER,
    purchase_date   DATE NOT NULL,
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',
    subtotal        DECIMAL(12,2) NOT NULL DEFAULT 0,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL DEFAULT 0,
    payment_status  VARCHAR(20) NOT NULL DEFAULT 'pending',
    notes           TEXT,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER NOT NULL,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_purchases_supplier FOREIGN KEY (supplier_id)
        REFERENCES suppliers(id) ON DELETE RESTRICT,
    CONSTRAINT fk_purchases_order FOREIGN KEY (purchase_order_id)
        REFERENCES purchase_orders(id) ON DELETE SET NULL,
    CONSTRAINT fk_purchases_created_by FOREIGN KEY (created_by)
        REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT uq_purchases_purchase_number UNIQUE (purchase_number),
    CONSTRAINT ck_purchases_status CHECK (status IN ('pending', 'confirmed', 'cancelled')),
    CONSTRAINT ck_purchases_payment_status CHECK (payment_status IN ('pending', 'partial', 'paid')),
    CONSTRAINT ck_purchases_subtotal CHECK (subtotal >= 0),
    CONSTRAINT ck_purchases_total CHECK (total >= 0)
);

CREATE INDEX ix_purchases_supplier_id ON purchases(supplier_id);
CREATE INDEX ix_purchases_status ON purchases(status);
CREATE INDEX ix_purchases_purchase_date ON purchases(purchase_date);
CREATE INDEX ix_purchases_purchase_order_id ON purchases(purchase_order_id);
```

### Tabla: purchase_details
```sql
CREATE TABLE purchase_details (
    id              SERIAL PRIMARY KEY,
    purchase_id     INTEGER NOT NULL,
    product_id      INTEGER NOT NULL,
    quantity        INTEGER NOT NULL,
    unit_price      DECIMAL(12,2) NOT NULL,
    tax_rate        DECIMAL(5,2) NOT NULL DEFAULT 0,
    subtotal        DECIMAL(12,2) NOT NULL,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_purchase_details_purchase FOREIGN KEY (purchase_id)
        REFERENCES purchases(id) ON DELETE CASCADE,
    CONSTRAINT fk_purchase_details_product FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE RESTRICT,
    CONSTRAINT ck_purchase_details_qty CHECK (quantity > 0),
    CONSTRAINT ck_purchase_details_price CHECK (unit_price >= 0)
);

CREATE INDEX ix_purchase_details_purchase_id ON purchase_details(purchase_id);
CREATE INDEX ix_purchase_details_product_id ON purchase_details(product_id);
```

---

## Módulo 5: Sales

### Tabla: orders
```sql
CREATE TABLE orders (
    id              SERIAL PRIMARY KEY,
    order_number    VARCHAR(30) NOT NULL,
    order_type      VARCHAR(10) NOT NULL,
    client_id       INTEGER,
    supplier_id     INTEGER,
    order_date      DATE NOT NULL,
    expected_date   DATE,
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',
    subtotal        DECIMAL(12,2) NOT NULL DEFAULT 0,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL DEFAULT 0,
    notes           TEXT,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER NOT NULL,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_orders_client FOREIGN KEY (client_id)
        REFERENCES clients(id) ON DELETE SET NULL,
    CONSTRAINT fk_orders_supplier FOREIGN KEY (supplier_id)
        REFERENCES suppliers(id) ON DELETE SET NULL,
    CONSTRAINT fk_orders_created_by FOREIGN KEY (created_by)
        REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT uq_orders_order_number UNIQUE (order_number),
    CONSTRAINT ck_orders_type CHECK (order_type IN ('sale', 'purchase')),
    CONSTRAINT ck_orders_status CHECK (status IN ('pending', 'confirmed', 'processing', 'completed', 'cancelled')),
    CONSTRAINT ck_orders_subtotal CHECK (subtotal >= 0),
    CONSTRAINT ck_orders_total CHECK (total >= 0),
    CONSTRAINT ck_orders_client_or_supplier CHECK (
        (order_type = 'sale' AND client_id IS NOT NULL AND supplier_id IS NULL) OR
        (order_type = 'purchase' AND supplier_id IS NOT NULL AND client_id IS NULL)
    )
);

CREATE INDEX ix_orders_order_type ON orders(order_type);
CREATE INDEX ix_orders_client_id ON orders(client_id);
CREATE INDEX ix_orders_supplier_id ON orders(supplier_id);
CREATE INDEX ix_orders_status ON orders(status);
CREATE INDEX ix_orders_order_date ON orders(order_date);
```

### Tabla: order_details
```sql
CREATE TABLE order_details (
    id              SERIAL PRIMARY KEY,
    order_id        INTEGER NOT NULL,
    product_id      INTEGER NOT NULL,
    quantity        INTEGER NOT NULL,
    unit_price      DECIMAL(12,2) NOT NULL,
    tax_rate        DECIMAL(5,2) NOT NULL DEFAULT 0,
    subtotal        DECIMAL(12,2) NOT NULL,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL,
    delivered_qty   INTEGER NOT NULL DEFAULT 0,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_order_details_order FOREIGN KEY (order_id)
        REFERENCES orders(id) ON DELETE CASCADE,
    CONSTRAINT fk_order_details_product FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE RESTRICT,
    CONSTRAINT ck_order_details_qty CHECK (quantity > 0),
    CONSTRAINT ck_order_details_price CHECK (unit_price >= 0),
    CONSTRAINT ck_order_details_delivered CHECK (delivered_qty >= 0),
    CONSTRAINT ck_order_details_delivered_lte CHECK (delivered_qty <= quantity)
);

CREATE INDEX ix_order_details_order_id ON order_details(order_id);
CREATE INDEX ix_order_details_product_id ON order_details(product_id);
```

### Tabla: sales
```sql
CREATE TABLE sales (
    id              SERIAL PRIMARY KEY,
    sale_number     VARCHAR(30) NOT NULL,
    client_id       INTEGER NOT NULL,
    sale_date       DATE NOT NULL,
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',
    subtotal        DECIMAL(12,2) NOT NULL DEFAULT 0,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    discount_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL DEFAULT 0,
    payment_status  VARCHAR(20) NOT NULL DEFAULT 'pending',
    notes           TEXT,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER NOT NULL,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_sales_client FOREIGN KEY (client_id)
        REFERENCES clients(id) ON DELETE RESTRICT,
    CONSTRAINT fk_sales_created_by FOREIGN KEY (created_by)
        REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT uq_sales_sale_number UNIQUE (sale_number),
    CONSTRAINT ck_sales_status CHECK (status IN ('pending', 'confirmed', 'cancelled')),
    CONSTRAINT ck_sales_payment_status CHECK (payment_status IN ('pending', 'partial', 'paid')),
    CONSTRAINT ck_sales_subtotal CHECK (subtotal >= 0),
    CONSTRAINT ck_sales_tax CHECK (tax_amount >= 0),
    CONSTRAINT ck_sales_discount CHECK (discount_amount >= 0),
    CONSTRAINT ck_sales_total CHECK (total >= 0)
);

CREATE INDEX ix_sales_client_id ON sales(client_id);
CREATE INDEX ix_sales_status ON sales(status);
CREATE INDEX ix_sales_sale_date ON sales(sale_date);
CREATE INDEX ix_sales_payment_status ON sales(payment_status);
```

### Tabla: sale_details
```sql
CREATE TABLE sale_details (
    id              SERIAL PRIMARY KEY,
    sale_id         INTEGER NOT NULL,
    product_id      INTEGER NOT NULL,
    quantity        INTEGER NOT NULL,
    unit_price      DECIMAL(12,2) NOT NULL,
    cost_price      DECIMAL(12,2) NOT NULL,
    tax_rate        DECIMAL(5,2) NOT NULL DEFAULT 0,
    discount_rate   DECIMAL(5,2) NOT NULL DEFAULT 0,
    subtotal        DECIMAL(12,2) NOT NULL,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    discount_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL,
    profit          DECIMAL(12,2) NOT NULL DEFAULT 0,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_sale_details_sale FOREIGN KEY (sale_id)
        REFERENCES sales(id) ON DELETE CASCADE,
    CONSTRAINT fk_sale_details_product FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE RESTRICT,
    CONSTRAINT ck_sale_details_qty CHECK (quantity > 0),
    CONSTRAINT ck_sale_details_price CHECK (unit_price >= 0),
    CONSTRAINT ck_sale_details_cost CHECK (cost_price >= 0),
    CONSTRAINT ck_sale_details_tax CHECK (tax_rate >= 0 AND tax_rate <= 100),
    CONSTRAINT ck_sale_details_discount CHECK (discount_rate >= 0 AND discount_rate <= 100)
);

CREATE INDEX ix_sale_details_sale_id ON sale_details(sale_id);
CREATE INDEX ix_sale_details_product_id ON sale_details(product_id);
```

---

## Módulo 6: Billing

### Tabla: invoices
```sql
CREATE TABLE invoices (
    id              SERIAL PRIMARY KEY,
    invoice_number  VARCHAR(30) NOT NULL,
    invoice_type    VARCHAR(20) NOT NULL DEFAULT 'sale',
    sale_id         INTEGER,
    purchase_id     INTEGER,
    client_id       INTEGER,
    supplier_id     INTEGER,
    invoice_date    DATE NOT NULL,
    due_date        DATE,
    status          VARCHAR(20) NOT NULL DEFAULT 'issued',
    subtotal        DECIMAL(12,2) NOT NULL DEFAULT 0,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL DEFAULT 0,
    amount_paid     DECIMAL(12,2) NOT NULL DEFAULT 0,
    balance_due     DECIMAL(12,2) NOT NULL DEFAULT 0,
    notes           TEXT,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER NOT NULL,
    updated_by      INTEGER,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT fk_invoices_sale FOREIGN KEY (sale_id)
        REFERENCES sales(id) ON DELETE SET NULL,
    CONSTRAINT fk_invoices_purchase FOREIGN KEY (purchase_id)
        REFERENCES purchases(id) ON DELETE SET NULL,
    CONSTRAINT fk_invoices_client FOREIGN KEY (client_id)
        REFERENCES clients(id) ON DELETE SET NULL,
    CONSTRAINT fk_invoices_supplier FOREIGN KEY (supplier_id)
        REFERENCES suppliers(id) ON DELETE SET NULL,
    CONSTRAINT fk_invoices_created_by FOREIGN KEY (created_by)
        REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT uq_invoices_invoice_number UNIQUE (invoice_number),
    CONSTRAINT ck_invoices_type CHECK (invoice_type IN ('sale', 'purchase')),
    CONSTRAINT ck_invoices_status CHECK (status IN ('issued', 'paid', 'overdue', 'cancelled')),
    CONSTRAINT ck_invoices_subtotal CHECK (subtotal >= 0),
    CONSTRAINT ck_invoices_total CHECK (total >= 0),
    CONSTRAINT ck_invoices_amount_paid CHECK (amount_paid >= 0),
    CONSTRAINT ck_invoices_balance_due CHECK (balance_due >= 0),
    CONSTRAINT ck_invoices_balance_calc CHECK (balance_due = total - amount_paid)
);

CREATE INDEX ix_invoices_invoice_number ON invoices(invoice_number);
CREATE INDEX ix_invoices_sale_id ON invoices(sale_id);
CREATE INDEX ix_invoices_purchase_id ON invoices(purchase_id);
CREATE INDEX ix_invoices_client_id ON invoices(client_id);
CREATE INDEX ix_invoices_supplier_id ON invoices(supplier_id);
CREATE INDEX ix_invoices_status ON invoices(status);
CREATE INDEX ix_invoices_due_date ON invoices(due_date);
```

### Tabla: invoice_details
```sql
CREATE TABLE invoice_details (
    id              SERIAL PRIMARY KEY,
    invoice_id      INTEGER NOT NULL,
    product_id      INTEGER NOT NULL,
    description     VARCHAR(255),
    quantity        INTEGER NOT NULL,
    unit_price      DECIMAL(12,2) NOT NULL,
    tax_rate        DECIMAL(5,2) NOT NULL DEFAULT 0,
    subtotal        DECIMAL(12,2) NOT NULL,
    tax_amount      DECIMAL(12,2) NOT NULL DEFAULT 0,
    total           DECIMAL(12,2) NOT NULL,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_invoice_details_invoice FOREIGN KEY (invoice_id)
        REFERENCES invoices(id) ON DELETE CASCADE,
    CONSTRAINT fk_invoice_details_product FOREIGN KEY (product_id)
        REFERENCES products(id) ON DELETE RESTRICT,
    CONSTRAINT ck_invoice_details_qty CHECK (quantity > 0),
    CONSTRAINT ck_invoice_details_price CHECK (unit_price >= 0)
);

CREATE INDEX ix_invoice_details_invoice_id ON invoice_details(invoice_id);
CREATE INDEX ix_invoice_details_product_id ON invoice_details(product_id);
```

### Tabla: payments
```sql
CREATE TABLE payments (
    id              SERIAL PRIMARY KEY,
    payment_number  VARCHAR(30) NOT NULL,
    invoice_id      INTEGER NOT NULL,
    amount          DECIMAL(12,2) NOT NULL,
    payment_method  VARCHAR(30) NOT NULL,
    payment_date    DATE NOT NULL,
    reference       VARCHAR(100),
    notes           TEXT,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      INTEGER NOT NULL,

    CONSTRAINT fk_payments_invoice FOREIGN KEY (invoice_id)
        REFERENCES invoices(id) ON DELETE RESTRICT,
    CONSTRAINT fk_payments_created_by FOREIGN KEY (created_by)
        REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT uq_payments_payment_number UNIQUE (payment_number),
    CONSTRAINT ck_payments_amount CHECK (amount > 0),
    CONSTRAINT ck_payments_method CHECK (payment_method IN ('cash', 'card', 'transfer', 'check', 'other'))
);

CREATE INDEX ix_payments_invoice_id ON payments(invoice_id);
CREATE INDEX ix_payments_payment_date ON payments(payment_date);
CREATE INDEX ix_payments_payment_method ON payments(payment_method);
```

---

## Resumen de Tablas

| Módulo | Tablas | Registros Estimados |
|--------|--------|---------------------|
| Security | 5 | Users: 100, Roles: 10, Tokens: 500, Audit: 100K+ |
| People | 9 | Persons: 1K, Employees: 100, Clients: 500, Suppliers: 100 |
| Inventory | 5 | Products: 1K, Categories: 100, Movements: 10K+ |
| Purchases | 4 | Orders: 1K, Purchases: 2K, Details: 10K+ |
| Sales | 4 | Orders: 5K, Sales: 10K, Details: 50K+ |
| Billing | 3 | Invoices: 10K, Payments: 20K+ |
| **Total** | **30** | |

---

## Estrategia de IDs

- **SERIAL**: Para la mayoría de tablas (entidades con volumen moderado)
- **BIGSERIAL**: Para tablas de alto volumen (audit_logs, inventory_movements)
- **UUID**: No se usa inicialmente, pero se puede migrar si se necesita distribuir

## Estrategia de Soft Delete

- Todas las entidades principales tienen columna `is_deleted`
- Las eliminaciones son lógicas, nunca físicas
- Las consultas filtran por `is_deleted = FALSE` por defecto
- Las migraciones físicas se programan periódicamente
