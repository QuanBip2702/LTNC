using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Data.DAL
{
    /// <summary>
    /// Data Access Object cho bảng LopHoc thao tác cơ sở dữ liệu bằng ADO.NET.
    /// YÊU CẦU: mọi phương thức đều ném ngoại lệ khi có lỗi để tầng trên xử lý.
    /// </summary>
    public class LopHocDAO
    {
        private readonly DataHelper dh = new DataHelper();

        /// <summary>
        /// Lấy toàn bộ danh sách lớp học.
        /// </summary>
        public List<LopHoc> GetAll()
        {
            try
            {
                const string sql = "SELECT MaLop, TenLop FROM LopHoc";
                DataTable dt = dh.GetDataTable(sql);
                var list = new List<LopHoc>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new LopHoc
                    {
                        MaLop = row["MaLop"]?.ToString(),
                        TenLop = row["TenLop"]?.ToString()
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (LopHocDAO.GetAll) thất bại: " + ex.Message, ex);
            }
        }

        public List<LopHoc> GetAllLopHoc() => GetAll();

        /// <summary>
        /// Lấy thông tin lớp học theo mã lớp.
        /// </summary>
        public LopHoc GetByMaLop(string maLop)
        {
            try
            {
                const string sql = "SELECT MaLop, TenLop FROM LopHoc WHERE MaLop = @MaLop";
                var parameters = new[] { new SqlParameter("@MaLop", maLop ?? string.Empty) };
                DataTable dt = dh.GetDataTable(sql, parameters);
                if (dt.Rows.Count == 0) return null;

                DataRow row = dt.Rows[0];
                return new LopHoc
                {
                    MaLop = row["MaLop"]?.ToString(),
                    TenLop = row["TenLop"]?.ToString()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (LopHocDAO.GetByMaLop) thất bại: " + ex.Message, ex);
            }
        }

        public LopHoc GetLopHocByMaLop(string maLop) => GetByMaLop(maLop);

        /// <summary>
        /// Kiểm tra mã lớp đã tồn tại hay chưa.
        /// </summary>
        public bool Exists(string maLop)
        {
            try
            {
                const string sql = "SELECT COUNT(1) FROM LopHoc WHERE MaLop = @MaLop";
                var parameters = new[] { new SqlParameter("@MaLop", maLop ?? string.Empty) };
                object result = dh.ExecuteScalar(sql, parameters);
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (LopHocDAO.Exists) thất bại: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Thêm mới một lớp học.
        /// </summary>
        public void Insert(LopHoc lop)
        {
            try
            {
                if (lop == null) throw new ArgumentNullException(nameof(lop));

                const string sql = "INSERT INTO LopHoc (MaLop, TenLop) VALUES (@MaLop, @TenLop)";
                var parameters = new[]
                {
                    new SqlParameter("@MaLop", lop.MaLop ?? string.Empty),
                    new SqlParameter("@TenLop", lop.TenLop ?? string.Empty)
                };
                dh.ExecuteNonQuery(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (LopHocDAO.Insert) thất bại: " + ex.Message, ex);
            }
        }

        public void AddLopHoc(LopHoc lop) => Insert(lop);

        /// <summary>
        /// Cập nhật thông tin lớp học.
        /// </summary>
        public void Update(LopHoc lop)
        {
            try
            {
                if (lop == null) throw new ArgumentNullException(nameof(lop));

                const string sql = "UPDATE LopHoc SET TenLop = @TenLop WHERE MaLop = @MaLop";
                var parameters = new[]
                {
                    new SqlParameter("@MaLop", lop.MaLop ?? string.Empty),
                    new SqlParameter("@TenLop", lop.TenLop ?? string.Empty)
                };
                int affected = dh.ExecuteNonQuery(sql, parameters);
                if (affected == 0)
                {
                    throw new Exception("Không tìm thấy lớp học có mã '" + lop.MaLop + "' để cập nhật.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (LopHocDAO.Update) thất bại: " + ex.Message, ex);
            }
        }

        public void UpdateLopHoc(LopHoc lop) => Update(lop);

        /// <summary>
        /// Xóa lớp học theo mã lớp.
        /// </summary>
        public void Delete(string maLop)
        {
            try
            {
                const string sql = "DELETE FROM LopHoc WHERE MaLop = @MaLop";
                var parameters = new[] { new SqlParameter("@MaLop", maLop ?? string.Empty) };
                int affected = dh.ExecuteNonQuery(sql, parameters);
                if (affected == 0)
                {
                    throw new Exception("Không tìm thấy lớp học có mã '" + maLop + "' để xóa.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Tầng dữ liệu (LopHocDAO.Delete) thất bại: " + ex.Message, ex);
            }
        }

        public void DeleteLopHoc(string maLop) => Delete(maLop);
    }
}
