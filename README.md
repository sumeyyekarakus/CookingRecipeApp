# 🍽️ CookingRecipeApp

**CookingRecipeApp**, kullanıcıların yemek tariflerini ve malzemeleri kolayca yönetebileceği, dinamik arama ve filtreleme özelliklerine sahip bir masaüstü uygulamasıdır. Uygulama C# ile geliştirilmiş olup SQL Server veritabanı ile desteklenmektedir.

## 📌 Proje Amacı

Bu projenin amacı, kullanıcı dostu bir arayüzle çalışan, SQL Server veritabanı üzerinden tarif ve malzeme yönetimi yapılabilen işlevsel bir masaüstü uygulaması geliştirmektir.

## 🧩 Uygulama Özellikleri

- Tarif ve malzeme ekleme, silme ve güncelleme
- Dinamik arama ve filtreleme (isim, süre, maliyet, kategori vb.)
- Malzeme uyum yüzdesine göre tarif önerisi
- Eksik malzeme maliyet hesaplama
- Kullanıcı arayüzünde renkli geri bildirim (yeşil: yeterli malzeme, kırmızı: eksik)
- Kategorilere, alfabetik sıralamaya ve maliyete göre sıralama

## 🛠️ Kullanılan Teknolojiler

- **C#**
- **.NET Windows Forms**
- **SQL Server**
- **ADO.NET** (veritabanı bağlantıları için)

## 🗂️ Veritabanı Yapısı

- **Malzemeler Tablosu**: ID, Ad, Toplam Miktar, Birim, Birim Fiyat
- **Tarifler Tablosu**: Ad, Kategori, Hazırlama Süresi, Talimatlar
- **Tarif-Malzeme İlişkisi**: Tarif ile malzemeleri eşleştirir

## 🖼️ Arayüz Özellikleri

- Ana sayfada tüm tariflerin listesi
- Menü üzerinden tarif ekleme/güncelleme/silme
- Arama çubuğu ve filtreleme butonları
- Kategori kutuları (Tatlılar, Et Yemekleri, Zeytinyağlılar, vb.)
- Tarif detay ekranı (malzeme listesi + maliyet bilgisi)

## 📦 Formlar ve Sınıflar

| Form/Sınıf             | Açıklama |
|------------------------|----------|
| `Form1.cs`             | Ana form, menüler ve genel arayüz |
| `MalzemelerimForm.cs`  | Malzeme yönetimi |
| `TarifEklemeForm.cs`   | Tarif ekleme ekranı |
| `TarifGuncellemeForm.cs` | Tarif güncelleme |
| `TarifSilmeForm.cs`    | Tarif silme |
| `TarifDetayForm.cs`    | Tarif detay görüntüleme |
| `TarifOnerisiForm.cs`  | Eksik/yeterli malzeme öneri ekranı |

## 📊 Deneysel Sonuçlar

- Tüm CRUD (ekle, sil, güncelle) işlemleri başarıyla çalışmaktadır
- Arama ve filtreleme kullanıcı deneyimini geliştirmiştir
- Otomatik tamamlama ve renkli uyarılar sayesinde kullanıcı yönlendirmesi etkili olmuştur
- Veritabanı tutarlılığı sağlanmıştır

## 📃 Lisans

Bu proje eğitim amaçlıdır.


___________________________________________________________________________________________________________________________________________________________________________________________________________________


# 🍽️ CookingRecipeApp

**CookingRecipeApp** is a desktop application that allows users to manage recipes and ingredients with dynamic search and filtering features. The app is developed using C# and utilizes a SQL Server database.

## 📌 Project Purpose

The goal of this project is to develop a functional desktop application that enables users to manage meal recipes and their ingredients through a user-friendly interface, supported by SQL Server.

## 🧩 Features

- Add, update, and delete recipes and ingredients
- Dynamic search and filtering (by name, duration, cost, category, etc.)
- Recipe suggestions based on available ingredients
- Cost calculation for missing ingredients
- Visual feedback: green for sufficient ingredients, red for missing ones
- Sort recipes by category, preparation time, cost, or name

## 🛠️ Technologies Used

- **C#**
- **.NET Windows Forms**
- **SQL Server**
- **ADO.NET** (for database interaction)

## 🗂️ Database Structure

- **Ingredients Table**: ID, Name, Total Amount, Unit, Unit Price
- **Recipes Table**: Name, Category, Preparation Time, Instructions
- **Recipe-Ingredient Relationship Table**: links recipes with their ingredients

## 🖼️ User Interface Overview

- Main screen displays a list of all recipes
- Menu for recipe operations: Add / Update / Delete
- Search bar and filter buttons
- Category boxes (Desserts, Meat Dishes, Vegetarian, etc.)
- Recipe detail view (ingredients + cost breakdown)

## 📦 Forms and Classes

| Form/Class               | Description |
|--------------------------|-------------|
| `Form1.cs`               | Main form and menu interface |
| `MalzemelerimForm.cs`    | Ingredient management |
| `TarifEklemeForm.cs`     | Add new recipe |
| `TarifGuncellemeForm.cs` | Update existing recipe |
| `TarifSilmeForm.cs`      | Delete recipe |
| `TarifDetayForm.cs`      | Show recipe details |
| `TarifOnerisiForm.cs`    | Suggest recipes based on available ingredients |

## 📊 Experimental Results

- All CRUD operations (create, read, update, delete) were tested successfully
- Search and filter functions improved usability
- Auto-complete and color-coded alerts enhanced user experience
- Database consistency and integrity were maintained

## 📃 License

This project was developed for educational purposes. For commercial use, please contact the developers.


