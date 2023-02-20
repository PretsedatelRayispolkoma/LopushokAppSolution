using LopushokApp.DB;
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

namespace LopushokApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private int PageNumber = 1;
        private int ElemntOnPage = 20;

        List<Product> products;
        List<Product> sortedProduct;
        List<string> SortBy;
        public MainPage()
        {
            InitializeComponent();

            products = MainWindow.db.Product.ToList();
            sortedProduct = products;

            PageNumberTb.Text = PageNumber.ToString();

            SortBy = new List<string>()
            {
                "По умолчанию",
                "По наименованию",
                "По номеру производственного цеха",
                "По минимальной стоимости"
            };

            SortByCb.ItemsSource = SortBy;

            var types = MainWindow.db.ProductType.ToList();
            types.Insert(0, new DB.ProductType
            {
                Name = "Все типы"
            });

            FilterByCb.ItemsSource = types;
            FilterByCb.DisplayMemberPath = "Name";
            FilterByCb.SelectedIndex = 0;
            SortByCb.SelectedIndex = 0;

            ProductLv.ItemsSource = sortedProduct;
        }

        private void UpdateList()
        {
            sortedProduct = products.ToList();

            if (SortByCb.SelectedIndex == 0)
                sortedProduct = sortedProduct.OrderBy(x => x.Id).ToList();
            else if (SortByCb.SelectedIndex == 1)
                sortedProduct = sortedProduct.OrderBy(x => x.Name).ToList();
            else if (SortByCb.SelectedIndex == 2)
                sortedProduct = sortedProduct.OrderBy(x => x.WorkshopId).ToList();
            else
                sortedProduct = sortedProduct.OrderBy(x => x.MinPrice).ToList();

            var selectedFilter = FilterByCb.SelectedItem as DB.ProductType;
            if(selectedFilter.Id != 0)
            {
                sortedProduct = sortedProduct.Where(p => p.ProductType == selectedFilter).ToList();
            }

            sortedProduct = sortedProduct.Where(p => p.Name.ToLower().Contains(SearchTb.Text.ToLower())).ToList();

            ProductLv.ItemsSource = sortedProduct;

            GoPagination();

            ProductLv.Items.Refresh();
        }

        private void SortByCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }

        private void FilterByCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddPage());
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ProductLv.SelectedItem as Product;
            if (selectedItem != null)
                NavigationService.Navigate(new AddPage(selectedItem));
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PageNumber == 1)
                return;
            PageNumber--;
            GoPagination();

        }

        private void GoPagination()
        {
            ProductLv.ItemsSource = sortedProduct.Skip((PageNumber - 1) * ElemntOnPage).Take(ElemntOnPage);
            PageNumberTb.Text = PageNumber.ToString();
            ProductLv.Items.Refresh();
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            var pagesCount = sortedProduct.Count() / ElemntOnPage + (sortedProduct.Count() % ElemntOnPage != 0 ? 1 : 0);
            if (PageNumber == pagesCount)
                return;
            PageNumber++;
            GoPagination();
        }
    }
}
