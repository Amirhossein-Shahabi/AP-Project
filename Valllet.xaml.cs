using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for Valllet.xaml
    /// </summary>
    public partial class Valllet : Window
    {
        string username = "";
        string userfamily = "";
        int usertype = 0;
        int userid = 0;
        int value = 0;

        public Valllet(string username, string userfamily, int usertype, int userid)
        {
            string connetionString;
            SqlConnection cnn = null;
            SqlCommand command;
            SqlDataReader DataReader;
            String sql = "";

            this.username = username;
            this.userfamily = userfamily;
            this.usertype = usertype;
            this.userid = userid;
            InitializeComponent();
            Title.Text += " " + username + " " + userfamily;
            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            sql = @"SELECT VALUE FROM Tbl_UserVallet WHERE UserId='" + userid + @"'";
            command = new SqlCommand(sql, cnn);
            try
            {
                cnn.Open();
                DataReader = command.ExecuteReader();
                if (DataReader.HasRows)
                {
                    while (DataReader.Read())
                    {
                        value = int.Parse(DataReader.GetValue(0).ToString());
                    }
                    cnn.Close();
                    Amount.Text = value.ToString();
                }
                else
                {
                    MessageBox.Show("کيف پول قبلا براي شما ايجاد نشده است");
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            MainMenu menu = new MainMenu(username, userfamily, usertype, userid);
            this.Close();
            menu.Show();
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            value = int.Parse(Payment.Text);
            InternetPurchase purchase = new InternetPurchase(username, userfamily, usertype, userid, value, "Vallet");
            this.Close();
            purchase.Show();
        }
    }
}
