using _2D_Graphics.Model;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
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

namespace _2D_Graphics.Views
{
    /// <summary>
    /// Interaction logic for ColorSwatch.xaml
    /// </summary>
    public partial class ColorSwatch : Window
    {
        static Color selectedColor = Color.FromArgb(255,255,255,255);
        public ColorSwatch()
        {
            InitializeComponent();
            _colorCanvas.SelectedColor = selectedColor;
            Measurements.SetColor(selectedColor);
            _colorCanvas.Cursor = System.Windows.Input.Cursors.Cross;
        }
        private void _colorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            selectedColor = (Color)_colorCanvas.SelectedColor;

            Measurements.SetColor((Color)_colorCanvas.SelectedColor);
        }
        
    }
}
