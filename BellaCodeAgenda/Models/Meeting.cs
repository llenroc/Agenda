using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BellaCodeAgenda.Models
{
    public class Meeting : BindableBase
    {
        public Meeting()
        {
            this.AgendaItems = new Collection<AgendaItem>();
        }

        private string _title = "Meeting Agenda";

        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                if (this._title != value)
                {
                    this._title = value;
                    this.RaisePropertyChanged();
                }
            }
        }


        private TimeSpan _startTime = DateTime.Now.TimeOfDay;

        /// <summary>
        /// The start time on the day the meeting occurs represented as a duration since midnight.
        /// </summary>
        public TimeSpan StartTime
        {
            get
            {
                return this._startTime;
            }
            set
            {
                if (this._startTime != value)
                {
                    this._startTime = value;
                    this.RaisePropertyChanged();
                }
            }
        }       

        private Collection<AgendaItem> _agendaItems;

        public Collection<AgendaItem> AgendaItems
        {
            get
            {
                return this._agendaItems;
            }
            private set
            {
                if (this._agendaItems != value)
                {
                    this._agendaItems = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
