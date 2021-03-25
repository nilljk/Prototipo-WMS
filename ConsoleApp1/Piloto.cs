using System;
//using System.Collections.Generic;
//using System.Text;
using System.Net;
//using System.Text.Json;
//using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;
//using System.Data.SqlClient;
//using System.Data.OleDb;

using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Data.Common;

namespace ConsoleApp1 {
    public class Piloto {
        public static string Serialize(object o, StringEscapeHandling stringEscapeHandling) {
            StringWriter wr = new StringWriter();
            var jsonWriter = new JsonTextWriter(wr);
            jsonWriter.StringEscapeHandling = stringEscapeHandling;
            new JsonSerializer().Serialize(jsonWriter, o);
            return wr.ToString();
        }
        public static void Listener1(string[] prefixes) {

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            //builder.DataSource = "DESKTOP-M50D7IG";
            builder.DataSource = "DESKTOP-OTNQ631";  
            builder.UserID = "sa";
            builder.Password = "nilladm";
            builder.InitialCatalog = "dbPrototipo";

            SqlConnection connection = new SqlConnection(builder.ConnectionString);

            Boolean bol = true;            
            if (!HttpListener.IsSupported) {
                Console.WriteLine("Seu ambiente não suporta os recursos da classe HttpListener.");
                return;
            }
            // Os prefixos URI são obrigatórios
            // por exemplo "http://macoratti.net:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");
            // Cria um listener.
            HttpListener listener = new HttpListener();
            // Adiciona os prefixos
            foreach (string s in prefixes) {
                listener.Prefixes.Add(s);                
            }

            //inicia o listener
            listener.Start();
            Console.WriteLine("Escutando...");
/*
            ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, errors) => {
                return true;
            };
*/
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            while (bol) {
                // O método GetContext bloqueia enquanto aguarda pela requisição
                HttpListenerContext context = listener.GetContext();

                HttpListenerRequest request = context.Request;
                System.IO.Stream output = null;

                string text = "";
                using (var reader = new StreamReader(request.InputStream,
                                     request.ContentEncoding)) {
                    text = reader.ReadToEnd();
                }

                if (request.RawUrl.Contains("tela")) {
                    HttpListenerResponse response = context.Response;
                    response.Headers.Add("Content-Type", "text/json");

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Serialize("", StringEscapeHandling.Default));
                    response.ContentLength64 = buffer.Length;
                    output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                }
                else if(request.RawUrl.Contains("login")) {
                    Login login = JsonConvert.DeserializeObject<Login>(text);
                    HttpListenerResponse response = context.Response;
                    response.Headers.Add("Content-Type", "text/json");

                    //ObservableCollection<Menus> menuList1 = new ObservableCollection<Menus>();
                    //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Serialize(menuList1, StringEscapeHandling.Default));
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Serialize(GetMenu(login.Id, login.Senha, connection), StringEscapeHandling.Default));
                    response.ContentLength64 = buffer.Length;
                    output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                }else if (request.RawUrl.Contains("processo")) {
                    Tela telaTemp = new Tela {
                        NomeProcesso = "Processo Alertório",
                        AcaoRequerida = "Informe a Onda",
                        InputDados = "66666666",
                        Alerta = text,
                        BotaoEnter = true,
                        BotaoVoltar = true,
                        BotaoAcaoGenerica = false
                    }; 
                    
                    HttpListenerResponse response = context.Response;
                    response.Headers.Add("Content-Type", "text/json");
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Serialize(telaTemp, StringEscapeHandling.Default));
                    response.ContentLength64 = buffer.Length;
                    output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                }


                // Obtém o objeto response
                // HttpListenerResponse response = context.Response;
                // response.Headers.Add("Content-Type", "text/json");

                // Constrói uma resposta
                //string responseString = "<HTML><BODY> Macoratti .Net - Quase tudo para .NET!</BODY></HTML>";

                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(postData);

                //  byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Serialize(telaTemp, StringEscapeHandling.Default));



                //byte[] buffer = System.Text.Encoding.ASCII.GetBytes(postData);
                //byte[] buffer = System.Text.Encoding.GetEncoding("850").GetBytes(postData);
                //byte[] buffer = JsonSerializer.SerializeToUtf8Bytes(telaTemp);



                // Obtem uma stream e escreve a resposta nela
                //response.ContentLength64 = buffer.Length;
                //response.ContentEncoding = Encoding.Unicode;
                //response.Headers.Add("Content-Type", "text/json");
                //response.ContentType = "text/json";


                //System.IO.Stream output = response.OutputStream;                
                //output.Write(buffer, 0, buffer.Length);

                if (!bol) {
                    // fecha o stream
                    output.Close();
                    //para o listner
                    listener.Stop();
                }
            }

        }

        public static ObservableCollection<Menus> GetMenu(string id, string senha, SqlConnection connection) {


            ObservableCollection<Menus> menuList = new ObservableCollection<Menus>();

            connection.Open();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.perfil, a.raiz, a.menu, a.processo ");
            sb.Append("FROM menus a INNER JOIN usuarios b ");
            sb.Append("ON a.perfil = b.perfil ");
            sb.AppendFormat("WHERE b.id  = '{0}' ", id);
            sb.AppendFormat("AND b.senha = '{0}' ", senha);
            sb.Append("ORDER BY sequencia ");
            String sql = sb.ToString();

            SqlCommand command = new SqlCommand(sql, connection);
            DataTable tab = ObterTabela(command.ExecuteReader());
            connection.Close();


            foreach (DataRow dr in tab.Rows) {
                
                if (Convert.ToString(dr["processo"]).Trim() == ""){
                    menuList.Add(new Menus( Convert.ToString(dr["raiz"]) , Convert.ToString(dr["menu"]), null));
                }
                else {
                    menuList[menuList.Count - 1].Submenu.Add(new Menus(Convert.ToString(dr["raiz"]), Convert.ToString(dr["menu"]), Convert.ToString(dr["processo"])));                   
                }
            }

            return menuList;

        }

            public static DataTable ObterTabela(DbDataReader reader) {
            DataTable tbEsquema = reader.GetSchemaTable();
            DataTable tbRetorno = new DataTable();

            foreach (DataRow r in tbEsquema.Rows) {
                if (!tbRetorno.Columns.Contains(r["ColumnName"].ToString())) {
                    DataColumn col = new DataColumn() {
                        ColumnName = r["ColumnName"].ToString(),
                        Unique = Convert.ToBoolean(r["IsUnique"]),
                        AllowDBNull = Convert.ToBoolean(r["AllowDBNull"]),
                        ReadOnly = Convert.ToBoolean(r["IsReadOnly"])
                    };
                    tbRetorno.Columns.Add(col);
                }
            }

            while (reader.Read()) {
                DataRow novaLinha = tbRetorno.NewRow();
                for (int i = 0; i < tbRetorno.Columns.Count; i++) {
                    novaLinha[i] = reader.GetValue(i);
                }
                tbRetorno.Rows.Add(novaLinha);
            }

            return tbRetorno;
        }

    }
}