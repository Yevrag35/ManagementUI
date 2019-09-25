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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MUI : Window
    {

        public MUI()
        {
            InitializeComponent();
            var ali = new AppListItem("Active Directory Users and Computers", @"C:\Windows\System32\dsadmin.dll");
            App._items.Add(ali);
            this.AppListView.Items.Add(ali);
            this.AppListView.Items.Refresh();
        }
    }
}
