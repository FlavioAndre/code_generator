using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;
using Gerador.Util;
using Gerador.strutura;
using System.IO;

namespace Gerador
{
    public partial class frmSelectionTables : Form
    {
        private IDictionary<String, SqlType> sqlTypes;
        private ColumnInfoS columns = new ColumnInfoS();
        private IDictionary<String, String> dicCampos = new Dictionary<String, String>();

        public ColumnInfoS Columns
        {
            get { return columns; }
            set { columns = value; }
        }
        public frmSelectionTables()
        {
            InitializeComponent();
            ColumnInfo columnInfo = new ColumnInfo();
            sqlTypes = columnInfo.GetSqlTypes();

            try
            {
                dicCampos = FileRead.ReadFile(@"c:\Exemplo\ExemploPro\Source\Lib\Include\BO\TableDefs.h");
                foreach (KeyValuePair<String, String> campos in dicCampos)
                {
                    this.lstBoxCampos.Items.Add(campos.Key);
                }

            }
            catch (IOException e)
            {
                Console.WriteLine("I/O error occured" + e);
                return;
            }
        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            string tableName = (string)this.lstBoxCampos.SelectedItem;
            if (tableName == null || tableName == String.Empty)
            {
                MessageBox.Show("Nenhuma tabela foi selecionada", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                this.lstBoxCampos.Focus();
                this.lstBoxCampos.Select();
                return;
            }

            string nameTableName = String.Empty;
            nameTableName = this.dicCampos[tableName];
            this.columns.TabelaDB = nameTableName;
            columns.Columns.Clear();

            //string tableName = (string)this.lstBoxCampos.SelectedItem;
            OracleCommand cmd = Conexao.Instance.Cmd;
            cmd.CommandText = "Select COLUMN_NAME, DATA_TYPE, DATA_LENGTH, DATA_PRECISION, DATA_SCALE, NULLABLE, CHARACTER_SET_NAME from  ALL_TAB_COLUMNS where TABLE_NAME = :TableName and OWNER = :Usuario order by COLUMN_NAME";
            cmd.Parameters.Add("TableName", OracleType.NVarChar, tableName.Length).Value = tableName;
            cmd.Parameters.Add("Usuario",OracleType.NVarChar, Conexao.Instance.Usuario.Length).Value = Conexao.Instance.Usuario;

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {

                    ColumnInfo columnInfo = new ColumnInfo();
                    columnInfo.Name = Convert.ToString(reader["COLUMN_NAME"]);
                    string dataType = Convert.ToString(reader["DATA_TYPE"]);
                    columnInfo.TypeOracle = dataType;
                    if (dataType == "NUMBER")
                    {
                        //dataType = string.Concat(new object[] { dataType, "(", Convert.ToInt32(reader["DATA_PRECISION"]), ",", Convert.ToInt32(reader["DATA_SCALE"]), ")" });
                        //[Changed by Vladimir Chuprinskiy] DATA_PRECISION and DATA_SCALE might be NULL for INTEGER
                        if (reader["DATA_PRECISION"] != DBNull.Value && (reader["DATA_SCALE"] != DBNull.Value))
                            dataType = string.Concat(new object[] { dataType, "(", Convert.ToInt32(reader["DATA_PRECISION"]), ",", Convert.ToInt32(reader["DATA_SCALE"]), ")" });
                        if (reader["DATA_PRECISION"] != DBNull.Value)
                            columnInfo.Precision = Convert.ToInt32(reader["DATA_PRECISION"]);

                        if (reader["DATA_SCALE"] != DBNull.Value)
                            columnInfo.Scale = Convert.ToInt32(reader["DATA_SCALE"]);
                    }
                    if (dataType == "FLOAT")
                    {
                        dataType = string.Concat(new object[] { dataType, "(", Convert.ToInt32(reader["DATA_PRECISION"]), ")" });
                    }
                    if (this.sqlTypes.ContainsKey(dataType))
                    {
                        columnInfo.SqlType = (SqlType)this.sqlTypes[dataType];
                    }
                    else
                    {
                        columnInfo.SqlType = (SqlType)this.sqlTypes["fallback for a nonexistent type"];
                    }
                    if (columnInfo.SqlType == SqlType.Char || columnInfo.SqlType == SqlType.AnsiChar || columnInfo.SqlType == SqlType.VarChar || columnInfo.SqlType == SqlType.AnsiVarChar)
                    {
                        columnInfo.Size = Convert.ToInt32(reader["DATA_LENGTH"]) / 2;
                    }
                    else if ((columnInfo.SqlType == SqlType.Binary) || (columnInfo.SqlType == SqlType.VarBinary))
                    {
                        columnInfo.Size = Convert.ToInt32(reader["DATA_LENGTH"]);
                    }
                    else if (columnInfo.SqlType == SqlType.Decimal)
                    {
                        //columnInfo.Size = Convert.ToInt32(reader["DATA_PRECISION"]);    //[Changed by Fredy Muñoz] The Size field was set with the Precision value because there wasn't a Precision field to use.
                        //columnInfo.Precision = Convert.ToInt32(reader["DATA_PRECISION"]);    //[Added by Fredy Muñoz] 

                        //[Changed by Vladimir Chuprinskiy] DATA_PRECISION and DATA_SCALE might be NULL for INTEGER
                        if (reader["DATA_PRECISION"] != DBNull.Value)
                            columnInfo.Precision = Convert.ToInt32(reader["DATA_PRECISION"]);

                        if (reader["DATA_SCALE"] != DBNull.Value)
                            columnInfo.Scale = Convert.ToInt32(reader["DATA_SCALE"]);
                    }
                    if (reader["NULLABLE"] != DBNull.Value)
                    {
                        string text2 = Convert.ToString(reader["NULLABLE"]);
                        columnInfo.AllowNull = "N" != text2;
                    }
                    if (columnInfo.TypeOracle == "NUMBER")
                    {
                        if (columnInfo.Scale > 0)
                        {
                            columnInfo.TipoColuna = ColumnType.BOColumnDouble;
                        }
                        else
                        {
                            columnInfo.TipoColuna = ColumnType.BOColumnLong;
                        }

                    }
                    else if (columnInfo.TypeOracle == "VARCHAR2")
                    {
                        columnInfo.TipoColuna = ColumnType.BOColumnString;
                    }
                    else if (columnInfo.TypeOracle == "DATE")
                    {
                        columnInfo.TipoColuna = ColumnType.BOColumnDate;
                    }

                    columns.Columns.Add(columnInfo);
                }
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}