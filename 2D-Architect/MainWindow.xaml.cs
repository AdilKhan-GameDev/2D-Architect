using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using _2D_Graphics.Model;
using _2D_Graphics.Views;
using System.Drawing.Design;
using System.Windows.Input;

namespace _2D_Graphics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        System.Windows.Controls.Canvas source = null;

        #region Properties

        // Drawing and Moving Cursor
        private bool drawingCursor = false;
        private bool MovingCursor = false;



        ColorSwatch win;
        //Grid Proprties
        int boxsize = 20;
        double totalwidth;
        double totalheight;
        //Drawing Properties
        ShapeObject TempShape = new ShapeObject();
        bool IsDrawing = false;
        Line myline = new Line();
        Point start = new Point();
        Size size = new Size();
        List<ShapeObject> MyShapeList = new List<ShapeObject>();
        ShapeObject CurrentShape = new ShapeObject();
        
        Label length = new Label();
        Label AreaLB = new Label();
        Canvas grid = new Canvas();


        /// drag controls
        protected bool isDragging;
        private Point clickPosition;
        private TranslateTransform originTT;

        #endregion 

        //Initial Start Function
        public MainWindow()
        {
            InitializeComponent();
            boxsize = int.Parse(boxsizeTB.Text);                        //getting initial boxsize from whatever in the textbox
            CreateCanvas();                                             //Creating canvas on window start
            colorPalletCanvas.Background = Measurements.GetColor();     //giving color pallet a initial color
            CurrentShape.Controls = new List<Control>();                //making a list of controls realted to any shape we create
        }



        //If user Enter new box size
        private void Change_Btn_Click(object sender, RoutedEventArgs e)
        {
            string tit = boxsizeTB.Text;
            boxsize = int.Parse(tit);
            CreateCanvas();
            size = new Size(boxsize, boxsize);
        }

        
        
        //Creating Canvas and Grid
        public void CreateCanvas()
        {
            totalwidth = paintSurface.Width;
            totalheight = paintSurface.Height;
            double no_horizontal = totalwidth / boxsize;
            double no_vertical = totalheight / boxsize;
            CreateGrid(no_horizontal, no_vertical);
            size = new Size(boxsize, boxsize);
        }
       


        // create grid function
        public void CreateGrid(double hor, double vert)
        {
            paintSurface.Children.Clear();
            double setleft = 0;
            double settop = 0;
            for(int v=0; v<vert; v++)
            {
                for(int h = 0; h< hor; h++) 
                {
                    Rectangle rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    rect.Fill = new SolidColorBrush(Colors.White);
                    rect.StrokeThickness = 0.1;
                    rect.Width = boxsize;
                    rect.Height = boxsize;
                    grid.Children.Add(rect);
                    //paintSurface.Children.Add(rect);
                    Canvas.SetLeft(rect, setleft);
                    Canvas.SetTop(rect, settop);
                    setleft += boxsize;
                }
                setleft = 0;
                settop += boxsize;
            }

            // oryginal was:
            // paintSurface.Children.Add(grid);

            // my changes
            // I removed this 'grid' from current collection and add it again. Is ok, grid is redrawed BUT points  
            // are in wrong places.. and line value text also is wrong, because have old values, should be updated
            // HERE IS TWO QUESTIONS:
            //      
            // 1 - if is possible stay grid size what is right now BUT change only size value between rectangles?? 
            //      I mean if right now 'box size: 20' 
            //      and i create box with 5x5 boxes (line have value 100). And when I will change thix box size:10'
            //      then this box also will have 5 boxes with same visual size
            //      BUT line lenght value will have 50 and value displayed on center line will be 50.00 not 100.00 
            //      like before
            // 2 - redraw grid/boxes visual size and recalculate drawed shapes in correct points positions 
            // SUMMARY THIS: I think this '1' option will be better, but for right now will be good have this two 
            // options, second one can be commented or something like that
            int index = -1;
            // here is crushed when grid size is changed, from what i see it was error:
            // 
            // 'The specified Visual object is child of another Visual or CompositionTarget root item.'
            // I checked in google, and i found something for this, is about remove and add again this 'grid' -> https://stackoverflow.com/questions/2970446/specified-visual-is-already-a-child-of-another-visual-or-the-root-of-a-compositi
            // and is looks like this
            if (paintSurface.Children.Contains(grid))
            {
                index = paintSurface.Children.IndexOf(grid);
                paintSurface.Children.Remove(grid);
            }
            if (index != -1)
                paintSurface.Children.Insert(index, grid);
            else
                paintSurface.Children.Add(grid);
        }



        //cursor mode used for drawing
        private void crosshair_Button_Click(object sender, RoutedEventArgs e)
        {
            drawingCursor = true;
            MovingCursor = false;
            crosshairbtn.IsEnabled = false;
            selectorbtn.IsEnabled = true;
            paintSurface.Cursor = System.Windows.Input.Cursors.Cross;
        }



        //cursor mode used for drag and drop
        private void selector_Button_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = System.Windows.Input.Cursors.Arrow;
            MovingCursor = true;
            drawingCursor = false;
            crosshairbtn.IsEnabled = true;
            selectorbtn.IsEnabled = false; 
        }



        //Calculating Nearest Vertex to Snap Point
        private Point SnapCalculate(Point p, Size s)
        {
            double snapX = p.X + ((Math.Round(p.X / s.Width) - p.X / s.Width) * s.Width);
            double snapY = p.Y + ((Math.Round(p.Y / s.Height) - p.Y / s.Height) * s.Height);
            return new Point(snapX, snapY);
        }



        //canvas click up, draw a circle on the initial point, set initial point and turn on ISDRAGGING flag
        private void Canvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
            // get current mouse click position. ABOUT clicking on a vertex and if position is correct, this is vertex then should be 
            // started redraw lines and go with mouse just like while drawing line and when user again click mouse then this two lines should
            // be redrawed and also line value should be updated and if have background then also should be updated/redrawed or something like that
            //var pos = e.GetPosition(sender as FrameworkElement);
            //if (isDragging && MovingCursor)
            //{
            //    isDragging = false;
            //    //var draggable = sender as Shape;
            //    //draggable.ReleaseMouseCapture();
            //}

            if (drawingCursor)
            {
                CloseColorPalleteSetColor();
                DrawingonMouseUp(sender, e);
            }
        }
        



        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MovingCursor)
            {
                source = (System.Windows.Controls.Canvas)sender;
                Mouse.Capture(source);
                captured = true;
                x_shape = Canvas.GetLeft(source);
                x_canvas = e.GetPosition(paintSurface).X;
                y_shape = Canvas.GetTop(source);
                y_canvas = e.GetPosition(paintSurface).Y;
                //MovingObjectonClick(sender, e);
            }
        }




        //Mouse move function to create lines when after click we move mouse on canvas, if ISDRAGGING is true
        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if (MovingCursor)
            //{
                if (captured)
                {
                    double x = e.GetPosition(paintSurface).X;
                    double y = e.GetPosition(paintSurface).Y;
                    x_shape += x - x_canvas;
                    Canvas.SetLeft(source, x_shape);
                    x_canvas = x;
                    y_shape += y - y_canvas;
                    Canvas.SetTop(source, y_shape);
                    y_canvas = y;
                }
                //we will put the drag and drop code here for MOUSE MOVE EVENT
            //}
            if (drawingCursor)
                DrawingonMouseMove(sender, e); //Calling on drawing method
        }
        


        //creating elipse and calling it on every mouseclickUP call in MOUSE CLICK UP EVENT
        public void CreateAnEllipse(Point point)
        {
            // Create an Ellipse
            Ellipse ellipse = new Ellipse();
            // Create a black Brush    
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            // Set Ellipse's Height/width and color    
            ellipse.Height = ellipse.Width = boxsize/2;
            ellipse.StrokeThickness = 1;
            ellipse.Stroke = blackBrush;
            //Set Ellipse Position on the anchor points
            Canvas.SetTop(ellipse, point.Y - boxsize / 4);
            Canvas.SetLeft(ellipse, point.X - boxsize / 4);
            // Add Ellipse to out canvas
            paintSurface.Children.Add(ellipse);
        }



        
        //Show Color Pallete
        private void Color_Button_Click(object sender, RoutedEventArgs e)
        {
            win = new ColorSwatch();
            win.Show();
        }
        
        
        
        
        //Close Color Pallete
        private void CloseColorPalleteSetColor()
        {
            if (win != null)
            {
                win.Close();
                colorPalletCanvas.Background = Measurements.GetColor();
            }
        }




        //Drawing POINT TO POINT whenever a point is clicked in canvas
        private void DrawingonMouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            if (IsDrawing)
            {
                // if isdrawing is true and we click then gets the current cursor position and mark it as a point of shape
                TempShape.ShapeCords.Add(SnapCalculate(e.GetPosition(paintSurface), size));

                //creating new line and label variables as  ON EVERY POINT CREATION old are being added.
                myline = new Line();
                length = new Label();
                AreaLB = new Label();

                //if canvas don't contain the current marked label which is NEW then we will add this label to canvas
                if (!paintSurface.Children.Contains(length))
                {
                    length.MouseEnter += Length_GotMouseCapture;

                    paintSurface.Children.Add(length);
                    //CurrentShape.Controls.Add(length);

                }

                //checking if current snap is Closing our shape
                if (SnapCalculate(e.GetPosition(paintSurface), size) == TempShape.ShapeCords.First<Point>())
                {
                    //if this is a closing SNAP then we will false the drawing flag and fill the data into shape ATTRIBUTES
                    IsDrawing = false;
                    if (TempShape.ShapeCords.Count != 0)
                    {
                    
                        //making point collection for making a polygon, as polygon is the only way to create random shape with background color.
                        PointCollection pc = new PointCollection();
                        foreach (Point p in TempShape.ShapeCords)
                        {
                            pc.Add(p);
                        }

                        //Making a Polygon out of points to fill and mark as a shape!!!
                        Polygon polygon = new Polygon
                        {
                            Fill = Measurements.GetColor(),
                            Points = pc
                        };
                        //polygon.MouseDown += MovingObjectonClick;
                        polygon.MouseMove += pol_OnMouseMove;

                        //Adding created shape data to out SHAPEOBJECT object.
                        CurrentShape.ShapeCords = TempShape.ShapeCords;
                        CurrentShape.NumberOfLines = TempShape.ShapeCords.Count;
                        CurrentShape.Area = Measurements.GetArea(TempShape);
                        CurrentShape.Center = Measurements.GetCenter(TempShape);
                        CurrentShape.Shape = polygon;
                        //Area Label
                        AreaLB.Content = "Area: " + CurrentShape.Area.ToString() + "m\xB2";
                        AreaLB.FontSize = boxsize / 2;
                        double topy = CurrentShape.Center.Y - (AreaLB.FontSize * 2);
                        double lefy = CurrentShape.Center.X - (AreaLB.FontSize * 2);
                        Canvas.SetTop(AreaLB, topy);
                        Canvas.SetLeft(AreaLB, lefy);


                        paintSurface.Children.Add(polygon);
                        

                        paintSurface.Children.Add(AreaLB);

                        AddTexttotheShape(sender, e, CurrentShape.Center);
                        //CurrentShape.Controls.Add(length);
                        CurrentShape.Controls.Add(AreaLB);

                        MyShapeList.Add(CurrentShape);      //Adding my created shape to the main List

                        TempShape.ShapeCords.Clear();               //clearing tempshape to store new shapes
                    }
                }
                else
                {
                    //In else clause so it don't create another circle on first point
                    CreateAnEllipse(SnapCalculate(e.GetPosition(paintSurface), size));
                }

            }
            else    //if is drawing is false means we have to start creating a shape then we will come to this else and start shape creation
            {
                start = e.GetPosition(paintSurface);
                CreateAnEllipse(SnapCalculate(start, size));
                IsDrawing = true;
                TempShape.ShapeCords.Add(SnapCalculate(start, size));
            }
        }

        private void DrawingonMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsDrawing )
            {
                //removing the line because when we move the cursor, on every single decimal point,
                //when we move the cursor, it will create a shape, so removing this,
                //we will only add stuff to canvas on the CLICKEVENT
                paintSurface.Children.Remove(myline);
                myline = new Line();
                myline.X1 = SnapCalculate(TempShape.ShapeCords.Last<Point>(), size).X;
                myline.Y1 = SnapCalculate(TempShape.ShapeCords.Last<Point>(), size).Y;
                myline.X2 = SnapCalculate(e.GetPosition(paintSurface), size).X;
                myline.Y2 = SnapCalculate(e.GetPosition(paintSurface), size).Y;
                myline.Stroke = new SolidColorBrush(Colors.Black);
                paintSurface.Children.Add(myline);
                
                //Every Line Length
                double Distance = Measurements.LineDistance(SnapCalculate(TempShape.ShapeCords.Last<Point>(), size), SnapCalculate(e.GetPosition(paintSurface), size));
                length.Content = Distance.ToString("0.00");
                length.FontSize = boxsize / 2;
                double fromtop = ((myline.Y2 - myline.Y1) / 2) + myline.Y1;
                double fromleft = ((myline.X2 - myline.X1) / 2) + myline.X1;

                double xDiff = myline.X2 - myline.X1;
                double yDiff = myline.Y2 - myline.Y1;
                double angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

                if (angle > 90 || angle < -90)
                {
                    angle = (angle) + 180;
                }

                Canvas.SetTop(length, fromtop);
                Canvas.SetLeft(length, fromleft);

                RotateTransform rotateTransform1 = new RotateTransform(angle);
                length.RenderTransform = rotateTransform1;
                paintSurface.Children.Remove(length);
                CurrentShape.Controls.Remove(length);
                if (!paintSurface.Children.Contains(length) && !CurrentShape.Controls.Contains(length))
                {
                    length.MouseEnter += Length_GotMouseCapture;
                    paintSurface.Children.Add(length);
                    CurrentShape.Controls.Add(length);

                }

            }
        }

        private void Length_GotMouseCapture(object sender, MouseEventArgs e)
        {
            Label len = sender as Label;
            len.Content = "Working";
        }

        #region ShapeName
        private void AddTexttotheShape(object sender, System.Windows.Input.MouseEventArgs e, Point Center)
        {
            TextBox name = new TextBox();
            name.HorizontalAlignment = HorizontalAlignment.Center;
            name.VerticalAlignment = VerticalAlignment.Center;
            name.FontSize = 12;
            SolidColorBrush mySolidColorBrush = Measurements.GetColor();
            mySolidColorBrush.Color = InvertColor(mySolidColorBrush.Color);
            name.Foreground = mySolidColorBrush;
            name.Background = null;
            name.TextChanged += newText_TextChanged;
            name.KeyDown += Enter_KeyDown;

            double topy = CurrentShape.Center.Y;
            double lefy = CurrentShape.Center.X;
            Canvas.SetTop(name, topy);
            Canvas.SetLeft(name, lefy);
            paintSurface.Children.Add(name);
        }
        private void Enter_KeyDown(object sender, EventArgs e)
        {
            TextBox name = sender as TextBox;
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                CurrentShape.Name = name.Text;
                name.IsEnabled = false;
            }
        }
        private void newText_TextChanged(object sender, EventArgs e)
        {
            TextBox name = sender as TextBox;
            name.UpdateLayout();

        }
        #endregion

        private void SelectObjectonClick(object sender, MouseButtonEventArgs e)
        {
            Polygon pol = e.Source as Polygon;
            if (MovingCursor && pol != null)
            {
                //pol.Fill = Brushes.SlateGray;
                foreach (ShapeObject so in MyShapeList)
                {
                    if (pol == so.Shape)
                    {
                        Console.WriteLine(so.Name);
                        foreach (Control ctr in so.Controls)
                        {
                            ctr.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        foreach (Control ctr in so.Controls)
                        {
                            ctr.Visibility = Visibility.Hidden;
                        }
                    }
                }
                Console.WriteLine("Mouse Entered");

            }
        }


        private void pol_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is Shape shape)
            {
                if (e.LeftButton == MouseButtonState.Pressed && MovingCursor)
                {
                    Cursor = Cursors.Hand;
                    Point p = e.GetPosition(paintSurface);
                    Canvas.SetLeft(shape, p.X - shape.ActualWidth / 2 );
                    Canvas.SetTop(shape,  p.Y - shape.ActualHeight / 2);
                    shape.CaptureMouse();
                }
                else
                {
                    shape.ReleaseMouseCapture();
                }
            }
        }



        Color InvertColor(Color sourceColor)
        {
            return Color.FromArgb((byte)(255),
                                  (byte)(255 - sourceColor.R),
                                  (byte)(255 - sourceColor.G),
                                  (byte)(255 - sourceColor.B));
        }







    }
}
