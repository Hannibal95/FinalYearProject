using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FinalProjectApp.Classes
{
    public class CreateCallValuesChart
    {
        private Canvas canGraph;
        private const double margin = 60;
        private double xmin = margin;
        private double ymin = margin;
        private double xmax, ymax;
        private double xStep, yStep;
        private double yScale;
        private double yUpperLimit, yLowerLimit;
        private bool labelSwitch;
        List<Option> options = new List<Option>();
        ArrayList xCountArray;

        public Canvas CreateGraph(Canvas canvas, List<Option> optionsList, double yUL, double yLL, bool label)
        {
            canGraph = canvas;
            xmax = canGraph.Width - xmin;
            ymax = canGraph.Height - ymin;
            yUpperLimit = yUL;
            yLowerLimit = yLL;
            yScale = ymax / (yUpperLimit - yLowerLimit);
            options = optionsList;
            xStep = (xmax - xmin) / options.Count();
            yStep = ymax / 10;
            labelSwitch = label;

            xCountArray = new ArrayList();

            //Generate the X Axis
            genXAxis();
            //Generate the Y Axis
            genYAxis();
            //Generate the graph grid
            genGrid();
            //Populate graph with Options
            populateOptions();

            return canGraph;
        }

        private void genXAxis()
        {
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
               new Point(margin, ymax), new Point(xmax, ymax)));

            double x = xmin;
            int length = options.Count();
            int count = 0;
            foreach (var item in options)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 8),
                    new Point(x, ymax + margin / 8)));

                xCountArray.Add(x);

                string stepName = item.Name;
                TextBlock text = new TextBlock();
                text.Text = stepName;
                text.Background = Brushes.White;
                text.RenderTransformOrigin = new Point(0,0);
                canGraph.Children.Add(text);
                Canvas.SetLeft(text, x-1);
                Canvas.SetBottom(text, ymin - 25);

                RotateTransform myRotateTransform = new RotateTransform(10);
                text.RenderTransform = myRotateTransform;

                if (x <= xmax) { x += xStep; }
                count++;
            }

            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            canGraph.Children.Add(xaxis_path);
        }

        private void genYAxis()
        {
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin, ymin-margin), new Point(xmin, ymax)));
            double count = 10;

            TextBlock minVal = new TextBlock();
            minVal.Text = Math.Round(yLowerLimit, 2).ToString();
            canGraph.Children.Add(minVal);
            Canvas.SetLeft(minVal, xmin - margin + 2);
            Canvas.SetBottom(minVal, canGraph.Height - (ymax+8));

            for (double y = ymax + yStep; y >= ymin; y -= yStep)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin - margin / 8, y - yStep),
                new Point(xmin + margin / 8, y - yStep)));

                double stepVal = yLowerLimit + (((yUpperLimit - yLowerLimit) / 10) * count);
                TextBlock text = new TextBlock();
                text.Text = Math.Round(stepVal, 2).ToString();
                canGraph.Children.Add(text);
                Canvas.SetLeft(text, xmin - margin + 2);
                Canvas.SetBottom(text, y + yStep - 25);
                count--;
            }
            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            canGraph.Children.Add(yaxis_path);
        }

        private void genGrid()
        {
            //Create the x axis grid lines
            for (double i = ymin-margin; i <= ymax; i += (yStep / 2))
            {
                Line xGrid = new Line()
                {
                    X1 = xmin,
                    X2 = xmax,
                    Y1 = i,
                    Y2 = i,
                    StrokeThickness = 1,
                    Stroke = Brushes.Gray
                };
                canGraph.Children.Add(xGrid);
            }
        }

        private void populateOptions()
        {

            ArrayList colours = new ArrayList()
            {
                "#002847", "#003d6b", "#00518f", "#0066b3", "#3284c2", "#66a3d1"
            };

            int count = 0;
            int colNum = 0;
            foreach (var option in options)
            {
                double yPos = (yScale * (option.CallValue - yLowerLimit));
                string optionText = option.Name + ": " + option.Description + Environment.NewLine
                    + "CallValue: " + option.CallValue + " Volatility: " + option.Volatility;
                double xPos = Convert.ToDouble(xCountArray[count]);

                double width;
                try { width = Convert.ToDouble(xCountArray[count + 1]) - xPos; }
                catch { width = xmax - xPos; }

                double height = 0 + yPos;

                RectangleGeometry myRectangleGeometry = new RectangleGeometry();
                myRectangleGeometry.Rect = new Rect() { X = xPos, Y = ymax - height, Width = width, Height = height };

                Path myPath = new Path();
                myPath.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(colours[colNum]));
                myPath.Stroke = Brushes.Black;
                myPath.StrokeThickness = 1;
                myPath.Data = myRectangleGeometry;
                myPath.ToolTip = optionText;

                canGraph.Children.Add(myPath);
                count++;
                if (colNum < colours.Count) { colNum++; }
                else { colNum = 0; }

                if (labelSwitch == true)
                {
                    TextBlock label = new TextBlock()
                    {
                        Background = Brushes.White,
                        Foreground = Brushes.Black,
                        FontSize = 12,
                        Text = Math.Round(option.CallValue, 2).ToString(),
                        Focusable = true
                    };
                    canGraph.Children.Add(label);
                    canGraph.UpdateLayout();

                    label.Measure(new Size(label.ActualWidth, label.ActualHeight));

                    Canvas.SetLeft(label, xPos+(width/2)-(label.ActualWidth/2)); 
                    Canvas.SetBottom(label, yPos + yStep); 
                }
            }
        }
    }
}
