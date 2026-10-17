using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Data.DAL
{
    /// <summary>
    /// Data Access Object cho bảng SinhVien thao tác cơ sở dữ liệu bằng ADO.NET.
    /// YÊU CẦU: mọi phương thức đều ném ngoại lệ khi có lỗi để tầng trên xử lý.
    /// </summary>
    public class SinhVienDAO
    {
        private readonly DataHelper dh = new DataHelper();

        private SinhVien MapRowToSinhVien(DataRow dr)
        {
            DateTime ngaySinh = DateTime.MinValue;
            if (dr["NgaySinh"] != DBNull.Value)
            {
                DateTime.TryParse(dr["NgaySinh"].ToString(), out ngaySinh);
            }

            double diem = 0.0;
            if (dr["Diem"] != DBNull.Value)
            {
                double.TryParse(dr["Diem"].ToString(), out diem);
            }

            return new SinhVien
            {
                MaSV = dr["MaSV"]?.ToString(),
                HoTen = dr["HoTen"]?.ToString(),
                GioiTinh = dr["GioiTinh"]?.ToString(),
                NgaySinh = ngaySinh,
                SoDienThoai = dr["SoDienThoai"] != DBNull.Value ? dr["SoDienThoai"].ToString() : string.Empty,
                Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : string.Empty,
                MaLop = dr["MaLop"]?.ToString(),
                Diem = diem
            };
        }

        /// <summary>
        /// Lấy toàn bộ danh sách sinh viên.
        /// </summary>
        public List<SinhVien> GetAll()
        {
            try
            {
                const string sql = "SELECT MaSV, MaLop, HoTen, GioiTinh, NgaySinh, Diem, SoDienThoai, Email FROM SinhVien";
                DataTable dt = dh.GetDataTable(sql);
                var list = new List<SinhVien>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapRowToSinhVien(row));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (SinhVienDAO.GetAll) thất bại: " + ex.Message, ex);
            }
        }

        public List<SinhVien> GetAllSinhVien() => GetAll();

        /// <summary>
        /// Tìm sinh viên theo mã sinh viên. Trả về null nếu không tìm thấy.
        /// </summary>
        public SinhVien GetByMaSV(string maSV)
        {
            try
            {
                const string sql = "SELECT MaSV, MaLop, HoTen, GioiTinh, NgaySinh, Diem, SoDienThoai, Email FROM SinhVien WHERE MaSV = @MaSV";
                var parameters = new[] { new SqlParameter("@MaSV", maSV ?? string.Empty) };
                DataTable dt = dh.GetDataTable(sql, parameters);
                if (dt.Rows.Count == 0) return null;
                return MapRowToSinhVien(dt.Rows[0]);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (SinhVienDAO.GetByMaSV) thất bại: " + ex.Message, ex);
            }
        }

        public SinhVien GetSinhVienByMaSV(string maSV) => GetByMaSV(maSV);

        /// <summary>
        /// Lấy danh sách sinh viên theo mã lớp (tìm kiếm phía CSDL).
        /// </summary>
        public List<SinhVien> GetByMaLop(string maLop)
        {
            try
            {
                const string sql = "SELECT MaSV, MaLop, HoTen, GioiTinh, NgaySinh, Diem, SoDienThoai, Email FROM SinhVien WHERE MaLop = @MaLop";
                var parameters = new[] { new SqlParameter("@MaLop", maLop ?? string.Empty) };
                DataTable dt = dh.GetDataTable(sql, parameters);
                var list = new List<SinhVien>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapRowToSinhVien(row));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (SinhVienDAO.GetByMaLop) thất bại: " + ex.Message, ex);
            }
        }

        public List<SinhVien> GetSinhViensByMaLop(string maLop) => GetByMaLop(maLop);

        /// <summary>
        /// Kiểm tra sinh viên có tồn tại theo mã hay không.
        /// </summary>
        public bool Exists(string maSV)
        {
            try
            {
                const string sql = "SELECT COUNT(1) FROM SinhVien WHERE MaSV = @MaSV";
                var parameters = new[] { new SqlParameter("@MaSV", maSV ?? string.Empty) };
                object result = dh.ExecuteScalar(sql, parameters);
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (SinhVienDAO.Exists) thất bại: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Đếm số lượng sinh viên thuộc một lớp học.
        /// </summary>
        public int CountByMaLop(string maLop)
        {
            try
            {
                const string sql = "SELECT COUNT(1) FROM SinhVien WHERE MaLop = @MaLop";
                var parameters = new[] { new SqlParameter("@MaLop", maLop ?? string.Empty) };
                object result = dh.ExecuteScalar(sql, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (SinhVienDAO.CountByMaLop) thất bại: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Thêm mới một sinh viên vào CSDL.
        /// </summary>
        public void Insert(SinhVien sv)
        {
            try
            {
                if (sv == null) throw new ArgumentNullException(nameof(sv));

                const string sql = @"INSERT INTO SinhVien (MaSV, MaLop, HoTen, GioiTinh, NgaySinh, Diem, SoDienThoai, Email)
                                     VALUES (@MaSV, @MaLop, @HoTen, @GioiTinh, @NgaySinh, @Diem, @SoDienThoai, @Email)";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaSV", sv.MaSV ?? string.Empty),
                    new SqlParameter("@MaLop", sv.MaLop ?? string.Empty),
                    new SqlParameter("@HoTen", sv.HoTen ?? string.Empty),
                    new SqlParameter("@GioiTinh", (object)sv.GioiTinh ?? DBNull.Value),
                    new SqlParameter("@NgaySinh", sv.NgaySinh != DateTime.MinValue ? (object)sv.NgaySinh : DBNull.Value),
                    new SqlParameter("@Diem", sv.Diem),
                    new SqlParameter("@SoDienThoai", string.IsNullOrEmpty(sv.SoDienThoai) ? DBNull.Value : (object)sv.SoDienThoai),
                    new SqlParameter("@Email", string.IsNullOrEmpty(sv.Email) ? DBNull.Value : (object)sv.Email)
                };
                dh.ExecuteNonQuery(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (SinhVienDAO.Insert) thất bại: " + ex.Message, ex);
            }
        }

        public void AddSinhVien(SinhVien sv) => Insert(sv);

        /// <summary>
        /// Cập nhật thông tin sinh viên theo mã sinh viên.
        /// </summary>
        public void Update(SinhVien sv)
        {
            try
            {
                if (sv == null) throw new ArgumentNullException(nameof(sv));

                const string sql = @"UPDATE SinhVien
                                     SET HoTen = @HoTen,
                                         MaLop = @MaLop,
                                         GioiTinh = @GioiTinh,
                                         NgaySinh = @NgaySinh,
                                         Diem = @Diem,
                                         SoDienThoai = @SoDienThoai,
                                         Email = @Email
                                     WHERE MaSV = @MaSV";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaSV", sv.MaSV ?? string.Empty),
                    new SqlParameter("@MaLop", sv.MaLop ?? string.Empty),
                    new SqlParameter("@HoTen", sv.HoTen ?? string.Empty),
                    new SqlParameter("@GioiTinh", (object)sv.GioiTinh ?? DBNull.Value),
                    new SqlParameter("@NgaySinh", sv.NgaySinh != DateTime.MinValue ? (object)sv.NgaySinh : DBNull.Value),
                    new SqlParameter("@Diem", sv.Diem),
                    new SqlParameter("@SoDienThoai", string.IsNullOrEmpty(sv.SoDienThoai) ? DBNull.Value : (object)sv.SoDienThoai),
                    new SqlParameter("@Email", string.IsNullOrEmpty(sv.Email) ? DBNull.Value : (object)sv.Email)
                };
                int affected = dh.ExecuteNonQuery(sql, parameters);
                if (affected == 0)
                {
                    throw new Exception("Không tìm thấy sinh viên có mã '" + sv.MaSV + "' để cập nhật.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (SinhVienDAO.Update) thất bại: " + ex.Message, ex);
            }
        }

        public void UpdateSinhVien(SinhVien sv) => Update(sv);

        /// <summary>
        /// Xóa sinh viên theo mã sinh viên.
        /// </summary>
        public void Delete(string maSV)
        {
            try
            {
                const string sql = "DELETE FROM SinhVien WHERE MaSV = @MaSV";
                var parameters = new[] { new SqlParameter("@MaSV", maSV ?? string.Empty) };
                int affected = dh.ExecuteNonQuery(sql, parameters);
                if (affected == 0)
                {
                    throw new Exception("Không tìm thấy sinh viên có mã '" + maSV + "' để xóa.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (SinhVienDAO.Delete) thất bại: " + ex.Message, ex);
            }
        }

        public void DeleteSinhVien(string maSV) => Delete(maSV);
    }
}
