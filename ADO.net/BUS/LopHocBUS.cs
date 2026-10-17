using System;
using System.Collections.Generic;
using QuanLySinhVien.Data.DAL;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Business
{
    /// <summary>
    /// Tầng nghiệp vụ của Lớp học: kiểm tra dữ liệu, xử lý quy tắc nghiệp vụ rồi gọi xuống tầng dữ liệu.
    /// YÊU CẦU: mọi phương thức đều ném ngoại lệ ra ngoài để tầng View bắt và hiển thị.
    /// </summary>
    public class LopHocBUS
    {
        private readonly LopHocDAO lopHocDAO = new LopHocDAO();
        private readonly SinhVienDAO sinhVienDAO = new SinhVienDAO();

        /// <summary>Lấy danh sách lớp học.</summary>
        public List<LopHoc> GetAllLopHoc()
        {
            try
            {
                return lopHocDAO.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: lấy danh sách lớp học thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Tìm lớp học theo mã. Trả về null nếu không tồn tại.</summary>
        public LopHoc GetLopHocByMaLop(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new ArgumentException("Mã lớp không được để trống.", nameof(maLop));

            try
            {
                return lopHocDAO.GetByMaLop(maLop);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: tìm lớp học thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Thêm lớp học mới.</summary>
        public void AddLopHoc(LopHoc lop)
        {
            KiemTraDuLieu(lop);

            try
            {
                // Quy tắc nghiệp vụ: mã lớp không được trùng
                if (lopHocDAO.Exists(lop.MaLop))
                    throw new Exception("Mã lớp '" + lop.MaLop + "' đã tồn tại trong hệ thống.");

                lopHocDAO.Insert(lop);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: thêm lớp học thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Cập nhật thông tin lớp học (MaLop là khóa chính nên không đổi).</summary>
        public void UpdateLopHoc(LopHoc lop)
        {
            KiemTraDuLieu(lop);

            try
            {
                if (!lopHocDAO.Exists(lop.MaLop))
                    throw new Exception("Không tìm thấy lớp '" + lop.MaLop + "' để cập nhật.");

                lopHocDAO.Update(lop);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: cập nhật lớp học thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Xóa lớp học. Không cho xóa nếu lớp còn sinh viên.</summary>
        public void DeleteLopHoc(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new ArgumentException("Mã lớp không được để trống.", nameof(maLop));

            try
            {
                if (!lopHocDAO.Exists(maLop))
                    throw new Exception("Không tìm thấy lớp '" + maLop + "' để xóa.");

                // Quy tắc nghiệp vụ: không cho xóa lớp còn sinh viên
                int soSinhVien = sinhVienDAO.CountByMaLop(maLop);
                if (soSinhVien > 0)
                    throw new Exception("Không thể xóa lớp '" + maLop + "' vì lớp đang có " + soSinhVien + " sinh viên.");

                lopHocDAO.Delete(maLop);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng nghiệp vụ: xóa lớp học thất bại. " + ex.Message, ex);
            }
        }

        /// <summary>Kiểm tra dữ liệu đầu vào. Ném ArgumentException nếu dữ liệu sai.</summary>
        private void KiemTraDuLieu(LopHoc lop)
        {
            if (lop == null)
                throw new ArgumentException("Dữ liệu lớp học không được để trống.");

            if (string.IsNullOrWhiteSpace(lop.MaLop))
                throw new ArgumentException("Mã lớp không được để trống.");

            if (lop.MaLop.Trim().Length > 10)
                throw new ArgumentException("Mã lớp tối đa 10 ký tự.");

            if (string.IsNullOrWhiteSpace(lop.TenLop))
                throw new ArgumentException("Tên lớp không được để trống.");
        }
    }
}
