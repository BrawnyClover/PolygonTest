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
    class Polygon
    {
        public List<Point> dots;
        public List<Line> lines;
        public bool rectInitialized = false;
        public Canvas _canvas;

        public List<int> baseDotId;

        class CustomEllipse : UIElement
        {
            Ellipse body;
            Point position;
            List<int> linkedLineIdx;
            int id;
            
            public CustomEllipse(Point pos, int id)
            {
                position = pos;
                this.id = id;
            }

            public Ellipse generateEllipse(MouseState ms, Canvas _canvas, Point pt)
            {
                Ellipse elps = new Ellipse();
                int elpsSize = 20;
                elps.Width = 20;
                elps.Height = 20;
                elps.Fill = Brushes.Blue;

                elps.MouseDown += (object sender, MouseButtonEventArgs e) =>
                {
                    ms = MouseState.InAction;
                };

                elps.MouseMove += (object sender, MouseEventArgs e) =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed && ms == MouseState.InAction)
                    {
                        var pos = Mouse.GetPosition(_canvas);
                        Canvas.SetLeft(elps, pos.X - (elpsSize / 2));
                        Canvas.SetTop(elps, pos.Y - (elpsSize / 2));
                    }

                };

                elps.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    ms = MouseState.Idle;
                };

                Canvas.SetLeft(elps, pt.X);
                Canvas.SetTop(elps, pt.Y);

                return elps;
            }
        }

        class CustomLine
        {
            int ptIdx1;
            int ptIdx2;

            Line body;
            int id;

            public CustomLine(int ptIdx1, int ptIdx2, int id)
            {
                this.ptIdx1 = ptIdx1;
                this.ptIdx2 = ptIdx2;
                this.id = id;
            }
        }

        enum MouseState
        {
            Idle,
            InAction
        }

        MouseState ms;

        public Polygon(Canvas canvas)
        {
            dots = new List<Point>();
            lines = new List<Line>();
            baseDotId = new List<int>();
            _canvas = canvas;
            ms = MouseState.Idle;
        }

        public void addPoint(Point pt)
        {
            int elpsId = 0;
            dots.Add(pt);
            Ellipse elps = new Ellipse();
            _canvas.Children.Add(elps);

            if (rectInitialized == false)
            {
                elpsId = _canvas.Children.Count - 1;
                baseDotId.Add(elpsId);
            }

            int ptIdx = dots.Count - 1;
            if (ptIdx != 0)
            {

                Line line = new Line();
                Point prevPt = dots[ptIdx-1];
                line.X1 = prevPt.X;
                line.Y1 = prevPt.Y;
                line.X2 = pt.X;
                line.Y2 = pt.Y;
                line.Stroke = Brushes.Red;
                line.StrokeThickness = 3;

                _canvas.Children.Add(line); 
            }
        }

        public void movePoint(int ptIdx, Point pt)
        {
            if (rectInitialized == false)
            {
                Canvas.SetLeft(_canvas.Children[ptIdx], pt.X);
                Canvas.SetTop(_canvas.Children[ptIdx], pt.Y);
            }
        }
    }
    public partial class MainWindow : Window
    {
        Point downPoint;
        Point endPoint;

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
                ms = MouseState.InAction;
            }
        }

        private void mouse_move(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && ms == MouseState.InAction)
            {
                var pt = Mouse.GetPosition(canvas1);
                polygon.movePoint(polygon.baseDotId[3], pt);
                polygon.movePoint(polygon.baseDotId[2], new Point(polygon.dots[0].X, pt.Y));
                polygon.movePoint(polygon.baseDotId[1], new Point(pt.X, polygon.dots[0].Y));
            }
        }

        private void mouse_up(object sender, MouseButtonEventArgs e)
        {
            polygon.rectInitialized = true;
            ms = MouseState.Idle;
        }

    }
}
