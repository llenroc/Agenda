using BellaCodeAgenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BellaCodeAgenda
{
    public class Session : BindableBase
    {
        private Meeting _meeting = new Meeting();

        public Meeting Meeting
        {
            get
            {
                return this._meeting;
            }
            set
            {
                if (this._meeting != value)
                {
                    this._meeting = value;
                    this.RaisePropertyChanged();
                }
            }
        }

    }
}
