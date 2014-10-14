using BellaCode.Mvvm;
using BellaCodeAgenda.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace BellaCodeAgenda.ViewModels
{
    public class MeetingViewModel : ViewModel<Meeting>
    {
        public MeetingViewModel()
        {
        }

        public Meeting Meeting
        {
            get
            {
                return this.Model;
            }
        }

        private string _agendaText = "One\r\nTwo\r\nThree\r\nFour\r\nFive\r\nSix\r\nSeven\r\nEight\r\nNine\r\nTen";

        /// <summary>
        /// The agenda the meeting represented by a series of items (one per line).
        /// Each item is an optional number of minutes followed by a title.
        /// </summary>
        public string AgendaText
        {
            get
            {
                return this._agendaText;
            }
            set
            {
                if (this._agendaText != value)
                {
                    this._agendaText = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        // This regular expression creates a match for each agenda item line
        // start of line
        // any whitespace
        // contiguous digits (captured as minutes)
        // if there are digits then at least one whitespace character
        // any characters that are not \r (captured as name)
        // any characters
        // end of line        

        private static char[] LineEndings = new char[] { '\r', '\n' };

        private void UpdateMeetingAgendaItems()
        {
            this.Meeting.AgendaItems.Clear();

            if (!string.IsNullOrEmpty(this.AgendaText))
            {
                var lines = this.AgendaText.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var agendaItem = TryParseAgendaItem(line);
                    if (agendaItem != null)
                    {
                        this.Meeting.AgendaItems.Add(agendaItem);
                    }
                }
            }

            // I split the minutes not used by agenda items across those without durations.
            var noDurationAgendaItems = this.Meeting.AgendaItems.Where(x => x.Duration == TimeSpan.Zero);
            if (noDurationAgendaItems.Any())
            {
                var allocatedMinutes = this.Meeting.AgendaItems.Sum(x => x.Duration.TotalMinutes);
                var leftOverMinutes = Math.Max(0, this.Meeting.ExpectedDuration.TotalMinutes - allocatedMinutes);
                //var leftOverMinutesPerItem = (int)Math.Floor(leftOverMinutes / (double)noDurationAgendaItems.Count());
                var leftOverMinutesPerItem = leftOverMinutes / (double)noDurationAgendaItems.Count();

                foreach (var noDurationAgendaItem in noDurationAgendaItems)
                {
                    noDurationAgendaItem.Duration = TimeSpan.FromMinutes(leftOverMinutesPerItem);
                }
            }
        }
     
        private static AgendaItem TryParseAgendaItem(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return null;
                }

                // The agenda item text is a string (possibly containing numbers), followed by an optional trailing number of minutes.
                var nameBuilder = new StringBuilder();
                var minutesBuilder = new StringBuilder();
                var inWhitespace = false;
                foreach (char c in text)
                {
                    // Ignore line-feed characters
                    if (c == '\r' || c == '\n')
                    {
                        continue;
                    }

                    if (char.IsDigit(c))
                    {
                        // If there is whitespace between two numbers, then the previous number is not the minutes.
                        if (minutesBuilder.Length > 0 && inWhitespace)
                        {
                            nameBuilder.Append(minutesBuilder.ToString());
                            minutesBuilder.Clear();
                            inWhitespace = false;
                        }

                        minutesBuilder.Append(c);
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        inWhitespace = true;

                        // Append whitespace to any existing minutes in case it ends up being part of the name.
                        if (minutesBuilder.Length > 0)
                        {
                            minutesBuilder.Append(c);
                        }
                        else
                        {
                            nameBuilder.Append(c);
                        }
                    }
                    else
                    {
                        inWhitespace = false;

                        // If there are additional non-digits, non-whitespace characters, then the previous number is not the minutes.
                        if (minutesBuilder.Length > 0)
                        {
                            nameBuilder.Append(minutesBuilder.ToString());
                            minutesBuilder.Clear();
                        }

                        nameBuilder.Append(c);
                    }
                }

                var name = nameBuilder.ToString().Trim();
                var minutes = minutesBuilder.ToString().Trim();

                int numberOfMinutes = (minutes.Length > 0) ? int.Parse(minutes) : 0;
                return new AgendaItem() { Name = name, Duration = TimeSpan.FromMinutes(numberOfMinutes) };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not parse agenda text item." + ex.Message);
                return null;
            }
        }


        public void StartMeeting()
        {
            this.UpdateMeetingAgendaItems();

            var window = Window.GetWindow(this.View as DependencyObject);

            // TODO: Move into ShowTimerWindowAction
            TimerWindow timerWindow = new TimerWindow();
            timerWindow.Owner = window;
            timerWindow.DataContext = this.Meeting;

            timerWindow.Closing += TimerWindow_Closing;
            timerWindow.Closed += TimerWindow_Closed;

            // TODO: Move into RestoreWindowPositionAction and trigger after load?
            if (this._lastTimerWindowPostion.HasValue)
            {
                timerWindow.Top = this._lastTimerWindowPostion.Value.Y;
                timerWindow.Left = this._lastTimerWindowPostion.Value.X;
                timerWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            }


            timerWindow.Show();

            window.Hide();
        }

        private Point? _lastTimerWindowPostion;

        void TimerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var timerWindow = (Window)sender;
            _lastTimerWindowPostion = new Point(timerWindow.Left, timerWindow.Top);
        }

        void TimerWindow_Closed(object sender, EventArgs e)
        {
            var timerWindow = (Window)sender;
            
            timerWindow.Closing -= TimerWindow_Closing;
            timerWindow.Closed -= this.TimerWindow_Closed;

            // TODO: Move into Show/HideMainWindowAction
            var window = Window.GetWindow(this.View as DependencyObject);
            window.Show();
        }

        public bool CanSetMeetingStartToNow
        {
            get
            {
                return this.Meeting != null;
            }
        }

        public void SetMeetingStartToNow()
        {
            this.Meeting.StartTime = TimeSpan.FromMinutes(Math.Floor(DateTime.Now.TimeOfDay.TotalMinutes));
        }
    }
}
