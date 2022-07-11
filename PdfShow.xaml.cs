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
using AcroPDFLib;
using Syncfusion.PdfViewer.Base;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;


namespace WpfProject
{
    /// <summary>
    /// Interaction logic for PdfShow.xaml
    /// </summary>
    public partial class PdfShow : Window
    {
        string path = "";
        int usertype = 0;

        public PdfShow(string path, int usertype)
        {
            this.path = path;
            this.usertype = usertype;

            InitializeComponent();
            if (usertype == 2)
            {

            }
            if(usertype==1)
            {
                //pdfViewer.Load(path);
            }
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
//            Books book = new Books(username, userfamily, usertype, userid);
            this.Close();
//            book.Show();
        }
    }
}
