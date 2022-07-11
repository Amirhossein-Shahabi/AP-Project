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
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;

namespace WpfProject
{
    public static class Luhn
    {
        /// <summary>
        /// Luhn Algorithm C#
        /// </summary>
        /// <param name="ccNumber">Credit/Debit/etc card number</param>
        /// <returns>bool</returns>
        public static bool check(string ccNumber)
        {
            int sum = 0;
            bool alternate = false;
            for (int i = ccNumber.Length - 1; i >= 0; i--)
            {
                char[] nx = ccNumber.ToArray();
                int n = int.Parse(nx[i].ToString());

                if (alternate)
                {
                    n *= 2;

                    if (n > 9)
                    {
                        n = (n % 10) + 1;
                    }
                }
                sum += n;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
        }
    }

    /// <summary>
    /// Interaction logic for InternetPurchase.xaml
    /// </summary>
    public partial class InternetPurchase : Window
    {
        string username = "";
        string userfamily = "";
        int usertype = 0;
        int userid = 0;
        int value = 0;
        string winname = "";

        public InternetPurchase(string username, string userfamily, int usertype, int userid, int value, string winname)
        {
            this.username = username;
            this.userfamily = userfamily;
            this.usertype = usertype;
            this.userid = userid;
            this.value = value;
            this.winname = winname;
            InitializeComponent();
            Title.Text += " به مقدار " + value.ToString();
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            switch(winname)
            {
                case "Vallet":
                    Valllet vallet = new Valllet(username,userfamily,usertype,userid);
                    this.Close();
                    vallet.Show();
                    break;
                case "Basket":
                    this.Close();
                    break;
            }
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            string connetionString;
            SqlConnection cnn = null;
            SqlCommand command, sqlcomm;
            string cardno = "";
            Regex cvvreg = new Regex(@"^[0-9]{3,4}$");
            string sql = "";


            if(winname== "Vallet")
            {
                connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
                cnn = new SqlConnection(connetionString);
                command = new SqlCommand("Stp_UserValletUpdate", cnn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", SqlDbType.Int).Value = userid;
                command.Parameters.AddWithValue("@Value", SqlDbType.BigInt).Value = value;
                command.Parameters.AddWithValue("@Type", SqlDbType.TinyInt).Value = 1;
                cardno = Card1.Text + Card2.Text + Card3.Text + Card4.Text;

                try
                {
                    cnn.Open();
                    if (!Luhn.check(cardno))
                    {
                        throw new FormatException("شماره کارت معتبر نيست");
                    }
                    else
                    {
                        if (!cvvreg.IsMatch(CVV2.Text))
                        {
                            throw new FormatException("صحيح نمي باشد CVV2");
                        }
                        else
                        {
                            sql = @"SELECT dbo.ShamsiToMilady(" + @"'14" + Year.Text + Month.Text + @"01')";
                            sqlcomm = new SqlCommand(sql, cnn);
                            sqlcomm.CommandType = CommandType.Text;
                            DateTime date = (DateTime)sqlcomm.ExecuteScalar();
                            DateTime now = DateTime.Now;
                            //                        MessageBox.Show(date.ToString());
                            //                        MessageBox.Show(now.ToString());
                            if ((now.Year > date.Year) || (now.Year == date.Year && now.Month > date.Month))
                            {
                                throw new FormatException("تاريخ کارت منقضي شده است");
                            }
                            else
                            {
                                command.ExecuteNonQuery();
                                MessageBox.Show("مقدار کيف پول با موقيت به روز رساني شد");
                                cnn.Close();
                                switch (winname)
                                {
                                    case "Vallet":
                                        Valllet vallet = new Valllet(username, userfamily, usertype, userid);
                                        this.Close();
                                        vallet.Show();
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    cnn.Close();
                }
            }
            if (winname == "Basket")
            {
                connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
                cnn = new SqlConnection(connetionString);
                cardno = Card1.Text + Card2.Text + Card3.Text + Card4.Text;

                try
                {
                    cnn.Open();
                    if (!Luhn.check(cardno))
                    {
                        throw new FormatException("شماره کارت معتبر نيست");
                    }
                    else
                    {
                        if (!cvvreg.IsMatch(CVV2.Text))
                        {
                            throw new FormatException("صحيح نمي باشد CVV2");
                        }
                        else
                        {
                            sql = @"SELECT dbo.ShamsiToMilady(" + @"'14" + Year.Text + Month.Text + @"01')";
                            sqlcomm = new SqlCommand(sql, cnn);
                            sqlcomm.CommandType = CommandType.Text;
                            DateTime date = (DateTime)sqlcomm.ExecuteScalar();
                            DateTime now = DateTime.Now;
                            if ((now.Year > date.Year) || (now.Year == date.Year && now.Month > date.Month))
                            {
                                throw new FormatException("تاريخ کارت منقضي شده است");
                            }
                            else
                            {
                                MessageBox.Show("پرداخت آنلاين با موفقيت انجام شد");
                                cnn.Close();
                                switch (winname)
                                {
                                    case "Basket":
                                        this.Close();
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
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
}
