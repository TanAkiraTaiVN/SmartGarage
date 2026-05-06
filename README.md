# SmartGarage - Nền tảng quản lý bãi giữ xe thông minh

## Giới thiệu

SmartGarage là nền tảng quản lý bãi giữ xe đa nền tảng, cung cấp:
- **Website quản trị**: Giao diện web cho nhân viên và quản lý
- **Backend API**: RESTful API hỗ trợ mobile app và web
- **Tính năng QR Code**: Check-in/Check-out nhanh bằng mã QR

## Kiến trúc hệ thống

```
SmartGarage/
├── SmartGarage.API/          # ASP.NET Core Web API + Admin UI
│   ├── Controllers/          # API Controllers
│   ├── wwwroot/              # Admin Web UI (HTML/CSS/JS)
│   └── Program.cs            # Entry point & DI configuration
├── SmartGarage.Core/         # Domain models & interfaces
│   ├── Entities/             # Database entities
│   ├── Enums/                # Enum definitions
│   ├── DTOs/                 # Data Transfer Objects
│   └── Interfaces/           # Service interfaces
├── SmartGarage.Infrastructure/  # Data access & service implementations
│   ├── Data/                 # DbContext & migrations
│   └── Services/             # Service implementations
└── SmartGarage.sln           # Solution file
```

## Công nghệ sử dụng

| Layer | Công nghệ |
|-------|-----------|
| Backend | ASP.NET Core 8.0 (C#) |
| Database | SQL Server / SQLite (dev) |
| ORM | Entity Framework Core 8.0 |
| Authentication | JWT Bearer Token |
| QR Code | QRCoder |
| API Docs | Swagger / OpenAPI |
| Frontend Admin | HTML5, CSS3, JavaScript (Vanilla) |
| Password Hash | BCrypt |

## Tính năng chính

### 1. Quản lý phương tiện
- CRUD phương tiện (xe máy, ô tô, xe đạp, xe tải)
- Theo dõi biển số, hãng xe, màu sắc
- Phân quyền theo chủ sở hữu

### 2. Quản lý vị trí đỗ xe
- Phân khu vực (Zone A, B, ...)
- Phân tầng (Floor 1, 2, 3, ...)
- Trạng thái real-time (Trống, Đang đỗ, Bảo trì)

### 3. Check-in / Check-out bằng QR Code
- Tạo vé xe với mã QR tự động
- Quét QR để check-out nhanh
- Tính phí tự động theo thời gian và loại xe

### 4. Thanh toán trực tuyến
- Hỗ trợ nhiều phương thức: Tiền mặt, Thẻ, MoMo, ZaloPay, Chuyển khoản
- Lịch sử giao dịch chi tiết
- Biên lai điện tử

### 5. Thống kê doanh thu
- Dashboard tổng quan
- Doanh thu theo ngày/tháng/năm
- Thống kê theo loại xe
- Biểu đồ trực quan

### 6. Hệ thống thông báo
- Thông báo check-in/check-out
- Đánh dấu đã đọc
- API cho mobile app

### 7. Phân quyền người dùng
- **Admin**: Toàn quyền quản lý
- **Manager**: Quản lý vé, phương tiện, xem thống kê
- **Staff**: Check-in/out, quản lý vé
- **Customer**: Xem thông tin cá nhân, lịch sử gửi xe

## Cài đặt & Chạy

### Yêu cầu
- .NET 8.0 SDK
- (Tùy chọn) SQL Server cho production

### Bước 1: Clone repository
```bash
git clone https://github.com/TanAkiraTaiVN/SmartGarage.git
cd SmartGarage
```

### Bước 2: Restore packages
```bash
dotnet restore
```

### Bước 3: Chạy ứng dụng
```bash
cd SmartGarage.API
dotnet run
```

### Bước 4: Truy cập
- **Admin UI**: http://localhost:5000
- **Swagger API**: http://localhost:5000/swagger
- **Tài khoản mặc định**: admin@smartgarage.com / Admin@123

## API Endpoints

### Authentication
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/Auth/login` | Đăng nhập |
| POST | `/api/Auth/register` | Đăng ký tài khoản |

### Vehicles (Phương tiện)
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/Vehicles` | Lấy danh sách phương tiện |
| GET | `/api/Vehicles/{id}` | Lấy chi tiết phương tiện |
| GET | `/api/Vehicles/my-vehicles` | Lấy phương tiện của tôi |
| POST | `/api/Vehicles` | Thêm phương tiện |
| PUT | `/api/Vehicles/{id}` | Cập nhật phương tiện |
| DELETE | `/api/Vehicles/{id}` | Xóa phương tiện |

### Parking Spots (Vị trí đỗ)
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/ParkingSpots` | Lấy tất cả vị trí |
| GET | `/api/ParkingSpots/available` | Lấy vị trí trống |
| POST | `/api/ParkingSpots` | Thêm vị trí |
| PATCH | `/api/ParkingSpots/{id}/status` | Cập nhật trạng thái |

### Tickets (Vé xe)
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/Tickets` | Lấy tất cả vé |
| GET | `/api/Tickets/active` | Lấy vé đang hoạt động |
| GET | `/api/Tickets/my-tickets` | Lấy vé của tôi |
| POST | `/api/Tickets/check-in` | Check-in xe |
| POST | `/api/Tickets/check-out` | Check-out xe |

### Payments (Thanh toán)
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/Payments` | Lấy lịch sử thanh toán |
| GET | `/api/Payments/my-payments` | Lấy thanh toán của tôi |

### Dashboard (Thống kê)
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/Dashboard` | Lấy thống kê tổng quan |
| GET | `/api/Dashboard/revenue` | Lấy doanh thu theo khoảng thời gian |

### Notifications (Thông báo)
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/Notifications` | Lấy thông báo |
| GET | `/api/Notifications/unread-count` | Đếm thông báo chưa đọc |
| PATCH | `/api/Notifications/{id}/read` | Đánh dấu đã đọc |
| PATCH | `/api/Notifications/read-all` | Đánh dấu tất cả đã đọc |

## Bảng giá mặc định

| Loại xe | Giá/giờ | Giá/ngày | Giá/tháng |
|---------|---------|----------|-----------|
| Xe đạp | 2,000 VND | 10,000 VND | 200,000 VND |
| Xe máy | 5,000 VND | 30,000 VND | 500,000 VND |
| Ô tô | 20,000 VND | 100,000 VND | 2,000,000 VND |
| Xe tải | 30,000 VND | 150,000 VND | 3,000,000 VND |

## License

MIT License
