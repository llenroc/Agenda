﻿using BellaCode.Mvvm;
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

        private string _agendaText = string.Join("\r\n", new string[] { "5 Introductions", "20 Old Business", "20 New Business", "10 Discussion", "5 Closing" });

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

        private static Regex _agendaItemsRegex = new Regex(@"^\s*(?<minutes>\d+){0,1}(?<name>.*)$", RegexOptions.Compiled | RegexOptions.Multiline);

        private void UpdateMeetingAgendaItems()
        {

            this.Meeting.AgendaItems.Clear();

            if (!string.IsNullOrEmpty(this.AgendaText))
            {
                var matches = _agendaItemsRegex.Matches(this.AgendaText);

                foreach (var match in matches.Cast<Match>())
                {
                    var minutesGroup = match.Groups["minutes"];
                    var nameGroup = match.Groups["name"];

                    int minutes = 0;
                    if (minutesGroup.Success)
                    {
                        // if this fails, it will still be zero minutes.
                        int.TryParse(minutesGroup.Value, out minutes);                        
                    }

                    string name = string.Empty;
                    if (nameGroup.Success)
                    {
                        name = nameGroup.Value.Trim();
                    }

                    // I skip lines with nothing of value in them
                    if (minutes == 0 && string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    // I ensure each item's duration is between 1 minute and 24 hours
                    minutes = Math.Min(Math.Max(1, minutes), 24 * 60);

                    this.Meeting.AgendaItems.Add(new AgendaItem() { Name = name, Duration = TimeSpan.FromMinutes(minutes) });
                }
            }
        }

#if false
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
                foreach (var noDurationAgendaItem in noDurationAgendaItems)
                {
                    noDurationAgendaItem.Duration = TimeSpan.FromMinutes(1);
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
#endif

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
            window.BringIntoView();
        }

        protected override void OnModelChanged(Meeting oldValue, Meeting newValue)
        {
            if (oldValue == null && newValue != null)
            {
                this.InitializeStartTime();
            }

            base.OnModelChanged(oldValue, newValue);
        }

        private void InitializeStartTime()
        {
            var now = DateTime.Now.TimeOfDay;
            this.Meeting.StartTime = new TimeSpan(now.Hours, now.Minutes + 1, 0);
        }
    }
}
