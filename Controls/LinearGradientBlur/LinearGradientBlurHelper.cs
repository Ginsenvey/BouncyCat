using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using Microsoft.UI.Composition;
using System;
using System.Numerics;
using Windows.UI; 
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml; 
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace LinearGradientBlurBrush
{
    internal readonly record struct ColorStop(float Offset, Color Color)
    {
        public static implicit operator ColorStop((float offset, Color color) tuple)
        {
            return new ColorStop(tuple.offset, tuple.color);
        }
    }

    internal class LinearGradientBlurHelper : IDisposable
    {
        private static CompositionEffectFactory? sharedEffectFactory;
        private static readonly object factoryLock = new();

        private readonly Compositor compositor;
        private bool disposedValue;

        private float maxBlurAmount;
        private Vector2 startPoint;
        private Vector2 endPoint;

        private readonly SpriteVisual effectVisual;
        private readonly CompositionLinearGradientBrush maskBrush;

        private readonly ExpressionAnimation blurAmountBind;
        private readonly ExpressionAnimation startPointBind;
        private readonly ExpressionAnimation endPointBind;

        public SpriteVisual RootVisual { get; }
        public CompositionPropertySet CompositionProperties { get; }

        public LinearGradientBlurHelper(Compositor compositor)
        {
            this.compositor = compositor;
            EnsureEffectFactory(this.compositor);
             
            ColorStop[] colorStops = [(0.40f, Colors.Black), (1f, Colors.Transparent)];

            CompositionProperties = this.compositor.CreatePropertySet();
            CompositionProperties.InsertScalar(nameof(MaxBlurAmount), maxBlurAmount);
            CompositionProperties.InsertVector2(nameof(StartPoint), startPoint);
            CompositionProperties.InsertVector2(nameof(EndPoint), endPoint);

            blurAmountBind = CreateBindExpression("props.MaxBlurAmount");
            startPointBind = CreateBindExpression("props.StartPoint");
            endPointBind = CreateBindExpression("props.EndPoint");

            maskBrush = CreateMaskBrush(colorStops);
            effectVisual = CreateEffectVisual(maskBrush);

            RootVisual = this.compositor.CreateSpriteVisual();
            RootVisual.RelativeSizeAdjustment = Vector2.One;
            RootVisual.Children.InsertAtTop(effectVisual);
        }
         

        private CompositionLinearGradientBrush CreateMaskBrush(ColorStop[] stops)
        {
            var maskBrush = compositor.CreateLinearGradientBrush();
            maskBrush.MappingMode = CompositionMappingMode.Relative;
            maskBrush.StartAnimation(nameof(StartPoint), startPointBind);
            maskBrush.StartAnimation(nameof(EndPoint), endPointBind);

            foreach (var stop in stops)
            {
                maskBrush.ColorStops.Add(compositor.CreateColorGradientStop(stop.Offset, stop.Color));
            }
            return maskBrush;
        }
         
        private SpriteVisual CreateEffectVisual(CompositionBrush maskBrush)
        {
            var brush = sharedEffectFactory!.CreateBrush();
            brush.SetSourceParameter("source", compositor.CreateBackdropBrush());
            brush.SetSourceParameter("mask", maskBrush); 

            brush.Properties.StartAnimation("BlurEffect.BlurAmount", blurAmountBind);

            var visual = compositor.CreateSpriteVisual();
            visual.RelativeSizeAdjustment = Vector2.One;
            visual.Brush = brush;
            return visual;
        }

        public float MaxBlurAmount
        {
            get => maxBlurAmount;
            set
            {
                ThrowIfDisposed();
                if (maxBlurAmount != value)
                {
                    maxBlurAmount = value;
                    CompositionProperties.InsertScalar(nameof(MaxBlurAmount), value);
                }
            }
        }
         

        public Vector2 StartPoint
        {
            get => startPoint;
            set
            {
                ThrowIfDisposed();
                if (startPoint != value)
                {
                    startPoint = value;
                    CompositionProperties.InsertVector2(nameof(StartPoint), value);
                }
            }
        }

        public Vector2 EndPoint
        {
            get => endPoint;
            set
            {
                ThrowIfDisposed();
                if (endPoint != value)
                {
                    endPoint = value;
                    CompositionProperties.InsertVector2(nameof(EndPoint), value);
                }
            }
        }
        private ExpressionAnimation CreateBindExpression(string expression)
        {
            var exp = compositor.CreateExpressionAnimation(expression);
            exp.SetReferenceParameter("props", CompositionProperties);
            return exp;
        }

        private static void EnsureEffectFactory(Compositor compositor)
        {
            if (sharedEffectFactory is not null) return;
            lock (factoryLock)
            {
                if (sharedEffectFactory is null)
                { 
                    var blurWithMaskEffect = new AlphaMaskEffect()
                    {
                        AlphaMask = new CompositionEffectSourceParameter("mask"),
                        Source = new GaussianBlurEffect()
                        {
                            Name = "BlurEffect",
                            BlurAmount = 0f,
                            BorderMode = EffectBorderMode.Soft,
                            Source = new CompositionEffectSourceParameter("source")
                        }
                    }; 
                    sharedEffectFactory = compositor.CreateEffectFactory(blurWithMaskEffect, new[] { "BlurEffect.BlurAmount" });
                }
            }
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                disposedValue = true;
                GC.SuppressFinalize(this);

                RootVisual.Children.RemoveAll();

                effectVisual.Brush?.Dispose();
                effectVisual.Dispose();
                maskBrush.Dispose(); 

                blurAmountBind.Dispose();
                startPointBind.Dispose();
                endPointBind.Dispose(); 

                RootVisual.Dispose();
                CompositionProperties.Dispose();
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposedValue) throw new ObjectDisposedException(GetType().Name);
        }
         
    }
}