using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FinalProjectApp.Classes
{
    public class CreateOptionsPlot
    {
        private Canvas canGraph;
        private const double margin = 40;
        private double xmin = margin;
        private double ymin = margin;
        private double xmax, ymax;
        private double xStep, yStep;
        private double xScale, yScale;
        private double xUpperLimit, xLowerLimit, yUpperLimit, yLowerLimit;
        private double minRisk, maxRisk;
        private double midVal, minVal, maxVal;
        private bool labelSwitch;
        List<Option> options = new List<Option>();

        public Canvas CreateGraph(Canvas canvas, List<Option> optionsList, double xUL, double xLL, 
            double yUL, double yLL, double minV, double maxR, bool label)
        {
            //Set class variables;
            canGraph = canvas;
            xmax = canGraph.Width - margin*1.5;
            ymax = canGraph.Height - margin;
            xStep = xmax / 10;
            yStep = ymax / 10;
            xUpperLimit = xUL;
            xLowerLimit = xLL;
            yUpperLimit = yUL;
            yLowerLimit = yLL;
            minRisk = minV;
            maxRisk = maxR;
            xScale = xmax / (xUpperLimit - xLowerLimit);
            yScale = ymax / (yUpperLimit - yLowerLimit);
            options = optionsList;
            labelSwitch = label;

            //Generate the X Axis
            genXAxis();
            //Generate the Y Axis
            genYAxis();
            //Generate investment strategy regions
            genRegions();
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
               new Point(margin, ymin), new Point(xmax, ymin)));
            int count = 1;
            for (double x = xStep + margin; x <= canGraph.Width; x += xStep)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymin - margin / 4),
                    new Point(x, ymin + margin / 4)));

                double stepVal = xLowerLimit + (((xUpperLimit - xLowerLimit) / 10) * count);
                TextBlock text = new TextBlock();
                text.Text = Math.Round(stepVal, 2).ToString();
                canGraph.Children.Add(text);
                Canvas.SetLeft(text, (x)-4);
                Canvas.SetBottom(text, ymax + (margin / 3));
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
                new Point(xmin, margin), new Point(xmin, ymin)));
            double count = 10;
            for (double y = yStep + margin; y <= canGraph.Height; y += yStep)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin - margin / 4, y),
                new Point(xmin + margin / 4, y)));

                double stepVal = yLowerLimit + (((yUpperLimit - yLowerLimit) / 10) * count);
                TextBlock text = new TextBlock();
                text.Text = Math.Round(stepVal, 2).ToString()+"%";
                canGraph.Children.Add(text);
                Canvas.SetLeft(text, xmin - margin);
                Canvas.SetBottom(text, ((ymin-(margin*4)) + y) + 33);
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
            for (double i = ymin; i <= canGraph.Height; i += (yStep / 2))
            {
                Line xGrid = new Line() { X1 = xmin, X2 = xmax + margin,
                    Y1 = i, Y2 = i, StrokeThickness = 1, Stroke = Brushes.Gray };
                canGraph.Children.Add(xGrid);
            }
            //Create the y axis grid lines
            for (double i = xmin; i <= xmax + margin; i += (xStep / 2))
            {
                Line yGrid = new Line() { Y1 = ymin, Y2 = canGraph.Height,
                    X1 = i, X2 = i, StrokeThickness = 1, Stroke = Brushes.Gray };
                canGraph.Children.Add(yGrid);
            }
        }

        private void genRegions()
        {
            try
            {
                //LeftSideOfGraph
                midVal = ((1 - xLowerLimit) * xScale) + margin;
                if (xLowerLimit < 1)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Height = canGraph.Height - margin,
                        Width = midVal - margin,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        Fill = Brushes.Yellow,
                        MaxHeight = canGraph.Height - margin,
                        MaxWidth = xmax
                    };
                    canGraph.Children.Add(rect);
                    Canvas.SetLeft(rect, xmin);
                    Canvas.SetTop(rect, ymin);
                }
            }
            catch { }
            try
            {
                //RightSideOfGraph
                if (xUpperLimit > 1)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Height = canGraph.Height - margin,
                        Width = (xmax - midVal) + margin,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        Fill = Brushes.GreenYellow,
                        MaxHeight = canGraph.Height - margin,
                        MaxWidth = xmax
                    };
                    canGraph.Children.Add(rect);
                    Canvas.SetRight(rect, xmin-margin/2);
                    Canvas.SetTop(rect, ymin);
                }
            }
            catch { }
            try
            {
                //LowerNeverRegion
                minVal = ((minRisk - yLowerLimit) * yScale) + margin;
                if (xLowerLimit < 1)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Height = minVal - margin,
                        Width = midVal - margin,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        Fill = Brushes.Red,
                        MaxHeight = canGraph.Height - margin,
                        MaxWidth = xmax
                    };
                    canGraph.Children.Add(rect);
                    Canvas.SetLeft(rect, xmin);
                    Canvas.SetTop(rect, ymin);
                }
            }
            catch { }
            try
            {
                //NowRegion
                minVal = ((minRisk - yLowerLimit) * yScale) + margin;
                if (xUpperLimit > 1)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Height = minVal - margin,
                        Width = (xmax - midVal) + margin,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        Fill = Brushes.Green,
                        MaxHeight = canGraph.Height - margin,
                        MaxWidth = xmax
                    };
                    canGraph.Children.Add(rect);
                    Canvas.SetRight(rect, xmin - margin / 2);
                    Canvas.SetTop(rect, ymin);
                }
            }
            catch { }
            try
            {
                //UpperNeverRegion
                maxVal = ymax - ((yUpperLimit - maxRisk) * yScale) + margin;
                if (yUpperLimit > maxRisk)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Height = (ymax - maxVal) + margin,
                        Width = xmax,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        Fill = Brushes.Red,
                        MaxHeight = canGraph.Height,
                        MaxWidth = xmax
                    };
                    canGraph.Children.Add(rect);
                    Canvas.SetLeft(rect, xmin);
                    Canvas.SetBottom(rect, ymin - margin);
                }
            }
            catch { }
            try
            {
                //Probably Never / Maybe Later Line
                if (xLowerLimit < 1)
                {
                    double yStepVal = ((yUpperLimit - yLowerLimit) / 10) / yStep;
                    double yPos = maxRisk / yStepVal;

                    Line probNeverLine = new Line()
                    {
                        X1 = midVal,
                        Y1 = minVal,
                        X2 = xmin,
                        Y2 = yPos + margin,
                        StrokeThickness = 3,
                        Stroke = Brushes.Black,
                        MaxHeight = canGraph.Height,
                        MaxWidth = canGraph.Width - margin/2

                    };
                    canGraph.Children.Add(probNeverLine);
                }
            }
            catch { }
            try
            {
                //Probably Later / Maybe Now Line
                if (xUpperLimit > 1)
                {
                    double yStepVal = ((yUpperLimit - yLowerLimit) / 10) / yStep;
                    double xStepVal = ((xUpperLimit - xLowerLimit) / 10) / xStep;
                    double yPos = maxRisk / yStepVal;
                    double xPos = 2 / xStepVal;
                    if (yUpperLimit < 100) { xPos -= margin*2; }

                    Line probLaterLine = new Line()
                    {
                        X1 = midVal,
                        Y1 = minVal,
                        X2 = xPos + margin,
                        Y2 = yPos + margin,
                        StrokeThickness = 3,
                        Stroke = Brushes.Black,
                        MaxHeight = canGraph.Height,
                        MaxWidth = canGraph.Width - margin/2
                    };
                    canGraph.Children.Add(probLaterLine);
                }
            }
            catch { }
        }

        private void populateOptions()
        {
            foreach (var option in options)
            {
                int width = 20;
                int height = 20;
                double xPos = xScale * (option.ValueToCostRatio - xLowerLimit) + (width);
                double yPos = yScale * (option.Volatility - yLowerLimit) + yStep - 1;
                string optionText = option.Name + ": " + option.Description + Environment.NewLine
                    + "ValueToCostRatio: " + option.ValueToCostRatio + " Volatility: " + option.Volatility;

                RectangleGeometry myRectangleGeometry = new RectangleGeometry();
                myRectangleGeometry.Rect = new Rect() { X = xPos + (width / 2), Y = yPos - (height / 2), Width = width, Height = height };
                myRectangleGeometry.RadiusX = 25;
                myRectangleGeometry.RadiusY = 25;

                Path myPath = new Path();
                myPath.Fill = Brushes.LemonChiffon;
                myPath.Stroke = Brushes.Black;
                myPath.StrokeThickness = 1;
                myPath.Data = myRectangleGeometry;
                myPath.ToolTip = optionText;

                canGraph.Children.Add(myPath);

                if (labelSwitch == true)
                {
                    TextBlock label = new TextBlock()
                    {
                        Background = Brushes.White,
                        Foreground = Brushes.Black,
                        FontSize = 12,
                        Text = option.Name,
                        Focusable = true
                    };
                    canGraph.Children.Add(label);
                    canGraph.UpdateLayout();

                    label.Measure(new Size(label.ActualWidth, label.ActualHeight));

                    if (xPos < canGraph.Width / 2) { Canvas.SetLeft(label, xPos + 15); }
                    else { Canvas.SetLeft(label, xPos - label.ActualWidth + 15); }

                    if (yPos < canGraph.Height / 2) { Canvas.SetTop(label, yPos + 10); }
                    else { Canvas.SetTop(label, yPos - 25); }
                }
            }
        }
    }
}
