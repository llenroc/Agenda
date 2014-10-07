using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Diagnostics;
using System;

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
            this.AssociatedObject.Loaded += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.MouseEnter += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.MouseLeave += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.PreviewMouseMove += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.IsMouseDirectlyOverChanged += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.IsMouseCaptureWithinChanged += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.GotFocus += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.LostFocus += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.GotKeyboardFocus += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.LostKeyboardFocus += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.TouchEnter += AssociatedObject_InteractiveChanged;
            this.AssociatedObject.TouchLeave += AssociatedObject_InteractiveChanged;

            this.UpdateIsInteractive();
        }
        
        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.MouseEnter -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.MouseLeave -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.PreviewMouseMove -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.IsMouseDirectlyOverChanged -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.IsMouseCaptureWithinChanged -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.GotFocus -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.LostFocus -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.GotKeyboardFocus -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.LostKeyboardFocus -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.TouchEnter -= AssociatedObject_InteractiveChanged;
            this.AssociatedObject.TouchLeave -= AssociatedObject_InteractiveChanged;

            this.UpdateIsInteractive();

            base.OnDetaching();
        }

        private void AssociatedObject_InteractiveChanged(object sender, EventArgs e)
        {
            this.UpdateIsInteractive();
        }

        private void AssociatedObject_InteractiveChanged(object sender, DependencyPropertyChangedEventArgs e)
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
