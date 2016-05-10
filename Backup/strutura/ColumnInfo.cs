using System;
using System.Collections.Generic;
using System.Text;
using Gerador.Util;
using System.ComponentModel;
using System.Xml.Serialization;


namespace Gerador.strutura
{
    /// <summary>
    /// Describes a single column of a table.
    /// </summary>

    [Serializable]
    [XmlRoot(ElementName = "ColumnInfo")]
    public struct ColumnInfo : IEquatable<ColumnInfo>
    {
       
        /// <summary>
        /// Data type of the column.
        /// </summary>
        private SqlType sqlType;
        [BrowsableAttribute(false)]
        [XmlElement(ElementName = "SqlType")]
        public SqlType SqlType
        {
            get { return sqlType; }
            set { sqlType = value; }
        }


        /// <summary>
        /// type Oracle.
        /// </summary>
        private String typeOracle;
        [ReadOnlyAttribute(true)]
        public String TypeOracle
        {
            get { return typeOracle; }
            set { typeOracle = value; }
        }


        
        [BrowsableAttribute(false)]
        public string DescAllowNull
        {
            get
            {
                if (AllowNull)
                    return "true";
                else
                    return "false";
            }
        }

        /// <summary>
        /// The name of the column.
        /// </summary>
        private string name;
        [ReadOnlyAttribute(true),CategoryAttribute("Name column"), DescriptionAttribute("The name of the column")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// if autoincrement column.
        /// </summary>
        private bool autoIncrement;
        [ReadOnlyAttribute(true)]
        public bool AutoIncrement
        {
            get { return autoIncrement; }
            set { autoIncrement = value; }
        }

        /// <summary>
        /// The .NET column Data type.
        /// </summary>
        private string netDataType;
        [BrowsableAttribute(false)]
        public string NetDataType
        {
            get { return netDataType; }
            set { netDataType = value; }
        }

        /// <summary>
        /// The Original SQL type.
        /// </summary>
        private string originalSQLType;
        [BrowsableAttribute(false)]
        public string OriginalSQLType
        {
            get { return originalSQLType; }
            set { originalSQLType = value; }
        }

        /// <summary>
        /// Default value of the column
        /// </summary>
        private string defaultValue;
        [BrowsableAttribute(false)]
        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }
        /// <summary>
        ///Nome do campo que será gerado 
        /// </summary>
        private string fieldName;
        [CategoryAttribute("Generate Code"), DescriptionAttribute("The name of the field to source code")]
        public string FieldName
        {
            get { return fieldName; }
            set {
                fieldName = Initcap(value); 
            }
        }
        private ColumnType tipoColuna;
        [CategoryAttribute("Generate Code"), BrowsableAttribute(true)]
        public ColumnType TipoColuna
        {
            get { return tipoColuna; }
            set {
                tipoColuna = value;
                if (tipoColuna == ColumnType.BOColumnDouble)
                {
                    Prefixo = "m_d";
                } else if(tipoColuna == ColumnType.BOColumnLong){
                    Prefixo = "m_l";
                } else if(tipoColuna == ColumnType.BOColumnDate ||
                          tipoColuna == ColumnType.BOColumnDateTime) { // ||
                         // tipoColuna == ColumnType.BOColumnTime){
                    Prefixo = "m_dt";
                } else if(tipoColuna == ColumnType.BOColumnString){
                    Prefixo = "m_s";
                } else {
                    Prefixo = "m_b";
                }
            }
        }

        private string prefixo;
        [CategoryAttribute("Generate Code"), BrowsableAttribute(true)]
        public string Prefixo
        {
            get { return prefixo; }
            set { prefixo = value; }
        }
        [CategoryAttribute("Generate Code")]
        public string FullName
        {
            get { return String.Format("{0}{1}", prefixo,  fieldName); }
        }

        private bool selecionar;
        [CategoryAttribute("Generate Code"), BrowsableAttribute(true), DescriptionAttribute("Selecionar o campo para o gerador?")]
        public bool Selecionar
        {
            get { return selecionar; }
            set { selecionar = value; }
        }


        /// <summary>
        /// Data size or leght of the column.
        /// </summary>
        private int size;
        [CategoryAttribute("Generate Code"), ReadOnlyAttribute(false)]
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// The precision of the numeric data in this column.
        /// </summary>
        private int precision;   //[Added by Fredy Muñoz] This field was absent.
        [CategoryAttribute("Generate Code"), ReadOnlyAttribute(false)]
        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        /// <summary>
        /// The scale of the numeric data in this column.
        /// </summary>
        private int scale;
        [CategoryAttribute("Generate Code"), ReadOnlyAttribute(false)]
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// if this column is nullable.
        /// </summary>
        private bool allowNull;
        [CategoryAttribute("Generate Code"), ReadOnlyAttribute(false)]
        public bool AllowNull
        {
            get { return allowNull; }
            set { allowNull = value; }
        }

        private bool upper;
        [CategoryAttribute("Generate Code"), ReadOnlyAttribute(false)]
        public bool Upper
        {
            get { return upper; }
            set { upper = value; }
        }

        [BrowsableAttribute(false)]
        public string UseUpper
        {
            get
            {
                if (Upper)
                    return "true";
                else
                    return "false";
            }
        }

        public override string ToString()
        {

            string campos = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", 
                this.Name,
                this.TypeOracle,
                this.Size,
                this.OriginalSQLType,
                this.AutoIncrement,
                this.AllowNull,
                this.Precision,
                this.Scale,
                this.DefaultValue);

            return campos;

                    
        }

        public  IDictionary<string, SqlType> GetSqlTypes()
        {
            IDictionary<String, SqlType> sqlTypes = new Dictionary<String, SqlType>(30);
            sqlTypes.Add("NVARCHAR2", SqlType.VarChar);
            sqlTypes.Add("NCLOB", SqlType.Text);
            sqlTypes.Add("NCHAR", SqlType.Char);
            sqlTypes.Add("VARCHAR ", SqlType.AnsiVarChar);
            sqlTypes.Add("VARCHAR2", SqlType.AnsiVarChar);
            sqlTypes.Add("LONG", SqlType.AnsiVarChar);
            sqlTypes.Add("CLOB", SqlType.AnsiText);
            sqlTypes.Add("CHAR", SqlType.AnsiChar);
            sqlTypes.Add("RAW", SqlType.Binary);
            sqlTypes.Add("LONG RAW", SqlType.VarBinary);
            sqlTypes.Add("NUMBER(1,0)", SqlType.Boolean);
            sqlTypes.Add("DATE", SqlType.DateTime);
            sqlTypes.Add("NUMBER(28,10)", SqlType.Decimal);
            sqlTypes.Add("DOUBLE PRECISION", SqlType.Double);
            sqlTypes.Add("FLOAT", SqlType.Double);
            sqlTypes.Add("FLOAT(126)", SqlType.Double);
            sqlTypes.Add("REAL", SqlType.Float);
            sqlTypes.Add("FLOAT(63)", SqlType.Float);
            sqlTypes.Add("NUMBER(3,0)", SqlType.Byte);
            sqlTypes.Add("NUMBER(5,0)", SqlType.Int16);
            sqlTypes.Add("NUMBER(10,0)", SqlType.Int32);
            sqlTypes.Add("NUMBER(11,0)", SqlType.UInt32);
            sqlTypes.Add("NUMBER(18,0)", SqlType.TimeStamp);
            sqlTypes.Add("NUMBER(19,0)", SqlType.Int64);
            sqlTypes.Add("NUMBER(20,0)", SqlType.UInt64);
            sqlTypes.Add("BLOB", SqlType.Image);
            sqlTypes.Add("NUMBER(15,4)", SqlType.Money);
            sqlTypes.Add("NUMBER(6,4)", SqlType.SmallMoney);
            sqlTypes.Add("ROWID", SqlType.Int64);
            sqlTypes.Add("fallback for a nonexistent type", SqlType.Unknown);

            return sqlTypes;
        }


        private string Initcap(string palavra)
        {
            string novaPalavra = null;
            if (!string.IsNullOrEmpty(palavra))
            {
                novaPalavra += palavra.Substring(0, 1).ToUpper().Trim(); // primeira posição com maiúsculo
                for (int i = 1; i < palavra.Length; i++) //Loop no tamanho da palavra
                {
                    if (palavra.Substring(i, 1) == " ") //Verificação do espaço
                    {
                        novaPalavra += " " + palavra.Substring(i + 1, 1).ToUpper(); //Coloca-se letra maiúscula para a letra posterior ao espaço em branco.
                        i++; //Incrementa o contator do loop pois já foi tratado a posição posterior do espaço em branco.
                    } else if (palavra.Substring(i, 1) == "_") //Verificação do _
                    {
                        novaPalavra += "_" + palavra.Substring(i + 1, 1).ToUpper(); //Coloca-se letra maiúscula para a letra posterior ao espaço em branco.
                        i++; //Incrementa o contator do loop pois já foi tratado a posição posterior do espaço em branco.
                    }
                    else
                    {
                        novaPalavra += palavra.Substring(i, 1); 
                    }
                }
            }
            return novaPalavra;
        }

        #region IEquatable<ColumnInfo> Members

        public bool Equals(ColumnInfo other)
        {
            if (this.name == other.name)
                return true;
            else
                return false;

        }

        #endregion
    }
}
