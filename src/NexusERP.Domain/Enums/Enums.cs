namespace NexusERP.Domain.Enums;

public enum DocumentType
{
    CC,
    CE,
    NIT,
    PASSPORT
}

public enum Gender
{
    male,
    female,
    other
}

public enum EmploymentType
{
    full_time,
    part_time,
    contract,
    intern
}

public enum EmployeeStatus
{
    active,
    inactive,
    terminated
}

public enum ClientStatus
{
    active,
    inactive,
    suspended
}

public enum SupplierStatus
{
    active,
    inactive,
    suspended
}

public enum UnitOfMeasure
{
    unit,
    kg,
    liter,
    meter,
    box,
    pack
}

public enum MovementType
{
    entry,
    exit,
    adjustment,
    transfer_in,
    transfer_out
}

public enum OrderType
{
    sale,
    purchase
}

public enum OrderStatus
{
    pending,
    confirmed,
    processing,
    completed,
    cancelled
}

public enum PurchaseOrderStatus
{
    pending,
    approved,
    received,
    cancelled
}

public enum PurchaseStatus
{
    pending,
    confirmed,
    cancelled
}

public enum PaymentStatus
{
    pending,
    partial,
    paid
}

public enum SaleStatus
{
    pending,
    confirmed,
    cancelled
}

public enum InvoiceType
{
    sale,
    purchase
}

public enum InvoiceStatus
{
    issued,
    paid,
    overdue,
    cancelled
}

public enum PaymentMethod
{
    cash,
    card,
    transfer,
    check,
    other
}

public enum AddressType
{
    billing,
    shipping,
    both
}

public enum ContactType
{
    phone,
    email,
    mobile,
    fax,
    website
}
