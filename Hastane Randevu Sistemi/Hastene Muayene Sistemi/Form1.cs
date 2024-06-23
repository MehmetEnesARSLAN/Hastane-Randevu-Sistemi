using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hastene_Muayene_Sistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        NpgsqlConnection baglantı = new NpgsqlConnection("server=localHost; port = 5432; Database=dbmuayenebilgi; user ID=postgres;  password=1234");


        private void btnKayıt_Click(object sender, EventArgs e)
        {
            long previousRandomNumber = 0;

            Random rnd = new Random();
            int randomNumber = rnd.Next(100, 999);

            while (randomNumber == previousRandomNumber)
            {
                randomNumber = rnd.Next(100, 999);
            }
            previousRandomNumber = randomNumber;

            baglantı.Open();
            NpgsqlCommand komut = new NpgsqlCommand("insert into hasta (tckn,ad,soyad,dogumyeri,dogumtarihi,medenidurumu,adres,telefon,hastasikayet,receteno) values (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10)", baglantı);

            komut.Parameters.AddWithValue("@p1", txttckn.Text);
            komut.Parameters.AddWithValue("@p2", txtad.Text);
            komut.Parameters.AddWithValue("@p3", txtsoyad.Text);
            komut.Parameters.AddWithValue("@p4", txtdogumyeri.Text);
            komut.Parameters.AddWithValue("@p5", txtdogumtarihi.Text);
            komut.Parameters.AddWithValue("@p6", txtmedenidurum.Text);
            komut.Parameters.AddWithValue("@p7", txtadres.Text);
            komut.Parameters.AddWithValue("@p8", txttelefon.Text);
            komut.Parameters.AddWithValue("@p9", txthastaşikayet.Text);
            komut.Parameters.AddWithValue("@p10", randomNumber);

            komut.ExecuteNonQuery();
            baglantı.Close();
            MessageBox.Show("Hasta kaydı başarılı.");
        }

        private void btnhastagörüntüle_Click(object sender, EventArgs e)
        {
            string sorgu = "select * from hasta";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglantı);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridViewhg.DataSource = ds.Tables[0];
        }

        private void dataGridViewhg_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            int seçilen = dataGridViewhg.SelectedCells[0].RowIndex;

            txttckn.Text = dataGridViewhg.Rows[seçilen].Cells[0].Value.ToString();
            txtad.Text = dataGridViewhg.Rows[seçilen].Cells[1].Value.ToString();
            txtsoyad.Text = dataGridViewhg.Rows[seçilen].Cells[2].Value.ToString();
            txtdogumyeri.Text = dataGridViewhg.Rows[seçilen].Cells[3].Value.ToString();
            txtdogumtarihi.Text = dataGridViewhg.Rows[seçilen].Cells[4].Value.ToString();
            txtmedenidurum.Text = dataGridViewhg.Rows[seçilen].Cells[5].Value.ToString();
            txtadres.Text = dataGridViewhg.Rows[seçilen].Cells[6].Value.ToString();
            txttelefon.Text = dataGridViewhg.Rows[seçilen].Cells[7].Value.ToString();
            txthastaşikayet.Text = dataGridViewhg.Rows[seçilen].Cells[8].Value.ToString();
        }


        private void btnbilgilerigetir_Click(object sender, EventArgs e)
        {
            string tckimlikNumarasi = txttckn.Text;


            baglantı.Open();

            NpgsqlCommand komutgiris = new NpgsqlCommand("select * from hasta where tckn =@tckn ", baglantı);
            
            komutgiris.Parameters.AddWithValue("@tckn", tckimlikNumarasi);
            NpgsqlDataReader reader = komutgiris.ExecuteReader();

            while (reader.Read())
            {
                txttcknreçete.Text = reader["tckn"].ToString();
                txtadreçete.Text = reader["ad"].ToString();
                txtsoyadreçete.Text = reader["soyad"].ToString();
                txtreçeteno.Text = reader["receteno"].ToString();
                txthastaşikayetreçete.Text = reader["hastasikayet"].ToString();

                if (reader[1] != null)
                {
                    txttcknreçete.Enabled = true;
                    txtadreçete.Enabled = true;
                    txtsoyadreçete.Enabled = true;
                    txtreçeteno.Enabled = true;
                    txthastaşikayetreçete.Enabled = true;

                }
            }

            baglantı.Close();

            tabhastagiriş.Hide();
            tabreçete.Show();
        }

        private void btnilaçlistele_Click(object sender, EventArgs e)
        {
            string sorgu = "select * from ilaç";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglantı);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridViewr.DataSource = ds.Tables[0];
        }

        private void btnekle_Click(object sender, EventArgs e)
        {
            string tckimlikNumarasi = txttcknreçete.Text;
            string muayeneTarihi = txttarihreçete.Text;

            string sorgu2 = "SELECT COUNT(*) FROM recete WHERE tckn = @tckimlikNumarasi AND tarih = @muayeneTarihi";
            baglantı.Open();
            NpgsqlCommand komut2 = new NpgsqlCommand(sorgu2, baglantı);
            komut2.Parameters.AddWithValue("@tckimlikNumarasi", tckimlikNumarasi);
            komut2.Parameters.AddWithValue("@muayeneTarihi", muayeneTarihi);

            int kayitSayisi = Convert.ToInt32(komut2.ExecuteScalar());
            baglantı.Close();
            if (kayitSayisi > 0)
            {
                MessageBox.Show("Bir gün içerisinde aynı hastaya bir kez reçete yazılabilir.");
                return;
            }

            baglantı.Open();
            NpgsqlCommand komut = new NpgsqlCommand("insert into recete (tckn,tarih,ilac,receteno,adet,barkodno) values (@p1,@p2,@p3,@p4,@p5,@p6)", baglantı);

            komut.Parameters.AddWithValue("@p1", txttckn.Text);
            komut.Parameters.AddWithValue("@p2", txttarihreçete.Text);
            komut.Parameters.AddWithValue("@p3", txtilaçadı.Text);
            komut.Parameters.AddWithValue("@p4", txtreçeteno.Text);
            komut.Parameters.AddWithValue("@p5", comboadet.Text);
            komut.Parameters.AddWithValue("@p6", txtbarkodno.Text);
            
           

            komut.ExecuteNonQuery();
            baglantı.Close();
            MessageBox.Show("Reçete kaydı başarılı.");
        }


            private void btnreçetegör_Click(object sender, EventArgs e)
        {
            string sorgu = "select * from recete";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglantı);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridViewreçete.DataSource = ds.Tables[0];
        }



        private void dataGridViewr_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int seçilen = dataGridViewr.SelectedCells[0].RowIndex;

            txtbarkodno.Text = dataGridViewr.Rows[seçilen].Cells[0].Value.ToString();
            txtilaçadı.Text = dataGridViewr.Rows[seçilen].Cells[1].Value.ToString();
            
           
        }

        private void btnmuayenekayıt_Click(object sender, EventArgs e)
        {
            baglantı.Open();
            NpgsqlCommand komut = new NpgsqlCommand("insert into muayene (tckn,muayenetarihi,hastasikayeti,tanı,tedavi,reçeteno) values (@p1,@p2,@p3,@p4,@p5,@p6)", baglantı);

            komut.Parameters.AddWithValue("@p1", txttckn.Text);
            komut.Parameters.AddWithValue("@p2", txttarihreçete.Text);
            komut.Parameters.AddWithValue("@p3", txthastaşikayetreçete.Text);
            komut.Parameters.AddWithValue("@p4", txttanı.Text);
            komut.Parameters.AddWithValue("@p5", txttedavi.Text);
            komut.Parameters.AddWithValue("@p6", txtreçeteno.Text);

            komut.ExecuteNonQuery();
            baglantı.Close();
            MessageBox.Show("Muayene kaydı başarılı.");

            string sorgu = "select * from muayene";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglantı);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridViewmuayene.DataSource = ds.Tables[0];


            tabreçete.Hide();
            tabmuayenekayıt.Show();
        }

        private void btnHastalarıGetir_Click(object sender, EventArgs e)
        {
            
            string sorgu = "SELECT * FROM recete WHERE barkodno = @barkodNumarasi";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglantı);
            komut.Parameters.AddWithValue("@barkodNumarasi", txtbarkodnoreçetev2.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridViewhasta.DataSource = dt;
            
        }

        private void btnilaçlarıgetir_Click(object sender, EventArgs e)
        {
            string sorgu = "SELECT * FROM recete WHERE tckn = @tckn";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglantı);
            komut.Parameters.AddWithValue("@tckn", txttcknreçetev2.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridViewilaç.DataSource = dt;
        }

        private void btnListelev2_Click(object sender, EventArgs e)
        {
            string sorgu = "select * from recete order by tarih";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglantı);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridViewilaç.DataSource = ds.Tables[0];

            string sorgu2 = "select * from ilaç";
            NpgsqlDataAdapter da2 = new NpgsqlDataAdapter(sorgu2, baglantı);
            DataSet ds2 = new DataSet();
            da2.Fill(ds2);
            dataGridViewhasta.DataSource = ds2.Tables[0];
        
        }

        private void btnListele_Click(object sender, EventArgs e)
        {
            tabreçete.Hide();
            tabreçetev2.Show();
        }
    }
}
