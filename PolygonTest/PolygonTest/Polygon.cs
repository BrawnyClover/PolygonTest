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
using System.Diagnostics;

namespace PolygonTest
{
    enum MouseState
    {
        Idle,
        InAction
    }
    enum Colors
    {
        Red,
        Green,
        Pink,
        Yellow,
        Blue
    }

    enum Strokes
    {
        small,
        big,
        vbig,
        giant
    }
    class CustomEllipse
    {
        public Ellipse body;
        public Point position;
        public List<int> linkedLineIdx;
        public int dot_id;
        public int canvas_id;
        private int CIRCLE_SIZE;

        public CustomEllipse(int CIRCLE_SIZE, Point pos, int dot_id, int canvas_id, Colors cls)
        {
            this.CIRCLE_SIZE = CIRCLE_SIZE;
            position = pos;
            this.dot_id = dot_id;
            this.canvas_id = canvas_id;
            linkedLineIdx = new List<int>();
            body = generateEllipse(cls);
        }

        public Ellipse generateEllipse(Colors cls)
        {
            Ellipse elps = new Ellipse();
            elps.Width = CIRCLE_SIZE;
            elps.Height = CIRCLE_SIZE;
            switch (cls)
            {
                case Colors.Red:
                    elps.Fill = Brushes.Red;
                    break;
                case Colors.Green:
                    elps.Fill = Brushes.Green;
                    break;
                case Colors.Pink:
                    elps.Fill = Brushes.Pink;
                    break;
                case Colors.Yellow:
                    elps.Fill = Brushes.Yellow;
                    break;
                case Colors.Blue:
                    elps.Fill = Brushes.Blue;
                    break;
                default:
                    elps.Fill = Brushes.Red;
                    break;
            }
            Canvas.SetLeft(elps, position.X);
            Canvas.SetTop(elps, position.Y);
            return elps;
        }
    }

    class CustomLine
    {
        public int point_1_dot_id;
        public int point_2_dot_id;

        public Line body;
        public int canvas_id;
        public int line_id;
        private int CIRCLE_SIZE;

        public CustomLine(int CIRCLE_SIZE, int point_1_dot_id, int point_2_dot_id, int canvas_id, int line_id, Point pt1, Point pt2, Strokes strk)
        {
            this.CIRCLE_SIZE = CIRCLE_SIZE;
            this.point_1_dot_id = point_1_dot_id;
            this.point_2_dot_id = point_2_dot_id;
            this.canvas_id = canvas_id;
            this.line_id = line_id;
            body = generateLine(pt1, pt2, strk);
        }

        public Line generateLine(Point pt1, Point pt2, Strokes strk)
        {
            Line line = new Line();
            Random rand = new Random();
            Brush brush = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 256), (byte)rand.Next(0, 256), (byte)rand.Next(0, 256)));
            line.Stroke = brush;
            switch (strk)
            {
                case Strokes.small:
                    line.StrokeThickness = 1;
                    break;
                case Strokes.big:
                    line.StrokeThickness = 3;
                    break;
                case Strokes.vbig:
                    line.StrokeThickness = 5;
                    break;
                case Strokes.giant:
                    line.StrokeThickness = 7;
                    break;
            }
            line.X1 = pt1.X+ CIRCLE_SIZE/2;
            line.Y1 = pt1.Y+ CIRCLE_SIZE/2;
            line.X2 = pt2.X+ CIRCLE_SIZE/2;
            line.Y2 = pt2.Y+ CIRCLE_SIZE/2;

            return line;
        }
    }
    class Polygon
    {
        public const int CIRCLE_SIZE = 30;
        public List<CustomEllipse> dots;
        public List<CustomLine> lines;
        public bool rectInitialized = false;
        public Canvas _canvas;

        MouseState ms;

        Colors cls;
        Strokes strk;

        public Polygon(Canvas canvas)
        {
            dots = new List<CustomEllipse>();
            lines = new List<CustomLine>();
            _canvas = canvas;
            ms = MouseState.Idle;
            cls = Colors.Green;
            strk = Strokes.small;
        }

        private CustomEllipse addElipseEvent(CustomEllipse elps)
        {
            elps.body.MouseDown += (object sender, MouseButtonEventArgs e) =>
            {
                ms = MouseState.InAction;
            };

            elps.body.MouseMove += (object sender, MouseEventArgs e) =>
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed && ms == MouseState.InAction)
                {
                    var pos = Mouse.GetPosition(_canvas);
                    Canvas.SetLeft(_canvas.Children[elps.canvas_id], pos.X - (CIRCLE_SIZE / 2));
                    Canvas.SetTop(_canvas.Children[elps.canvas_id], pos.Y - (CIRCLE_SIZE / 2));
                    Canvas.SetZIndex(_canvas.Children[elps.canvas_id], 10);
                    elps.position = pos;
                    foreach (int line_id in elps.linkedLineIdx)
                    {
                        if (lines[line_id].point_1_dot_id == elps.dot_id)
                        {
                            lines[line_id].body.X1 = pos.X;
                            lines[line_id].body.Y1 = pos.Y;
                        }
                        else
                        {
                            lines[line_id].body.X2 = pos.X;
                            lines[line_id].body.Y2 = pos.Y;
                        }
                        _canvas.Children[lines[line_id].canvas_id] = lines[line_id].body;
                        Canvas.SetZIndex(_canvas.Children[lines[line_id].canvas_id], 1);
                       
                    }
                }

            };

            elps.body.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                ms = MouseState.Idle;
            };
            return elps;
        }

        private CustomLine addLineEvent(CustomLine ln)
        {
            ln.body.MouseDown += (object sender, MouseButtonEventArgs e) =>
            {
                var pos = Mouse.GetPosition(_canvas);
                ms = MouseState.InAction;
                Debug.WriteLine(pos);
                addPoint(new Point(pos.X - CIRCLE_SIZE/2, pos.Y - CIRCLE_SIZE / 2));
            };

            ln.body.MouseMove += (object sender, MouseEventArgs e) =>
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed && ms == MouseState.InAction)
                {
                    
                }

            };

            ln.body.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                ms = MouseState.Idle;
            };
            return ln;
        }

        public void addPoint(Point pt)
        {
            
            CustomEllipse elps = new CustomEllipse(CIRCLE_SIZE, pt, dots.Count, _canvas.Children.Count, cls++);
            elps = addElipseEvent(elps);
            _canvas.Children.Add(elps.body);
            dots.Add(elps);
            Canvas.SetZIndex(_canvas.Children[elps.canvas_id], 10);
        }

        public void addLine(int elps_dot_id1, int elps_dot_id2)
        {
            CustomLine line = new CustomLine(CIRCLE_SIZE, elps_dot_id1, elps_dot_id2, _canvas.Children.Count, lines.Count, dots[elps_dot_id1].position, dots[elps_dot_id2].position, strk++);
            line = addLineEvent(line);
            _canvas.Children.Add(line.body);
            dots[elps_dot_id1].linkedLineIdx.Add(line.line_id);
            dots[elps_dot_id2].linkedLineIdx.Add(line.line_id);
            lines.Add(line);
        }

        public void movePoint(int elps_dot_id, Point pt)
        {
            if (rectInitialized == false)
            {
                int canvas_id = dots[elps_dot_id].canvas_id;
                Canvas.SetLeft(_canvas.Children[canvas_id], pt.X- CIRCLE_SIZE/2);
                Canvas.SetTop(_canvas.Children[canvas_id], pt.Y- CIRCLE_SIZE/2);
                foreach (int line_id in dots[elps_dot_id].linkedLineIdx)
                {
                    if (lines[line_id].point_1_dot_id == elps_dot_id)
                    {
                        lines[line_id].body.X1 = pt.X;
                        lines[line_id].body.Y1 = pt.Y;
                    }
                    else
                    {
                        lines[line_id].body.X2 = pt.X;
                        lines[line_id].body.Y2 = pt.Y;
                    }
                    _canvas.Children[lines[line_id].canvas_id] = lines[line_id].body;
                }
            }
        }
    }
}


/*
 * Line 위에 점을 누르면
 * 누른 점 좌표 저장
 * line의 point_1_dot_id의 좌표와 누른 점을 잇는 새로운 line 생성
 * line의 point_2_dot_id의 좌표와 누른 점을 잇는 새로운 line 생성
 * 원래 line 제거
 * 새로운 line들 canvas에 추가
 * CustomEllipse의 linkedLineIdx에 line_id 추가(line의 canvas_id가 아님!)
 */