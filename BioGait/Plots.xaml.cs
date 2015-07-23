using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IronPlot;


namespace BioGait
{
    /// <summary>
    /// Interaction logic for Plots.xaml
    /// </summary>
    public partial class Plots : System.Windows.Controls.UserControl, ISwitchable
    {
        
        public Plots(List<double> list1, List<double> list2, List<double> list3, List<double> list4, List<double> list5, List<double> list6, List<double> list7, List<double> list8, List<double> list9, List<double> list10, List<Coordinate> list11)
        {
            InitializeComponent();

            PlotOne(list1);
            PlotTwo(list2,list3);
            PlotThree(list4,list5);
            PlotFour(list6,list7);
            PlotFive(list8,list9);
            PlotSix(list10);
            PlotSeven(list11);

            this.Loaded += Plots_Loaded;

        }

        void Plots_Loaded(object sender, RoutedEventArgs e)
        {
            var controls = Utilities.GetLogicalChildren<UIElement>(this).ToList();
        //    CbxUIElements.ItemsSource = controls.Select(x => x.GetValue(NameProperty));
        }

        private void savePlotButtonClick(object sender, RoutedEventArgs e)
        {
            // Add code to perform some action here.
            var control = this.FindName("TabLayout") as DependencyObject;
            UIElement UiElement = control as UIElement;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png files (*.png)|*.png|Bmp Files (*.bmp)|*.bmp";
            var result = saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (saveFileDialog.DefaultExt == ".bmp")
                {
                    ImageCapturer.SaveToBmp((UiElement as FrameworkElement), saveFileDialog.FileName);
                }
                else
                {
                    ImageCapturer.SaveToPng((UiElement as FrameworkElement), saveFileDialog.FileName);
                }
            }
        }

        void PlotOne(List<double> list)
        {
            int n = list.Count;
            double[] x = new double[n];
            double[] y = new double[n];
            
            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = list[i];
            }
           
            plotone.AddLine(x, y).Title="Pelvic_obliquity";
            plotone.Axes.YAxes.Left.FormatOverride = FormatOverrides.Currency;
            
            plotone.BottomLabel.Text = "Time";
            plotone.LeftLabel.Text = "Pelvic_obliquity";
            plotone.FontSize = 14;
            
            plotone.Axes.XAxes[0].FontStyle = plotone.Axes.YAxes[0].FontStyle = FontStyles.Oblique;
            
            plotone.Axes.XAxes.Top.TickLength = 5;
            plotone.Axes.YAxes.Left.TickLength = 5;

            plotone.UseDirect2D = true;
        }

        void PlotSix(List<double> list)
        {
            int n = list.Count;
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = list[i];
            }

            plotsix.AddLine(x, y).Title = "Pelvic_rotation";

            plotsix.Axes.YAxes.Left.FormatOverride = FormatOverrides.Currency;

            plotsix.BottomLabel.Text = "Time";
            plotsix.LeftLabel.Text = "Pelvic_rotation";
            plotsix.FontSize = 14;

            plotsix.Axes.XAxes[0].FontStyle = plotsix.Axes.YAxes[0].FontStyle = FontStyles.Oblique;

            plotsix.Axes.XAxes.Top.TickLength = 5;
            plotsix.Axes.YAxes.Left.TickLength = 5;

            plotsix.UseDirect2D = true;
        }


        void PlotTwo(List<double> list1, List<double> list2)
        {
            int n = list1.Count;
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = list1[i];
            }

            Plot2DCurve curve = plottwo.AddLine(x, y);

            curve.Stroke = Brushes.Blue; curve.StrokeThickness = 1.5; curve.Title = "Hip abduction Left";

            int n2 = list2.Count;

            double[] x2 = new double[n2];
            double[] y2 = new double[n2];

            for (int i = 0; i < n2; i++)
            {
                x2[i] = i;
                y2[i] = list2[i];
            }
            Plot2DCurve curve2 = new Plot2DCurve(new Curve(x2, y2)) { QuickLine = "r-" };
            plottwo.Children.Add(curve2);
            curve2.Title = "Hip abduction Right";
           
            plottwo.Legend.Background = Brushes.White;
            plottwo.BackgroundPlot = Brushes.White;

            plottwo.BottomLabel.Text = "Time";
            plottwo.LeftLabel.Text = "Hip Abduction-adduction";
            plottwo.FontSize = 14;
           
            plottwo.Axes.XAxes[0].FontStyle = plottwo.Axes.YAxes[0].FontStyle = FontStyles.Oblique;

            plottwo.Axes.XAxes.Top.TickLength = 5;
            plottwo.Axes.YAxes.Left.TickLength = 5;

            plottwo.UseDirect2D = true;
        }

        void PlotThree(List<double> list1, List<double> list2)
        {
            int n = list1.Count;
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = list1[i];
            }

            Plot2DCurve curve = plotthree.AddLine(x, y);

            curve.Stroke = Brushes.Blue; curve.StrokeThickness = 1.5;
            curve.Title = "Knee flexion left";
            int n2 = list2.Count;

            double[] x2 = new double[n2];
            double[] y2 = new double[n2];

            for (int i = 0; i < n2; i++)
            {
                x2[i] = i;
                y2[i] = list2[i];
            }
            Plot2DCurve curve2 = new Plot2DCurve(new Curve(x2, y2)) { QuickLine = "r-" };
            curve2.Title = "Knee flexion Right";
            plotthree.Children.Add(curve2);
            plotthree.Legend.Background = Brushes.White;
            plotthree.BackgroundPlot = Brushes.White;

            plotthree.BottomLabel.Text = "Time";
            plotthree.LeftLabel.Text = "Knee Flexion-Extension";
            plotthree.FontSize = 14;
            
            plotthree.Axes.XAxes[0].FontStyle = plotthree.Axes.YAxes[0].FontStyle = FontStyles.Oblique;

            plotthree.Axes.XAxes.Top.TickLength = 5;
            plotthree.Axes.YAxes.Left.TickLength = 5;

            plotthree.UseDirect2D = true;
        }

        void PlotFour(List<double> list1, List<double> list2)
        {
            int n = list1.Count;
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = list1[i];
            }

            Plot2DCurve curve = plotfour.AddLine(x, y);

            curve.Stroke = Brushes.Blue; curve.StrokeThickness = 1.5;
            curve.Title="Hip flexion left";

            int n2 = list2.Count;

            double[] x2 = new double[n2];
            double[] y2 = new double[n2];

            for (int i = 0; i < n2; i++)
            {
                x2[i] = i;
                y2[i] = list2[i];
            }
            Plot2DCurve curve2 = new Plot2DCurve(new Curve(x2, y2)) { QuickLine = "r-" };
            plotfour.Children.Add(curve2);
            curve2.Title = "Hip flexion right";
            plotfour.Legend.Background = Brushes.White;
            plotfour.BackgroundPlot = Brushes.White;

            plotfour.BottomLabel.Text = "Time";
            plotfour.LeftLabel.Text = "Hip flexion-extension";
            plotfour.FontSize = 14;
            
            plotfour.Axes.XAxes[0].FontStyle = plotfour.Axes.YAxes[0].FontStyle = FontStyles.Oblique;

            plotfour.Axes.XAxes.Top.TickLength = 5;
            plotfour.Axes.YAxes.Left.TickLength = 5;

            plotfour.UseDirect2D = true;
        }

        void PlotFive(List<double> list1, List<double> list2)
        {
            int n = list1.Count;
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = list1[i];
            }

            Plot2DCurve curve = plotfive.AddLine(x, y);

            curve.Stroke = Brushes.Blue; curve.StrokeThickness = 1.5;
            curve.Title = "Ankle flexion left";
            int n2 = list2.Count;

            double[] x2 = new double[n2];
            double[] y2 = new double[n2];

            for (int i = 0; i < n2; i++)
            {
                x2[i] = i;
                y2[i] = list2[i];
            }
            Plot2DCurve curve2 = new Plot2DCurve(new Curve(x2, y2)) { QuickLine = "r-" };
            plotfive.Children.Add(curve2);
            curve2.Title = "Ankle flexion right";
            plotfive.Legend.Background = Brushes.White;
            plotfive.BackgroundPlot = Brushes.White;

            plotfive.BottomLabel.Text = "Time";
            plotfive.LeftLabel.Text = "Ankle Dors-plantarflexion";
            plotfive.FontSize = 14;
            
            plotfive.Axes.XAxes[0].FontStyle = plotfive.Axes.YAxes[0].FontStyle = FontStyles.Oblique;

            plotfive.Axes.XAxes.Top.TickLength = 5;
            plotfive.Axes.YAxes.Left.TickLength = 5;

            plotfive.UseDirect2D = true;
        }

        void PlotSeven(List<Coordinate> list)
        {
            int n = list.Count;
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = list[i].Y;
                //System.Console.WriteLine("y is "+y[i]);
            
            }

            Plot2DCurve curve = plotseven.AddLine(x, y);

            curve.Stroke = Brushes.Blue; curve.StrokeThickness = 1.5;
            curve.Title = "Ground Reaction Forces";
            
            plotseven.Legend.Background = Brushes.White;
            plotseven.BackgroundPlot = Brushes.White;

            plotseven.BottomLabel.Text = "Time";
            plotseven.LeftLabel.Text = "Ground reaction force(Vertical)";
            plotseven.FontSize = 14;

            plotseven.Axes.XAxes[0].FontStyle = plotseven.Axes.YAxes[0].FontStyle = FontStyles.Oblique;

            plotseven.Axes.XAxes.Top.TickLength = 5;
            plotseven.Axes.YAxes.Left.TickLength = 5;

            plotseven.UseDirect2D = true;
        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }


        #endregion

    }

       

}
