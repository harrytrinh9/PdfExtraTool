﻿using System;
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

namespace PdfExtraTool.ChungPages
{
    /// <summary>
    /// Interaction logic for DanhSoTrang.xaml
    /// </summary>
    public partial class DanhSoTrang
    {
        public DanhSoTrang()
        {
            InitializeComponent();
            DataContext = new ViewModel.DanhSoTrangViewModel();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var rectangle = (Rectangle)clickedColor.Content;
            var color = ((SolidColorBrush)rectangle.Fill).Color;

            CurrentColor.Fill = new SolidColorBrush(color);

            myColorButton.Flyout.Hide();
        }
    }
}
