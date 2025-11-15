using LinearGradientBlurBrush;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System.Numerics;
using Windows.Foundation;
using Windows.UI; 
using System;


//首选方案：XamlCompositionBrushBase。这是实现此类效果的现代、标准且最高效的方式。它能让你的代码库更清晰，也更容易维护和扩展。除非有特殊理由，否则你应该选择这个方案。
//备选方案：对象池（Pooling）。如果无法重构为 XamlCompositionBrushBase，对象池是第二选择，它能有效解决列表虚拟化场景下的性能瓶颈，让滚动体验如丝般顺滑。
namespace BouncyCat.Controls.LinearGradientBlurEffect
{
    public partial class LinearGradientBlurPanel : Grid
    {
        private readonly object locker = new();
        private LinearGradientBlurHelper? helper;
        private bool isDisposed = false;

        public LinearGradientBlurPanel()
        {
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            isDisposed = false;
            EnsureHelper();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            lock (locker)
            {
                if (helper != null)
                {
                    ElementCompositionPreview.SetElementChildVisual(this, null);
                    helper.Dispose();
                    helper = null;
                }
                isDisposed = true;
            }
        }

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not LinearGradientBlurPanel sender) return;

            if (Equals(e.NewValue, e.OldValue)) return;

            if (!sender.IsLoaded || sender.helper is null)
            {
                return;
            }

            var property = e.Property;
            if (property == MaxBlurAmountProperty)
            {
                sender.helper.MaxBlurAmount = (float)(double)e.NewValue;
            }
            // REMOVED: 'else if' block for TintColorProperty was here.
            else if (property == StartPointProperty)
            {
                sender.helper.StartPoint = ((Point)e.NewValue).ToVector2();
            }
            else if (property == EndPointProperty)
            {
                sender.helper.EndPoint = ((Point)e.NewValue).ToVector2();
            }
        }

        private LinearGradientBlurHelper? EnsureHelper()
        {
            if (helper is not null) return helper;

            lock (locker)
            {
                if (helper is not null) return helper;

                if (isDisposed) return null;

                var visual = ElementCompositionPreview.GetElementVisual(this);
                if (visual is null)
                {
                    return null;
                }

                var compositor = visual.Compositor;

                var newHelper = new LinearGradientBlurHelper(compositor)
                {
                    MaxBlurAmount = (float)MaxBlurAmount,
                    // REMOVED: No longer setting TintColor.
                    StartPoint = StartPoint.ToVector2(),
                    EndPoint = EndPoint.ToVector2(),
                };

                ElementCompositionPreview.SetElementChildVisual(this, newHelper.RootVisual);
                helper = newHelper;
            }
            return helper;
        }

        #region Dependency Properties

        public double MaxBlurAmount { get => (double)GetValue(MaxBlurAmountProperty); set => SetValue(MaxBlurAmountProperty, value); }
        public static readonly DependencyProperty MaxBlurAmountProperty = DependencyProperty.Register(nameof(MaxBlurAmount), typeof(double), typeof(LinearGradientBlurPanel), new PropertyMetadata(16.0d, OnDependencyPropertyChanged));

        // REMOVED: TintColor and TintColorProperty were here.

        public Point StartPoint { get => (Point)GetValue(StartPointProperty); set => SetValue(StartPointProperty, value); }
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(nameof(StartPoint), typeof(Point), typeof(LinearGradientBlurPanel), new PropertyMetadata(new Point(0, 1), OnDependencyPropertyChanged));

        public Point EndPoint { get => (Point)GetValue(EndPointProperty); set => SetValue(EndPointProperty, value); }
        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(nameof(EndPoint), typeof(Point), typeof(LinearGradientBlurPanel), new PropertyMetadata(new Point(0, 0), OnDependencyPropertyChanged));

        #endregion

        #region Composition Animation Methods

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(LinearGradientBlurPanel), "The panel has been unloaded and its composition resources disposed.");
            }
        }

        public void StartMaxBlurAmountAnimation(CompositionAnimation animation) => StartHelperCompositionAnimation(nameof(MaxBlurAmount), animation);
        public void StopMaxBlurAmountAnimation() => StopHelperCompositionAnimation(nameof(MaxBlurAmount));

        public void StartStartPointAnimation(CompositionAnimation animation) => StartHelperCompositionAnimation(nameof(StartPoint), animation);
        public void StopStartPointAnimation() => StopHelperCompositionAnimation(nameof(StartPoint));

        public void StartEndPointAnimation(CompositionAnimation animation) => StartHelperCompositionAnimation(nameof(EndPoint), animation);
        public void StopEndPointAnimation() => StopHelperCompositionAnimation(nameof(EndPoint));

        private void StartHelperCompositionAnimation(string propertyName, CompositionAnimation animation)
        {
            ThrowIfDisposed();
            EnsureHelper()?.CompositionProperties.StartAnimation(propertyName, animation);
        }

        private void StopHelperCompositionAnimation(string propertyName)
        {
            ThrowIfDisposed();
            helper?.CompositionProperties.StopAnimation(propertyName);
        }

        #endregion
    }

    internal static class PointExtensions
    {
        public static Vector2 ToVector2(this Point p) => new((float)p.X, (float)p.Y);
    }
}