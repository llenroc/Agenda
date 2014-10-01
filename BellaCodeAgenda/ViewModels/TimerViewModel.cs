using BellaCode.Mvvm;
using BellaCodeAgenda.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace BellaCodeAgenda.ViewModels
{
    public class TimerViewModel : ViewModel<Meeting>
    {
        private DispatcherTimer _timer = new DispatcherTimer();

        private CollectionViewSource _activeAgendaItemsViewSource = new CollectionViewSource();

        public TimerViewModel()
        {
            this._timer.Interval = TimeSpan.FromSeconds(1);
            this._timer.Tick += Timer_Tick;

            this._activeAgendaItemsViewSource.Filter += _activeAgendaItemsViewSource_Filter;
        }

        void _activeAgendaItemsViewSource_Filter(object sender, FilterEventArgs e)
        {
            var agendaItem = e.Item as AgendaItem;
            e.Accepted = (agendaItem != null && !agendaItem.IsComplete);
        }

        public Meeting Meeting
        {
            get
            {
                return this.Model;
            }
        }

        private double _percentMeetingTimeUsed;

        public double PercentMeetingTimeUsed
        {
            get
            {
                return this._percentMeetingTimeUsed;
            }
            private set
            {
                if (this._percentMeetingTimeUsed != value)
                {
                    this._percentMeetingTimeUsed = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private ICollectionView _activeAgendaItems;

        public ICollectionView ActiveAgendaItems
        {
            get
            {
                return this._activeAgendaItems;
            }
            private set
            {
                if (this._activeAgendaItems != value)
                {
                    this._activeAgendaItems = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void SetActiveAgendaItemsViewSource()
        {
            if (this.Meeting != null)
            {
                this._activeAgendaItemsViewSource.Source = this.Meeting.AgendaItems;
                this.ActiveAgendaItems = this._activeAgendaItemsViewSource.View;
            }
            else
            {
                this._activeAgendaItemsViewSource.Source = null;
                this.ActiveAgendaItems = null;
            }

        }

        protected override void OnModelChanged(Meeting oldValue, Meeting newValue)
        {
            base.OnModelChanged(oldValue, newValue);

            if (oldValue != null)
            {
                foreach (var agendaItem in oldValue.AgendaItems)
                {
                    agendaItem.PropertyChanged -= agendaItem_PropertyChanged;
                }
            }

            this.SetActiveAgendaItemsViewSource();

            if (newValue != null)
            {
                foreach (var agendaItem in newValue.AgendaItems)
                {
                    agendaItem.PropertyChanged += agendaItem_PropertyChanged;
                }
            }
        }

        private void agendaItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ActiveAgendaItems != null && e.PropertyName == "IsComplete")
            {
                this.ActiveAgendaItems.Refresh();
            }
        }

        protected override void OnViewChanged(object oldValue, object newValue)
        {
            base.OnViewChanged(oldValue, newValue);

            if (newValue != null)
            {
                this.UpdatePercentMeetingTimeUsed();
                this._timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.UpdatePercentMeetingTimeUsed();
        }

        private void UpdatePercentMeetingTimeUsed()
        {
            if (this.Meeting != null)
            {
                var elapsedTime = DateTime.Now.TimeOfDay - this.Meeting.StartTime;
                if (elapsedTime > TimeSpan.Zero)
                {
                    this.PercentMeetingTimeUsed = Math.Min(100, ((elapsedTime.TotalSeconds / this.Meeting.ExpectedDuration.TotalSeconds) * 100.0));
                }
            }
        }
    }
}
