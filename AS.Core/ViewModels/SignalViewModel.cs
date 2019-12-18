﻿using AS.Core.Models;
using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core.ViewModels
{
    public class SignalViewModel : ViewModelBase
    {
        public SignalViewModel()
        {
            _model = new Signal();
        }
        public SignalViewModel(Signal m)
        {
            _model = m;
        }
        public SignalViewModel(string name) : this()
        {
            _model.SignalName = name;
        }

        public SignalViewModel(string name, string pmu) : this(name)
        {
            _model.PMUName = pmu;
        }

        private Signal _model;

        public bool IsChecked
        {
            get { return _model.IsChecked; }
            set
            {
                if (_model.IsChecked != value)
                {
                    _model.IsChecked = value;
                    //SignalTree aTree = null; // point to one of the signalTree for later to invoke the plot redraw
                    //foreach (var p in SignalTreeContained)
                    //{
                    //    if (p != null)
                    //    {
                    //        p.ChangeIsCheckedStatus(value); // change check status of p
                    //        p.CheckDirectParent(value); // change check status of p's parent
                    //        if (aTree == null)
                    //        {
                    //            aTree = p;
                    //        }
                    //    }
                    //}
                    //if (aTree != null)
                    //{
                    //    aTree.OnSignalCheckStatusChanged(); // redraw plot
                    //}
                    OnPropertyChanged();
                }
            }
        }
        public string SignalName
        {
            get { return _model.SignalName; }
        }
        public string Type
        {
            get { return _model.TypeAbbreviation; }
        }
        public string Unit
        {
            get { return _model.Unit; }
        }
        public List<SignalTree> SignalTreeContained { get; set; } = new List<SignalTree>();
        public int SamplingRate 
        {
            set { _model.SamplingRate = value; }
            get { return _model.SamplingRate; } 
        }

        public string TypeAbbreviation 
        {
            set { _model.TypeAbbreviation = value; }
            get { return _model.TypeAbbreviation; } 
        }

        public string PMUName 
        {
            get { return _model.PMUName; }
            set { _model.PMUName = value; } 
        }
        //private string _envelopePosition;
        // change the check status of the signal in the table, invoked from the signal tree when the the tree's node is check or uncheck to avoid other functions in the IsChecked setter such as redraw plot.
        //internal void ChangeIsCheckedStatus(bool? value)
        //{
        //    _isChecked = (bool)value;
        //    OnPropertyChanged("IsChecked");
        //}
    }
}
