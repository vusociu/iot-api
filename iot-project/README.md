# IoT Smart Lock API

Hệ thống API quản lý khóa thông minh IoT, cho phép đăng ký người dùng, quản lý thẻ căn cước, theo dõi lịch sử kiểm tra thẻ và điều khiển mở cửa từ xa thông qua giao thức MQTT.

## 📋 Mục lục

- [Tính năng](#tính-năng)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Cài đặt](#cài-đặt)
- [Cấu hình](#cấu-hình)
- [Chạy ứng dụng](#chạy-ứng-dụng)
- [API Endpoints](#api-endpoints)
- [Cấu trúc dự án](#cấu-trúc-dự-án)
- [Database](#database)

## ✨ Tính năng

- 🔐 **Xác thực người dùng**: Đăng ký và đăng nhập với JWT token
- 👤 **Quản lý người dùng**: Phân quyền Admin và User
- 🆔 **Quản lý thẻ căn cước**: Đăng ký và lưu trữ thông tin thẻ căn cước
- 📊 **Lịch sử kiểm tra thẻ**: Theo dõi lịch sử quét thẻ và trạng thái
- 🚪 **Điều khiển từ xa**: Mở cửa thông qua MQTT
- 📡 **Tích hợp MQTT**: Giao tiếp real-time với thiết bị IoT
- 💾 **Caching**: Sử dụng Memory Cache để tối ưu hiệu suất

## 🛠 Công nghệ sử dụng

- **.NET 8.0**: Framework chính
- **ASP.NET Core Web API**: Xây dựng RESTful API
- **Entity Framework Core 9.0**: ORM cho database
- **SQL Server**: Hệ quản trị cơ sở dữ liệu
- **MQTTnet 4.3.7**: Thư viện MQTT client
- **JWT (System.IdentityModel.Tokens.Jwt)**: Xác thực và phân quyền
- **BCrypt.Net**: Mã hóa mật khẩu
- **Memory Cache**: Caching dữ liệu

## 📦 Yêu cầu hệ thống

- .NET 8.0 SDK hoặc cao hơn
- SQL Server (LocalDB hoặc SQL Server Express)
- MQTT Broker (để kết nối với thiết bị IoT)
- Visual Studio 2022 hoặc VS Code (khuyến nghị)

## 🚀 Cài đặt

1. **Clone repository**
```bash
git clone <repository-url>
cd iot-project
```

2. **Khôi phục packages**
```bash
dotnet restore
```

3. **Cấu hình database**
   - Tạo database SQL Server
   - Cập nhật connection string trong `appsettings.json`

4. **Chạy migrations**
```bash
dotnet ef database update
```

## ⚙️ Cấu hình

### appsettings.json

Cấu hình các thông số trong file `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=iot;Integrated Security=True;..."
  },
  "Mqtt": {
    "Server": "192.168.1.13",
    "Port": 1883,
    "Username": "your_username",
    "Password": "your_password"
  }
}
```

### Các thông số cần cấu hình:

- **ConnectionStrings.DefaultConnection**: Chuỗi kết nối SQL Server
- **Mqtt.Server**: Địa chỉ IP hoặc hostname của MQTT broker
- **Mqtt.Port**: Port của MQTT broker (mặc định: 1883)
- **Mqtt.Username**: Username để kết nối MQTT
- **Mqtt.Password**: Password để kết nối MQTT

## ▶️ Chạy ứng dụng

### Development mode
```bash
dotnet run
```

### Production mode
```bash
dotnet build --configuration Release
dotnet run --configuration Release
```

Ứng dụng sẽ chạy tại: `https://localhost:5001` hoặc `http://localhost:5000`

## 📡 API Endpoints

### Authentication

#### Đăng ký
```
POST /iot/api/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123",
  "fullName": "Nguyễn Văn A",
  "role": 0  // 0: ADMIN, 1: USER
}
```

#### Đăng nhập
```
POST /iot/api/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}

Response:
{
  "token": "jwt_token_here",
  "fullName": "Nguyễn Văn A",
  "role": "ADMIN"
}
```

### IoT Operations

#### Đăng ký người dùng (thẻ căn cước)
```
POST /iot/api/register-user
Authorization: Bearer {token}
Content-Type: application/json

{
  "fullName": "Nguyễn Văn A",
  "birthday": "01/01/1990",
  "phone": "0123456789"
}
```

#### Lấy lịch sử kiểm tra thẻ
```
GET /iot/api/history
Authorization: Bearer {token}
```

#### Mở cửa từ xa
```
GET /iot/api/open-door
Authorization: Bearer {token}
```

## 📁 Cấu trúc dự án

```
iot-project/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   └── IoTController.cs
├── Data/                 # Repository pattern và DbContext
│   ├── AppDbContext.cs
│   ├── IUserRepository.cs
│   ├── UserRepository.cs
│   ├── IIdentityCardRepository.cs
│   ├── IdentityCardRepository.cs
│   ├── ICheckCardHistoryRepository.cs
│   └── CheckCardHistoryRepository.cs
├── DTOs/                 # Data Transfer Objects
│   ├── Authenticate/
│   │   ├── LoginDTO.cs
│   │   └── RegisterDTO.cs
│   └── IoT/
│       └── RegisterUserDTO.cs
├── Enum/                 # Enumerations
│   ├── CheckCardStatus.cs
│   ├── MqttTopic.cs
│   └── Role.cs
├── Helpers/              # Services và utilities
│   ├── JwtService.cs
│   ├── MqttService.cs
│   └── MqttSubcriberService.cs
├── Middlewares/          # Custom middlewares
│   └── AuthMiddleware.cs
├── Models/               # Entity models
│   ├── User.cs
│   ├── IdentityCard.cs
│   ├── CheckCardHistory.cs
│   └── ErrorViewModel.cs
├── Migrations/           # Entity Framework migrations
├── Transformer/          # Data transformers
│   ├── CheckCardHistoryTransformer.cs
│   └── ListCheckCardHistoryTransformer.cs
├── Views/                # Razor views (nếu có)
├── wwwroot/              # Static files
├── appsettings.json      # Cấu hình ứng dụng
├── Program.cs            # Entry point và DI configuration
└── iot-project.csproj    # Project file
```

## 🗄️ Database

### Các bảng chính:

#### Users
- `id` (int, PK)
- `email` (string, unique)
- `password` (string, hashed)
- `fullName` (string)
- `role` (enum: ADMIN, USER)

#### IdentityCards
- `id` (int, PK)
- `idCard` (string, unique)
- `fullName` (string)
- `birthday` (DateTime)
- `phone` (string)

#### CheckCardHistories
- `id` (int, PK)
- `idCard` (string)
- `status` (enum: CheckCardStatus)
- `time` (DateTime)
- `fullName` (string)

### Chạy migrations:

```bash
# Tạo migration mới
dotnet ef migrations add MigrationName

# Cập nhật database
dotnet ef database update
```

## 🔒 Bảo mật

- Mật khẩu được mã hóa bằng BCrypt
- JWT token cho authentication
- Middleware xác thực cho các API endpoints
- HttpOnly cookies cho JWT token

## 📝 Ghi chú

- JWT secret key hiện đang hardcode trong `JwtService.cs`. Nên chuyển sang cấu hình trong `appsettings.json` hoặc sử dụng User Secrets cho production.
- MQTT topics được định nghĩa trong `MqttTopic` enum:
  - `OPEN_DOOR`: Topic để mở cửa
  - `DEVICE_REGISTER`: Topic đăng ký thiết bị
  - `SERVER`: Topic server để gửi lệnh

## 📄 License


## 👥 Tác giả

VuND

