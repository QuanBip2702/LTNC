using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Data.Entity
{
    public class SinhVien
    {
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        public string MaSV { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string HoTen { get; set; }

        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }

        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        public string Email { get; set; }

        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 số và bắt đầu bằng số 0")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn mã lớp")]
        public string MaLop { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm phải nằm trong thang điểm từ 0 đến 10")]
        public double Diem { get; set; }

        // Phương thức kiểm tra tính hợp lệ dữ liệu bằng DataAnnotations
        public List<ValidationResult> IsInValid()
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this, null, null);
            Validator.TryValidateObject(this, context, results, true);
            return results;
        }
    }
}
