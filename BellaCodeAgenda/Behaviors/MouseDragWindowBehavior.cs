using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;

namespace BellaCodeAgenda.Behaviors
{
    public class MouseDragWindowBehavior : Behavior<FrameworkElement>
    {
#if MOUSEDRAG_HOOK
        private Window _associatedWindow;
        private HwndSourceHook _windowHook;
#endif

        protected override void OnAttached()
        {
            base.OnAttached();

#if MOUSEDRAG_HOOK
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
#else
            this.AssociatedObject.MouseDown += this.AssociatedObject_MouseDown;
#endif
        }

        protected override void OnDetaching()
        {
#if MOUSEDRAG_HOOK
            if (this._associatedWindow != null && this._windowHook != null)
            {
                IntPtr hwnd = new WindowInteropHelper(this._associatedWindow).Handle;
                HwndSource.FromHwnd(hwnd).RemoveHook(this._windowHook);
            }
#else
            this.AssociatedObject.MouseDown -= this.AssociatedObject_MouseDown;
#endif
            base.OnDetaching();
        }        
 
        #if MOUSEDRAG_HOOK

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            //Get the window and add a message hook.
            this._associatedWindow = Window.GetWindow(this.AssociatedObject);

            IntPtr hwnd = new WindowInteropHelper(this._associatedWindow).Handle;
            this._windowHook = new HwndSourceHook(WndProc);
            HwndSource.FromHwnd(hwnd).AddHook(this._windowHook);
        }

        private const int WM_NCHITTEST = 0x0084;
        private const int HTCAPTION = 2;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCHITTEST)
            {
                // get screen coordinates  
                Point point = GetPointFromIntPtr(lParam);

                if (IsPointOverAssociatedObject(point))
                {
                    //The point is not in a noncontainer control(like TextBox or Button),
                    //In other words, it is on a Panel, a Grid or the other container element.
                    //So we lie the system that the point is on the caption bar.
                    handled = true;
                    return new IntPtr(HTCAPTION);
                }
            }

            return IntPtr.Zero;
        }

        private static Point GetPointFromIntPtr(IntPtr intPtr)
        {
            Point point = new Point();
            int pInt = intPtr.ToInt32();
            point.X = (pInt << 16) >> 16; // lo order word  
            point.Y = pInt >> 16; // hi order word  
            return point;
        }

        private bool IsPointOverAssociatedObject(Point screenPoint)
        {
            var overElement = this.AssociatedObject.InputHitTest(this.AssociatedObject.PointFromScreen(screenPoint));
            return object.ReferenceEquals(overElement, this.AssociatedObject);
        }       
#else
        void AssociatedObject_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var window = Window.GetWindow(this.AssociatedObject);
                window.DragMove();
            }
        }
#endif


    }
}
