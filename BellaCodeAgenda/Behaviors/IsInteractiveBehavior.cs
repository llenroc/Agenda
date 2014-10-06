using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Diagnostics;

namespace BellaCodeAgenda.Behaviors
{
    /// <summary>
    /// Determines if the user is interacting with the element using mouse, keyboard, or touch (kinda).
    /// </summary>
    public class IsInteractiveBehavior : Behavior<FrameworkElement>
    {
        public bool IsInteractive
        {
            get { return (bool)GetValue(IsInteractiveProperty); }
            set { SetValue(IsInteractiveProperty, value); }
        }

        public static readonly DependencyProperty IsInteractiveProperty = DependencyProperty.Register("IsInteractive", typeof(bool), typeof(IsInteractiveBehavior), new FrameworkPropertyMetadata(false));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;            
            this.AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            this.AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
            this.AssociatedObject.IsMouseDirectlyOverChanged += AssociatedObject_IsMouseDirectlyOverChanged;
            this.AssociatedObject.IsMouseCaptureWithinChanged += AssociatedObject_IsMouseCaptureWithinChanged;
            this.AssociatedObject.GotFocus += AssociatedObject_GotFocus;
            this.AssociatedObject.LostFocus += AssociatedObject_LostFocus;
            this.AssociatedObject.GotKeyboardFocus += AssociatedObject_GotKeyboardFocus;
            this.AssociatedObject.LostKeyboardFocus += AssociatedObject_LostKeyboardFocus;
            this.AssociatedObject.TouchEnter += AssociatedObject_TouchEnter;
            this.AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;

            this.UpdateIsInteractive();
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            this.AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
            this.AssociatedObject.PreviewMouseMove -= AssociatedObject_PreviewMouseMove;
            this.AssociatedObject.IsMouseDirectlyOverChanged -= AssociatedObject_IsMouseDirectlyOverChanged;
            this.AssociatedObject.IsMouseCaptureWithinChanged -= AssociatedObject_IsMouseCaptureWithinChanged;
            this.AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
            this.AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
            this.AssociatedObject.GotKeyboardFocus -= AssociatedObject_GotKeyboardFocus;
            this.AssociatedObject.LostKeyboardFocus -= AssociatedObject_LostKeyboardFocus;
            this.AssociatedObject.TouchEnter -= AssociatedObject_TouchEnter;
            this.AssociatedObject.TouchLeave -= AssociatedObject_TouchLeave;

            this.UpdateIsInteractive();

            base.OnDetaching();
        }

        void AssociatedObject_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        void AssociatedObject_IsMouseCaptureWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void AssociatedObject_MouseEnter(object sender, MouseEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void AssociatedObject_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void AssociatedObject_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        void AssociatedObject_TouchEnter(object sender, TouchEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void UpdateIsInteractive()
        {
            this.IsInteractive = (this.AssociatedObject != null) &&
                (this.AssociatedObject.IsMouseOver ||
                this.AssociatedObject.IsMouseCaptureWithin ||
                this.AssociatedObject.IsKeyboardFocusWithin ||
                this.AssociatedObject.TouchesOver.Any(x => x.IsActive));        
        }
    }
}
