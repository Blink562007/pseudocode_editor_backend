# Pseudocode Editor API Contract

## Base URL
- **Development**: `https://localhost:7065` or `http://localhost:5062`

## Endpoints

### 1. Get All Documents
**GET** `/api/pseudocode`

**Response**: `200 OK`
```json
[
  {
    "id": "string (guid)",
    "title": "string",
    "content": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime",
    "language": "string"
  }
]
```

---

### 2. Get Document by ID
**GET** `/api/pseudocode/{id}`

**Parameters**:
- `id` (path): Document ID

**Response**: `200 OK`
```json
{
  "id": "string (guid)",
  "title": "string",
  "content": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "language": "string"
}
```

**Error Response**: `404 Not Found`
```json
{
  "message": "Document not found"
}
```

---

### 3. Create Document
**POST** `/api/pseudocode`

**Request Body**:
```json
{
  "title": "string",
  "content": "string",
  "language": "string (default: 'pseudocode')"
}
```

**Response**: `201 Created`
```json
{
  "id": "string (guid)",
  "title": "string",
  "content": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "language": "string"
}
```

**Headers**:
- `Location`: `/api/pseudocode/{id}`

---

### 4. Update Document
**PUT** `/api/pseudocode/{id}`

**Parameters**:
- `id` (path): Document ID

**Request Body**:
```json
{
  "title": "string",
  "content": "string",
  "language": "string"
}
```

**Response**: `200 OK`
```json
{
  "id": "string (guid)",
  "title": "string",
  "content": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "language": "string"
}
```

**Error Response**: `404 Not Found`
```json
{
  "message": "Document not found"
}
```

---

### 5. Delete Document
**DELETE** `/api/pseudocode/{id}`

**Parameters**:
- `id` (path): Document ID

**Response**: `204 No Content`

**Error Response**: `404 Not Found`
```json
{
  "message": "Document not found"
}
```

---

## CORS Configuration
- **Allowed Origins**: All (`*`)
- **Allowed Methods**: All (GET, POST, PUT, DELETE, etc.)
- **Allowed Headers**: All

---

## Example Usage (JavaScript/TypeScript)

```typescript
// Base URL
const API_BASE_URL = 'https://localhost:7065/api/pseudocode';

// Get all documents
const getAllDocuments = async () => {
  const response = await fetch(API_BASE_URL);
  return await response.json();
};

// Get document by ID
const getDocument = async (id: string) => {
  const response = await fetch(`${API_BASE_URL}/${id}`);
  return await response.json();
};

// Create document
const createDocument = async (data: { title: string; content: string; language?: string }) => {
  const response = await fetch(API_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  return await response.json();
};

// Update document
const updateDocument = async (id: string, data: { title: string; content: string; language: string }) => {
  const response = await fetch(`${API_BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  return await response.json();
};

// Delete document
const deleteDocument = async (id: string) => {
  await fetch(`${API_BASE_URL}/${id}`, {
    method: 'DELETE'
  });
};
```

---

## Notes
- All datetime values are in UTC format (ISO 8601)
- Document IDs are automatically generated GUIDs
- The API uses in-memory storage (data will be lost on restart)
- Content-Type for requests should be `application/json`
