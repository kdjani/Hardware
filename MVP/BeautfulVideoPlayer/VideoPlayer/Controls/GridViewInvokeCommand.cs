using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App4.Controls
{
  public class GridViewWithClickCommand : GridView
  {
    public static DependencyProperty ClickCommandProperty =
      DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(GridViewWithClickCommand), null);

    public ICommand ClickCommand
    {
      get
      {
        return ((ICommand)base.GetValue(ClickCommandProperty));
      }
      set
      {
        base.SetValue(ClickCommandProperty, value);
      }
    }
    public GridViewWithClickCommand()
    {
      this.ItemClick += OnItemClick;
      this.IsItemClickEnabled = true;
    }
    void OnItemClick(object sender, ItemClickEventArgs e)
    {
      if ((this.ClickCommand != null) &&
        (this.ClickCommand.CanExecute(e.ClickedItem)))
      {
        this.ClickCommand.Execute(e.ClickedItem);
      }
    }
  }
}