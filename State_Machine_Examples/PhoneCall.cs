using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using Stateless.Graph;

namespace State_Machine_Examples
{
    class PhoneCall
    {
        enum Trigger
        {
            CallDialed,
            CallConnected,
            LeftMessage,
            PlacedOnHold,
            TakenOffHold,
            PhoneHurledAgainstWall,
            MuteMicrophone,
            UnmuteMicrophone,
            SetVolume
        }
        enum State
        {
            OffHook,
            Ringing,
            Connected,
            OnHold,
            PhoneDestroyed
        }

        State _state = State.OffHook;
        StateMachine<State, Trigger> _machine;  //definicja maszyny stanów
        StateMachine<State, Trigger>.TriggerWithParameters<int> _setVolumeTrigger;  //definicja maszyny sterującej głośnością
        StateMachine<State, Trigger>.TriggerWithParameters<string> _setCalleeTrigger;   //definicja maszyny persony dzwoniącej

        string _caller;//rozmówca telefoniczny
        string _callee; //wywołanie

        public PhoneCall(string caller)
        {
            //konstruktor
            this._caller = caller;  //przekazanie rozmówcy z parametru do zmiennej prywatnej
            this._machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);    //deklaracja maszyny 

            this._setVolumeTrigger = _machine.SetTriggerParameters<int>(Trigger.SetVolume);     //deklaracja maszyny ustawienia głośności
            this._setCalleeTrigger = _machine.SetTriggerParameters<string>(Trigger.CallDialed); //deklaracja maszyny wywołania rozmowy

            this._machine.Configure(State.OffHook)              //konfiguracja pierwszego stanu = odłożony
                .Permit(Trigger.CallDialed, State.Ringing);     //trigger = wybrane połączenie, status = dzwoni

            this._machine.Configure(State.Ringing)
                .OnEntryFrom(_setCalleeTrigger, callee => OnDialed(callee), "Numer dzwoniącego, aby zadzwonić")
                .Permit(Trigger.CallConnected, State.Connected);

            this._machine.Configure(State.Connected)
                .OnEntry(t => StartCallTimer())
                .OnExit(t => StopCallTimer())
                .InternalTransition(Trigger.MuteMicrophone, t => OnMute())
                .InternalTransition(Trigger.UnmuteMicrophone, t => OnUnMute())
                .InternalTransition<int>(_setVolumeTrigger, (volume, t) => OnSetVolume(volume))
                .Permit(Trigger.LeftMessage, State.OffHook)
                .Permit(Trigger.PlacedOnHold, State.OnHold);

            this._machine.Configure(State.OnHold)
                .SubstateOf(State.Connected)
                .Permit(Trigger.TakenOffHold, State.Connected)
                .Permit(Trigger.PhoneHurledAgainstWall, State.PhoneDestroyed);

            this._machine.OnTransitioned(t => Console.WriteLine($"Po przejściu: {t.Source} -> {t.Destination} przez {t.Trigger}({string.Join(", ", t.Parameters)})"));
        }

    #region "Public method"
        public void Mute()
        {
            //ustaw na wyciszenie
            _machine.Fire(Trigger.MuteMicrophone);  //odpalenie stanu
        }
        public void UnMute()
        {
            //ustaw na niewyciszony
            _machine.Fire(Trigger.UnmuteMicrophone);    //odpalenie maszyny stanowej
        }
        public void SetVolume(int volume)
        {
            //ustaw głośność
            _machine.Fire(_setVolumeTrigger, volume);   //odpalenie maszyny stanowej
        }
        public void Print()
        {
            //wyświetl
            Console.WriteLine("[{1}] nawiązał połączenie i [Status:] {0}", _machine.State, _caller);
        }
        public void Dialed(string callee)
        {
            // wybrany
            this._machine.Fire(_setCalleeTrigger, callee);  //odpalenie maszyny stanowej
        }
        public void Connected()
        {
            //połączony
            this._machine.Fire(Trigger.CallConnected);  //odpalenie maszyny stanów 
        }
        public void Hold()
        {
            //wstrzymaj rozmowę
            this._machine.Fire(Trigger.PlacedOnHold);   //odpalenie maszyny stanowej
        }
        public void Resume()
        {
            //przywróć rozmowę
            this._machine.Fire(Trigger.TakenOffHold);   //odpalenie maszyny stanowej
        }
        public string ToDotGraph()
        {
            //wykonaj graf
            return UmlDotGraph.Format(_machine.GetInfo()); 
        }

    #endregion


    #region "Private method"
        void OnSetVolume(int volume)
        {
            //ustaw głośność
            Console.WriteLine("Ustawiam głośność na " + volume + "!");
        }
        void OnUnMute()
        {
            //ustaw na nie wyciszony
            Console.WriteLine("Mikrofon włączony!");
        }
        void OnMute()
        {
            //ustaw na wyciszony
            Console.WriteLine("Mikrofon wyciszony!");
        }
        void OnDialed(string callee)
        {
            //wywołanie 
            _callee = callee;
            Console.WriteLine("[Połączenie telefoniczne] do : [{0}]", _callee);
        }
        void StartCallTimer()
        {
            //start czasu rozmowy
            Console.WriteLine("[Czas:] rozmowa rozpoczeła się 0 {0}", DateTime.Now);
        }
        void StopCallTimer()
        {
            //stop czasu rozmowy
            Console.WriteLine("[Czas:] rozmowa zakończyła się {0}", DateTime.Now);
        }
    #endregion

    }
}
