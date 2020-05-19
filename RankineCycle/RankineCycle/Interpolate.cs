using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

namespace RankineCycle
{
    class Interpolate
    {
        /*public static DataTable InterpolateRow(DataTable localDataTable, int rowIndex, double valueSearch, TextBox [] textDisplay)
        {//(Tabla donde interpolar, Fila dodne está el valor mayor, valor a interpolar, arreglo de los textos en el form)
            DataTable TempDataTable = new DataTable();
            double x1 = double.Parse(localDataTable.Rows[rowIndex - 1][0].ToString());
            double x3 = double.Parse(localDataTable.Rows[rowIndex][0].ToString());

            double y1, y3;

            double[] interpolation = new double[localDataTable.Columns.Count];
            interpolation[0] = valueSearch;
            for (int i = 1; i < localDataTable.Columns.Count; i++)
            {
                y1 = double.Parse(localDataTable.Rows[rowIndex - 1][i].ToString());
                y3 = double.Parse(localDataTable.Rows[rowIndex][i].ToString());

                interpolation[i] = ((valueSearch - x1) * (y3 - y1) / (x3 - x1)) + y1;
            }

            for (int i = 0; i < localDataTable.Columns.Count; i++)
            {
                TempDataTable.Columns.Add();
            }

            DataRow row = TempDataTable.NewRow();
            for (int i = 0; i < localDataTable.Columns.Count; i++)
            {
                row[i] = interpolation[i];
            }
            TempDataTable.Rows.Add(row);
            return TempDataTable;
        }

        public static DataTable DoubleInterpolate(DataTable localDataTable, int rowIndex, double valueSearch)
        {
            DataTable TempDataTable = new DataTable();
            localDataTable.Columns["P"].SetOrdinal(0);
            localDataTable.Columns["T"].SetOrdinal(1);
            int lugaresRetrocedidos;
            double x1 = 0;
            double x3 = double.Parse(localDataTable.Rows[rowIndex][0].ToString());
            for (lugaresRetrocedidos = 1; rowIndex - lugaresRetrocedidos > 0; lugaresRetrocedidos++)
            {
                if (double.Parse(localDataTable.Rows[rowIndex][1].ToString()) == double.Parse(localDataTable.Rows[rowIndex - lugaresRetrocedidos][1].ToString()))
                {
                    x1 = double.Parse(localDataTable.Rows[rowIndex - lugaresRetrocedidos][0].ToString());
                    break;
                }
            }
            double y1, y3;

            double[] interpolation = new double[localDataTable.Columns.Count];
            interpolation[0] = valueSearch;
            for (int i = 1; i < localDataTable.Columns.Count; i++)
            {
                y1 = double.Parse(localDataTable.Rows[rowIndex - lugaresRetrocedidos][i].ToString());
                y3 = double.Parse(localDataTable.Rows[rowIndex][i].ToString());

                interpolation[i] = ((valueSearch - x1) * (y3 - y1) / (x3 - x1)) + y1;
            }
            localDataTable = Array2DataTable(interpolation, localDataTable.Columns.Count);
            return localDataTable;
        }

        /*public static DataTable SuperInterpolate(DataTable localDataTable, int tempRowIndex, int PresRowIndex, double tempValue, double PresValue)
        {
            int tempRowIndex2 = 0;
            int lugaresRetrocedidos;
            for (lugaresRetrocedidos = 1; tempRowIndex - lugaresRetrocedidos > 0; lugaresRetrocedidos++)
            {
                if (double.Parse(localDataTable.Rows[tempRowIndex][0].ToString()) == double.Parse(localDataTable.Rows[tempRowIndex - lugaresRetrocedidos][0].ToString()))
                {
                    tempRowIndex2 = tempRowIndex - lugaresRetrocedidos;
                    break;
                }
            }
            /*DataTable TempDataTableUp = Interpolate.InterpolateRow(localDataTable, tempRowIndex2, tempValue);
            DataTable TempDataTableDown = Interpolate.InterpolateRow(localDataTable, tempRowIndex, tempValue);

            DataTable TotalDataTable = localDataTable.Clone();
            TotalDataTable.Rows.Add(TempDataTableUp.Rows[0].ItemArray);
            TotalDataTable.Rows.Add(TempDataTableDown.Rows[0].ItemArray);

            TotalDataTable.Columns["P"].SetOrdinal(0);
            TotalDataTable.Columns["T"].SetOrdinal(1);

            DataTable FinalDataTable = InterpolateRow(TotalDataTable, 1, PresValue);

            return FinalDataTable;
        }

        public static DataTable Array2DataTable(double[] array, int size)
        {
            DataTable TempDataTable = new DataTable();
            for (int i = 0; i < size; i++)
            {
                TempDataTable.Columns.Add();
            }

            DataRow row = TempDataTable.NewRow();
            for (int i = 0; i < size; i++)
            {
                row[i] = array[i];
            }
            TempDataTable.Rows.Add(row);
            return TempDataTable;
        }*/
    }
}
