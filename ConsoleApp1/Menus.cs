using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ConsoleApp1 {
    public class Menus {
        public string MenuPrincipal { get; set; }
        public string Menu { get; set; }
        public string Processo { get; set; }
        public ObservableCollection<Menus> Submenu { get; set; }

        public Menus(string menuPrincipal, string menu, string processo) {
            MenuPrincipal = menuPrincipal;
            Menu = menu;
            Processo = processo;
            Submenu = new ObservableCollection<Menus>();
        }
    }
}
