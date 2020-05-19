using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RankineCycle
{
    class DataBaseConect
    {
        static OleDbConnection con;
        static OleDbCommand cmd;
        static OleDbDataReader rd;

        public static DataTable conect(string path, int table)//Función que te conecta con la DataBase (Direccion de la database, Tabla que se abrirá)
        {
            DataTable currentData;//Aloja los datos que se manejan en ese momento.
            string tableString = "";
            switch (table)
            {
                case 3:
                    tableString = "P, T, vg, ug, hg, sg from Tabla5";
                    break;
                case 4:
                    tableString = "P, T, vf, uf, hf, sf from Tabla5";
                    break;
                case 5:
                    tableString = "P, T, vf, vg, uf, ufg, ug, hf, hfg, hg, sf, sfg, sg from Tabla5";
                    break;
                case 6:
                    tableString = "P, T, v, u, h, s from Tabla6";
                    break;
                case 7:
                    tableString = "P, T, v, u, h, s from Tabla7";
                    break;
                default:
                    break;
            }                        //Provider=Microsoft.Jet.OLEDB.4.0;Data Source="E:\Documentos\DocsUni\4to Semestre\Termo\RankineCycle\RankineCycle\bin\Debug\termo.mdb"
            con = new OleDbConnection(@"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + @path);
            con.Open();
            cmd = new OleDbCommand("select " + tableString , con);
            rd = cmd.ExecuteReader();
            currentData = new DataTable();
            currentData.Load(rd);
            con.Close();
            return currentData;
        }
    }
}