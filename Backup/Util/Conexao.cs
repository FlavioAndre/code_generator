using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;
using System.Data;


namespace Gerador.Util
{
    public sealed  class Conexao
    {
        private static readonly Conexao instance = new Conexao();
        private Conexao() { }
        public static Conexao Instance
        {
            get 
            {
              return instance; 
           }
       }

        private string usuario;

        public string Usuario
        {
            get { return usuario; }
            set { usuario = value; }
        }
        private string senha;

        public string Senha
        {
            get { return senha; }
            set { senha = value; }
        }
        private string banco;

        public string Banco
        {
            get { return banco; }
            set { banco = value; }
        }

        private OracleConnection connection;

        public OracleConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }
        private OracleCommand cmd;

        public OracleCommand Cmd
        {
            get { return cmd; }
            set { cmd = value; }
        }

        public void Open()
        {
            string conexao = String.Format("Data Source={0};User ID={1};Password={2}",this.Banco,this.Usuario,this.Senha);
            try
            {
                this.Connection = new OleConnection(conexao);

                IDbCommand cmdDB = connection.CreateCommand();
                cmdDB.CommandTimeout = 0;
                this.Cmd = (OracleCommand)cmdDB;
                this.Cmd.Connection.Open();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Close()
        {
            this.Connection.Close();
            this.Connection.Dispose();
        }

    }
}
