using System;
using System.ComponentModel;

namespace fxLife
{
    public class CellOfLife : INotifyPropertyChanged
    {
        public bool IsAlive
        {
            get
            {
                return _isAlive;
            }

            set
            {
                _isAlive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAlive"));
            }
        }

        private bool _isAlive = false;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
