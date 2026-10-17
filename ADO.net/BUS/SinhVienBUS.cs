using System;
using System.Collections.Generic;
using QuanLySinhVien.Data.DAL;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Business
{
    /// <summary>
    /// Tầng nghiệp vụ của Sinh viên: kiểm tra dữ liệu, xử lý quy tắc nghiệp vụ rồi gọi xuống tầng dữ liệu.
    /// YÊU CẦU: mọi phương thức đều ném ngoại lệ ra ngoài để tầng View bắt và hiển thị.
    /// </summary>
    public class SinhVienBUS
    {
        private readonly SinhVienDAO sinhVienDAO = new SinhVienDAO();
        private readonly LopHocDAO lopHocDAO = new LopHocDAO();

        /// <summary>Lấy toàn bộ danh sách sinh viên.</summary>
        public List<SinhVien> GetAllSinhVien()
        {
            try
            {
                return sinhVienDAO.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: lấy danh sách sinh viên thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Tìm sinh viên theo mã. Trả về null nếu không tồn tại.</summary>
        public SinhVien GetSinhVienByMaSV(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                throw new ArgumentException("Mã sinh viên không được để trống.", nameof(maSV));

            try
            {
                return sinhVienDAO.GetByMaSV(maSV);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: tìm sinh viên thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Lấy danh sách sinh viên theo lớp (tìm kiếm phía CSDL).</summary>
        public List<SinhVien> GetSinhVienByMaLop(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new ArgumentException("Mã lớp không được để trống.", nameof(maLop));

            try
            {
                return sinhVienDAO.GetByMaLop(maLop);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: lấy sinh viên theo lớp thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Thêm sinh viên mới (có kiểm tra dữ liệu + quy tắc nghiệp vụ).</summary>
        public void AddSinhVien(SinhVien sv)
        {
            KiemTraDuLieu(sv);

            try
            {
                // Quy tắc nghiệp vụ 1: mã sinh viên không được trùng
                if (sinhVienDAO.Exists(sv.MaSV))
                    throw new Exception("Mã sinh viên '" + sv.MaSV + "' đã tồn tại trong hệ thống.");

                // Quy tắc nghiệp vụ 2: lớp học phải tồn tại
                if (!lopHocDAO.Exists(sv.MaLop))
                    throw new Exception("Mã lớp '" + sv.MaLop + "' không tồn tại trong hệ thống.");

                sinhVienDAO.Insert(sv);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: thêm sinh viên thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Cập nhật sinh viên (có kiểm tra dữ liệu + quy tắc nghiệp vụ).</summary>
        public void UpdateSinhVien(SinhVien sv)
        {
            KiemTraDuLieu(sv);

            try
            {
                if (!sinhVienDAO.Exists(sv.MaSV))
                    throw new Exception("Không tìm thấy sinh viên '" + sv.MaSV + "' để cập nhật.");

                if (!lopHocDAO.Exists(sv.MaLop))
                    throw new Exception("Mã lớp '" + sv.MaLop + "' không tồn tại trong hệ thống.");

                sinhVienDAO.Update(sv);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: cập nhật sinh viên thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Xóa sinh viên theo mã.</summary>
        public void DeleteSinhVien(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                throw new ArgumentException("Mã sinh viên không được để trống.", nameof(maSV));

            try
            {
                if (!sinhVienDAO.Exists(maSV))
                    throw new Exception("Không tìm thấy sinh viên '" + maSV + "' để xóa.");

                sinhVienDAO.Delete(maSV);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: xóa sinh viên thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Kiểm tra dữ liệu đầu vào trước khi thao tác. Ném ArgumentException nếu dữ liệu sai.</summary>
        private void KiemTraDuLieu(SinhVien sv)
        {
            if (sv == null)
                throw new ArgumentException("Dữ liệu sinh viên không được để trống.");

            if (string.IsNullOrWhiteSpace(sv.MaSV))
                throw new ArgumentException("Mã sinh viên không được để trống.");

            if (sv.MaSV.Trim().Length > 10)
                throw new ArgumentException("Mã sinh viên tối đa 10 ký tự.");

            if (string.IsNullOrWhiteSpace(sv.HoTen))
                throw new ArgumentException("Họ tên sinh viên không được để trống.");

            if (string.IsNullOrWhiteSpace(sv.MaLop))
                throw new ArgumentException("Mã lớp không được để trống.");

            if (sv.Diem < 0 || sv.Diem > 10)
                throw new ArgumentException("Điểm phải nằm trong khoảng từ 0 đến 10.");

            if (!string.IsNullOrWhiteSpace(sv.Email) && !sv.Email.Contains("@"))
                throw new ArgumentException("Email không đúng định dạng.");

            if (sv.NgaySinh != DateTime.MinValue && sv.NgaySinh > DateTime.Now)
            {
                throw new ArgumentException("Ngày sinh không được lớn hơn ngày hiện tại.");
            }
        }

        #region Hỗ trợ tìm kiếm trên giao diện (In-memory Filtering)
        /// <summary>
        /// Lọc danh sách sinh viên theo lớp trực tiếp trên giao diện (không truy vấn lại CSDL).
        /// </summary>
        public List<SinhVien> FilterSinhVienByMaLop(List<SinhVien> list, string maLop)
        {
            if (list == null) return new List<SinhVien>();
            if (string.IsNullOrWhiteSpace(maLop)) return list;
            return list.FindAll(sv => string.Equals(sv.MaLop, maLop, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tìm kiếm sinh viên theo từ khóa (Mã SV hoặc Họ tên) trên giao diện.
        /// </summary>
        public List<SinhVien> SearchSinhVien(List<SinhVien> list, string keyword)
        {
            if (list == null) return new List<SinhVien>();
            if (string.IsNullOrWhiteSpace(keyword)) return list;
            keyword = keyword.Trim().ToLower();
            return list.FindAll(sv => 
                (sv.MaSV != null && sv.MaSV.ToLower().Contains(keyword)) ||
                (sv.HoTen != null && sv.HoTen.ToLower().Contains(keyword)));
        }
        #endregion
    }
}
