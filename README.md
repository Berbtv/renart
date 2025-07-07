# Projeyi Çalıştırma Adımları

## 1. Gerekli Programları Kur
- **.NET SDK** (7 veya 8 ve üzeri):  
  https://dotnet.microsoft.com/download
- **Node.js** (16 ve üzeri):  
  https://nodejs.org/

---

## 2. Projeyi Bilgisayarına İndir
Eğer GitHub'dan klonlayacaksan:
```sh
git clone https://github.com/kullaniciadiniz/renart.git
cd renart
```
Veya dosyaları doğrudan indirip bir klasöre çıkar.

---

## 3. Backend'i (API) Başlat

```sh
cd Proje
dotnet restore
dotnet run
```
- Backend, `http://localhost:5113` adresinde çalışacaktır.

---

## 4. Frontend'i (React) Başlat

Başka bir terminal aç:
```sh
cd frontend
npm install
npm start
```
- Frontend, `http://localhost:3000` adresinde açılır.
- Frontend, API isteklerini otomatik olarak backend'e yönlendirir (proxy ayarı sayesinde).

---

## 5. Her İkisini Aynı Anda Başlatmak (Opsiyonel)

Frontend klasöründe şu komutu kullanabilirsin:
```sh
npm run dev
```
Bu komut hem backend'i hem frontend'i aynı anda başlatır.  
(Bunun için `concurrently` paketi otomatik olarak kurulmuş olmalı.)

---

## 6. Tarayıcıda Aç

- Frontend otomatik açılmazsa, tarayıcıda `http://localhost:3000` adresine git.
- Ürünler ve filtreleme paneli ekranda görünecek.

---

## 7. Sık Karşılaşılan Sorunlar

- **Port çakışması:** Eğer 3000 veya 5113 portu başka bir uygulama tarafından kullanılıyorsa, ilgili uygulamayı kapat veya portu değiştir.
- **API bağlantı hatası:** Backend'in çalıştığından ve proxy ayarının doğru olduğundan emin ol.
- **node_modules veya bin/obj gibi klasörler:** Bunlar `.gitignore` ile repoya eklenmez, eksikse `npm install` ve `dotnet restore` komutlarını tekrar çalıştır.

---

Herhangi bir hata veya sorunla karşılaşırsan, hata mesajını bana iletebilirsin! 