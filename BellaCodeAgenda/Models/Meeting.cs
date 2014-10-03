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

        private TimeSpan _startTime = TimeSpan.FromMinutes(Math.Floor(DateTime.Now.TimeOfDay.TotalMinutes));

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

        private TimeSpan _expectedDuration = TimeSpan.FromMinutes(10);

        /// <summary>
        /// How long the meeting is expected to last.
        /// </summary>
        public TimeSpan ExpectedDuration
        {
            get
            {
                return this._expectedDuration;
            }
            set
            {
                if (this._expectedDuration != value)
                {
                    this._expectedDuration = value;
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
