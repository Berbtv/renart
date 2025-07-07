# Renart API Dokümantasyonu

Bu dokümantasyon, Renart projesi için geliştirilen RESTful API endpoint'lerini açıklamaktadır.

## Base URL
```
https://localhost:5001/api
```

## Genel Bilgiler

### Response Format
Tüm API yanıtları aşağıdaki format kullanır:

```json
{
  "success": true,
  "message": "İşlem başarılı",
  "data": {},
  "timestamp": "2024-01-01T00:00:00.000Z"
}
```

### HTTP Status Kodları
- `200 OK`: İşlem başarılı
- `400 Bad Request`: Geçersiz istek
- `404 Not Found`: Kaynak bulunamadı
- `500 Internal Server Error`: Sunucu hatası

## Endpoint'ler

### Ürünler API

#### 1. Tüm Ürünleri Getir
```
GET /api/products
```

**Response:**
```json
{
  "success": true,
  "message": "Ürünler başarıyla getirildi",
  "data": [
    {
      "name": "Ürün Adı",
      "popularityScore": 85.5,
      "weight": 2.5,
      "images": {
        "yellow": "yellow-image-url",
        "rose": "rose-image-url",
        "white": "white-image-url"
      }
    }
  ],
  "timestamp": "2024-01-01T00:00:00.000Z"
}
```

#### 2. Belirli Ürünü Getir
```
GET /api/products/{name}
```

**Parameters:**
- `name` (string): Ürün adı

**Response:**
```json
{
  "success": true,
  "message": "Ürün başarıyla getirildi",
  "data": {
    "name": "Ürün Adı",
    "popularityScore": 85.5,
    "weight": 2.5,
    "images": {
      "yellow": "yellow-image-url",
      "rose": "rose-image-url",
      "white": "white-image-url"
    }
  },
  "timestamp": "2024-01-01T00:00:00.000Z"
}
```

#### 3. Ürün Arama
```
GET /api/products/search?query={query}
```

**Parameters:**
- `query` (string): Arama terimi

**Response:**
```json
{
  "success": true,
  "message": "'arama' için 3 ürün bulundu",
  "data": [
    {
      "name": "Arama Sonucu Ürün",
      "popularityScore": 85.5,
      "weight": 2.5,
      "images": {
        "yellow": "yellow-image-url",
        "rose": "rose-image-url",
        "white": "white-image-url"
      }
    }
  ],
  "timestamp": "2024-01-01T00:00:00.000Z"
}
```



## Test Araçları

### Swagger UI
Development ortamında Swagger UI'a erişmek için:
```
https://localhost:5001/api-docs
```

### API Test Sayfası
Web arayüzünden API'leri test etmek için:
```
https://localhost:5001/Home/ApiTest
```

## Örnek Kullanım

### JavaScript ile API Kullanımı

```javascript
// Tüm ürünleri getir
const response = await fetch('/api/products');
const data = await response.json();
console.log(data.data);

// Sepete ürün ekle
const addResponse = await fetch('/api/cart/add', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    },
    body: JSON.stringify({
        productName: 'Yüzük',
        metalType: 'yellow'
    })
});
const addData = await addResponse.json();
console.log(addData.message);
```

### cURL ile API Kullanımı

```bash
# Tüm ürünleri getir
curl -X GET "https://localhost:5001/api/products"

# Belirli ürünü getir
curl -X GET "https://localhost:5001/api/products/Yüzük"

# Sepete ürün ekle
curl -X POST "https://localhost:5001/api/cart/add" \
  -H "Content-Type: application/json" \
  -d '{"productName":"Yüzük","metalType":"yellow"}'

# Sepeti getir
curl -X GET "https://localhost:5001/api/cart"
```

## Hata Yönetimi

API'de oluşabilecek hatalar ve çözümleri:

### 400 Bad Request
- Eksik veya geçersiz parametreler
- Boş arama sorgusu
- Geçersiz metal türü

### 404 Not Found
- Ürün bulunamadı
- Geçersiz endpoint

### 500 Internal Server Error
- Sunucu hatası
- Veritabanı bağlantı sorunu

## Güvenlik

- API CORS politikası ile korunmaktadır
- Session tabanlı sepet yönetimi
- Input validation ile güvenlik sağlanmaktadır 