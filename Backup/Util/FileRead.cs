using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Windows.Forms; 


namespace Gerador.Util
{
   

    class FileRead
    {
        public const String CODIGO_FUL_REGEX = @"^([\d]+\w|\.)*";
        public const String COIDGO_REQUISITOS = @" ?[A-Za-z]{2}[0-9]{3}[._]?[0-9]?";
        public const String DESCRICAO_UC_REGEX = @"  ?([A-Za-z]+[0-9]+)*[:.;_^]?[A-Za-z\W]+";

        public const String CODIGO_REQUISITOS_REGEX = @" ?[[]((?'Interface'[A-Za-z]+[0-9]+)*[.;_]?(?'Sequencia'[0-9]+)?)*[]]]*"; // interface é a marcação do grupo

        string filereadbuf; // buffer to store the content of file 
        public void ReadFile(string FileName, int FileSize)
        {
            char[] buf = new char[FileSize]; // lets define an array of type char field (i.e. variable) buf 
            // for more help please see .net sdk 
            StreamReader sr = new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read));
            int retval = sr.ReadBlock(buf, 0, FileSize); // no. of bytes read 
            Console.Write("Total Bytes Read = " + retval + "\n");
            filereadbuf = new string(buf); // store it in our field 
            Console.WriteLine(filereadbuf); // lets print on screen 
            sr.Close();
        }

        public static IDictionary<String, String> ReadFile(string filePath)
        {

           IDictionary<String, String> dicCampos = new Dictionary<String, String>();
            

            using (StreamReader r = new StreamReader(filePath))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    if (line.Contains("#define") && line.Contains("TABLE_NAME"))
                    {

                        String  codigoEX =@"[[\w]+";
                        Regex exp = new Regex(codigoEX, RegexOptions.IgnoreCase);

                        MatchCollection m = exp.Matches(line);


                        int contaCampos = 0;
                        String campoKey = String.Empty;
                        String campoValue = String.Empty;
                        foreach (Match match in m)
                        {

                            switch (contaCampos)
                            {
                                case 1:
                                    campoKey = match.Value;
                                    break;
                                case 3:
                                    campoValue = match.Value;
                                    break;
                            }
                            contaCampos++;
                            
                        }
                        if (campoKey != String.Empty)
                        {
                            dicCampos.Add(campoValue, campoKey);
                        }
                    }
                }
            }
            return dicCampos;
        }

      

            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evento"></param>
        /// <returns></returns>
        public static StringCollection pegaCodigoRequisitos(String evento)
        {
            StringCollection builder = new StringCollection();
            string codigo = String.Empty;
            codigo = "";
            //Regex exp = new Regex(@" ?[[]((?'Interface'[A-Za-z]+[0-9]+)*[.;]?)*[]]]*", RegexOptions.IgnoreCase);
            Regex exp = new Regex(FileRead.CODIGO_REQUISITOS_REGEX, RegexOptions.IgnoreCase);
          
            MatchCollection m = exp.Matches(evento);
            try
            {
                foreach (Match match in m)
                {
                    GroupCollection groups = match.Groups;
                    CaptureCollection capturesInterface = groups["Interface"].Captures;
                    CaptureCollection capturesSequencia = groups["Sequencia"].Captures;
                    for (int j = 0; j < capturesInterface.Count; j++)
                    {
                        builder.Add(capturesInterface[j].Value);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas na função pegaCodigoRequisitos " + ex.Message,"Atenção",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

            return builder;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evento"></param>
        /// <returns></returns>
        public static string pegaDescricaoUC(String evento)
        {
            
            string descricao = String.Empty;
            descricao = "";
            Regex exp = new Regex(FileRead.DESCRICAO_UC_REGEX, RegexOptions.IgnoreCase);
            
            try
            {
                Match m = exp.Match(evento);
                if (m.Success)
                {
                    descricao = m.Value;
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas na função pegaDescricaoUC " + ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return descricao;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evento"></param>
        /// <returns></returns>
        public static string pegaCodigoFull(string evento)
        {
            string codigo = String.Empty;
            Regex exp = new Regex(FileRead.CODIGO_FUL_REGEX, RegexOptions.IgnoreCase);
            Match m = exp.Match(evento);
            try
            {
                if (m.Groups[0].Value != String.Empty)
                {
                    codigo = m.Groups[0].Value;
                    if (codigo[codigo.Length - 1] == '.')
                        codigo = codigo.Remove(codigo.Length - 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas na função pegaCodigoFull " + ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return codigo;
        }

        


    }
}
