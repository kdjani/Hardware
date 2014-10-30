namespace App4.Items
{
  using System.Windows.Input;

  class CommandableItem
  {
    public CommandableItem(string name, ICommand command)
    {
      this.Name = name;
      this.Command = command;
    }

    public string DetailedName
    {
        get
        {
            return this.Name;
        }
    }

    public virtual string Name { get; private set; }
    public ICommand Command { get; private set; }
  }
}
