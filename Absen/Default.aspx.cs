using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Security.Policy;


namespace Absen
{
    public partial class _Default : Page
    {
        SqlConnection con = new SqlConnection("Data Source=NATANAELS\\SQLEXPRESS01;Initial Catalog=tesklg;Integrated Security=True;TrustServerCertificate=True");
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
             {
                fillDropDown();
                printGridAbsenEmployee();
                printGridAbsenList();
                printGridAbsenBulanan();
            }
        }

        public DataTable getEmployee()
        {
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT *, (Nama + ' - ' + NIK) AS NamaNik FROM employee", con);
                cmd.CommandType = CommandType.Text;
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            } catch (Exception ex)
            {
                throw new Exception("Error fetching employee data: " + ex.Message);
            } finally
            {
                con.Close();
            }
           
            return dt;
        }

        public DataTable getAbsenEmployee()
        {
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT e.NIK, e.Nama, a.Tanggal_Absen as [Tanggal Absen]  FROM employee AS e JOIN absen AS a ON e.NIK = a.NIK", con);
                cmd.CommandType = CommandType.Text;
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            } catch (Exception ex)
            {
                throw new Exception("Error fetching employee's attendance data: " + ex.Message);
            } finally
            {
                con.Close();
            }

            return dt;
        }

        public DataTable getAbsenList()
        {
            SqlCommand dateCmd = new SqlCommand("SELECT DISTINCT Tanggal_Absen FROM absen ORDER BY Tanggal_Absen ASC", con);
            SqlDataAdapter dateDa = new SqlDataAdapter(dateCmd);
            DataTable dateTable = new DataTable();
            dateDa.Fill(dateTable);

            string dateColumns = string.Join(", ", dateTable.AsEnumerable()
                                    .Select(row => $"MAX(CASE WHEN CONVERT(VARCHAR(10), a.Tanggal_Absen, 105) = '{Convert.ToDateTime(row["Tanggal_Absen"]).ToString("dd-MM-yyyy")}' THEN 'X' ELSE '' END) AS [{Convert.ToDateTime(row["Tanggal_Absen"]).ToString("dd-MM-yyyy")}]"));

            string query = $@"
                SELECT e.NIK, e.Nama, 
                       {dateColumns},
                       COUNT(a.Tanggal_Absen) AS Total
                FROM employee AS e
                LEFT JOIN absen AS a ON e.NIK = a.NIK
                GROUP BY e.NIK, e.Nama;
            ";

            try
            {
                con.Open();
                cmd = new SqlCommand(query, con);
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            } catch (Exception ex)
            {
                throw new Exception("Error fetching attendace list data: " + ex.Message);
            } finally
            {
                con.Close();
            }

            return dt;
        }

        public DataTable getAbsenBulanan()
        {
            SqlCommand dateCmd = new SqlCommand("SELECT DISTINCT CONVERT(VARCHAR(6), Tanggal_Absen, 112) AS Tanggal_Absen FROM absen", con);
            SqlDataAdapter dateDa = new SqlDataAdapter(dateCmd);
            DataTable dateTable = new DataTable();
            dateDa.Fill(dateTable);

            string dateColumns = string.Join(", ", dateTable.AsEnumerable()
                                    .Select(row => $"COUNT(CASE WHEN CONVERT(VARCHAR(6), a.Tanggal_Absen, 112) = '{row["Tanggal_Absen"]}' THEN 1 ELSE NULL END) AS [{row["Tanggal_Absen"]}]"));

            string query = $@"
                SELECT e.NIK, e.Nama, 
                       {dateColumns},
                       COUNT(a.Tanggal_Absen) AS Total
                FROM employee AS e
                LEFT JOIN absen AS a ON e.NIK = a.NIK
                GROUP BY e.NIK, e.Nama;
            ";

            try
            {
                con.Open();
                cmd = new SqlCommand(query, con);
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching employee monthly attendance data: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        private void fillDropDown()
        {
            DropDownNikNama.DataSource = getEmployee();
            DropDownNikNama.DataTextField = "NamaNik";
            DropDownNikNama.DataValueField = "NIK";
            DropDownNikNama.DataBind();
        }

        private void printGridAbsenEmployee()
        {
            GridView1.DataSource = getAbsenEmployee();
            GridView1.DataBind();
        }

        private void printGridAbsenList()
        {
            GridView2.DataSource = getAbsenList();
            GridView2.DataBind();
        }

        private void printGridAbsenBulanan()
        {
            GridView3.DataSource = getAbsenBulanan();
            GridView3.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var tanggalAbsen = DataBinder.Eval(e.Row.DataItem, "Tanggal Absen");

                if (tanggalAbsen != DBNull.Value)
                {
                    DateTime date = Convert.ToDateTime(tanggalAbsen);
                    e.Row.Cells[2].Text = date.ToString("dd/MM/yyyy"); 
                }
            }
        }

        protected void saveButton_Click(object sender, EventArgs e)
        {
            string nik = DropDownNikNama.SelectedValue;
            string tanggalText = datepicker.Text;

            try
            {
                DateTime tanggalAbsen = DateTime.ParseExact(tanggalText, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (isDuplicateAttendance(nik, tanggalAbsen))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script",
                        "alert('Karyawan sudah absen di tanggal tersebut!'); window.location.href = '/';", true);
                    return;
                }

                saveData(nik, tanggalAbsen);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Data berhasil ditambahkan'); window.location.href = '/';", true);
            }
            catch (FormatException ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Format tanggal tidak valid. Harap masukkan tanggal dengan format dd/MM/yyyy.'); window.location.href = '/';", true);
            }
        }

        public void saveData(string nik, DateTime tanggalAbsen)
        {
            string query = "INSERT INTO absen (NIK, Tanggal_Absen) VALUES (@nik, @tanggalAbsen)";
            try
            {
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nik", nik);
                cmd.Parameters.AddWithValue("@tanggalAbsen", tanggalAbsen);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saat eksekusi query: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private bool isDuplicateAttendance(string nik, DateTime tanggalAbsen)
        {
            try
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM absen WHERE NIK = @nik AND CONVERT(date, Tanggal_Absen) = CONVERT(date, @tanggalAbsen)";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nik", nik);
                cmd.Parameters.AddWithValue("@tanggalAbsen", tanggalAbsen);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            finally
            {
                con.Close();
            }
        }
    }
}