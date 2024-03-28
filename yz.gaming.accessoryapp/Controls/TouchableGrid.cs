using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace yz.gaming.accessoryapp.Controls
{
    public class TouchableGrid : Grid
    {
        public delegate void TouchableGridOnTouchHandler(Point point, double width, double height);
        public event TouchableGridOnTouchHandler OnTouch;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var point = e.GetPosition(this);
            OnTouch?.Invoke(point, this.ActualWidth, this.ActualHeight);
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            var point = e.GetTouchPoint(this);
            OnTouch?.Invoke(point.Position, this.ActualWidth, this.ActualHeight);
            base.OnTouchUp(e);
        }
    }
}
