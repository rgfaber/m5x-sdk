using System;
using M5x.Tty.Views;
using NStack;

namespace Example
{
    public class MenuItemDetails : MenuItem
    {
        private Action action;
        private string help;
        private ustring title;

        public MenuItemDetails(ustring title, string help, Action action) : base(title, help, action)
        {
            this.title = title;
            this.help = help;
            this.action = action;
        }

        public static MenuItemDetails Instance(MenuItem mi)
        {
            return (MenuItemDetails) mi.GetMenuItem();
        }
    }
}