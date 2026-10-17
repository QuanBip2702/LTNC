namespace QuanLySinhVien.Data.Entity
{
    public class LopHoc
    {
        public string MaLop { get; set; }
        public string TenLop { get; set; }

        public LopHoc() { }

        public LopHoc(string maLop, string tenLop)
        {
            MaLop = maLop;
            TenLop = tenLop;
        }

        public override string ToString()
        {
            return $"{TenLop} ({MaLop})";
        }
    }
}
