using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZadatakA16_IzlozbaPasa
{
    public partial class Form1 : Form
    {
        SqlConnection konekcija = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Skola\MATURA\Programiranje\ZadatakA16-IzlozbaPasa\ZadatakA16-IzlozbaPasa\A16.mdf;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }
        
        public void PopuniCBPas()
        {
            string sqlUpit = "Select PasID, concat(PasID, ' - ', Ime) as PasImeID from Pas order by PasID asc";
            SqlCommand komanda = new SqlCommand(sqlUpit,konekcija);
            SqlDataAdapter adapter = new SqlDataAdapter(komanda);
            DataTable dt = new DataTable();

            try
            {
                adapter.Fill(dt);

                comboBoxPas.DataSource = dt;
                comboBoxPas.ValueMember= "PasID";
                comboBoxPas.DisplayMember = "PasImeID";
                comboBoxPas.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void PopuniCBIzlozbaF1()
        {
            string sqlUpit = "Select IzlozbaID, concat(IzlozbaID, ' - ', Mesto, ' - ', CONVERT(VARCHAR(10),Datum,105)) as Sifra from Izlozba order by IzlozbaID asc";
            SqlCommand komanda = new SqlCommand(sqlUpit, konekcija);
            SqlDataAdapter adapter = new SqlDataAdapter(komanda);
            DataTable dt = new DataTable();

            try
            {
                adapter.Fill(dt);

                comboBoxIzlozbaF1.DataSource = dt;
                comboBoxIzlozbaF1.ValueMember = "IzlozbaID";
                comboBoxIzlozbaF1.DisplayMember = "Sifra";
                comboBoxIzlozbaF1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PopuniCBKategorija()
        {
            string sqlUpit = "Select KategorijaID, concat(KategorijaID, ' - ', Naziv) as KatNaz from Kategorija order by KategorijaID asc";
            SqlCommand komanda = new SqlCommand(sqlUpit, konekcija);
            SqlDataAdapter adapter = new SqlDataAdapter(komanda);
            DataTable dt = new DataTable();

            try
            {
                adapter.Fill(dt);

                comboBoxKategorija.DataSource = dt;
                comboBoxKategorija.ValueMember = "KategorijaID";
                comboBoxKategorija.DisplayMember = "KatNaz";
                comboBoxKategorija.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PopuniCBIzlozbaF2()
        {
            string sqlUpit = "Select IzlozbaID, concat(IzlozbaID, ' - ', Mesto, ' - ', CONVERT(VARCHAR(10),Datum,105)) as Sifra from Izlozba " +
                "where Datum < @TodayDate order by IzlozbaID asc";
            SqlCommand komanda = new SqlCommand(sqlUpit, konekcija);
            SqlDataAdapter adapter = new SqlDataAdapter(komanda);
            DataTable dt = new DataTable();

            komanda.Parameters.AddWithValue("@TodayDate", DateTime.Today);

            try
            {
                adapter.Fill(dt);

                comboBoxIzlozbaF2.DataSource = dt;
                comboBoxIzlozbaF2.ValueMember = "IzlozbaID";
                comboBoxIzlozbaF2.DisplayMember = "Sifra";
                comboBoxIzlozbaF2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopuniCBPas();
            PopuniCBIzlozbaF1();
            PopuniCBKategorija();
            PopuniCBIzlozbaF2();
        }

        private void buttonPrijava_Click(object sender, EventArgs e)
        {
            if (comboBoxPas.Text == "" || comboBoxIzlozbaF1.Text == "" || comboBoxKategorija.Text == "")
            {
                MessageBox.Show("Popnite podatke!!!");
                return;
            }

            SqlCommand proveraCMD = new SqlCommand("Select * from Rezultat " +
                "where IzlozbaID = @parIzlozba and KategorijaID = @parKategorija and PasID = @parPas",konekcija);

            proveraCMD.Parameters.AddWithValue("@parIzlozba", comboBoxIzlozbaF1.SelectedValue);
            proveraCMD.Parameters.AddWithValue("@parKategorija", comboBoxKategorija.SelectedValue);
            proveraCMD.Parameters.AddWithValue("@parPas", comboBoxPas.SelectedValue);


            SqlDataAdapter adapter = new SqlDataAdapter(proveraCMD);
            DataTable dt = new DataTable();

            try
            {
                adapter.Fill(dt);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if(dt.Rows.Count > 0)
            {
                MessageBox.Show("Pas je vec prijavljen");
                return;
            }

            string sqlUpit = "INSERT INTO Rezultat(IzlozbaID, KategorijaID, PasID) VALUES(@parIzlozbaa, @parKategorijaa, @parPass)";
        
            SqlCommand cmd = new SqlCommand(sqlUpit,konekcija);
            cmd.Parameters.AddWithValue("@parIzlozbaa", comboBoxIzlozbaF1.SelectedValue);
            cmd.Parameters.AddWithValue("@parKategorijaa", comboBoxKategorija.SelectedValue);
            cmd.Parameters.AddWithValue("@parPass", comboBoxPas.SelectedValue);

            try
            {
                konekcija.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Pas je prijavljen");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }

        private void buttonZatvoriAplikaciju_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //drugi tab
        private void buttonPrikazi_Click(object sender, EventArgs e)
        {
            string sqlUpit = "Select Kategorija.KategorijaID as Sifra, Kategorija.Naziv as 'Naziv kategorije', " +
                "COUNT(Rezultat.PasID) as 'Broj pasa' from Kategorija inner join Rezultat " +
                "on Rezultat.KategorijaID=Kategorija.KategorijaID " +
                "where Rezultat.IzlozbaID = @izID group by Kategorija.KategorijaID, Kategorija.Naziv"; 
            SqlCommand komanda = new SqlCommand(sqlUpit, konekcija);
            SqlDataAdapter adapter = new SqlDataAdapter(komanda);
            DataTable dt = new DataTable();

            komanda.Parameters.AddWithValue("@izID", comboBoxIzlozbaF2.SelectedValue);

            try
            {
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                chart1.DataSource = dt;
                chart1.Series[0].XValueMember = "Naziv kategorije";
                chart1.Series[0].YValueMembers = "Broj pasa";
                chart1.Series[0].IsValueShownAsLabel = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            string upit1 = "Select COUNT(PasID) from Rezultat where IzlozbaID = @izID";
            string upit2 = "Select COUNT(PasID) from Rezultat where IzlozbaID = @izID and len(Napomena)>0";

            SqlCommand komanda1 = new SqlCommand(upit1, konekcija);
            SqlCommand komanda2 = new SqlCommand(upit2, konekcija);

            komanda1.Parameters.AddWithValue("@izID", comboBoxIzlozbaF2.SelectedValue);
            komanda2.Parameters.AddWithValue("@izID", comboBoxIzlozbaF2.SelectedValue);

            try
            {
                konekcija.Open();
                int ukupno1 = Convert.ToInt32(komanda1.ExecuteScalar());
                int ukupno2 = Convert.ToInt32(komanda2.ExecuteScalar());

                labelPrijavljeno.Text = "Ukupan broj pasa koji je prijavljen: " + ukupno1.ToString();
                labelTakmicilo.Text = "Ukupan broj pasa koji se takmicio: " + ukupno2.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 3)
            {
                this.Close();
            }
        }

    }
}
