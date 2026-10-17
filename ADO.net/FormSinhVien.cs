using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;
using QuanLySinhVien.Business;
using QuanLySinhVien.Data.Entity;

namespace QuanLySinhVien.Presentation
{
    /// <summary>
    /// Tầng Presentation: Form Quản lý Sinh viên
    /// Chỉ giữ lại logic xử lý giao diện, gọi tầng BUS và bắt/xử lý ngoại lệ từ BUS/DAO.
    /// </summary>
    public class FormSinhVien : Form
    {
        // Khởi tạo các đối tượng tầng Business
        private readonly LopHocBUS lopBUS = new LopHocBUS();
        private readonly SinhVienBUS svBUS = new SinhVienBUS();

        // Danh sách sinh viên lưu trên bộ nhớ dùng cho tìm kiếm/lọc trên giao diện
        private List<SinhVien> danhSachTatCaSinhVien = new List<SinhVien>();

        // Các điều khiển giao diện (Windows Forms Controls)
        private TextBox txtMaSV;
        private TextBox txtHoTen;
        private DateTimePicker dtpNgaySinh;
        private RadioButton rdoNam;
        private RadioButton rdoNu;
        private TextBox txtEmail;
        private TextBox txtDienThoai;
        private ComboBox cboLop;
        private NumericUpDown numDiem;
        private DataGridView dgvSinhVien;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private RadioButton rdoTimKiemTrenGiaoDien;
        private RadioButton rdoTimKiemCSDL;
        private ErrorProvider errProvider;

        public FormSinhVien()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtMaSV = new TextBox();
            this.txtHoTen = new TextBox();
            this.dtpNgaySinh = new DateTimePicker();
            this.rdoNam = new RadioButton();
            this.rdoNu = new RadioButton();
            this.txtEmail = new TextBox();
            this.txtDienThoai = new TextBox();
            this.cboLop = new ComboBox();
            this.numDiem = new NumericUpDown();
            this.dgvSinhVien = new DataGridView();
            this.btnThem = new Button();
            this.btnSua = new Button();
            this.btnXoa = new Button();
            this.btnLamMoi = new Button();
            this.rdoTimKiemTrenGiaoDien = new RadioButton();
            this.rdoTimKiemCSDL = new RadioButton();
            this.errProvider = new ErrorProvider();

            // Đăng ký các sự kiện chính
            this.Load += new EventHandler(FormSinhVien_Load);
            this.cboLop.SelectedIndexChanged += new EventHandler(cboLop_SelectedIndexChanged);
            this.txtMaSV.Leave += new EventHandler(txtMaSV_Leave);
            this.btnThem.Click += new EventHandler(btnThem_Click);
            this.btnSua.Click += new EventHandler(btnSua_Click);
            this.btnXoa.Click += new EventHandler(btnXoa_Click);
            this.btnLamMoi.Click += new EventHandler(btnLamMoi_Click);
        }

        #region 1. Tải dữ liệu ban đầu (Form_Load)
        private void FormSinhVien_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDanhSachLop();
                LoadDanhSachSinhVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu ban đầu: " + ex.Message, "Lỗi hệ thống",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDanhSachLop()
        {
            try
            {
                var dsLop = lopBUS.GetAllLopHoc();
                cboLop.DataSource = dsLop;
                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";
                if (dsLop.Count > 0)
                {
                    cboLop.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể nạp danh sách lớp: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDanhSachSinhVien()
        {
            try
            {
                danhSachTatCaSinhVien = svBUS.GetAllSinhVien();
                dgvSinhVien.DataSource = null;
                dgvSinhVien.DataSource = danhSachTatCaSinhVien;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể nạp danh sách sinh viên: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 2. Chọn lớp trên ComboBox -> Hiển thị sinh viên của lớp (Tìm kiếm trên giao diện & CSDL)
        private void cboLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLop.SelectedValue == null) return;
            string maLopChon = cboLop.SelectedValue.ToString();

            try
            {
                if (rdoTimKiemCSDL.Checked)
                {
                    // Cách 1: Tìm kiếm phía cơ sở dữ liệu (truy vấn SQL qua tầng BUS/DAO)
                    List<SinhVien> dsTheoLop = svBUS.GetSinhVienByMaLop(maLopChon);
                    dgvSinhVien.DataSource = null;
                    dgvSinhVien.DataSource = dsTheoLop;
                }
                else
                {
                    // Cách 2: Tìm kiếm trên giao diện (lọc trên danh sách bộ nhớ)
                    List<SinhVien> dsLoc = svBUS.FilterSinhVienByMaLop(danhSachTatCaSinhVien, maLopChon);
                    dgvSinhVien.DataSource = null;
                    dgvSinhVien.DataSource = dsLoc;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lọc sinh viên theo lớp: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 3. Nhập mã SV vào txtMaSV -> Nếu tồn tại thì điền, không tồn tại thì xóa trống
        private void txtMaSV_Leave(object sender, EventArgs e)
        {
            string maSV = txtMaSV.Text.Trim();
            if (string.IsNullOrEmpty(maSV))
            {
                XoaTrangForm(giuLaiMaSV: false);
                return;
            }

            try
            {
                SinhVien sv = svBUS.GetSinhVienByMaSV(maSV);
                if (sv != null)
                {
                    // Tồn tại: hiển thị thông tin lên các điều khiển
                    txtHoTen.Text = sv.HoTen;
                    dtpNgaySinh.Value = sv.NgaySinh != DateTime.MinValue ? sv.NgaySinh : DateTime.Today;
                    if (string.Equals(sv.GioiTinh, "Nam", StringComparison.OrdinalIgnoreCase))
                        rdoNam.Checked = true;
                    else
                        rdoNu.Checked = true;

                    txtEmail.Text = sv.Email;
                    txtDienThoai.Text = sv.SoDienThoai;
                    if (!string.IsNullOrEmpty(sv.MaLop))
                    {
                        cboLop.SelectedValue = sv.MaLop;
                    }
                    numDiem.Value = (decimal)sv.Diem;
                }
                else
                {
                    // Không tồn tại: xóa trống các ô điều khiển nhập liệu còn lại
                    XoaTrangForm(giuLaiMaSV: true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tra cứu sinh viên: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 4. Thêm sinh viên (Kiểm tra dữ liệu + ErrorProvider + Xử lý ngoại lệ)
        private void btnThem_Click(object sender, EventArgs e)
        {
            // Xóa sạch các thông báo lỗi cũ
            XoaLoiErrorProvider();

            // Tạo entity từ giao diện
            SinhVien sv = LayDuLieuTuGiaoDien();

            // Kiểm tra hợp lệ bằng DataAnnotations
            List<ValidationResult> errors = sv.IsInValid();
            if (errors.Count > 0)
            {
                HienThiLoiErrorProvider(errors);
                return;
            }

            try
            {
                // Gọi tầng Business để thêm sinh viên
                svBUS.AddSinhVien(sv);
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại dữ liệu lên giao diện
                LoadDanhSachSinhVien();
                XoaTrangForm(giuLaiMaSV: false);
            }
            catch (ArgumentException argEx)
            {
                // Ngoại lệ do vi phạm nghiệp vụ đầu vào
                MessageBox.Show(argEx.Message, "Lỗi dữ liệu",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Ngoại lệ từ tầng dữ liệu / cơ sở dữ liệu
                MessageBox.Show(ex.Message, "Lỗi thực thi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 5. Sửa sinh viên (Kiểm tra dữ liệu + ErrorProvider + Xử lý ngoại lệ)
        private void btnSua_Click(object sender, EventArgs e)
        {
            XoaLoiErrorProvider();

            SinhVien sv = LayDuLieuTuGiaoDien();
            List<ValidationResult> errors = sv.IsInValid();
            if (errors.Count > 0)
            {
                HienThiLoiErrorProvider(errors);
                return;
            }

            try
            {
                svBUS.UpdateSinhVien(sv);
                MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDanhSachSinhVien();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Lỗi dữ liệu",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi thực thi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 6. Xóa sinh viên
        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maSV = txtMaSV.Text.Trim();
            if (string.IsNullOrEmpty(maSV))
            {
                errProvider.SetError(txtMaSV, "Vui lòng nhập mã sinh viên cần xóa.");
                txtMaSV.Focus();
                return;
            }

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên {maSV} không?",
                                                  "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                svBUS.DeleteSinhVien(maSV);
                MessageBox.Show("Xóa sinh viên thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDanhSachSinhVien();
                XoaTrangForm(giuLaiMaSV: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi khi xóa",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 7. Các hàm tiện ích giao diện
        private SinhVien LayDuLieuTuGiaoDien()
        {
            return new SinhVien
            {
                MaSV = txtMaSV.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value,
                GioiTinh = rdoNam.Checked ? "Nam" : "Nữ",
                Email = txtEmail.Text.Trim(),
                SoDienThoai = txtDienThoai.Text.Trim(),
                MaLop = cboLop.SelectedValue != null ? cboLop.SelectedValue.ToString() : string.Empty,
                Diem = (double)numDiem.Value
            };
        }

        private void HienThiLoiErrorProvider(List<ValidationResult> errors)
        {
            foreach (var error in errors)
            {
                if (error.MemberNames != null && error.MemberNames.Any())
                {
                    string fieldName = error.MemberNames.First();
                    switch (fieldName)
                    {
                        case "MaSV":
                            errProvider.SetError(txtMaSV, error.ErrorMessage);
                            txtMaSV.Focus();
                            break;
                        case "HoTen":
                            errProvider.SetError(txtHoTen, error.ErrorMessage);
                            txtHoTen.Focus();
                            break;
                        case "Email":
                            errProvider.SetError(txtEmail, error.ErrorMessage);
                            txtEmail.Focus();
                            break;
                        case "SoDienThoai":
                            errProvider.SetError(txtDienThoai, error.ErrorMessage);
                            txtDienThoai.Focus();
                            break;
                        case "MaLop":
                            errProvider.SetError(cboLop, error.ErrorMessage);
                            cboLop.Focus();
                            break;
                        case "Diem":
                            errProvider.SetError(numDiem, error.ErrorMessage);
                            numDiem.Focus();
                            break;
                        default:
                            MessageBox.Show(error.ErrorMessage, "Lỗi dữ liệu",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
                else
                {
                    MessageBox.Show(error.ErrorMessage, "Lỗi dữ liệu",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void XoaLoiErrorProvider()
        {
            errProvider.Clear();
        }

        private void XoaTrangForm(bool giuLaiMaSV)
        {
            if (!giuLaiMaSV) txtMaSV.Clear();
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Today;
            rdoNam.Checked = true;
            txtEmail.Clear();
            txtDienThoai.Clear();
            numDiem.Value = 0;
            XoaLoiErrorProvider();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            XoaTrangForm(giuLaiMaSV: false);
            LoadDanhSachSinhVien();
        }
        #endregion
    }
}
