using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace BellaCodeAgenda.Actions
{
    public class OpenHelpWindowAction : TriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            var window = new HelpWindow();
            window.ShowDialog();
        }
    }
}
