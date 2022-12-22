using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryLab4
{
    public interface IObservable
    {
        void AddObserver(IObserver o);
        void RemoveObserver(IObserver o);
        void NotifyGameStart();
        void NotifyGameEnd();
        void NotifyGameAnswer();
    }


    public interface IObserver
    {
        void UpdateGameStart();
        void UpdateGameEnd();
        void UpdateGameAnswer();
    }
}
