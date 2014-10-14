using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BellaCodeAgenda.Models
{
    public class AgendaItem : BindableBase
    {
        private string _name;

        public string Name
        {
            get
            {
                return this._name;
            }   
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private TimeSpan _duration;

        public TimeSpan Duration
        {
            get
            {
                return this._duration;
            }
            set
            {
                if (this._duration != value)
                {
                    this._duration = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool _isComplete;

        public bool IsComplete
        {
            get
            {
                return this._isComplete;
            }
            set
            {
                if (this._isComplete != value)
                {
                    this._isComplete = value;
                    this.RaisePropertyChanged();
                }
            }
        }        
    }
}
