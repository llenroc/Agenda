using BellaCode.Mvvm;
using BellaCodeAgenda.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        private CollectionViewSource _pastAgendaItemsViewSource = new CollectionViewSource();

        public TimerViewModel()
        {
            this._timer.Interval = TimeSpan.FromSeconds(1);
            this._timer.Tick += Timer_Tick;

            this._pastAgendaItemsViewSource.Filter += _pastAgendaItemsViewSource_Filter;
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

        private AgendaItem _currentAgendaItem;

        public AgendaItem CurrentAgendaItem
        {
            get
            {
                return this._currentAgendaItem;
            }
            private set
            {
                if (this._currentAgendaItem != value)
                {
                    this._currentAgendaItem = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private ICollectionView _pastAgendItemsView;

        public ICollectionView PastAgendaItems
        {
            get
            {
                return this._pastAgendItemsView;
            }
            private set
            {
                if (this._pastAgendItemsView != value)
                {
                    this._pastAgendItemsView = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void SetPastAgendaItemsViewSource()
        {
            if (this.Meeting != null)
            {
                this._pastAgendaItemsViewSource.Source = this.Meeting.AgendaItems;
                this.PastAgendaItems = this._pastAgendaItemsViewSource.View;
            }
            else
            {
                this._pastAgendaItemsViewSource.Source = null;
                this.PastAgendaItems = null;
            }
        }

        private void _pastAgendaItemsViewSource_Filter(object sender, FilterEventArgs e)
        {
            var agendaItem = e.Item as AgendaItem;
            e.Accepted = (agendaItem != null && !agendaItem.IsComplete && agendaItem.IsPast);
        }

        private bool _isInteractive;

        public bool IsInteractive
        {
            get
            {
                return this._isInteractive;
            }
            private set
            {
                if (this._isInteractive != value)
                {
                    Debug.WriteLine("Interactive:" + value);
                    this._isInteractive = value;
                    this.RaisePropertyChanged();
                }
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

            this.SetPastAgendaItemsViewSource();

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
            if (this.PastAgendaItems != null)
            {
                if  (e.PropertyName == "IsComplete" || e.PropertyName == "IsPast")
                this.PastAgendaItems.Refresh();
            }
        }

        protected override void OnViewChanged(object oldValue, object newValue)
        {
            base.OnViewChanged(oldValue, newValue);

            if (newValue != null)
            {
                this.UpdateProperties();
                this._timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.UpdateProperties();
        }

        private void UpdateProperties()
        {
            if (this.Meeting != null)
            {
                var elapsedTime = DateTime.Now.TimeOfDay - this.Meeting.StartTime;
                if (elapsedTime > TimeSpan.Zero)
                {
                    this.PercentMeetingTimeUsed = Math.Min(100, ((elapsedTime.TotalSeconds / this.Meeting.ExpectedDuration.TotalSeconds) * 100.0));
                }

                var candidateCurrentAgendaItem = (AgendaItem)null;
                if (this.Meeting.AgendaItems != null)
                {
                    var itemStart = TimeSpan.Zero;
                    var itemEnd = TimeSpan.Zero;
                    foreach (var agendaItem in this.Meeting.AgendaItems)
                    {
                        itemEnd = itemStart + agendaItem.Duration;

                        // I look for the first non-complete agenda item
                        // that is within the start/end timeframe for the item.
                        if (!agendaItem.IsComplete)
                        {
                            if (candidateCurrentAgendaItem == null && elapsedTime <= itemEnd)
                            {
                                candidateCurrentAgendaItem = agendaItem;
                            }                            
                        }

                        // I update the IsPast for every agenda item
                        agendaItem.IsPast = elapsedTime > itemEnd;                        

                        itemStart += agendaItem.Duration;
                    }                    
                }

                this.CurrentAgendaItem = candidateCurrentAgendaItem;
            }
        }
    }
}
