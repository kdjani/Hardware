namespace App4.Common
{
  using System;
  using System.Windows.Input;

  class SimpleCommand : ICommand
  {
    Action action;
    Action<object> paramAction;
    public SimpleCommand(Action action)
    {
      this.action = action;
    }
    public SimpleCommand(Action<object> action)
    {
      this.paramAction = action;
    }
    public bool CanExecute(object parameter)
    {
      return (true);
    }

#pragma warning disable 0067
    public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

    public void Execute(object parameter)
    {
      if (this.action != null)
      {
        this.action();
      }
      else if (this.paramAction != null)
      {
        this.paramAction(parameter);
      }
    }
  }
}
