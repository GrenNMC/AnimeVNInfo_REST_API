# AnimeVNInfo_REST_API
---
# Dự án AnimeVNInfo Rest API

## Giới thiệu

Dự án AnimeVNInfo Rest API là một dự án nhằm tạo ra một RESTful API để quản lý các thông tin liên quan đến nội dung anime. API này cung cấp các điểm cuối để xử lý dữ liệu anime, trạng thái, nhà sản xuất và nhiều thông tin khác.

## Yêu cầu cơ bản

Trước khi bắt đầu, hãy chắc chắn rằng bạn đã cài đặt các công cụ và công nghệ cần thiết:

- [.NET Core 7](https://dotnet.microsoft.com/download/dotnet/7.0)
- [PostgreSQL](https://www.postgresql.org/download/)

## Cài đặt và Thiết lập
1. **Sao chép Dự án:** Sử dụng Git để sao chép dự án AnimeVNInfo Rest API về máy tính của bạn:

   ```bash
   git clone https://github.com/your-username/AnimeVNInfo_Rest_API.git
   ````
2. Cấu hình PostgreSQL: Tạo một cơ sở dữ liệu PostgreSQL mới và cập nhật chuỗi kết nối trong tệp appsettings.json:
   ````json
   "ConnectionStrings": {"DefaultConnection": "Host=localhost;Database=animevninfo;Username=yourusername;Password=yourpassword"}
   ````
3. Khởi động Ứng dụng: Chạy lệnh sau để khởi động ứng dụng:
````bash
dotnet run
````
## Truy cập API: Mở một trình duyệt web hoặc sử dụng công cụ như Postman để truy cập các điểm cuối của API tại https://localhost:port/api.
