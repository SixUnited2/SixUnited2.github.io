using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace yz.gaming.accessoryapp.Utils.Command
{
    public class ClickCommand : ICommand
    {
        public bool CanExecutedFlag { get; set; }
        public Action<object> Action { get; set; }
        public ClickCommand(Action<object> action)
        {
            Action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecutedFlag;
        }

        public void Execute(object parameter)
        {
            CanExecutedFlag = !CanExecutedFlag;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            Action?.Invoke(parameter);
            CanExecutedFlag = !CanExecutedFlag;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
