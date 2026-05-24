# Frontend API Examples — Purchase Receipt

Dưới đây là ví dụ ngắn để frontend gọi API `PurchaseReceipt` (POST / PUT / GET).

Base URL: `/api/PurchaseReceipt`

1) Create (POST)

Request JSON (send at minimum `supplierId` và `items`):

```json
{
  "purchaseId": 0,
  "supplierId": 101,
  "warehouseId": 1,
  "supplierName": "Công ty TNHH ABC",
  "receiptDate": "2026-05-19T15:30:00.000Z",
  "referenceCode": "HD-12345",
  "note": "Nhập hàng đợt 1 tháng 5",
  "items": [
    { "productId": 12, "quantity": 20, "unitCost": 500000 }
  ]
}
```

Fetch example (using fetch API):

```js
const body = { /* JSON above */ };
const res = await fetch('/api/PurchaseReceipt', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
  body: JSON.stringify(body)
});
if (res.status === 201) {
  const data = await res.json();
  // data.purchaseId, data.receiptCode, data.totalCost, data.items[]
}
```

2) Update (PUT)

URL: `/api/PurchaseReceipt/{id}`

Body: same shape as POST; server will replace all items for that purchase and recalc totals.

```js
const body = { /* same JSON but items updated */ };
await fetch(`/api/PurchaseReceipt/${id}`, {
  method: 'PUT',
  headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
  body: JSON.stringify(body)
});
// expect 204 No Content on success
```

3) Get (single)

GET `/api/PurchaseReceipt/{id}` returns full object:

```json
{
  "purchaseId": 123,
  "supplierId": 101,
  "warehouseId": 1,
  "supplierName": "Công ty TNHH ABC",
  "receiptDate": "2026-05-19T15:30:00.000Z",
  "receiptCode": "PO-20260519-001",
  "referenceCode": "HD-12345",
  "note": "Nhập hàng đợt 1 tháng 5",
  "totalCost": 15000000,
  "createDate": "2026-05-19T15:37:03.436Z",
  "lastModifiedDate": "2026-05-19T15:37:03.436Z",
  "status": 1,
  "isCanceled": false,
  "cancelReason": null,
  "canceledDate": null,
  "canceledBy": null,
  "items": [
    { "purchaseId": 123, "productId": 12, "quantity": 20, "unitCost": 500000, "totalPrice": 10000000 }
  ]
}
```

4) List (GET)

GET `/api/PurchaseReceipt` returns an array of the objects above.

Notes for frontend:
- Do not rely on `receiptCode` or `totalCost` from client — server will generate and compute.
- If product variants matter, prefer sending `productVariantId` (server currently maps by `productId` → first active variant).
- Handle `400` responses for validation errors and `500` for server errors.

