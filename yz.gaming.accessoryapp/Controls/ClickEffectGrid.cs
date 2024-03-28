using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace yz.gaming.accessoryapp.Controls
{
    public class ClickEffectGrid : Grid
    {
        public delegate void ClickEffectGridOnClickHandler(ClickEffectGrid sender);
        public delegate void ClickEffectGridOnPressStatusChangedHandler(ClickEffectGrid sender, bool isPressing);

        static Thickness DEFAULT_MARGIN_THICKNESS = new Thickness(0);
        static Thickness HOVER_MARGIN_THICKNESS = new Thickness(2);

        public event ClickEffectGridOnPressStatusChangedHandler OnPressing;
        public event ClickEffectGridOnClickHandler OnClick;

        public bool IsPressing
        {
            get { return (bool)GetValue(IsPressingProperty); }
            set { SetValue(IsPressingProperty, value); }
        }

        public static readonly DependencyProperty IsPressingProperty =
            DependencyProperty.Register("IsPressing", typeof(bool), typeof(ClickEffectGrid), new PropertyMetadata(false));


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.Margin = HOVER_MARGIN_THICKNESS;
            base.OnMouseLeftButtonDown(e);

            IsPressing = true;
            OnPressing?.Invoke(this, true);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.Margin = DEFAULT_MARGIN_THICKNESS;
            base.OnMouseLeftButtonUp(e);

            OnClick?.Invoke(this);

            IsPressing = false;
            OnPressing?.Invoke(this, false);
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            this.Margin = HOVER_MARGIN_THICKNESS;
            base.OnTouchDown(e);

            IsPressing = true;
            OnPressing?.Invoke(this, true);
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            this.Margin = DEFAULT_MARGIN_THICKNESS;
            base.OnTouchUp(e);

            IsPressing = false;
            OnPressing?.Invoke(this, false);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.Margin = DEFAULT_MARGIN_THICKNESS;
            base.OnMouseLeave(e);

            IsPressing = false;
            OnPressing?.Invoke(this, false);
        }

        protected override void OnTouchLeave(TouchEventArgs e)
        {
            this.Margin = DEFAULT_MARGIN_THICKNESS;
            base.OnTouchLeave(e);

            IsPressing = false;
            OnPressing?.Invoke(this, false);
        }
    }
}
