using PdfRenderByHarryTrinhWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PdfExtraTool.CustomControls
{
    /// <summary>
    /// Interaction logic for RotationControlBox.xaml
    /// </summary>
    public partial class RotationControlBox : UserControl
    {

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(int), typeof(RotationControlBox));

        public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register("Orientation", typeof(int), typeof(RotationControlBox));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int Orientation
        {
            get { return (int)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public RotationControlBox()
        {
            InitializeComponent();
            DataContext = this;
            TxtDegree.Text = "0";
            TxtDegree.TextChanged += TxtDegree_TextChanged;
        }

        private void TxtDegree_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(TxtDegree.Text, out int _val);
            Value = _val;
        }

        private void BtnRotateLeft_Click(object sender, RoutedEventArgs e)
        {
            var deg = int.Parse(TxtDegree.Text);
            TxtDegree.Text = (deg += 90).ToString();
            Orientation += 90;
            //Value -= 90;
        }

        private void BtnRotateRight_Click(object sender, RoutedEventArgs e)
        {
            Orientation += 90;
            var deg = int.Parse(TxtDegree.Text);
            TxtDegree.Text = (deg -= 90).ToString();
            //if (Value < 270)
            //{
            //    Value += 90;
            //}
            //else
            //{
            //    Value = 0;
            //}
        }
    }
}
