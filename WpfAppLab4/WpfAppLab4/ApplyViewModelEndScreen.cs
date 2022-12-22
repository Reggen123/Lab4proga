using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppLab4
{
    class ApplyViewModelEndScreen : INotifyPropertyChanged
    {
        private string labeltext;
        public ApplyViewModelEndScreen(string text)
        {
            labeltext = text;
        }
        public string LabelText
        {
            get
            {
                return labeltext;
            }
            set
            {
                labeltext = value;
                OnPropertyChanged("LabelText");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
