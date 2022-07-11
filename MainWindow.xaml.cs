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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Enterance_Click(object sender, RoutedEventArgs e)
        {
            string connetionString;
            SqlConnection cnn;
            SqlCommand command;
            SqlDataReader DataReader;
            String sql = "";
            string username = "";
            string userfamily = "";
            int usertype = 0;
            int userid = 0;

            if (UserName.Text == "" || Password.Password == "")
            {
                MessageBox.Show("کد کاربري يا رمز عبور وارد نشده است");
            }
            else
            {
                connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
                cnn = new SqlConnection(connetionString);
                try
                {
                    cnn.Open();
                    sql = @"SELECT USERID,USERNAME,USERFAMILY,USERTYPE FROM Tbl_Users WHERE Email='" + UserName.Text + @"' AND Password='" + Password.Password + @"'";
                    command = new SqlCommand(sql, cnn);
                    DataReader = command.ExecuteReader();
                    if (DataReader.HasRows)
                    {
                        while (DataReader.Read())
                        {
                            userid = int.Parse(DataReader.GetValue(0).ToString());
                            username = DataReader.GetValue(1).ToString();
                            userfamily = DataReader.GetValue(2).ToString();
                            usertype = int.Parse(DataReader.GetValue(3).ToString());
                        }
                        if (usertype == 1)
                        {
                            //admin user
                            AdminMenu admin = new AdminMenu(username, userfamily, usertype, userid);
                            admin.Show();
                        }
                        else
                        {
                            //ordinary user
                            MainMenu menu = new MainMenu(username, userfamily, usertype, userid);
                            menu.Show();
                        }
                        this.Close();
                        cnn.Close();
                    }
                    else
                    {
                        MessageBox.Show("کد کاربري يا رمز عبور اشتباه است");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        private void Enrollment_Click(object sender, RoutedEventArgs e)
        {
            Enrollment enroll = new Enrollment();
            enroll.Show();
            this.Close();
        }
    }
}
