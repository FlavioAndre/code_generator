using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gerador.Util;
using System.IO;
using System.Data.OracleClient;
using System.Collections;
using Gerador.strutura;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using System.Xml; 

namespace Gerador
{
    public partial class frmMain : Form
    {
        
        private ColumnInfoS columns = new ColumnInfoS();
        private TreeNode treeNodeSelecionado = null;
        // private TreeNode rootNode;
        public frmMain()
        {
            InitializeComponent();
        }

        private DialogResult callLogin()
        {
            frmLogin login = new frmLogin();
            try
            {
                return login.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return callLogin();
            }
            return DialogResult.None;
        }

        private void callSelectionTables()
        {
            frmSelectionTables seleciontTables = new frmSelectionTables();
            try
            {
                if (seleciontTables.ShowDialog() == DialogResult.OK)
                {
                    this.columns = seleciontTables.Columns;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }

        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
          

        }

        private void LoadLibraries()
        {
            
            this.LibrariesTreeView.Nodes.Clear();
            this.LibrariesTreeView.Update();
            this.LibrariesTreeView.Refresh();


            TreeNode tablesNode = new TreeNode("Colunas");
            tablesNode.Tag = "Colunas";


            foreach (ColumnInfo pair in columns.Columns)
            {
                TreeNode columnsNode = new TreeNode(pair.Name);
                columnsNode.Tag = pair;
                if (pair.Selecionar)
                {
                    columnsNode.ForeColor = Color.Red;

                }
                tablesNode.Nodes.Add(columnsNode);
                
            }
            this.LibrariesTreeView.Nodes.Add(tablesNode);
            LibrariesTreeView.ExpandAll();
        }

        public void verificaDiretoriosBO()
        {
            string diretorio = String.Format(@"C:\Profiles\gerador\{0}\bo\", this.txtNomeClasse.Text);
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
        }

        public void verificaDiretoriosServer()
        {
            string diretorio = String.Format(@"C:\Profiles\gerador\{0}\server\", this.txtNomeClasse.Text);
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
        }


        public void generateCodeRowBoCPP()
        {
            verificaDiretoriosBO();
            string nameFile;
            nameFile = String.Format("{0}{1}RowBO{2}", String.Format(@"C:\Profiles\gerador\{0}\bo\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".cpp");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();

            string nameTableName = this.columns.TabelaDB;
            string idTableName = Regex.Replace(nameTableName, "_NAME_", "_ID_");


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            builderCode.Append("#include \"stdafx.h\"").Append("\r\n");
            builderCode.Append(String.Format("#include \"MasterData\\{0}RowBO.h\"", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("#include \"BO\\TableDefs.h\"").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#ifdef __BO_UNMANAGED__").Append("\r\n");
            builderCode.Append("#include \"BO\\BOTrace.h\"").Append("\r\n");
            builderCode.Append("#include \"BO\\VauStrings.h\"").Append("\r\n");
            builderCode.Append("#else").Append("\r\n");
            builderCode.Append("#include \"BO\\TraceMacros.h\"").Append("\r\n");
            builderCode.Append("using namespace ExemploPro::Lib::Trace;").Append("\r\n");
            builderCode.Append("using namespace ExemploPro::Lib::BO;").Append("\r\n");
            builderCode.Append("#endif").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("IMPLEMENT_Exemplo_BASE({0}RowBO, BORowBase, ExemploPro::Lib::BO::MasterData )", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("__CLASS_DECO_MD_IMP {0}RowBO::{1}RowBO(void) : BORowBase()", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{	").Append("\r\n");
            TreeNodeCollection treeColunasCollection = LibrariesTreeView.Nodes;

            foreach (TreeNode treeColunas in treeColunasCollection)
            {
                foreach (TreeNode tn in treeColunas.Nodes)
                {
                    if (tn.Level > 0)
                    {
                        ColumnInfo coluna = (ColumnInfo)tn.Tag;
                        if (coluna.Selecionar)
                        {
                            builderCode.Append(String.Format("	{0,-30}= NULL;", coluna.FullName)).Append("\r\n");
                        }
                    }
                }
            }

            builderCode.Append("    ").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("__CLASS_DECO_MD_IMP {0}RowBO::~{1}RowBO(void)", this.txtNomeClasse.Text,this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}RowBO::OnRegisterColumns()", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnRegisterColumns\"));").Append("\r\n");
            builderCode.Append("	//register the base-columns (here ID, Company and Version").Append("\r\n");
            builderCode.Append("    _NBaseColumns cols = (_NBaseColumns)(BC_ID | BC_COMPANY | BC_VERSION);").Append("\r\n");
            builderCode.Append("	//register the other db-columns").Append("\r\n");

            builderCode.Append(String.Format("	RegisterBaseColumns({0}, cols);", idTableName)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            int valor_inicial_coluna = 100;
            foreach (TreeNode treeColunas in treeColunasCollection)
            {
                foreach (TreeNode tn in treeColunas.Nodes)
                {
                    if (tn.Level > 0)
                    {
                        ColumnInfo coluna = (ColumnInfo)tn.Tag;
                        if (coluna.Selecionar)
                        {
                            // builderCode.Append(String.Format("	{0,-30}= __scs(RegisterColumn(new {1,-17}(false,0,0,false),{2}, {3}));", coluna.FullName, coluna.TipoColuna, valor_inicial_coluna++,idTableName)).Append("\r\n");
                            builderCode.Append(formatedRegisterColmn(coluna, ++valor_inicial_coluna, idTableName)).Append("\r\n");
                        }
                    }
                }
            }
            builderCode.Append("    ").Append("\r\n");
            builderCode.Append("    // TODO: ...").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("/**").Append("\r\n");
            builderCode.Append(" * \\brief Sets the Column Names in the DB- Table; used from AdoRecord").Append("\r\n");
            builderCode.Append(" */").Append("\r\n");
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}RowBO::InitTableColumnName()", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"InitTableColumnName\"));").Append("\r\n");
            builderCode.Append("	//connect all the base-columns (here ID, Company and version ").Append("\r\n");
            builderCode.Append(String.Format("    BORowBase::InitBaseTableColumnName(_T(\"\"), {0});    ", nameTableName)).Append("\r\n");
            builderCode.Append("	//connect the other columns with the db-columns-names").Append("\r\n");
            builderCode.Append("").Append("\r\n");

            foreach (TreeNode treeColunas in treeColunasCollection)
            {
                foreach (TreeNode tn in treeColunas.Nodes)
                {
                    if (tn.Level > 0)
                    {
                        ColumnInfo coluna = (ColumnInfo)tn.Tag;
                        if (coluna.Selecionar)
                        {
                            builderCode.Append(String.Format("	{0,-30}->SetTableColumnName(_T(\"\"), _T(\"{1,-30}{2});", coluna.FullName, String.Format("{0}\"),", coluna.Name), nameTableName)).Append("\r\n");
                        }
                    }
                }
            }
            builderCode.Append("    ").Append("\r\n");
            builderCode.Append("    // TODO: ...").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("/*").Append("\r\n");
            builderCode.Append(" * \\brief Sets the Column Names for the display in a form/grid.").Append("\r\n");
            builderCode.Append(" */").Append("\r\n");
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}RowBO::OnInitColumnValues()", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnInitColumnValues\"));").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("    // TODO: ...").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}RowBO::OnSetSearchResultSelectList()", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnSetSearchResultSelectList\"));").Append("\r\n");
            builderCode.Append("    // TODO: ...").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");



            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            file.WriteLine(builderCode.ToString());
            file.Close();
        }

        public void generateCodeListBOSH()
        {
            verificaDiretoriosServer();

            string nameFile;
            nameFile = String.Format("{0}{1}ListBOS{2}", String.Format(@"C:\Profiles\gerador\{0}\server\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".h");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();
            builderCode.Append("#include \"StdAfx.h\"").Append("\r\n");
            builderCode.Append("#pragma once").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("#include \"MasterData\\{0}ListBO.h\"", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("#include \"AdoRecordBO.h\"").Append("\r\n");
            builderCode.Append("#include \"BO\\LoginListBO.h\"").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("class {0}ListBOS : public {0}ListBO", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("    DEFINE_Exemplo_BASE({0}ListBOS)", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append(String.Format("	CREATE_INSTANCE({0}ListBOS);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("    CREATE_INSTANCE_REF_REG({0}ListBOS);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("protected:").Append("\r\n");
            builderCode.Append(String.Format("	{0}ListBOS(LoginListBO *loginBO);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append(String.Format("	virtual ~{0}ListBOS(void);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("protected:").Append("\r\n");
            builderCode.Append("	void OnSaveChanges();").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append("	void Search();").Append("\r\n");
            builderCode.Append("	void SelectByID(BOListBase::_NSelectAction nSAC);").Append("\r\n");
            builderCode.Append("    void SelectAll();").Append("\r\n");
            builderCode.Append("};").Append("\r\n");


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            file.WriteLine(builderCode.ToString());
            file.Close();
        }


        public void generateCodeListBOSCPP(){
            this.verificaDiretoriosServer();
            string nameFile;
            nameFile = String.Format("{0}{1}ListBOS{2}", String.Format(@"C:\Profiles\gerador\{0}\server\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".cpp");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();
            builderCode.Append("#include \"StdAfx.h\"").Append("\r\n");
            builderCode.Append(String.Format("#include \"{0}ListBOS.h\"", this.txtNomeClasse.Text)).Append("\r\n");
            // todo: check include correct
            builderCode.Append(String.Format("#include \"MasterData\\{0}RowBO.h\"", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("#include \"BO\\BOTrace.h\"").Append("\r\n");
            builderCode.Append("#include \"Key.h\"").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // IMPLEMENT_Exemplo_BASE
            builderCode.Append(String.Format("IMPLEMENT_Exemplo_BASE({0}ListBOS, {1}ListBO, ExemploPro::Lib::BO::MasterData )", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // ListBOS
            builderCode.Append(String.Format("{0}ListBOS::{1}ListBOS(LoginListBO *loginBO)", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format(": {0}ListBO(loginBO)", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // ~ListBOS
            builderCode.Append(String.Format("{0}ListBOS::~{1}ListBOS(void)", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // SelectByID
            builderCode.Append(String.Format("void {0}ListBOS::SelectByID(BOListBase::_NSelectAction nSAC)", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"SelectByID\"));").Append("\r\n");
            builderCode.Append("    CAdoRecordBO	record(GetSysInfo());").Append("\r\n");
            builderCode.Append("	record.SelectBOById(this);").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // SelectAll
            builderCode.Append(String.Format("void {0}ListBOS::SelectAll()", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"SelectAll\"));").Append("\r\n");
            builderCode.Append("    CAdoRecordBO	record(GetSysInfo());").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	record.SelectBO(this);").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // Search
            builderCode.Append(String.Format("void {0}ListBOS::Search()", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"Search\"));").Append("\r\n");
            builderCode.Append("    CAdoRecordBO	record(GetSysInfo());").Append("\r\n");
            builderCode.Append("	record.PrepareAdoWhere();").Append("\r\n");
            builderCode.Append("	record.AddWhereCompany(PARAM_GROUP_KDFZK);").Append("\r\n");
            builderCode.Append("	record.EndAdoWhere();").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	record.Search(this);").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // OnSaveChanges
            builderCode.Append(String.Format("void {0}ListBOS::OnSaveChanges()", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnSaveChanges\"));").Append("\r\n");
            builderCode.Append("    CAdoRecordBO	record(GetSysInfo());").Append("\r\n");
            builderCode.Append("	record.SaveBO(this, true, true);").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            file.WriteLine(builderCode.ToString());
            file.Close();

        }


        public void generateCodeRowBOH()
        {
            this.verificaDiretoriosBO();
            string nameFile;
            nameFile = String.Format("{0}{1}RowBO{2}", String.Format(@"C:\Profiles\gerador\{0}\bo\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".h");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();
            builderCode.Append("#pragma once").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#include \"stdafx.h\"").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#include \"MasterData\\MasterDataApi.h\"").Append("\r\n");
            builderCode.Append("#ifdef __BO_UNMANAGED__").Append("\r\n");
            builderCode.Append("#include \"BO\\BORowBase.h\"").Append("\r\n");
            builderCode.Append("#include \"BO\\BOColumnTypes.h\"").Append("\r\n");
            builderCode.Append("#else").Append("\r\n");
            builderCode.Append("using namespace ExemploPro::Lib::BO;").Append("\r\n");
            builderCode.Append("#include \"BO\\ExemploBaseMacros.h\"").Append("\r\n");
            builderCode.Append("#endif").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("__CLASS_DECO_MD __gc").Append("\r\n");
            builderCode.Append(String.Format("class _BO_MASTER_DATA_Api_API_ {0}RowBO : public BORowBase", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("	DEFINE_Exemplo_BASE({0}RowBO)", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append(String.Format("	{0}RowBO(void);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	~{0}RowBO(void);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append("	void OnRegisterColumns();").Append("\r\n");
            builderCode.Append("	void InitTableColumnName();").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("//Columns").Append("\r\n");
            builderCode.Append("public:").Append("\r\n");

            TreeNodeCollection treeColunasCollection = LibrariesTreeView.Nodes;

            foreach (TreeNode treeColunas in treeColunasCollection)
            {
                foreach (TreeNode tn in treeColunas.Nodes)
                {
                    if (tn.Level > 0)
                    {
                        ColumnInfo coluna = (ColumnInfo)tn.Tag;
                        if (coluna.Selecionar)
                        {
                            builderCode.Append(String.Format("	{0,-20}*{1};\t//\t{2}", coluna.TipoColuna, coluna.FullName, coluna.Name)).Append("\r\n");
                        }
                    }
                }
            }
            builderCode.Append("").Append("\r\n");
            builderCode.Append("//virtuals").Append("\r\n");
            builderCode.Append("	void OnInitColumnValues();").Append("\r\n");
            builderCode.Append("	void OnSetSearchResultSelectList();").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("//Events").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("};").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("__CLASS_DECO_MD_END").Append("\r\n");


            file.WriteLine(builderCode.ToString());
            file.Close();
        }


        public string formatedRegisterColmn(ColumnInfo coluna, int valor_inicial_coluna, string idTableName)
        {
            switch (coluna.TipoColuna)
            {
                case ColumnType.BOColumnBool:
                    return String.Format("	{0,-30}= __scb(RegisterColumn(new {1,-17}(),{2},{3}));", coluna.FullName, coluna.TipoColuna, valor_inicial_coluna, idTableName);
                case ColumnType.BOColumnDate:
                    return String.Format("	{0,-30}= __scd(RegisterColumn(new {1,-17}({2}),{3},{4}));", coluna.FullName, coluna.TipoColuna, coluna.DescAllowNull, valor_inicial_coluna, idTableName);
                case ColumnType.BOColumnDateTime:
                    return String.Format("	{0,-30}= __scdt(RegisterColumn(new {1,-17}({2}),{3},{4}));", coluna.FullName, coluna.TipoColuna, coluna.DescAllowNull, valor_inicial_coluna, idTableName);
               // case ColumnType.BOColumnTime:
                //    return String.Format("	{0,-30}= __scdt(RegisterColumn(new {1,-17}({2}),{3},{4}));", coluna.FullName, coluna.TipoColuna, coluna.DescAllowNull, valor_inicial_coluna, idTableName);
                case ColumnType.BOColumnLong:
                    return String.Format("	{0,-30}= __scl(RegisterColumn(new {1,-17}({2},0,{3}),{4}, {5}));", coluna.FullName, coluna.TipoColuna, coluna.DescAllowNull, coluna.Size, valor_inicial_coluna, idTableName);
                case ColumnType.BOColumnDouble:
                    return String.Format("	{0,-30}= __scdb(RegisterColumn(new {1,-17}({2},{3},{4}),{5}, {6}));", coluna.FullName, coluna.TipoColuna, coluna.DescAllowNull, coluna.Precision, coluna.Scale, valor_inicial_coluna, idTableName);
                case ColumnType.BOColumnString:
                    return String.Format("	{0,-30}= __scs(RegisterColumn(new {1,-17}({2},0,{3},{4}),{5}, {6}));", coluna.FullName, coluna.TipoColuna, coluna.DescAllowNull, coluna.Size, coluna.UseUpper, valor_inicial_coluna, idTableName);
            }
            return String.Empty;
        }

        public void generateCodeListBOH()
        {
            this.verificaDiretoriosBO();
            string nameFile;
            nameFile = String.Format("{0}{1}ListBO{2}", String.Format(@"C:\Profiles\gerador\{0}\bo\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".h");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();
            builderCode.Append("#pragma once").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#include \"stdafx.h\"").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#include \"MasterData\\MasterDataApi.h\"").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#ifdef __BO_UNMANAGED__").Append("\r\n");
            builderCode.Append("#include \"BO\\BOListBase.h\"").Append("\r\n");
            builderCode.Append("#else").Append("\r\n");
            builderCode.Append("using namespace ExemploPro::Lib::BO;").Append("\r\n");
            builderCode.Append("#include \"BO\\ExemploBaseMacros.h\"").Append("\r\n");
            builderCode.Append("#endif").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("__CLASS_DECO_MD __gc").Append("\r\n");
            builderCode.Append(String.Format("class _BO_MASTER_DATA_Api_API_ {0}ListBO : public BOListBase", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("    DEFINE_Exemplo_BASE({0}ListBO)", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append(String.Format("	CREATE_INSTANCE({0}ListBO);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("protected:").Append("\r\n");
            builderCode.Append(String.Format("	{0}ListBO(LoginListBO *loginBO);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append(String.Format("	virtual ~{0}ListBO(void);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append("	virtual void SelectByID(BOListBase::_NSelectAction nSAC);").Append("\r\n");
            builderCode.Append("	static  void OnSelect(BOListBase* parentList, BOListBase* lovList); ").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("protected:").Append("\r\n");
            builderCode.Append("	BORowBase*	OnCreateRow();").Append("\r\n");
            builderCode.Append("	void		OnRegisterReferences();").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	void OnSaveChanges();").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append("	void		 Search();    ").Append("\r\n");
            builderCode.Append("	void         SelectAll();").Append("\r\n");
            builderCode.Append("};").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("__CLASS_DECO_MD_END").Append("\r\n");

            file.WriteLine(builderCode.ToString());
            file.Close();
        }

        public void generateCodeListBoCPP()
        {
            this.verificaDiretoriosBO();
            string nameFile;
            nameFile = String.Format("{0}{1}ListBO{2}", String.Format(@"C:\Profiles\gerador\{0}\bo\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".cpp");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();
            builderCode.Append("#include \"StdAfx.h\"").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#define GetClassName  GetClassNameW").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("#include \"MasterData\\{0}ListBO.h\"", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("#include \"MasterData\\{0}RowBO.h\"", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("#include \"MasterDataProxy.h\"").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#ifdef __BO_UNMANAGED__").Append("\r\n");
            builderCode.Append("#include \"BO\\BOTrace.h\"").Append("\r\n");
            builderCode.Append("#else").Append("\r\n");
            builderCode.Append("#include \"BO\\TraceMacros.h\"").Append("\r\n");
            builderCode.Append("using namespace ExemploPro::Lib::Trace;").Append("\r\n");
            builderCode.Append("#endif").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("IMPLEMENT_Exemplo_BASE({0}ListBO, BOListBase, ExemploPro::Lib::BO::MasterData )", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // Construtor
            builderCode.Append(String.Format("__CLASS_DECO_MD_IMP {0}ListBO::{1}ListBO(LoginListBO *loginBO) ", this.txtNomeClasse.Text,this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(": BOListBase(loginBO)").Append("\r\n");
            builderCode.Append("{	").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // UnConstrutor
            builderCode.Append(String.Format("__CLASS_DECO_MD_IMP {0}ListBO::~{1}ListBO(void)",this.txtNomeClasse.Text,this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // OnCreateRow
            builderCode.Append(String.Format("BORowBase* __CLASS_DECO_MD_IMP {0}ListBO::OnCreateRow()",this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("	return new {0}RowBO();",this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // SelectByID()
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}ListBO::SelectByID(BOListBase::_NSelectAction nSAC)", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"SelectByID\"));").Append("\r\n");
            builderCode.Append("	// create the proxy for the rpc").Append("\r\n");
            builderCode.Append("    USE_UNMANAGED_PROXY").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	PROXY_CREATE( CMasterDataProxy, pMasterDataProxy, __FILE__, __LINE__)").Append("\r\n");
            builderCode.Append("	").Append("\r\n");
            builderCode.Append("	// remove all previous rows from the bo").Append("\r\n");
            builderCode.Append("	RemoveAllRows();").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	// prepare the select (pack the control-data to the arrays for the call)").Append("\r\n");
            builderCode.Append("	PROXY_PREPARE_SELECT").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	// make the rpc").Append("\r\n");
            builderCode.Append(String.Format("	pMasterDataProxy->Select{0}ByID(PROXY_WORK_DATA);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	// extract the data from the arrays, analyze the result, throw exceptions, ...").Append("\r\n");
            builderCode.Append("	PROXY_ANALYSE_SELECT(pMasterDataProxy)").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	// write back the data to the bo in order of the given select-action").Append("\r\n");
            builderCode.Append("	ManageReturnedData(nSAC);").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");

            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}ListBO::SelectAll()", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"SelectAll\"));").Append("\r\n");
            builderCode.Append("    USE_UNMANAGED_PROXY").Append("\r\n");
            builderCode.Append("	PROXY_CREATE( CMasterDataProxy, pMasterDataProxy, __FILE__, __LINE__)").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	PROXY_PREPARE_SELECT").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	pMasterDataProxy->Select{0}All(PROXY_WORK_DATA);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	PROXY_ANALYSE_SELECT(pMasterDataProxy)").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	ReplaceWithReturnedData();").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");

            builderCode.Append("").Append("\r\n");
            // OnRegisterReferences
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}ListBO::OnRegisterReferences()",this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("    BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnRegisterReferences\")); ").Append("\r\n");
            builderCode.Append("    // TODO: ...").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // OnSaveChanges
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}ListBO::OnSaveChanges()",this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnSaveChanges\"));").Append("\r\n");
            builderCode.Append("    USE_UNMANAGED_PROXY").Append("\r\n");
            builderCode.Append("	PROXY_CREATE( CMasterDataProxy, pMasterDataProxy, __FILE__, __LINE__)").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	PROXY_PREPARE_UPDATE").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	pMasterDataProxy->SaveData{0}(PROXY_WORK_DATA);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	PROXY_ANALYSE_UPDATE(pMasterDataProxy);").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            // Search
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}ListBO::Search()",this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{	").Append("\r\n");
            builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"Search\"));").Append("\r\n");
            builderCode.Append("    USE_UNMANAGED_PROXY").Append("\r\n");
            builderCode.Append("	PROXY_CREATE( CMasterDataProxy, pMasterDataProxy, __FILE__, __LINE__)").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	PROXY_PREPARE_SEARCH").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	pMasterDataProxy->Search{0}(PROXY_WORK_DATA);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	PROXY_ANALYSE_SEARCH(pMasterDataProxy)").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	ReplaceSearchData();").Append("\r\n");
            builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            // Lov-Reference
            builderCode.Append("/**").Append("\r\n");
            builderCode.Append(" * \\brief put to LOV- Reference").Append("\r\n");
            builderCode.Append("*/").Append("\r\n");
            builderCode.Append(String.Format("void __CLASS_DECO_MD_IMP {0}ListBO::OnSelect(BOListBase*, BOListBase* lovList)",this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("    {0}ListBO *boList = static_cast<{1}ListBO*>(lovList);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("	// Todo: ...").Append("\r\n");
            builderCode.Append("}").Append("\r\n");


            file.WriteLine(builderCode.ToString());
            file.Close();
        }

       

        public void generateCodeUCSH()
        {
            this.verificaDiretoriosServer();
            string nameFile;
            nameFile = String.Format("{0}{1}UCS{2}", String.Format(@"C:\Profiles\gerador\{0}\server\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".h");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();
            builderCode.Append("#pragma once").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#include \"BO\\UCBase.h\"").Append("\r\n");
            builderCode.Append("#include \"BO\\LoginListBO.h\"").Append("\r\n");
            builderCode.Append(String.Format("#include \"{0}ListBOS.h\"", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("class {0}UCS : public UCBase", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("    DEFINE_Exemplo_BASE({0}UCS)", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("public:").Append("\r\n");
            builderCode.Append(String.Format("	{0}UCS({0}ListBOS* bo, LoginListBO* login);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	virtual ~{0}UCS();", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("	").Append("\r\n");
            builderCode.Append("	virtual void OnPrepareSaveForInsertUC();").Append("\r\n");
            builderCode.Append("	virtual void OnPrepareSaveForUpdateUC();").Append("\r\n");
            builderCode.Append("    virtual void OnPrepareSaveForDeleteUC();").Append("\r\n");
            builderCode.Append("};").Append("\r\n");
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            file.WriteLine(builderCode.ToString());
            file.Close();
        }

        public void generateCodeUCSCPP()
        {
            this.verificaDiretoriosServer();
            string nameFile;
            nameFile = String.Format("{0}{1}UCS{2}", String.Format(@"C:\Profiles\gerador\{0}\server\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".cpp");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();

			builderCode.Append("#include \"stdafx.h\"").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append("#include \"AdoProcedureBO.h\"").Append("\r\n");
			builderCode.Append(String.Format("#include \"{0}UCS.h\"", this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append(String.Format("#include \"MasterData\\{0}RowBO.h\"", this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("#include \"BO\\LoginRowBO.h\"").Append("\r\n");
			builderCode.Append("#include \"BO\\BOTrace.h\"").Append("\r\n");
			builderCode.Append("#include \"NlsFormat.h\"").Append("\r\n");
			builderCode.Append("#include \"BO\\VauStrings.h\"").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append(String.Format("IMPLEMENT_Exemplo_BASE({0}UCS, {0}UC, ExemploPro::Lib::BO::MasterData )", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append(String.Format("{0}UCS::{0}UCS({0}ListBOS* bo, LoginListBO* login)", this.txtNomeClasse.Text, this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append(": UCBase(bo)").Append("\r\n");
			builderCode.Append("{").Append("\r\n");
			builderCode.Append("	SetLoginBO(login);").Append("\r\n");
			builderCode.Append("}").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append(String.Format("{0}UCS::~{0}UCS()", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("{").Append("\r\n");
			builderCode.Append("}").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append("/*").Append("\r\n");
			builderCode.Append(" * \\brief Things to be done before a new  is added.").Append("\r\n");
			builderCode.Append(" */").Append("\r\n");
			builderCode.Append(String.Format("void {0}UCS::OnPrepareSaveForInsertUC()", this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("{").Append("\r\n");
			builderCode.Append("	BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnPrepareSaveForInsertUC\"));").Append("\r\n");
			builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("    {0}RowBO* row = static_cast<{0}RowBO*>( m_pBOListIsolate->GetRowForInternalData());", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("    LoginRowBO* lr = static_cast<LoginRowBO*>(GetLoginBO()->GetRowCurrentFE());").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append("    //TODO:...").Append("\r\n");
			builderCode.Append("    ").Append("\r\n");
			builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
			builderCode.Append("}").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append("/*").Append("\r\n");
			builderCode.Append(" * \\brief Things to be done before  is updated.").Append("\r\n");
			builderCode.Append(" */").Append("\r\n");
			builderCode.Append(String.Format("void {0}UCS::OnPrepareSaveForUpdateUC()", this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("{").Append("\r\n");
			builderCode.Append("    BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnPrepareSaveForUpdateUC\"));").Append("\r\n");
			builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("    {0}RowBO* row = static_cast<{0}RowBO*>( m_pBOListIsolate->GetRowForInternalData());", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("    LoginRowBO* lr = static_cast<LoginRowBO*>(GetLoginBO()->GetRowCurrentFE());").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append("	//TODO: ...").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
			builderCode.Append("}").Append("\r\n");
			builderCode.Append("").Append("\r\n");
			builderCode.Append("/*").Append("\r\n");
			builderCode.Append(" * \\brief Things to be done before deleted.").Append("\r\n");
			builderCode.Append(" */").Append("\r\n");
			builderCode.Append(String.Format("void {0}UCS::OnPrepareSaveForDeleteUC()", this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("{").Append("\r\n");
			builderCode.Append("    BO_SCOPE_IN(BOTrace::_NTraceFilter::TF_SCOPE_4, BOTrace::_NScopeType::SCT_NORMAL, _T(\"OnPrepareSaveForDeleteUC\"));").Append("\r\n");
			builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("    {0}RowBO* row = static_cast<{0}RowBO*>( m_pBOListIsolate->GetRowForInternalData());", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("   ").Append("\r\n");
			builderCode.Append("     // TODO: ...").Append("\r\n");
			builderCode.Append("    BO_SCOPE_OUT").Append("\r\n");
			builderCode.Append("}").Append("\r\n");


			///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            file.WriteLine(builderCode.ToString());
            file.Close();
        }
        public void genreateTxtConferencia()
        {
            if (!Directory.Exists(@"C:\Profiles\gerador\conferencia\"))
            {
                Directory.CreateDirectory(@"C:\Profiles\gerador\conferencia\");
            }

            string nameFile;
            nameFile = String.Format("{0}{1}Conf_{2}", @"C:\Profiles\gerador\conferencia\", this.txtNomeClasse.Text, ".txt");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            int i = 1;
            foreach (ColumnInfo coluna in this.columns.Columns)
            {
                if (coluna.Selecionar)
                {
                    file.WriteLine(String.Format("{0:000} | {1,-15} | {2,-30} | {3,-30}",i++, this.columns.NomeClasse, coluna.Name, coluna.FieldName));
                }

            }
            file.Close();
        }
        
        public void genreateTxtToDo()
        {
            this.verificaDiretoriosServer();
            string nameFile;
            nameFile = String.Format("{0}{1}ToDo{2}", String.Format(@"C:\Profiles\gerador\{0}\server\", this.txtNomeClasse.Text), this.txtNomeClasse.Text, ".txt");
            System.IO.StreamWriter file = new System.IO.StreamWriter(nameFile);
            StringBuilder builderCode = new StringBuilder();
            builderCode.Append("******************************************************************************************************").Append("\r\n");
            builderCode.Append("******************************************************************************************************").Append("\r\n");
            builderCode.Append("*****************************************    S E R V E R    ******************************************").Append("\r\n");
            builderCode.Append("******************************************************************************************************").Append("\r\n");
            builderCode.Append("******************************************************************************************************").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("// incluir metodos em NvsMasterDataMain.cpp e incluir a declaração dos metodos em NvsMasterData.idl").Append("\r\n");
            builderCode.Append("// o numero de id(XX) tem que ser renumerado").Append("\r\n");
            builderCode.Append("// ***************************************************************************************").Append("\r\n");
            builderCode.Append("// C:\\Exemplo\\ExemploPro\\Source\\Server\\MasterData\\NvsMasterData.idl").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("#pragma region {0}", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("    [id(XX), helpstring(\"Methode Select{0}ByID\")] HRESULT Select{1}ByID(VARIANT vSi, [out] VARIANT* paRecord, [in,out] VARIANT* paLongArr, [in,out] VARIANT* paStringArr, [out] VARIANT* paException, [out] BSTR* pbMsg, [out,retval] VARIANT_BOOL* pbSuccess);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("    [id(XX), helpstring(\"Methode Search{0}\")] HRESULT Search{1}(VARIANT vSi, [out] VARIANT* paRecord, [in,out] VARIANT* paLongArr, [in,out] VARIANT* paStringArr, [out] VARIANT* paException, [out] BSTR* pbMsg, [out,retval] VARIANT_BOOL* pbSuccess);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("    [id(XX), helpstring(\"Methode SaveData{0}\")] HRESULT SaveData{1}([in] VARIANT vSi, [in,out] VARIANT* paRecord, [in,out] VARIANT* paLongArr, [in,out] VARIANT* paStringArr, [out] VARIANT* paException, [out] BSTR* pbMsg, [out,retval] VARIANT_BOOL *);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("    [id(xx), helpstring(\"Methode Select{0}All\")] HRESULT Select{1}All(VARIANT vSi, [out] VARIANT* paRecord, [in,out] VARIANT* paLongArr, [in,out] VARIANT* paStringArr, [out] VARIANT* paException, [out] BSTR* pbMsg, [out,retval] VARIANT_BOOL* pbSuccess);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("#pragma endregion").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("// ***************************************************************************************").Append("\r\n");
            builderCode.Append("C:\\Exemplo\\ExemploPro\\Source\\Server\\MasterData\\NvsMasterDataMain.h").Append("\r\n");
            builderCode.Append(String.Format("#pragma region {0}", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	STDMETHOD(Select{0}ByID)(VARIANT vSi, VARIANT* paRecord, VARIANT* paLongArr, VARIANT* paStringArr, VARIANT* paException, BSTR* pbMsg, VARIANT_BOOL * pbSuccess);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	STDMETHOD(Search{0})(VARIANT vSi, VARIANT* paRecord, VARIANT* paLongArr, VARIANT* paStringArr, VARIANT* paException, BSTR* pbMsg, VARIANT_BOOL * pbSuccess);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	STDMETHOD(SaveData{0})(VARIANT vSi, VARIANT* paRecord, VARIANT* paLongArr, VARIANT* paStringArr, VARIANT* paException, BSTR* pbMsg, VARIANT_BOOL * pbSuccess);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	STDMETHOD(Select{0}All)(VARIANT vSi, VARIANT* paRecord, VARIANT* paLongArr, VARIANT* paStringArr, VARIANT* paException, BSTR* pbMsg, VARIANT_BOOL * pbSuccess);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("#pragma endregion").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("// ***************************************************************************************").Append("\r\n");
            builderCode.Append("source -> C:\\Exemplo\\ExemploPro\\Source\\Server\\MasterData\\NvsMasterDataMain.cpp").Append("\r\n");
            builderCode.Append("// Incluir o arquivo de cabeçalho do UCS").Append("\r\n");
            builderCode.Append(String.Format("#include \"{0}UCS.h\"", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("STDMETHODIMP CNvsMasterDataMain::Select{0}ByID(VARIANT vSi, ", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("												       VARIANT* paRecord, ").Append("\r\n");
            builderCode.Append("												       VARIANT* paLongArr, ").Append("\r\n");
            builderCode.Append("												       VARIANT* paStringArr, ").Append("\r\n");
            builderCode.Append("												       VARIANT* paException, ").Append("\r\n");
            builderCode.Append("												       BSTR* pbMsg, ").Append("\r\n");
            builderCode.Append("												       VARIANT_BOOL * pbSuccess)").Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("    BEGIN_BO_SERVER_METHOD(vSi, _T(\"Select{0}ByID\"));", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	{0}ListBOS	*p{1}ListBOS = NULL;", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	BOListPtr  boPtr =  p{0}ListBOS = {1}ListBOS::CreateInstance(&m_loginBO);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	p{0}ListBOS->ArrayToVOPreSelect(paLongArr, paStringArr);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	p{0}ListBOS->SelectByID(BOListBase::SAC_REPLACE);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("    p{0}ListBOS->VOToArrayPostSelect(paRecord, paLongArr, paStringArr);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	END_BO_SERVER_METHOD(pbMsg, pbSuccess, paException);").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("STDMETHODIMP CNvsMasterDataMain::Search{0}(VARIANT vSi,                // in:  system-information (like normal server-method)", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("												VARIANT* paRecord,          // out: data of the bo").Append("\r\n");
            builderCode.Append("												VARIANT* paLongArr,         // in,out: numeric control-data for the bo").Append("\r\n");
            builderCode.Append("												VARIANT* paStringArr,       // in,out: string control-data for the bo").Append("\r\n");
            builderCode.Append("												VARIANT* paException,       // out: exceptions throws during the server-method").Append("\r\n");
            builderCode.Append("												BSTR* pbMsg,                // out: messages (like normal server-method) ").Append("\r\n");
            builderCode.Append("												VARIANT_BOOL * pbSuccess)   // retval: success (like normal server-method)").Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("    BEGIN_BO_SERVER_METHOD(vSi, _T(\"Search{0}\"));", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("    // create a new instance of the server-bo").Append("\r\n");
            builderCode.Append(String.Format("	{0}ListBOS		*p{1}BOS = NULL;", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	BOListPtr  boPtr =  p{0}BOS = {1}ListBOS::CreateInstance(&m_loginBO);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("    // extract the (control-) data from the arrays to the bo").Append("\r\n");
            builderCode.Append(String.Format("	p{0}BOS->ArrayToVOPreSearch(paLongArr, paStringArr);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("    // call the method in the bo").Append("\r\n");
            builderCode.Append(String.Format("	p{0}BOS->Search();", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("    // pack the data/results from the bo to the arrays").Append("\r\n");
            builderCode.Append(String.Format("    p{0}BOS->VOToArrayPostSearch(paRecord, paLongArr, paStringArr);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("    END_BO_SERVER_METHOD(pbMsg, pbSuccess, paException);").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("STDMETHODIMP CNvsMasterDataMain::SaveData{0}(VARIANT vSi, ", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("                                                  VARIANT* paRecord, ").Append("\r\n");
            builderCode.Append("                                                  VARIANT* paLongArr, ").Append("\r\n");
            builderCode.Append("                                                  VARIANT* paStringArr, ").Append("\r\n");
            builderCode.Append("                                                  VARIANT* paException, ").Append("\r\n");
            builderCode.Append("                                                  BSTR* pbMsg, ").Append("\r\n");
            builderCode.Append("                                                  VARIANT_BOOL* pbSuccess)").Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append(String.Format("    BEGIN_BO_SERVER_METHOD(vSi, _T(\"SaveData{0}\"));", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("    {0}ListBOS		*p{1}ListBOS = NULL;", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	BOListPtr    boPtr = p{0}ListBOS = {1}ListBOS::CreateInstance(&m_loginBO);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("			").Append("\r\n");
            builderCode.Append(String.Format("	p{0}ListBOS->ArrayToVOPreUpdate(paRecord, paLongArr, paStringArr);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	{0}UCS	{1}UCS(p{2}ListBOS, &m_loginBO);", this.txtNomeClasse.Text, this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	{0}UCS.StartUseCase(false);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	UCBase * uc = {0}UCS.GetSaveUC();", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	{0}UCS.CreateChildUseCaseAsTopMost(uc, true,false);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	p{0}ListBOS->VOToArrayPostUpdate(paRecord, paLongArr, paStringArr);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("    END_BO_SERVER_METHOD(pbMsg, pbSuccess, paException);").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");


            builderCode.Append(String.Format("STDMETHODIMP CNvsMasterDataMain::Select{0}All(VARIANT vSi, ", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("  										     VARIANT* paRecord,").Append("\r\n"); 
            builderCode.Append("											 VARIANT* paLongArr,").Append("\r\n"); 
            builderCode.Append("											 VARIANT* paStringArr,").Append("\r\n"); 
            builderCode.Append("											 VARIANT* paException, ").Append("\r\n");
            builderCode.Append("											 BSTR* pbMsg, ").Append("\r\n");
            builderCode.Append("											 VARIANT_BOOL * pbSuccess)").Append("\r\n");
            builderCode.Append("{").Append("\r\n");
            builderCode.Append("    BEGIN_BO_SERVER_METHOD(vSi, _T(\"SelectInvoiceAll\"));").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	{0}ListBOS		*p{1}ListBOS = NULL;", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	BOListPtr  boPtr =  p{0}ListBOS = {1}ListBOS::CreateInstance(&m_loginBO);", this.txtNomeClasse.Text, this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	p{0}ListBOS->ArrayToVOPreSelect(paLongArr, paStringArr);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	p{0}ListBOS->SelectAll();", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("    p{0}ListBOS->VOToArrayPostSelect(paRecord, paLongArr, paStringArr);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("    END_BO_SERVER_METHOD(pbMsg, pbSuccess, paException);").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");

            builderCode.Append("******************************************************************************************************").Append("\r\n");
            builderCode.Append("******************************************************************************************************").Append("\r\n");
            builderCode.Append("*****************************************     P R O X Y     ******************************************").Append("\r\n");
            builderCode.Append("******************************************************************************************************").Append("\r\n");
            builderCode.Append("******************************************************************************************************").Append("\r\n");
            builderCode.Append("}").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("C:\\Exemplo\\ExemploPro\\Source\\Proxy\\Include\\MasterDataProxy.h").Append("\r\n");
            builderCode.Append(String.Format("#pragma region {0}", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	bool Select{0}ByID 		(CStringArray* paRecord, CLongArray* paLongArr, CStringArray* paStringArr, CStringArray* paException);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("    bool Search{0}			(CStringArray* paRecord, CLongArray* paLongArr, CStringArray* paStringArr, CStringArray* paException);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("    bool SaveData{0}		(CStringArray* paRecord, CLongArray* paLongArr, CStringArray* paStringArr, CStringArray* paException);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("    bool Select{0}All			(CStringArray* paRecord, CLongArray* paLongArr, CStringArray* paStringArr, CStringArray* paException);", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("#pragma endregion").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("// ***************************************************************************************").Append("\r\n");
            builderCode.Append("C:\\Exemplo\\ExemploPro\\Source\\Proxy\\MasterData\\MasterDataProxy.cpp").Append("\r\n");
            builderCode.Append(String.Format("#pragma region {0}", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append(String.Format("	IMPLEMENT_PROXY_Si4  (CMasterDataProxy, Select{0}ByID,OUT(CStringArray),", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("																  INOUT(CLongArray),").Append("\r\n");
            builderCode.Append("																  INOUT(CStringArray),").Append("\r\n");
            builderCode.Append("																  OUT(CStringArray));").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("	").Append("\r\n");
            builderCode.Append(String.Format("	IMPLEMENT_PROXY_Si4  (CMasterDataProxy, Search{0},	  OUT(CStringArray),", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("																  INOUT(CLongArray),").Append("\r\n");
            builderCode.Append("																  INOUT(CStringArray),").Append("\r\n");
            builderCode.Append("															  OUT(CStringArray));").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("	IMPLEMENT_PROXY_Si4  (CMasterDataProxy, SaveData{0},  OUT(CStringArray),", this.txtNomeClasse.Text)).Append("\r\n");
            builderCode.Append("																  INOUT(CLongArray),").Append("\r\n");
            builderCode.Append("																  INOUT(CStringArray),").Append("\r\n");
            builderCode.Append("																  OUT(CStringArray));").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append(String.Format("IMPLEMENT_PROXY_Si4  (CMasterDataProxy, Select{0}All, OUT(CStringArray),", this.txtNomeClasse.Text)).Append("\r\n");
			builderCode.Append("													  INOUT(CLongArray),").Append("\r\n");
			builderCode.Append("													  INOUT(CStringArray),").Append("\r\n");
			builderCode.Append("													  OUT(CStringArray));").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("").Append("\r\n");
            builderCode.Append("#pragma endregion").Append("\r\n");
	        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            file.WriteLine(builderCode.ToString());
            file.Close();
        }

        private bool validaCampoNomeClasse()
        {
            if (this.txtNomeClasse.Text == String.Empty)
            {
                MessageBox.Show("Nome da Classe está vazio", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                this.txtNomeClasse.Focus();
                this.txtNomeClasse.Select();
                return false;
            }
            return true;
        }

      
        private void LibrariesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //LibraryProperties.SelectedObject = e.Node.Tag;
            this.LibraryProperties.SelectedObject = e.Node.Tag;
            this.LibraryProperties.Refresh();
            treeNodeSelecionado = this.LibrariesTreeView.SelectedNode;
            treeNodeSelecionado.ForeColor = Color.White;
            treeNodeSelecionado.BackColor = Color.DarkBlue;
        }

        private void LibraryProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {

            PropertyGrid propriedades = (PropertyGrid)(PropertyGrid)s;
            ColumnInfo coluna = (ColumnInfo)propriedades.SelectedObject;
            if (treeNodeSelecionado != null)
            {
                int indice = this.columns.Columns.IndexOf(coluna);
                if (indice >= 0)
                {
                    this.columns.Columns[indice] = coluna;
                }
                if (coluna.Selecionar)
                {
                    treeNodeSelecionado.ForeColor = Color.Red;
                }
                this.LibrariesTreeView.Refresh();

            }
        }

        private void LibrariesTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void LibrariesTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeNodeSelecionado != null)
            {
                if (treeNodeSelecionado.Level > 0)
                {

                    treeNodeSelecionado.ForeColor = Color.Black;
                    treeNodeSelecionado.BackColor = Color.White;
                    ColumnInfo coluna = (ColumnInfo)treeNodeSelecionado.Tag;

                    if (coluna.Selecionar)
                    {
                        treeNodeSelecionado.ForeColor = Color.Red;

                    }
                }
            }
        }

        

        /// <summary>
        /// CLASSE RESPONSÁVEL PARA SERIALIZAR OS OBJETOS
        /// </summary>
        /// <param name="objeto">Uma variável do tipo Referência</param>
        /// <returns>Retorna uma string contendo o objeto serializado. (formato XML)</returns>
        public static string Serializar(object objeto)
        {
            StringWriter result = new StringWriter();
            XmlSerializer xmlTrace = new XmlSerializer(objeto.GetType());

            try
            {
                xmlTrace.Serialize(result, objeto);
            }
            catch (Exception ex)
            {
                xmlTrace.Serialize(result, ex);
            }
            finally
            {
                result.Close();
            }

            return result.ToString();
        }


        private void DeserializeObject(string filename, object objeto)
        {
            // Create an instance of the XmlSerializer specifying type and namespace.
            XmlSerializer serializer = new  XmlSerializer(objeto.GetType());
            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filename, FileMode.Open);
            try
            {
                XmlReader reader = new XmlTextReader(fs);

                // Declare an object variable of the type to be deserialized.
                ColumnInfoS i;

                // Use the Deserialize method to restore the object's state.
                i = (ColumnInfoS)serializer.Deserialize(reader);
                this.txtNomeClasse.Text = i.NomeClasse;
                this.columns = i;
                
                this.LoadLibraries();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                fs.Close();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = callLogin();
            if (dialogResult != DialogResult.OK)
            {
                this.txtStatusToopTip.Text = Conexao.Instance.Connection.State.ToString();
                return;
            }

            this.txtStatusToopTip.Text = Conexao.Instance.Connection.State.ToString();
            callSelectionTables();
            LoadLibraries();
            toolStripButtonSalvar.Enabled = true;
            this.txtNomeClasse.Enabled = true;
        }

        private void loadXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(@"C:\Profiles\gerador\xml\"))
            {
                Directory.CreateDirectory(@"C:\Profiles\gerador\xml\");
            }
            XmlDocument documento = new XmlDocument();

            string Pfad = string.Empty;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "xml files (*.xml)|*.xml";
            openFileDialog1.InitialDirectory = @"C:\Profiles\gerador\xml\";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Pfad = openFileDialog1.FileName;
                DeserializeObject(Pfad, this.columns);
                toolStripButtonSalvar.Enabled = true;
                this.txtNomeClasse.Enabled = true;
            }
        }

        private void saveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.validaCampoNomeClasse() == false)
                return;

            if (!Directory.Exists(@"C:\Profiles\gerador\xml\"))
            {
                Directory.CreateDirectory(@"C:\Profiles\gerador\xml\");
            }


            XmlDocument documento = new XmlDocument();
            this.columns.NomeClasse = this.txtNomeClasse.Text.ToString();
            this.columns.Banco = Conexao.Instance.Banco;
            this.columns.Usuario = Conexao.Instance.Usuario;
            this.columns.Senha = Conexao.Instance.Senha;
            string respostaXml = Serializar(this.columns);

            documento.LoadXml(respostaXml);
            string arquivoXML = String.Format(@"C:\Profiles\gerador\xml\{0}.xml", this.txtNomeClasse.Text);
            if (File.Exists(arquivoXML))
            {
                string nomeJaExite = String.Format("Já existe um arquivo {0}.xml.\nDeseja substituir?", this.txtNomeClasse.Text);

                if (MessageBox.Show(nomeJaExite, "Questão", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    documento.Save(arquivoXML);
                }

            }
            else
            {
                documento.Save(arquivoXML);
            }
            



        }

        private void gerarFontesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.validaCampoNomeClasse() == false)
                return;
            generateCodeRowBOH();
            generateCodeRowBoCPP();

            generateCodeListBOH();
            generateCodeListBoCPP();

            // server
            generateCodeListBOSH();
            generateCodeListBOSCPP();

            generateCodeUCSH();
            generateCodeUCSCPP();

            genreateTxtToDo();
            MessageBox.Show("Código gerado!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButtonGerarTXT_Click(object sender, EventArgs e)
        {
            this.genreateTxtConferencia();
            MessageBox.Show("Arquivo txt gerado!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }


    }
}

