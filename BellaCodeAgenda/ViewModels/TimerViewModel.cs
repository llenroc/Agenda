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
        }        

        public Meeting Meeting
        {
            get
            {
                return this.Model;
            }
        }

        private TimerStatus _status;

        public TimerStatus Status
        {
            get
            {
                return this._status;
            }
            set
            {
                if (this._status != value)
                {
                    this._status = value;
                    this.RaisePropertyChanged();
                }
            }
        }


        private TimeSpan _elapsedTime;

        public TimeSpan ElapsedTime
        {
            get
            {
                return this._elapsedTime;
            }
            set
            {
                if (this._elapsedTime != value)
                {
                    this._elapsedTime = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private TimeSpan _remainingTime;

        public TimeSpan RemainingTime
        {
            get
            {
                return this._remainingTime;
            }
            set
            {
                if (this._remainingTime != value)
                {
                    this._remainingTime = value;
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

        private TimeSpan _currentItemElapsedTime;

        public TimeSpan CurrentItemElapsedTime
        {
            get
            {
                return this._currentItemElapsedTime;
            }
            set
            {
                if (this._currentItemElapsedTime != value)
                {
                    this._currentItemElapsedTime = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private TimeSpan _currentItemRemainingTime;

        public TimeSpan CurrentItemRemainingTime
        {
            get
            {
                return this._currentItemRemainingTime;
            }
            set
            {
                if (this._currentItemRemainingTime != value)
                {
                    this._currentItemRemainingTime = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged("CanGoToNextAgendaItem");
                }
            }
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
                    this._isInteractive = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool CanGoToNextAgendaItem
        {
            get
            {
                return this.CurrentAgendaItem != null;
            }
        }

        public void GoToNextAgendaItem()
        {
            if (this.CurrentAgendaItem != null)
            {
                this.CurrentAgendaItem.IsComplete = true;
                this.UpdateProperties();
            }
        }

        protected override void OnModelChanged(Meeting oldValue, Meeting newValue)
        {
            base.OnModelChanged(oldValue, newValue);            
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
            this.UpdateElapsedTime();
            this.UpdateRemainingTime();
            this.UpdateCurrentAgendaItem();
            this.UpdateStatus();
        }        

        private void UpdateElapsedTime()
        {
            if (this.Meeting != null)
            {
                this.ElapsedTime = DateTime.Now.TimeOfDay - this.Meeting.StartTime;

                if (this.ElapsedTime < TimeSpan.Zero)
                {
                    this.ElapsedTime = TimeSpan.Zero;
                }
            }
            else
            {
                this.ElapsedTime = TimeSpan.Zero;
            }       
        }

        // ElapsedTime should be updated before calling this method.
        private void UpdateRemainingTime()
        {
            if (this.Meeting != null)
            {
                this.RemainingTime = this.Meeting.ExpectedDuration - this.ElapsedTime;

                if (this.RemainingTime < TimeSpan.Zero)
                {
                    this.RemainingTime = TimeSpan.Zero;
                }
            }
            else
            {
                this.RemainingTime = TimeSpan.Zero;
            }
        }

        // ElapsedTime should be updated before calling this method.
        private void UpdateCurrentAgendaItem()
        {
            var candidateCurrentAgendaItem = (AgendaItem)null;
            var candidateItemStartTime = (TimeSpan?)null;
            var candidateItemEndTime = (TimeSpan?)null;
            if (this.Meeting != null && this.Meeting.AgendaItems != null)
            {
                var itemStart = TimeSpan.Zero;
                var itemEnd = TimeSpan.Zero;
                foreach (var agendaItem in this.Meeting.AgendaItems)
                {
                    itemEnd = itemStart + agendaItem.Duration;

                    // I look for the first non-complete agenda item that is not in the past.             
                    if (!agendaItem.IsComplete)
                    {
                        if (candidateCurrentAgendaItem == null && this.ElapsedTime <= itemEnd)
                        {
                            candidateCurrentAgendaItem = agendaItem;
                            candidateItemStartTime = itemStart;
                            candidateItemEndTime = itemEnd;
                        }
                    }

                    itemStart += agendaItem.Duration;
                }
            }

            this.CurrentAgendaItem = candidateCurrentAgendaItem;

            if (candidateCurrentAgendaItem != null && candidateItemStartTime.HasValue && candidateItemEndTime.HasValue)
            {
                this.CurrentItemElapsedTime = this.ElapsedTime - candidateItemStartTime.Value;

                if (this.CurrentItemElapsedTime < TimeSpan.Zero)
                {
                    this.CurrentItemElapsedTime = TimeSpan.Zero;
                }

                this.CurrentItemRemainingTime = candidateCurrentAgendaItem.Duration - this.CurrentItemElapsedTime;
               
                if (this.CurrentItemRemainingTime < TimeSpan.Zero)
                {
                    this.CurrentItemRemainingTime = TimeSpan.Zero;
                }
            }
            else
            {
                this.CurrentItemElapsedTime = TimeSpan.Zero;
                this.CurrentItemRemainingTime = TimeSpan.Zero;            
            }
        }       

        private void UpdateStatus()
        {
            if (this.Meeting != null)
            {
                if (this.RemainingTime == TimeSpan.Zero)
                {
                    this.Status = TimerStatus.Completed;
                }
                else if (DateTime.Now.TimeOfDay >= this.Meeting.StartTime)
                {
                    this.Status = TimerStatus.InProgress;
                }
                else
                {
                    this.Status = TimerStatus.NotStarted;
                }
            }
            else
            {
                this.Status = TimerStatus.NotStarted;
            }
        }
    }
}
