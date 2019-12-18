﻿using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core.ViewModels
{
    public class SignalTree : ViewModelBase
    {
        public SignalTree()
        {

        }
        public SignalTree(string label)
        {
            this._label = label;
            _signal = null;
            _signalList = new ObservableCollection<SignalTree>();
            _isChecked = false;
            _parent = null;
        }

        public SignalTree(SignalViewModel signal)
        {
            _signal = signal;
            _signalList = null;
            _isChecked = false;
            signal.SignalTreeContained.Add(this);
            _parent = null;
        }

        private string _label;
        public string Label
        {
            get
            {
                if (string.IsNullOrEmpty(_label))
                {
                    return _signal.SignalName;
                }
                else
                {
                    return _label;
                }
            }
        }
        private SignalViewModel _signal;
        public SignalViewModel Signal
        {
            get { return _signal; }
            set
            {
                if (_signal != value)
                {
                    _signal = value;
                    _signalList = null;
                    _signal.SignalTreeContained.Add(this);
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<SignalTree> _signalList;
        public ObservableCollection<SignalTree> SignalList
        {
            get { return _signalList; }
            set
            {
                if (_signalList != value)
                {
                    _signalList = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool? _isChecked;
        public bool? IsChecked
        {
            get
            {
                if (_signal != null)
                {
                    return _signal.IsChecked;
                }
                else
                {
                    return _isChecked;
                }
            }
            set
            {
                if ((_signal == null && _isChecked != value) || (_signal != null && _signal.IsChecked != value))
                {
                    _isChecked = value;

                    //1, check direct children without invoke the ischecked property, but also call onpropertychanged
                    CheckDirectChildren(value);

                    //2, check direct parents without invoke the ischecked property, but also call onpropertychanged
                    CheckDirectParent(value);

                    //3, invoke plot redraw
                    OnSignalCheckStatusChanged();

                    OnPropertyChanged();
                }
            }
        }
        // this event is for redraw the plot when fired.
        public event SignalCheckStatusChangedEventHandler SignalCheckStatusChanged;
        public delegate void SignalCheckStatusChangedEventHandler(SignalTree e);
        public void OnSignalCheckStatusChanged()
        {
            SignalCheckStatusChanged?.Invoke(this);
        }
        //change check status of direct parent of this tree, it recursively calls the parent's check direct parent function
        public void CheckDirectParent(bool? value)
        {
            if (_parent != null)
            {
                _parent.DetermineIsCheckedStatus(value); // determine check status of parent node
                _parent.CheckDirectParent(value); //determine check status of parent node's parent
            }
        }
        //change check status of direct children of the tree
        public void CheckDirectChildren(bool? value)
        {
            if (_signal != null && value != null && _signal.IsChecked != (bool)value)
            {// if the tree is a leaf node, it only have a signal, but no signal list, the leaf node's ischecked value cannot be null as it only has one signal
                ////change the signal's check status without invoke the setter of IsChecked
                //_signal.ChangeIsCheckedStatus(value);
                //find the other tree that also contain this signal and change the other's tree's check status if the node(s) is a parent of this signal
                foreach (var p in _signal.SignalTreeContained)
                {
                    //the other tree cannot be null or the same as this tree
                    if (p != this && p != null)
                    {
                        p.CheckDirectParent(value);
                    }
                }
            }
            else if (_signalList != null && _signalList.Count() > 0 && value != null)
            {// if the tree is not a leaf node, need to change the check status of each child
                foreach (var item in SignalList)
                {
                    //item.IsChecked = value;
                    item.ChangeIsCheckedStatus(value);
                    //item.CheckDirectChildren(value);
                }
            }
        }
        // change check status of this tree without invoke the setter of IsChecked property
        public void ChangeIsCheckedStatus(bool? value)
        {
            _isChecked = value;
            CheckDirectChildren(value); // propagate the check status to direct children
            OnPropertyChanged("IsChecked");
        }
        // determin check status without invoke IsChecked property's setter
        public void DetermineIsCheckedStatus(bool? value)
        {
            bool hasChecked = false;
            bool hasUnChecked = false;
            foreach (var item in SignalList)
            {
                if (item.IsChecked == null)
                {
                    // if one of the child is null, this node should be null
                    _isChecked = null;
                    OnPropertyChanged("IsChecked");
                    return;
                }
                if (item.IsChecked == true && hasUnChecked)
                {
                    // if one of the children is checked and also has a unchecked child, this node should be null
                    _isChecked = null;
                    OnPropertyChanged("IsChecked");
                    return;
                }
                if (item.IsChecked == false && hasChecked)
                {
                    // if one of the children is unchecked and also has a checked child, this node should be null
                    _isChecked = null;
                    OnPropertyChanged("IsChecked");
                    return;
                }
                if (item.IsChecked == true && hasChecked != true)
                {
                    hasChecked = true;
                }
                if (item.IsChecked == false && hasUnChecked != true)
                {
                    hasUnChecked = true;
                }
            }
            // this node should be the status as the hasChecked flag, if true, all children are checked, if false, all children are unchecked
            _isChecked = hasChecked;
            OnPropertyChanged("IsChecked");
        }

        //public void CheckAllChildren(bool? value)
        //{
        //    if (_signal != null)
        //    {
        //        _signal.IsChecked = (bool)value;
        //        IsChecked = value;
        //    }
        //    else
        //    {
        //        foreach (var item in SignalsList)
        //        {
        //            item.IsChecked = value;
        //            item.CheckAllChildren(value);
        //        }
        //    }
        //}
        private SignalTree _parent;
        public SignalTree Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }
}
