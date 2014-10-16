using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BellaCodeAgenda
{
    public static class MainCommands
    {
        public static RoutedCommand StartMeeting = new RoutedCommand("StartMeeting", typeof(MainCommands));        

        public static RoutedCommand GoToNextAgendaItem = new RoutedCommand("GoToNextAgendaItem", typeof(MainCommands));
    }
}
