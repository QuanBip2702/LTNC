using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLySinhVien.Data.DAL
{
    public class DataHelper
    {
        private static string defaultConnectionString = @"Server=LAPTOP-6SN65NCS\HUECTMDB;Database=QLSV;Integrated Security=True;TrustServerCertificate=True";

        public static string ConnectionString
        {
            get => defaultConnectionString;
            set => defaultConnectionString = value;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // Thực thi Insert, Update, Delete với Parameter
        public int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (parameters != null && parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        // Đọc dữ liệu ra DataTable
        public DataTable GetDataTable(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (parameters != null && parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Lỗi đọc dữ liệu: " + ex.Message, ex);
            }
        }

        // Thực thi trả về giá trị đơn (SELECT COUNT, MAX, ...)
        public object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (parameters != null && parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Lỗi thực thi truy vấn scalar: " + ex.Message, ex);
            }
        }
    }
}
