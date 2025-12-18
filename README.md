# 🧾 POS_TranVietTraLam_Fresher

**POS_TranVietTraLam_Fresher** là một hệ thống **Point-of-Sale (POS)** được xây dựng trong khuôn khổ dự án **Fresher**, nhằm mô phỏng một hệ thống bán hàng thực tế với đầy đủ các chức năng cốt lõi như bán hàng, thanh toán, quản lý đơn hàng và cập nhật realtime.

---

## 🌐 Demo & URLs

| Thành phần            | URL                                                                                                                                      | Mô tả                                       |
| --------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------- |
| **Frontend**          | [https://pos-lamtran213-ui.vercel.app/](https://pos-lamtran213-ui.vercel.app/)                                                           | Giao diện POS cho người dùng (React + Vite) |
| **Backend (Swagger)** | [https://pos-tranviettralam-fresher.onrender.com/swagger/index.html](https://pos-tranviettralam-fresher.onrender.com/swagger/index.html) | Swagger UI để xem & test API                |

> 🔎 **Lưu ý:** Khi truy cập Backend URL gốc, hệ thống sẽ tự động chuyển đến trang **Swagger UI**.

---

## 🛠️ Tech Stack

### Frontend

* ⚛️ ReactJS
* ⚡ Vite
* 🎨 Tailwind CSS

### Backend

* 🧩 .NET 8 Web API
* 🗄️ Entity Framework Core
* 🐘 PostgreSQL

### Infrastructure

* 🚀 Frontend: **Vercel**
* ☁️ Backend: **Render**
* 🗃️ Database: **PostgreSQL (Supabase)**

### Security & Realtime

* 🔐 JWT Authentication
* 🔄 Realtime (SignalR)

---

## ✨ Core Features

### 1️⃣ POS Screen – Màn hình bán hàng

* Hiển thị danh sách sản phẩm

  * Tên sản phẩm
  * Giá bán
* Thêm / xoá sản phẩm khỏi giỏ hàng
* Tự động tính tổng tiền
* Nút **Thanh toán**

---

### 2️⃣ Thanh toán & xử lý đơn hàng

* Gửi yêu cầu thanh toán từ Frontend lên Backend
* Hỗ trợ nhiều phương thức thanh toán (COD / Online)
* Hiển thị thông báo **Thanh toán thành công**
* Tự động **clear giỏ hàng** sau khi đặt đơn

---

### 3️⃣ Realtime Screen – Màn hình theo dõi đơn hàng

* Hiển thị danh sách đơn hàng **realtime**
* Tự động cập nhật, **không cần reload trang**
* Thêm sản phẩm, upload ảnh thông qua **Supabase Storage S3 AWS**
* Mỗi đơn hàng hiển thị:

  * 🆔 Mã đơn hàng
  * 💰 Tổng tiền
  * ⏰ Thời gian thanh toán

---

## 🚀 How to Use

### Trải nghiệm giao diện POS

1. Truy cập Frontend:
   👉 [https://pos-lamtran213-ui.vercel.app/](https://pos-lamtran213-ui.vercel.app/)
2. Thực hiện thêm sản phẩm vào giỏ hàng
3. Thanh toán và quan sát kết quả realtime

### Test API với Swagger

1. Truy cập Swagger UI:
   👉 [https://pos-tranviettralam-fresher.onrender.com/swagger/index.html](https://pos-tranviettralam-fresher.onrender.com/swagger/index.html)
2. Chọn API cần test
3. Gửi request và xem response trực tiếp

---

## 📌 Project Goals

* Áp dụng kiến thức **Fullstack Web Development**
* Làm quen với mô hình **POS thực tế**
* Triển khai hệ thống **Realtime**
* Chuẩn hoá kiến trúc Backend (Layered Architecture)
* Sẵn sàng mở rộng cho môi trường production

---

## 👨‍💻 Author

* **Name:** Tran Viet Tra Lam
* **Role:** Fresher .NET / Fullstack Developer
* **Email:** 📧 [lamtranmonkey@gmail.com](mailto:lamtranmonkey@gmail.com)
* **GitHub:** 🌐 [https://github.com/Lamtran213](https://github.com/Lamtran213)

---

⭐ Nếu bạn thấy dự án hữu ích, hãy để lại một **star** trên GitHub để ủng hộ nhé!
