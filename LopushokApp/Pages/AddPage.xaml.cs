using LopushokApp.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Логика взаимодействия для AddPage.xaml
    /// </summary>
    public partial class AddPage : Page
    {

        private Product _product;

        List<Material> _materials = new List<Material>();

        public AddPage()
        {
            InitializeComponent();
            RemoveBtn.Visibility = Visibility.Collapsed;
            AddImgBtn.Visibility = Visibility.Collapsed;
        }

        public AddPage(Product product)
        {
            InitializeComponent();
            RemoveBtn.Visibility = Visibility.Visible;
            AddImgBtn.Visibility = Visibility.Visible;

            _product = MainWindow.db.Product.Attach(product);
            ArticleTb.Text = product.Article.ToString();
            NameTb.Text = product.Name;
            CountTb.Text = product.ManForProduction.ToString();
            DescriptionTb.Text = product.Description;
            WorkshopCb.SelectedItem = product.Workshop;
            TypeCb.SelectedItem = product.ProductType;
            var prodmat = MainWindow.db.ProductMaterial.Where(p => p.ProductId == product.Id).ToList();
            foreach (var pm in prodmat)
                _materials.Add(pm.Material);
            MaterialLv.ItemsSource = _materials;
            MinPriceTb.Text = product.MinPrice.ToString();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void AddImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };

            if (fileDialog.ShowDialog().Value)
            {
                _product.Image = File.ReadAllBytes(fileDialog.FileName);

            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            double price = 0;

            if (ArticleTb.Text == "" || NameTb.Text == "" || CountTb.Text == ""
                || WorkshopCb.SelectedItem == null || TypeCb.SelectedItem == null || MaterialLv.ItemsSource == null)
            {
                MessageBox.Show("Enter the data");
                return;
            }

            try
            {
                price = Convert.ToDouble(MinPriceTb.Text); 
            }
            catch
            {
                MessageBox.Show("Incorrect input of min price");
                return;
            }

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(int));
            if (!typeConverter.IsValid(CountTb.Text))
            {
                MessageBox.Show("Incorrect input of the count");
                return;
            }

            if (!typeConverter.IsValid(ArticleTb.Text))
            {
                MessageBox.Show("Incorrect input of the article");
                return;
            }

            if (price < 0 || Convert.ToInt32(CountTb.Text) < 0 || Convert.ToInt32(ArticleTb.Text) < 0)
            {
                MessageBox.Show("Negative numbers can't be used");
                return;
            }

            if (_product == null)
            {
                var prod = MainWindow.db.Product.Where(p => p.Article.ToString() == ArticleTb.Text).FirstOrDefault();
                if (prod != null)
                {
                    MessageBox.Show("The product with this article is already exists");
                    return;
                }

                Product product = new Product();
                product.Article = Convert.ToInt32(ArticleTb.Text);
                product.Name = NameTb.Text;
                product.Description = DescriptionTb.Text;
                product.ManForProduction = Convert.ToInt32(CountTb.Text);

                var selectedWs = WorkshopCb.SelectedItem as Workshop;
                product.Workshop = selectedWs;

                var selectedType = TypeCb.SelectedItem as ProductType;
                product.ProductType = selectedType;

                foreach (var mat in _materials)
                {
                    ProductMaterial pm = new ProductMaterial();
                    pm.Product = product;
                    pm.Material = mat;
                    MainWindow.db.ProductMaterial.Add(pm);
                }
                    
                product.MinPrice = Convert.ToDecimal(price);

                MainWindow.db.Product.Add(product);
            }
            else
            {
                _product.Article = Convert.ToInt32(ArticleTb.Text);
                _product.Name = NameTb.Text;
                _product.Description = DescriptionTb.Text;
                _product.ManForProduction = Convert.ToInt32(CountTb.Text);

                var selectedWs = WorkshopCb.SelectedItem as Workshop;
                _product.Workshop = selectedWs;

                var selectedType = TypeCb.SelectedItem as ProductType;
                _product.ProductType = selectedType;

                foreach(var mat in _materials)
                {
                    var item = MainWindow.db.ProductMaterial.Where(p => p.MaterialId == mat.Id && p.ProductId == _product.Id).FirstOrDefault();
                    if (item != null)
                        continue;
                    else
                    {
                        ProductMaterial pm = new ProductMaterial();
                        pm.Product = _product;
                        pm.Material = mat;
                        MainWindow.db.ProductMaterial.Add(pm);
                    }
                }

                _product.MinPrice = Convert.ToDecimal(price);
            }
            try
            {
                MainWindow.db.SaveChanges();

                new MainPage().ProductLv.Items.Refresh();

                this.NavigationService.Navigate(new MainPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MaterialCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_materials.Contains(MaterialCb.SelectedItem as Material))
                return;

            _materials.Add(MaterialCb.SelectedItem as Material);
            MaterialLv.ItemsSource = _materials;
            MaterialLv.Items.Refresh();
        }

        private void DeleBtn_Click(object sender, RoutedEventArgs e)
        {
            _materials.Remove((sender as Button).DataContext as Material);
            MaterialLv.ItemsSource = _materials;
            MaterialLv.Items.Refresh();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            MaterialCb.ItemsSource = MainWindow.db.Material.ToList();
            WorkshopCb.ItemsSource = MainWindow.db.Workshop.ToList();
            TypeCb.ItemsSource = MainWindow.db.ProductType.ToList();
            MaterialLv.ItemsSource = _materials;
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы уверены, что хотите удалить объект {_product.Article}?", "Удаление", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var prodSale = MainWindow.db.ProductSale.Where(p => p.ProductId == _product.Id).ToList();
                if (prodSale.Any())
                {
                    MessageBox.Show("You can`t remove this product");
                    return;
                }

                var materials = MainWindow.db.ProductMaterial.Where(p => p.ProductId == _product.Id).ToList();
                if (materials.Any())
                {
                    foreach (var item in materials)
                        MainWindow.db.ProductMaterial.Remove(item);
                }

                MainWindow.db.Product.Remove(_product);

                MainWindow.db.SaveChanges();

                new MainPage().ProductLv.Items.Refresh();

                this.NavigationService.Navigate(new MainPage());
            } 
        }
    }
}
