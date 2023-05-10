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

namespace PolygonTest
{
    
    public partial class MainWindow : Window
    {
        enum MouseState
        {
            Idle,
            InAction
        }

        MouseState ms;

        Polygon polygon;
        public MainWindow()
        {
            InitializeComponent();
            base.MouseDown += mouse_down;
            base.MouseMove += mouse_move;
            base.MouseUp += mouse_up;
            polygon = new Polygon(canvas1);
            ms = MouseState.Idle;
        }

        private void mouse_down(object sender, MouseButtonEventArgs e)
        {
            if (polygon.rectInitialized == false)
            {
                var pt = Mouse.GetPosition(canvas1);
                Console.WriteLine(pt);
                polygon.addPoint(pt);
                polygon.addPoint(pt);
                polygon.addPoint(pt);
                polygon.addPoint(pt);
                polygon.addLine(0, 1);
                polygon.addLine(0, 2);
                polygon.addLine(1, 3);
                polygon.addLine(2, 3);
                ms = MouseState.InAction;
            }
        }

        private void mouse_move(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && ms == MouseState.InAction)
            {
                var pt = Mouse.GetPosition(canvas1);
                polygon.movePoint(3, pt);
                polygon.movePoint(2, new Point(polygon.dots[0].position.X, pt.Y));
                polygon.movePoint(1, new Point(pt.X, polygon.dots[0].position.Y));
            }
        }

        private void mouse_up(object sender, MouseButtonEventArgs e)
        {
            polygon.rectInitialized = true;
            ms = MouseState.Idle;
        }

    }
}
