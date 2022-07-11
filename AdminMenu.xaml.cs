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

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for AdminMenu.xaml
    /// </summary>
    public partial class AdminMenu : Window
    {
        string username = "";
        string userfamily = "";
        int usertype = 0;
        int userid = 0;

        public AdminMenu(string username, string userfamily, int usertype, int userid)
        {
            this.username = username;
            this.userfamily = userfamily;
            this.usertype = usertype;
            this.userid = userid;
            InitializeComponent();
            UserNameFamily.Text = "نام کاربر : " + username + " " + userfamily;
            switch (usertype)
            {
                case 1:
                    UserType.Text = "نوع کاربر : مدير";
                    break;
                case 2:
                    UserType.Text = "نوع کاربر : عادي";
                    break;
                default:
                    UserType.Text = "نوع کاربر : نامشخص";
                    break;
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Books book = new Books(username, userfamily, usertype, userid);
            this.Close();
            book.Show();
        }
    }
}
