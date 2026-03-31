QLBS - Hệ Thống Quản Lý Bán Sách Online
QLBS (Quản Lý Bán Sách) là một ứng dụng web thương mại điện tử chuyên về sách, được xây dựng với kiến trúc hiện đại, tách biệt Backend và Frontend. Dự án tập trung vào trải nghiệm mua sắm mượt mà cho người dùng và công cụ quản trị hiệu quả cho người bán.

Công Nghệ Sử Dụng
Dự án được phát triển dựa trên các công nghệ mạnh mẽ sau:

Backend: ASP.NET Core API.

Frontend: Blazor.

Database: SQL Server.

Xác thực: JSON Web Token (JWT) cho tính năng Đăng ký và Đăng nhập.

Thanh toán: Tích hợp cổng thanh toán VNPay.

Tính Năng Chính
Người dùng (Khách hàng):

Xem danh mục sách và chi tiết từng sản phẩm.

Đăng ký/Đăng nhập tài khoản bảo mật với JWT và Google OAuth.

Giỏ hàng và thanh toán trực tuyến qua VNPay.

Theo dõi đơn hàng đã mua.

Thêm, xóa yêu thích.

Đánh giá sau khi đã hoàn thành đơn hàng.

Quản trị viên (Admin):

Quản lý danh mục sách (thêm, xóa, sửa).

Quản lý sách, số lượng tồn kho (thêm, xóa, sửa).

Quản lý đơn hàng và trạng thái vận chuyển.

Quản lý mã giảm giá (thêm, xóa, sửa).

Cấu Trúc Thư Mục
Dựa trên cấu trúc repository hiện tại:

/QLBS: Chứa mã nguồn Backend (ASP.NET Core).

/FE_QLBS: Chứa mã nguồn Frontend (Blazor).

QLBS.sql: File kịch bản (script) để khởi tạo cơ sở dữ liệu SQL Server.

QLBS.sln: File solution để mở toàn bộ dự án bằng Visual Studio.

Hướng Dẫn Cài Đặt
Clone repository:

Bash
git clone https://github.com/tanhung2312/QLBS.git
Thiết lập Cơ sở dữ liệu:

Mở SQL Server Management Studio (SSMS).

Chạy file QLBS.sql để tạo database và dữ liệu mẫu.

Cấu hình Backend:

Mở file appsettings.json trong thư mục QLBS.

"Jwt": {
    "Key": " ",
    "Issuer": " ",
    "Audience": " "
},




"CloudinarySettings": {
    "CloudName": " ",
    "ApiKey": " ",
    "ApiSecret": " "
},




"Vnpay": {
    "TmnCode": " ",
    "HashSecret": " ",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "Version": "2.1.0",
    "ReturnUrl": " "
},




"GhnSettings": {
    "ApiBaseUrl": "https://dev-online-gateway.ghn.vn/shiip/public-api",
    "ApiToken": " ",
    "ShopId": ,
    "FromDistrictId": ,
    //"PaymentTypeId": 2,
    "ServiceTypeId": 2,
    "DefaultWeight": 200
},





"EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": ,
    "SenderEmail": " ",
    "SenderName": " ",
    "Password": " "
},




"GoogleAuth": {
    "ClientId": " ",
    "ClientSecret": " "
}

Cập nhật ConnectionStrings để trỏ đúng vào SQL Server của bạn.

Chạy ứng dụng:

Mở QLBS.sln bằng Visual Studio.

Thiết lập chạy đồng thời cả project Backend API và Frontend (FE_QLBS).


