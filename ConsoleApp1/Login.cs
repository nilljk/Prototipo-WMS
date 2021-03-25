using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1 {
    public class Login {
        public string Id { get; set; }
        public string Senha { get; set; }
        


        public Login(string id, string senha) {
            this.Id = id;
            this.Senha = senha;
        }

    }
}
