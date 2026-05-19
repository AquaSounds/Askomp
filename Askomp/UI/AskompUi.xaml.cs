using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Askomp.UI;

public partial class AskompUi : UserControl
{
    private Askomp Plugin { get; init; }
    
    public AskompUi(Askomp plugin)
    {
        InitializeComponent();
        Plugin = plugin;
        LinkButtonInit();
        InitWave();
        RatioKnob.SizeChanged += (o, args) =>
        {
            DrawArc(RatioKnob, _ratioKnobV, RatioKnobMinV, RatioKnobMaxV, RatioKnobCanvas, 5);
        };
        AttackKnob.SizeChanged += (o, args) =>
        {
            DrawArc(AttackKnob, _attackKnobV, AttackKnobMinV, AttackKnobMaxV, AttackKnobCanvas, 3);
        };
        ReleaseKnob.SizeChanged += (o, args) =>
        {
            DrawArc(ReleaseKnob, _releaseKnobV, ReleaseKnobMinV, ReleaseKnobMaxV, ReleaseKnobCanvas, 3);
        };
    }

    #region Positive Threshold Mouse Logic
    
    private void PosThMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is not Border parent) return;
        if(parent.Child is Border child)
        {
            child.BeginAnimation(OpacityProperty,new DoubleAnimation(1,new Duration(TimeSpan.FromSeconds(0.1))));
            PosThNum.BeginAnimation(OpacityProperty,new DoubleAnimation(1,new Duration(TimeSpan.FromSeconds(0.1))));
        }
    }

    private void PosThMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is not Border parent) return;
        if(parent.Child is Border child)
        {
            child.BeginAnimation(OpacityProperty,new DoubleAnimation(0.2,new Duration(TimeSpan.FromSeconds(0.1))));
            PosThNum.BeginAnimation(OpacityProperty,new DoubleAnimation(0,new Duration(TimeSpan.FromSeconds(0.1))));
        }
    }
    
    private void PosThMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is Border border && border.IsMouseCaptured)
        {
            if (border.Parent is Grid)
            {
                var min = 0;
                var max = WaveGrid.ActualHeight/2 - PosBorder.ActualHeight;
                var value = double.Clamp(e.GetPosition(WaveGrid).Y - PosBorder.ActualHeight/2,min,max);
                value = value / (max-min) * -36;
                SetPosThValue(value, 0);
            }
        };
    }

    //使用source防止递归
    public void SetPosThValue(double value, int source)
    {
        var min = 0;
        var max = WaveGrid.ActualHeight/2 - PosBorder.ActualHeight;
        var pos = value / -36 * (max - min);
        PosBorder.Margin = new Thickness(0, pos, 0, 0);
        PosThNum.Text = value.ToString("0.0 dB");
        Plugin.SetPosTh(value);
        if(_linked && source == 0)
            SetNegThValue(value, source);
    }

    #endregion

    #region Negative Threshold Mouse Logic
    
    private void NegThMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is not Border parent) return;
        if(parent.Child is Border child)
        {
            child.BeginAnimation(OpacityProperty,new DoubleAnimation(1,new Duration(TimeSpan.FromSeconds(0.1))));
            NegThNum.BeginAnimation(OpacityProperty,new DoubleAnimation(1,new Duration(TimeSpan.FromSeconds(0.1))));
        }
    }

    private void NegThMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is not Border parent) return;
        if(parent.Child is Border child)
        {
            child.BeginAnimation(OpacityProperty,new DoubleAnimation(0.2,new Duration(TimeSpan.FromSeconds(0.1))));
            NegThNum.BeginAnimation(OpacityProperty,new DoubleAnimation(0,new Duration(TimeSpan.FromSeconds(0.1))));
        }
    }

    private void NegThMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is Border border && border.IsMouseCaptured)
        {
            if (border.Parent is Grid)
            {
                var min = 0;
                var max = WaveGrid.ActualHeight/2 - NegBorder.ActualHeight;
                var value = double.Clamp(WaveGrid.ActualHeight - e.GetPosition(WaveGrid).Y - NegBorder.ActualHeight / 2,min,max);
                value = value / (max-min) * -36;
                SetNegThValue(value, 1);
            }
        };
    }

    //使用source防止递归
    public void SetNegThValue(double value, int source)
    {
        var min = 0;
        var max = WaveGrid.ActualHeight/2 - NegBorder.ActualHeight;
        var pos = value / -36 * (max-min);
        NegBorder.Margin = new Thickness(0, 0, 0, pos);
        NegThNum.Text = value.ToString("0.0 dB");
        Plugin.SetNegTh(value);
        if(_linked && source == 1)
            SetPosThValue(value, source);
    }

    #endregion
    
    private void ThMouseDown(object sender, MouseButtonEventArgs e)
    {
        Mouse.Capture(sender as Border);
    }
    
    private void ParamMouseUp(object sender, MouseButtonEventArgs e)
    {
        Mouse.Capture(null);
    }
    
    #region Link Button Logic
    
    private bool _linkHover = false;
    private bool _linked = true;
    private readonly Brush _linkBrush = (Brush)new BrushConverter().ConvertFrom("#ccc")!;
    private readonly Brush _linkToggleBrush = (Brush)new BrushConverter().ConvertFrom("#328cfa")!;
    private readonly Brush _linkHoverBrush = (Brush)new BrushConverter().ConvertFrom("#999")!;
    private readonly Brush _linkToggleHoverBrush = (Brush)new BrushConverter().ConvertFrom("#2872cc")!;

    private void LinkButtonInit()
    {
        LinkButton.Background = _linkToggleBrush;
        _linked = Plugin.Linked;
    }
    private void LinkButtonMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Border border)
        {
            _linkHover = true;
            border.Background = Plugin.Linked ? _linkToggleHoverBrush : _linkHoverBrush;
        }
    }

    private void LinkButtonMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Border border)
        { 
            _linkHover = false;
            border.Background = Plugin.Linked ? _linkToggleBrush : _linkBrush;
        }
    }

    private void LinkButtonMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border && _linkHover)
        {
            ToggleLink(!_linked);
        }
    }

    public void ToggleLink(bool value)
    {
        Plugin.Linked = value;
        if(!_linkHover)
            LinkButton.Background = value ? _linkToggleBrush : _linkBrush;
    }

    private void LinkButtonMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border)
        {
            _linked = Plugin.Linked;
            border.Background = Plugin.Linked ? _linkToggleHoverBrush : _linkHoverBrush;
        }
    }
    
    #endregion
    
    private Point _startPos;
    private double _startVal;
    
    #region Ratio Knob Logic
    
    private const double RatioKnobMinV = 1;
    private const double RatioKnobMaxV = 20;
    private double _ratioKnobV = 2;
    
    private void RatioKnobMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            Plugin.ResetRatio();
            return;
        }
        if(sender is Ellipse ellipse)
        {
            _startPos = e.GetPosition(ellipse);
            _startVal = _ratioKnobV;
            Mouse.Capture(ellipse);
        }
    }

    private void RatioKnobMouseMove(object sender, MouseEventArgs e)
    {
        if(sender is Ellipse ellipse)
            if (ellipse.IsMouseCaptured)
            {
                Point pos = e.GetPosition(sender as Ellipse);
                double offset = _startPos.Y - pos.Y;
                double target = _startVal + offset * 0.1;

                SetRatioValue(target);
                Plugin.SetRatio(_ratioKnobV);
            }
    }

    public void SetRatioValue(double value)
    {
        _ratioKnobV = double.Round(double.Clamp(value, RatioKnobMinV, RatioKnobMaxV),0);
        DrawArc(RatioKnob, _ratioKnobV, RatioKnobMinV, RatioKnobMaxV, RatioKnobCanvas, 5);
        RatioValue.Text = _ratioKnobV.ToString("0 : 1");
    }
    
    #endregion

    #region Attack Knob Logic

    private const double AttackKnobMinV = 0;
    private const double AttackKnobMaxV = 50;
    private double _attackKnobV = 10;
    
    private void AttackKnobMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            Plugin.ResetAttack();
            return;
        }
        if(sender is Ellipse ellipse)
        {
            _startPos = e.GetPosition(ellipse);
            _startVal = _attackKnobV;
            Mouse.Capture(ellipse);
        }
    }

    private void AttackKnobMouseMove(object sender, MouseEventArgs e)
    {
        if(sender is Ellipse ellipse)
            if (ellipse.IsMouseCaptured)
            {
                Point pos = e.GetPosition(sender as Ellipse);
                double offset = _startPos.Y - pos.Y;
                double target = _startVal + offset * 0.1;

                SetAttackValue(target);
                Plugin.SetAttack(_attackKnobV);
            }
    }

    public void SetAttackValue(double value)
    {
        _attackKnobV = double.Round(double.Clamp(value, AttackKnobMinV, AttackKnobMaxV),0);
        DrawArc(AttackKnob, _attackKnobV, AttackKnobMinV, AttackKnobMaxV, AttackKnobCanvas, 3);
        AttackValue.Text = _attackKnobV.ToString("0ms");
    }
    
    #endregion
    
    #region Release Knob Logic

    private const double ReleaseKnobMinV = 20;
    private const double ReleaseKnobMaxV = 200;
    private double _releaseKnobV = 50;
    
    private void ReleaseKnobMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            Plugin.ResetRelease();
            return;
        }
        if(sender is Ellipse ellipse)
        {
            _startPos = e.GetPosition(ellipse);
            _startVal = _releaseKnobV;
            Mouse.Capture(ellipse);
        }
    }

    private void ReleaseKnobMouseMove(object sender, MouseEventArgs e)
    {
        if(sender is Ellipse ellipse)
            if (ellipse.IsMouseCaptured)
            {
                Point pos = e.GetPosition(sender as Ellipse);
                double offset = _startPos.Y - pos.Y;
                double target = _startVal + offset * 0.5;

                SetReleaseValue(target);
                Plugin.SetRelease(_releaseKnobV);
            }
    }

    public void SetReleaseValue(double value)
    {
        _releaseKnobV = double.Round(double.Clamp(value, ReleaseKnobMinV, ReleaseKnobMaxV),0);
        DrawArc(ReleaseKnob, _releaseKnobV, ReleaseKnobMinV, ReleaseKnobMaxV, ReleaseKnobCanvas, 3);
        ReleaseValue.Text = _releaseKnobV.ToString("0ms");
    }
    
    #endregion
    
    private void KnobMouseEnter(object sender, MouseEventArgs e)
    {
        if(sender is Grid grid)
        {
            grid.BeginAnimation(WidthProperty, new DoubleAnimation(grid.MinWidth + 2, new Duration(TimeSpan.FromSeconds(0.1))));
            grid.BeginAnimation(HeightProperty, new DoubleAnimation(grid.MinHeight + 2, new Duration(TimeSpan.FromSeconds(0.1))));
        }
    }

    private void KnobMouseLeave(object sender, MouseEventArgs e)
    {
        if(sender is Grid grid)
        {
            grid.BeginAnimation(WidthProperty, new DoubleAnimation(grid.MinWidth, new Duration(TimeSpan.FromSeconds(0.1))));
            grid.BeginAnimation(HeightProperty, new DoubleAnimation(grid.MinHeight, new Duration(TimeSpan.FromSeconds(0.1))));
        }
    }

    #region Draw Waveform
    
    private bool _isDrawingOut = false;
    private bool _isDrawingIn = false;
    
    private Polyline _waveInPosPoly;
    private Polyline _waveInNegPoly;
    private Polyline _waveOutPosPoly;
    private Polyline _waveOutNegPoly;
    
    private readonly PointCollection _waveInPosPoints = new();
    private readonly PointCollection _waveInNegPoints = new();
    private readonly PointCollection _waveOutPosPoints = new();
    private readonly PointCollection _waveOutNegPoints = new();

    private void InitWave()
    {
        _waveInPosPoly = new Polyline
        {
            Stroke = new SolidColorBrush(Color.FromRgb(0x32, 0x8C, 0xFA)),
            SnapsToDevicePixels = true,
        };
        _waveInNegPoly = new Polyline
        {
            Stroke = new SolidColorBrush(Color.FromRgb(0x32, 0x8C, 0xFA)),
            SnapsToDevicePixels = true,
        };

        CanPosIn.Children.Add(_waveInPosPoly);
        CanNegIn.Children.Add(_waveInNegPoly);
        
        _waveOutPosPoly = new Polyline
        {
            Stroke = Brushes.White,
            Fill = new SolidColorBrush(Colors.White) { Opacity = 0.5 },
            SnapsToDevicePixels = true,
        };
        _waveOutNegPoly = new Polyline
        {
            Stroke = Brushes.White,
            Fill = new SolidColorBrush(Colors.White) { Opacity = 0.5 },
            SnapsToDevicePixels = true,
        };

        CanPosOut.Children.Add(_waveOutPosPoly);
        CanNegOut.Children.Add(_waveOutNegPoly);
    }
    
    public void DrawWaveOut(double[] data)
    {
        if (_isDrawingOut) return;
        try
        {
            double w = CanPosOut.ActualWidth;
            double h = CanPosOut.ActualHeight;
            int count = data.Length;

            var p = _waveOutPosPoints;
            var n = _waveOutNegPoints;

            p.Clear();
            n.Clear();

            p.Add(new Point(0, h));
            n.Add(new Point(0, 0));

            for (int i = 0; i < count; i++)
            {
                double x = i * w / count;
                double s = data[i];

                p.Add(new Point(x, h - Math.Max(s, 0) * h));
                n.Add(new Point(x, -Math.Min(s, 0) * h));
            }

            p.Add(new Point(w, h));
            n.Add(new Point(w, 0));

            _waveOutPosPoly.Points = p;
            _waveOutNegPoly.Points = n;
        }
        catch
        {
            // ignored
        }
        finally { _isDrawingOut = false; }
    }

    public void DrawWaveIn(double[] data)
    {
        if (_isDrawingIn) return;
        try
        {
            double w = CanPosIn.ActualWidth;
            double h = CanPosIn.ActualHeight;
            int count = data.Length;

            var p = _waveInPosPoints;
            var n = _waveInNegPoints;

            p.Clear();
            n.Clear();

            for (int i = 0; i < count; i++)
            {
                double x = i * w / count;
                double s = data[i];

                p.Add(new Point(x, h - Math.Max(s, 0) * h));
                n.Add(new Point(x, -Math.Min(s, 0) * h));
            }

            _waveInPosPoly.Points = p;
            _waveInNegPoly.Points = n;
        }
        catch
        {
            // ignored
        }
        finally { _isDrawingIn = false; }
    }
    
    private void DrawArc(Ellipse el, double value, double min, double max, Canvas canvas, double thickness)
    {
        var ratioKnobActualWidth = el.ActualWidth;
        var ratioKnobActualHeight = el.ActualHeight;
        Point startPoint = new Point(ratioKnobActualWidth / 2, ratioKnobActualHeight);
        double endRad = (value - min) / (max - min) * Double.Pi * 2 - 0.001;
        var radius = ratioKnobActualHeight / 2;
        var center = new Point(ratioKnobActualWidth/2, ratioKnobActualHeight/2);
        
        Point endPoint = new Point(
            center.X - radius * Math.Sin(endRad),
            center.Y + radius * Math.Cos(endRad)
        );

        PathFigure figure = new PathFigure
        {
            StartPoint = startPoint
        };

        ArcSegment arc = new ArcSegment
        {
            Point = endPoint,
            Size = new Size(radius, radius),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc = max - value < value - min
        };

        figure.Segments.Add(arc);
        PathGeometry geo = new PathGeometry();
        geo.Figures.Add(figure);

        var path = new Path
        {
            Data = geo,
            Stroke = (Brush)new BrushConverter().ConvertFrom("#328cfa")!,
            StrokeThickness = thickness
        };
        canvas.Children.Clear();
        canvas.Children.Add(path);
    }
    
    #endregion

    #region Speed Slider Logic
    
    private void SpeedSliderThumbMouseDown(object sender, MouseButtonEventArgs e)
    {
        Mouse.Capture(SpeedSliderThumb);
    }

    private void SpeedSliderThumbMouseMove(object sender, MouseEventArgs e)
    {
        if(SpeedSliderThumb.IsMouseCaptured)
        {
            var width = SpeedSliderTrack.ActualWidth;
            var tick = width / (8 - 1);
            var value = e.GetPosition(SpeedSliderTrack).X;
            value = double.Clamp(value,0,width);
            //tick表示speed的值
            value = double.Round(value / tick, 0);
            Plugin.SetStep((int)value);
            SpeedSliderBackground.Width = value * tick;
            SpeedSliderThumb.Margin = SpeedSliderThumb.Margin with { Left = value * tick - 5 };
        }
    }
    
    #endregion
    
    #region Settings Button Logic

    private void SettingsButtonMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Border border)
        {
            border.Background = Brushes.DarkGray;
        }
    }

    private void SettingsButtonMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Border border)
        {
            border.Background = Brushes.White;
        }
    }

    private void SettingsButtonMouseDown(object sender, MouseButtonEventArgs e)
    {
        SettingsBorder.Visibility =
            SettingsBorder.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }
    
    #endregion

    private void LinkMouseDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            string url = "https://aqua-sounds.top";
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
        catch
        {
            // ignored
        }
    }
}