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
using System.Text.RegularExpressions;
//using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for Enrollment.xaml
    /// </summary>
    public partial class Enrollment : Window
    {
        public Enrollment()
        {
            InitializeComponent();
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }

        private void EnrollBt_Click(object sender, RoutedEventArgs e)
        {
            string connetionString;
            SqlConnection cnn = null;
            SqlCommand command;
            SqlDataReader DataReader;
            String sql = "";

            try
            {
                Regex namereg = new Regex("^[a-zA-Z]{3,32}$");
                Regex mailreg = new Regex("^[a-zA-Z]{1,32}@[a-zA-Z]{1,32}.[a-zA-Z]{1,32}$");
                Regex phonereg = new Regex("^09[0-9]{9}$");
                Regex passreg = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])[A-Za-z\d@$!%*?&]{8,40}$");

                connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                if(!namereg.IsMatch(Name.Text))
                {
                    throw new FormatException("نام وارد شده در قالب صحيح قرار ندارد");
                }
                else
                {
                    if (!namereg.IsMatch(Family.Text))
                    {
                        throw new FormatException("نام خانوادگی وارد شده در قالب صحيح قرار ندارد");
                    }
                    else
                    {
                        if (!mailreg.IsMatch(Email.Text))
                        {
                            throw new FormatException("ايميل وارد شده در قالب صحيح قرار ندارد");
                        }
                        else
                        {
                            if (!phonereg.IsMatch(Cellphone.Text))
                            {
                                throw new FormatException("شماره همراه وارد شده در قالب صحيح قرار ندارد");
                            }
                            else
                            {
                                if (!passreg.IsMatch(Password.Password))
                                {
                                    throw new FormatException("رمز عبور وارد شده در قالب صحيح قرار ندارد");
                                }
                                else
                                {
                                    if (!passreg.IsMatch(Rpassword.Password))
                                    {
                                        throw new FormatException("تکرار رمز وارد شده در قالب صحيح قرار ندارد");
                                    }
                                    else
                                    {
                                        if (Password.Password != Rpassword.Password)
                                        {
                                            throw new FormatException("رمز وارد شده با تکرار آن هم خوانی ندارد");
                                        }
                                        else
                                        {
                                            sql = @"SELECT USERNAME,USERFAMILY,USERTYPE FROM Tbl_Users WHERE Email='" + Email.Text + @"'";
                                            command = new SqlCommand(sql, cnn);
                                            DataReader = command.ExecuteReader();
                                            if(DataReader.HasRows)
                                            {
                                                throw new FormatException("ايميل وارد شده قبلا برای کاربر ديگری استفاده است");
                                            }
                                            else
                                            {
                                                DataReader.Close();
                                                SqlCommand sql_cmnd = new SqlCommand("Stp_UsersInsert", cnn);
                                                sql_cmnd.CommandType = CommandType.StoredProcedure;
                                                sql_cmnd.Parameters.AddWithValue("@UserName", SqlDbType.NVarChar).Value = Name.Text;
                                                sql_cmnd.Parameters.AddWithValue("@UserFamily", SqlDbType.NVarChar).Value = Family.Text;
                                                sql_cmnd.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = Email.Text;
                                                sql_cmnd.Parameters.AddWithValue("@Password", SqlDbType.NVarChar).Value = Password.Password;
                                                sql_cmnd.Parameters.AddWithValue("@Cellphoneno", SqlDbType.NVarChar).Value = Cellphone.Text;
                                                sql_cmnd.Parameters.AddWithValue("@UserType", SqlDbType.TinyInt).Value = 2; //Normal User
                                                sql_cmnd.ExecuteNonQuery();
                                                MessageBox.Show("اطلاعات کاربر با موقيت ثبت شد");
                                                cnn.Close();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
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
}
