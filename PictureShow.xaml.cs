using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for PictureShow.xaml
    /// </summary>
    public partial class PictureShow : Window
    {
        string bookname = "";
        string authorname = "";
        int price = 0;
        int stock = 0;
        int favavg = 0;
        string path = "";

        public PictureShow(string path, string bookname, string authorname, int price, int stock, int favavg)
        {
            this.bookname = bookname;
            this.authorname = authorname;
            this.price = price;
            this.stock = stock;
            this.favavg = favavg;
            this.path = path;
            InitializeComponent();

            try
            {
                ImageSource imgsource = new BitmapImage(new Uri(path));
                Cover.Source = imgsource;
                BookName.Text += " " + bookname;
                AuthorName.Text += " " + authorname;
                Price.Text += " " + price.ToString();
                Stock.Text += " " + stock.ToString();
                for(int i=0;i<favavg;i++)
                {
                    Favorite.Text += "v";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
//            Books book = new Books(username, userfamily, usertype, userid);
            this.Close();
 //           book.Show();
        }
    }
}
