# ĐẠI HỌC QUỐC GIA THÀNH PHỐ HỒ CHÍ MINH
## TRƯỜNG ĐẠI HỌC CÔNG NGHỆ THÔNG TIN
### KHOA CÔNG NGHỆ PHẦN MỀM

---

**TÀI LIỆU MÔ HÌNH HÓA VẤN ĐỀ VÀ GIẢI PHÁP**
**ỨNG DỤNG THƯƠNG MẠI ĐIỆN TỬ**

* **GVHD:** Nguyễn Công Hoan
* **Sinh viên 1:** Lê Minh Phát
* **Sinh viên 2:** Trần Quang Mạnh

*Tp. Hồ Chí Minh, 5/2026*

---

## 1. MÔ HÌNH HÓA VẤN ĐỀ (PROBLEM MODELING)

### 1.1. Thực trạng và sự cấp thiết của việc số hóa vận hành thương mại điện tử
* Sự kỳ vọng của khách hàng về tốc độ phản hồi, tính chính xác của tồn kho, trạng thái đơn hàng và minh bạch trong thanh toán đặt ra yêu cầu cao cho hệ thống backend.
* Quy trình bán hàng trực tuyến nếu phụ thuộc nhiều vào thao tác thủ công sẽ dễ tạo ra nút thắt ở các khâu quản lý sản phẩm, giỏ hàng, đặt hàng, thanh toán, hủy đơn và hoàn tiền.
* Việc chuyển đổi từ các xử lý rời rạc sang một API backend có cấu trúc, có quan sát hệ thống và có bộ nhớ đệm là yêu cầu cấp thiết để đảm bảo khả năng mở rộng.

### 1.2. Phân tích đa chiều về các điểm đau (Pain Points)
* **Sự đứt gãy trong luồng đơn hàng và thanh toán (Order and Payment Discontinuity):** Đơn hàng, thanh toán, hủy đơn và hoàn tiền nếu không được đồng bộ theo trạng thái rõ ràng sẽ làm tăng rủi ro xử lý sai, hoàn tiền trùng hoặc cập nhật tồn kho không nhất quán.
* **Sự quá tải ở lớp truy vấn danh mục (Catalog Read Pressure):** Sản phẩm và danh mục là nhóm dữ liệu được đọc thường xuyên. Nếu mọi yêu cầu đều đi trực tiếp đến SQL Server, hệ thống dễ gặp độ trễ cao khi lưu lượng tăng.
* **Sự thiếu hụt khả năng quan sát lỗi vận hành (Observability Gap):** Log rời rạc trên từng container khiến việc truy vết lỗi API, lỗi nền trong xử lý thanh toán chờ và lỗi hoàn tiền mất nhiều thời gian.
* **Sự phân mảnh cấu hình môi trường (Configuration Fragmentation):** SQL Server, Redis, Elasticsearch, Kibana, JWT và email cần được chuẩn hóa qua `.env` để giảm sai lệch giữa môi trường local, Docker và triển khai.

### 1.3. Mô hình hóa nguyên nhân gốc rễ (Ishikawa Diagram)
* Sơ đồ xương cá chỉ ra cốt lõi vấn đề nằm ở sự thiếu tự động hóa và thiếu đồng bộ giữa các yếu tố: Con người, Phương pháp, Công nghệ và Dữ liệu.
* **Nhận xét:** Việc xây dựng một backend thương mại điện tử có kiến trúc phân lớp, log tập trung, cache Redis và cấu hình Docker thống nhất là cấp bách để giảm rủi ro vận hành.

---

## 2. MÔ HÌNH HÓA GIẢI PHÁP TỔNG THỂ (SOLUTION MODELING)
Dự án đề xuất kiến trúc **"Observable E-Commerce Backend"** - một hệ thống API thương mại điện tử lấy dữ liệu giao dịch làm trung tâm, có bộ nhớ đệm cho dữ liệu đọc nhiều và có hạ tầng quan sát tập trung.

### 2.1. Giải pháp Giao dịch: Hệ thống đơn hàng, thanh toán, hủy đơn và hoàn tiền
* **Kỹ thuật:** Chuẩn hóa luồng nghiệp vụ qua các controller, service, repository, DTO và entity riêng cho Order, Payment, Cancellation và Refund.
* **Tích hợp:** SQL Server đóng vai trò nguồn dữ liệu chính; Entity Framework Core quản lý quan hệ giữa Customer, Address, Cart, Product, Order, Payment, Cancellation và Refund.
* **Thực thi:** Trạng thái nghiệp vụ được mô hình hóa bằng bảng Status và các enum, giúp cập nhật đơn hàng, COD payment, yêu cầu hủy và xử lý hoàn tiền theo quy trình có kiểm soát.
* *(Hình ảnh minh họa: Sequence Diagram luồng hoạt động của tính năng đặt hàng và thanh toán)*.

### 2.2. Giải pháp Hiệu năng: Redis Cache cho dữ liệu danh mục
* **Kỹ thuật:** Triển khai `IDistributedCache` với Redis trong Docker và cơ chế fallback in-memory khi không có Redis connection string.
* **Cơ chế:** Cache dữ liệu chi tiết sản phẩm và danh mục bằng key có version; khi tạo, cập nhật, xóa hoặc đổi trạng thái dữ liệu danh mục, version được làm mới để vô hiệu hóa cache cũ.
* **Thực thi:** `ProductService` và `CategoryService` đọc cache trước, sau đó mới truy vấn SQL Server. Thời gian sống cache được cấu hình bằng `Cache__CatalogExpirationMinutes`.
* *(Hình ảnh minh họa: Activity Diagram luồng cache hit/cache miss của dữ liệu catalog)*.

### 2.3. Giải pháp Quan sát hệ thống: Serilog, Elasticsearch và Kibana
* **Kỹ thuật:** Serilog được cấu hình ở mức host, ghi log console/file và đẩy log có cấu trúc sang Elasticsearch.
* **Dữ liệu:** Log được làm giàu với Application, Server, MachineName, EnvironmentName và log context để hỗ trợ truy vết request, response, exception và background service.
* **Vận hành:** Elasticsearch và Kibana chạy bằng Docker Compose; Kibana dùng để tìm kiếm, lọc và trực quan hóa log theo index cấu hình trong `.env`.
* *(Hình ảnh minh họa: ERD/Deployment Diagram cho cụm API - SQL Server - Redis - Elasticsearch - Kibana)*.

### 2.4. Giải pháp Hạ tầng và Bảo mật: Dockerized API với JWT
* **Kỹ thuật:** ASP.NET Core API được đóng gói bằng Dockerfile và chạy cùng SQL Server, Redis, Elasticsearch, Kibana trong Docker Compose.
* **Thực thi:** JWT Authentication bảo vệ các API quản trị như tạo, cập nhật, xóa sản phẩm; cấu hình issuer, audience, key và danh sách admin email được đưa vào `.env`.
* **Cá nhân hóa vận hành:** Cấu hình môi trường thống nhất giúp nhóm phát triển thay đổi cổng, connection string, SMTP, cache TTL và log level mà không cần sửa mã nguồn.
* *(Hình ảnh minh họa: Solution Architecture tổng thể của backend thương mại điện tử)*.

---

## 3. PHÂN TÍCH HIỆU QUẢ CHUYỂN ĐỔI (TRANSFORMATION ANALYSIS)

| Chỉ số đánh giá | Mô hình hiện trạng (Thủ công/rời rạc) | Mô hình đề xuất (Tự động hóa và quan sát tập trung) |
| :--- | :--- | :--- |
| **Độ trễ truy vấn catalog** | Phụ thuộc trực tiếp vào SQL Server ở mọi request | Giảm tải bằng Redis cache cho sản phẩm và danh mục |
| **Khả năng truy vết lỗi** | Log phân tán theo container hoặc console local | Log có cấu trúc tập trung trong Elasticsearch/Kibana |
| **Độ tin cậy cấu hình** | Dễ sai lệch giữa local và Docker | Chuẩn hóa qua `.env`, `appsettings` và Docker Compose |
| **Khả năng mở rộng vận hành** | Khó mở rộng khi lưu lượng đọc và số đơn hàng tăng | Tách rõ API, database, cache và observability stack |

---

## 4. KẾT LUẬN VÀ GIÁ TRỊ CỐT LÕI
* Dự án không chỉ tập trung vào việc lập trình các API riêng lẻ, mà hướng tới việc tái cấu trúc luồng vận hành của một hệ thống thương mại điện tử backend.
* Sự kết hợp giữa Luồng Giao dịch (Transaction Flow) nhất quán, Luồng Dữ liệu (Data Flow) có cache và Luồng Quan sát (Observability Flow) tập trung là nền tảng để xây dựng một hệ thống hiện đại, minh bạch và có khả năng mở rộng.
