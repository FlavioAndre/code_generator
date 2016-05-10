using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gerador.strutura
{
    [Serializable]
    [XmlRoot(ElementName = "Perguntas")]
    public class ColumnInfoS
    {
      
        private string nomeClasse;

        [XmlAttribute(AttributeName = "NomeClasse")]
        public string NomeClasse
        {
            get { return nomeClasse; }
            set { nomeClasse = value; }
        }
        private List<ColumnInfo> columns;
        [XmlElement(ElementName = "Columns")]
        public List<ColumnInfo> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        private string tabelaDB;
        [XmlAttribute(AttributeName = "TabelaDB")]
        public string TabelaDB
        {
            get { return tabelaDB; }
            set { tabelaDB = value; }
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

        public ColumnInfoS()
        {
            columns = new List<ColumnInfo>();
        }

    }
}
