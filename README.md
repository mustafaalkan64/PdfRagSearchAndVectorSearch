# PDF Vector Search and Rag Search Demo

[🇹🇷 Türkçe](#türkçe) | [🇺🇸 English](#english)

---

## English

### 🔍 Overview

PDF Semantic Search Demo is a full-stack application that enables AI-powered semantic search and Retrieval-Augmented Generation (RAG) on PDF documents. Users can upload PDF files, and the system will process them into searchable chunks using vector embeddings, allowing for both traditional semantic search and AI-generated answers based on document content.

### ✨ Features

- **📄 PDF Upload & Processing**: Upload PDF documents and automatically extract and chunk content
- **🔍 Vector Search**: Semantic similarity search using AI embeddings
- **🤖 RAG Search**: AI-generated answers based on document content using Ollama LLM
- **📊 Dual Search Modes**: Toggle between vector search and RAG search
- **🎯 Source Attribution**: View source documents and relevance scores
- **⚡ Real-time Processing**: Fast document processing and search capabilities
- **🐳 Docker Support**: Complete containerized deployment with Docker Compose
- **🌐 Modern UI**: React TypeScript frontend with responsive design

## 📸 Demo

Here’s a preview of the AI-powered PDF semantic search interface:

![Demo](https://github.com/mustafaalkan64/PdfRagSearchAndVectorSearch/blob/master/PdfSemanticSearchDemo/docs/images/ss-1.png)
![Demo](https://github.com/mustafaalkan64/PdfRagSearchAndVectorSearch/blob/master/PdfSemanticSearchDemo/docs/images/ss-2.png)

### 🏗️ Architecture

#### Backend (.NET 9)
- **ASP.NET Core Web API** with clean architecture
- **Vector Database**: Qdrant for storing and querying embeddings
- **LLM Integration**: Ollama for text embeddings and generation
- **PDF Processing**: iText7 for PDF text extraction
- **Services**:
  - `PdfService`: PDF text extraction and chunking
  - `QdrantVectorService`: Vector storage and similarity search
  - `OllamaService`: LLM integration for embeddings and generation
  - `RagService`: Retrieval-Augmented Generation implementation

#### Frontend (React TypeScript)
- **React 18** with TypeScript
- **Modern UI**: Clean, responsive design with CSS3
- **Dual Search Interface**: Vector search and RAG search modes
- **File Upload**: Drag-and-drop PDF upload functionality
- **Real-time Results**: Live search results with source attribution

#### Infrastructure
- **Qdrant**: Vector database for embeddings storage
- **Ollama**: Local LLM server with nomic-embed-text and llama3.2 models
- **Docker Compose**: Complete containerized deployment

### 🚀 Quick Start

#### Prerequisites
- Docker and Docker Compose
- React JS (for frontend development)
- .NET 9 SDK (for backend development)

#### Using Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd PdfSemanticSearchDemo
   ```

2. **Start all services**
   ```bash
   docker-compose up -d
   ```

3. **Wait for models to download** (first run only)
   ```bash
   # Monitor Ollama container logs
   docker-compose logs -f ollama
   ```

4. **Access the application**
   - Backend API: http://localhost:5000
   - Qdrant Dashboard: http://localhost:6333/dashboard
   - Ollama API: http://localhost:11434
   - React APP: http://localhost:3000

#### Manual Setup

##### Backend Setup
```bash
cd PdfSemanticSearchDemo
dotnet restore
dotnet run
```

##### Frontend Setup
```bash
cd frontend
npm install
npm start
```

##### External Services
- **Qdrant**: Run locally on port 6333
- **Ollama**: Install and run with models:
  ```bash
  ollama pull nomic-embed-text
  ollama pull llama3.2
  ```

### 📖 Usage

1. **Upload PDF Documents**
   - Drag and drop PDF files or click to select
   - System automatically processes and creates vector embeddings

2. **Vector Search Mode**
   - Enter search queries to find semantically similar content
   - View relevance scores and source documents

3. **RAG Search Mode**
   - Ask questions about your documents
   - Get AI-generated answers with source attribution
   - View token usage and response time metrics

### 🔧 API Endpoints

#### Document Management
- `POST /api/document/upload` - Upload and process PDF
- `POST /api/document/search` - Vector similarity search

#### RAG Operations
- `POST /api/rag/search` - RAG-based question answering

#### Health Checks
- `GET /api/document/health` - Document service health
- `GET /api/rag/health` - RAG service health

### 🛠️ Configuration

#### Environment Variables
```bash
# Backend
ASPNETCORE_URLS=http://+:5000
OLLAMA_API_URL=http://localhost:11434
QDRANT_API_URL=http://localhost:6333

# Ollama Models
EMBEDDING_MODEL=nomic-embed-text
GENERATION_MODEL=llama3.2
```

#### Frontend Configuration
```typescript
// src/services/api.ts
const API_BASE_URL = 'http://localhost:5000/api';
```

### 🧪 Development

#### Backend Development
```bash
cd PdfSemanticSearchDemo
dotnet watch run
```

#### Frontend Development
```bash
cd frontend
npm start
```

#### Running Tests
```bash
# Backend tests
dotnet test

# Frontend tests
cd frontend
npm test
```

### 📦 Technologies Used

#### Backend
- .NET 9
- ASP.NET Core Web API
- Qdrant.Client
- iText7
- System.Text.Json

#### Frontend
- React 18
- TypeScript
- Axios
- CSS3

#### Infrastructure
- Docker & Docker Compose
- Qdrant Vector Database
- Ollama LLM Platform

### 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Türkçe

### 🔍 Genel Bakış

PDF Semantic Search Demo, PDF belgeler üzerinde AI destekli semantik arama ve Retrieval-Augmented Generation (RAG) özelliklerini sunan tam yığın bir uygulamadır. Kullanıcılar PDF dosyaları yükleyebilir ve sistem bunları vektör gömme kullanarak aranabilir parçalara işler, hem geleneksel semantik arama hem de belge içeriğine dayalı AI üretimli yanıtlar sağlar.

### ✨ Özellikler

- **📄 PDF Yükleme ve İşleme**: PDF belgelerini yükleyin ve içeriği otomatik olarak çıkarıp parçalayın
- **🔍 Vektör Arama**: AI gömme kullanarak semantik benzerlik araması
- **🤖 RAG Arama**: Ollama LLM kullanarak belge içeriğine dayalı AI üretimli yanıtlar
- **📊 İkili Arama Modları**: Vektör arama ve RAG arama arasında geçiş
- **🎯 Kaynak Atıfı**: Kaynak belgeleri ve ilgililik puanlarını görüntüleme
- **⚡ Gerçek Zamanlı İşleme**: Hızlı belge işleme ve arama yetenekleri
- **🐳 Docker Desteği**: Docker Compose ile tam konteynerleştirilmiş dağıtım
- **🌐 Modern UI**: Duyarlı tasarımlı React TypeScript ön yüzü

### 🏗️ Mimari

#### Arka Uç (.NET 9)
- **ASP.NET Core Web API** temiz mimari ile
- **Vektör Veritabanı**: Gömmeleri depolamak ve sorgulamak için Qdrant
- **LLM Entegrasyonu**: Metin gömme ve üretim için Ollama
- **PDF İşleme**: PDF metin çıkarma için iText7
- **Servisler**:
  - `PdfService`: PDF metin çıkarma ve parçalama
  - `QdrantVectorService`: Vektör depolama ve benzerlik araması
  - `OllamaService`: Gömme ve üretim için LLM entegrasyonu
  - `RagService`: Retrieval-Augmented Generation implementasyonu

#### Ön Yüz (React TypeScript)
- **React 18** TypeScript ile
- **Modern UI**: CSS3 ile temiz, duyarlı tasarım
- **İkili Arama Arayüzü**: Vektör arama ve RAG arama modları
- **Dosya Yükleme**: Sürükle-bırak PDF yükleme işlevselliği
- **Gerçek Zamanlı Sonuçlar**: Kaynak atıfı ile canlı arama sonuçları

#### Altyapı
- **Qdrant**: Gömme depolama için vektör veritabanı
- **Ollama**: nomic-embed-text ve llama3.2 modelleri ile yerel LLM sunucusu
- **Docker Compose**: Tam konteynerleştirilmiş dağıtım

### 🚀 Hızlı Başlangıç

#### Ön Koşullar
- Docker ve Docker Compose
- React JS (ön yüz geliştirme için)
- .NET 9 SDK (arka uç geliştirme için)

#### Docker Compose Kullanarak (Önerilen)

1. **Depoyu klonlayın**
   ```bash
   git clone <repository-url>
   cd PdfSemanticSearchDemo
   ```

2. **Tüm servisleri başlatın**
   ```bash
   docker-compose up -d
   ```

3. **Modellerin indirilmesini bekleyin** (sadece ilk çalıştırma)
   ```bash
   # Ollama konteyner loglarını izleyin
   docker-compose logs -f ollama
   ```

4. **Uygulamaya erişin**
   - Arka Uç API: http://localhost:5000
   - Qdrant Dashboard: http://localhost:6333/dashboard
   - Ollama API: http://localhost:11434
   - React App: http://localhost:3000

#### Manuel Kurulum

##### Arka Uç Kurulumu
```bash
cd PdfSemanticSearchDemo
dotnet restore
dotnet run
```

##### Ön Yüz Kurulumu
```bash
cd frontend
npm install
npm start
```

##### Harici Servisler
- **Qdrant**: 6333 portunda yerel olarak çalıştırın
- **Ollama**: Modeller ile kurun ve çalıştırın:
  ```bash
  ollama pull nomic-embed-text
  ollama pull llama3.2
  ```

### 📖 Kullanım

1. **PDF Belgelerini Yükleyin**
   - PDF dosyalarını sürükleyip bırakın veya seçmek için tıklayın
   - Sistem otomatik olarak işler ve vektör gömmeleri oluşturur

2. **Vektör Arama Modu**
   - Semantik olarak benzer içerik bulmak için arama sorguları girin
   - İlgililik puanlarını ve kaynak belgeleri görüntüleyin

3. **RAG Arama Modu**
   - Belgeleriniz hakkında sorular sorun
   - Kaynak atıfı ile AI üretimli yanıtlar alın
   - Token kullanımı ve yanıt süresi metriklerini görüntüleyin

### 🔧 API Uç Noktaları

#### Belge Yönetimi
- `POST /api/document/upload` - PDF yükle ve işle
- `POST /api/document/search` - Vektör benzerlik araması

#### RAG İşlemleri
- `POST /api/rag/search` - RAG tabanlı soru yanıtlama

#### Sağlık Kontrolleri
- `GET /api/document/health` - Belge servisi sağlığı
- `GET /api/rag/health` - RAG servisi sağlığı

### 🛠️ Yapılandırma

#### Ortam Değişkenleri
```bash
# Arka Uç
ASPNETCORE_URLS=http://+:5000
OLLAMA_API_URL=http://localhost:11434
QDRANT_API_URL=http://localhost:6333

# Ollama Modelleri
EMBEDDING_MODEL=nomic-embed-text
GENERATION_MODEL=llama3.2
```

#### Ön Yüz Yapılandırması
```typescript
// src/services/api.ts
const API_BASE_URL = 'http://localhost:5000/api';
```

### 🧪 Geliştirme

#### Arka Uç Geliştirme
```bash
cd PdfSemanticSearchDemo
dotnet watch run
```

#### Ön Yüz Geliştirme
```bash
cd frontend
npm start
```

#### Testleri Çalıştırma
```bash
# Arka uç testleri
dotnet test

# Ön yüz testleri
cd frontend
npm test
```

### 📦 Kullanılan Teknolojiler

#### Arka Uç
- .NET 9
- ASP.NET Core Web API
- Qdrant.Client
- iText7
- System.Text.Json

#### Ön Yüz
- React 18
- TypeScript
- Axios
- CSS3

#### Altyapı
- Docker & Docker Compose
- Qdrant Vektör Veritabanı
- Ollama LLM Platformu

### 🤝 Katkıda Bulunma

1. Depoyu fork edin
2. Özellik dalı oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Dalı push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

### 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır - detaylar için [LICENSE](LICENSE) dosyasına bakın.
