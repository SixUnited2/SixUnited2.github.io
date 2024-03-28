using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace yz.gaming.accessoryapp.Controls
{
    public class ChooseEffectGrid : Border
    {
        public delegate void ClickEffectGridOnClickHandlerOnClickHandler(ChooseEffectGrid sender);
        public delegate void ClickEffectGridChooseHandler(ChooseEffectGrid sender);

        public event ClickEffectGridChooseHandler OnChoose;
        public event ClickEffectGridOnClickHandlerOnClickHandler OnClick;

        static SolidColorBrush DEFAULT_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
        static SolidColorBrush SELECTED_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x1A, 0xA7, 0x4F));

        public List<ChooseEffectGrid> GroupGrid { get; set; }

        public ChooseEffectGrid()
        {
            GroupGrid = new List<ChooseEffectGrid>();
            Background = DEFAULT_BACKGROUND_BRUSH;
        }

        public bool IsChoose
        {
            get { return (bool)GetValue(IsChooseProperty); }
            set
            {
                SetValue(IsChooseProperty, value);
                if (!value)
                {
                    Background = DEFAULT_BACKGROUND_BRUSH;
                }
            }
        }

        public static readonly DependencyProperty IsChooseProperty =
            DependencyProperty.Register("IsChoose", typeof(bool), typeof(ChooseEffectGrid), new PropertyMetadata(false));

        public bool IsSupportUnchoose
        {
            get { return (bool)GetValue(IsSupportUnchooseProperty); }
            set
            {
                SetValue(IsSupportUnchooseProperty, value);
            }
        }

        public static readonly DependencyProperty IsSupportUnchooseProperty =
            DependencyProperty.Register("IsSupportUnchoose", typeof(bool), typeof(ChooseEffectGrid), new PropertyMetadata(false));

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {            
            base.OnMouseLeftButtonUp(e);

            OnClick?.Invoke(this);

            if (!IsChoose)
            {
                Choose();
            }
            else
            {
                UnChoose();
            }
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            OnClick?.Invoke(this);

            if (!IsChoose) Choose();
        }

        public void Choose()
        {
            foreach (var item in GroupGrid)
            {
                item.IsChoose = false;
            }

            if (!IsChoose)
            {
                IsChoose = true;
                Background = SELECTED_BACKGROUND_BRUSH;
                OnChoose?.Invoke(this);
            }
        }

        public void UnChoose()
        {
            if (IsSupportUnchoose)
            {
                foreach (var item in GroupGrid)
                {
                    item.IsChoose = false;
                }

                if (IsChoose)
                {
                    IsChoose = false;
                    Background = DEFAULT_BACKGROUND_BRUSH;
                    OnChoose?.Invoke(this);
                }
            }
        }
    }
}
