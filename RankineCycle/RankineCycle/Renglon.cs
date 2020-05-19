using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;


namespace RankineCycle
{
    class Renglon
    {
        public TextBox [] Data;
        
        double[] limiteMaximo = new double[6];
        double[] limiteMinimo = new double[6];
        public int index;  //Guarda los indices de los dos valores que se recibieron
        public double[] vals;

        DataTable interpolatedVals;

        DataTable TempDataTable;

        public bool jumpSat = false;
        public bool isCalculated;


        public Renglon(TextBox presion ,TextBox temperatura, TextBox volumen, TextBox energiaInterna, TextBox entalpia, TextBox entropia)
        {
            Data = new TextBox[6];
            Data[0] = presion;
            Data[1] = temperatura;
            Data[2] = volumen;
            Data[3] = energiaInterna;
            Data[4] = entalpia;
            Data[5] = entropia;

            vals = new double[2];
            TempDataTable = new DataTable();
            interpolatedVals = new DataTable();
            isCalculated = false;
        }

        /// <summary>
        /// Métodos
        /// </summary>
        /// <returns></returns>
        /// Cheque de valores
        public bool CheckValues()
        {
            bool canCalculate = true;
            index = 10;                           //Valor por defecto 
            for(int i = 1; i < Data.Length; i++)  //Recorre el arreglo
            {
                if (Data[i].Text != "")
                {
                    //Checa si el texto es nulo
                    index = i;
                    break;
                }
            }
            //MessageBox.Show("Indexes found : [" + indexes[0] + " - " + indexes[1] + "]");


            if (index != 10)
            {//En caso de haber puesto los dos valores 
                if (!double.TryParse(Data[0].Text, out vals[0]) || !double.TryParse(Data[index].Text, out vals[1]))
                {//trata de parsear lo que le pongan
                    MessageBox.Show("Invalid values");
                    canCalculate = false;
                }
                else
                {
                    //CHECAR SI ESTAN DENTRO DEL RANGO
                    if (CheckLimiteMaximo2Vals() || CheckLimiteMinimo2Vals())
                    {
                        MessageBox.Show("Out of range");
                        vals[0] = 0;
                        vals[1] = -1;
                        canCalculate = false;
                    }
                }
            }
            else
            { //Si recibe solo un valor
                //MessageBox.Show("Se recibió un solo valor");
                if(!double.TryParse(Data[0].Text, out vals[0]))
                {//Trata de parsear el valor
                    MessageBox.Show("Invalid value");
                    canCalculate = false;
                }
                else
                {
                    if (CheckLimiteMaximo() || CheckLimiteMinimo())
                    {//Cheacar si esta dentro del rango
                        MessageBox.Show("Out of range");
                        vals[0] = 0;
                        vals[1] = -1;
                        canCalculate = false;

                    }
                }
            }
            return canCalculate;
        }

        public static bool CheckSingleValue(TextBox textBox)
        {
            bool isValid = false;
            double x;
            if (textBox.Text != "")
            {
                if (double.TryParse(textBox.Text, out x))
                {

                    if (x <= 0|| 100 <= x )
                    {
                        MessageBox.Show("Out of range");
                        isValid = false;
                    }
                    else
                        isValid = true;
                }
                else
                {
                    MessageBox.Show("Invalid Value");
                    isValid = false;
                }
            }
            if(!isValid)
                MessageBox.Show("Check your values");
            return isValid;
        }

        public bool GetThirdValue()
        {//Utilizada para obtener un tercer valor. 
            bool isValid = false;
            for(int i = 2; i < Data.Length; i++)
            {
                if(Data[i].Text != "")
                {
                    if (double.TryParse(Data[i].Text, out vals[1]))
                    {
                        
                        index = i;
                        if (CheckLimiteMinimo2Vals() || CheckLimiteMinimo2Vals())
                        {
                            MessageBox.Show("Out of range");
                            isValid = false;
                            break;
                        }
                        else
                        {
                            isValid = true;
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Value");
                        isValid = false;
                    }
                }
            }
            return isValid;
        }

        public void SearchIn7(DataTable tabla)//Busca los valores que tienen index y vla en la tabla 7
        {
            //try
            //{
                //MessageBox.Show("will search");
                if(vals[0] <= 5000)
                {//Si es igual o menor a 5000
                    for (int i = 1; i < 15; i++)
                    {
                        if (double.Parse(tabla.Rows[i][index].ToString()) == vals[1])
                        {//Checa si el segundo valor de la persona es válido
                            //MessageBox.Show("Se encontró");
                            displayTable7(i, tabla);
                            break;
                        }
                        if (double.Parse(tabla.Rows[i][index].ToString()) >= vals[1])
                        {//checa si hay que intetepolar
                            interpolatedVals = new DataTable();
                            interpolatedVals.Clear();
                            //MessageBox.Show("Necesita interpolar");
                            interpolatedVals =  InterpolateIn7(tabla, i, vals[1], index);
                            displayTable7(0, interpolatedVals, index);
                            break;
                        }
                    }
                }
                else
                {//Si es mayor a 5000
                for (int i = 0; i < tabla.Rows.Count; i++)
                    {//REcorre toda la base
                        if (vals[0] == double.Parse(tabla.Rows[i][0].ToString()))
                        {//Checa si está la presión 
                            //MessageBox.Show("Esta la presión");
                        }
                        else if (vals[0] <= double.Parse(tabla.Rows[i][0].ToString()))
                        {//Checa si hay que interpolar la presión
                            //MessageBox.Show("Hay que interpolar la presión");

                            if (double.Parse(tabla.Rows[i][index].ToString()) == vals[1])
                            {//Checa si el segundo valor de la persona es válido
                                //MessageBox.Show("Se encontró");
                                interpolatedVals.Clear();
                                interpolatedVals = IterpolatePress(tabla,i,vals[0],index);
                                displayTable7(0, interpolatedVals);
                                break;
                            }
                            else if (double.Parse(tabla.Rows[i][index].ToString()) >= vals[1])
                            {//checa si hay que intetepolar
                                if (!jumpSat)
                                {
                                    jumpSat = true;
                                    continue;
                                }
                                //MessageBox.Show("Interpolar temperatura");
                                interpolatedVals = new DataTable();
                                interpolatedVals.Clear();
                                interpolatedVals = DoubleInterpolate(tabla, i, vals[1], index);
                                jumpSat = true;
                                displayTable7(0, interpolatedVals, index);
                                break;
                            }
                        }   

                    }
                }
           // }
            //catch{
                //MessageBox.Show("unable to search");
        //    }
        }

        public void SearchIn6(DataTable tabla)//Busca los valores que tienen index y vla en la tabla 7
        {

            for (int i = 0; i < tabla.Rows.Count; i++)
            {//REcorre toda la base
                if (vals[0] == double.Parse(tabla.Rows[i][0].ToString()))
                {//Checa si está la presión 
                    //MessageBox.Show("Esta la presión");
                    if (double.Parse(tabla.Rows[i][index].ToString()) == vals[1])
                    {//Checa si el segundo valor de la persona es válido
                        //MessageBox.Show("Se encontró");
                        displayTable7(i, tabla);
                        break;
                    }
                    if (double.Parse(tabla.Rows[i][index].ToString()) >= vals[1])
                    {//checa si hay que intetepolar
                        interpolatedVals = new DataTable();
                        interpolatedVals.Clear();
                        //MessageBox.Show("Necesita interpolar");
                        interpolatedVals = InterpolateIn7(tabla, i, vals[1], index);
                        displayTable7(0, interpolatedVals, index);
                        break;
                    }
                }
                else if (vals[0] <= double.Parse(tabla.Rows[i][0].ToString()))
                {//Checa si hay que interpolar la presión
                    //MessageBox.Show("Hay que interpolar la presión");

                    if (double.Parse(tabla.Rows[i][index].ToString()) == vals[1])
                    {//Checa si el segundo valor de la persona es válido
                        //MessageBox.Show("Se encontró");
                        interpolatedVals.Clear();
                        interpolatedVals = IterpolatePress(tabla, i, vals[0], index);
                        displayTable7(0, interpolatedVals);
                        break;
                    }
                    else if (double.Parse(tabla.Rows[i][index].ToString()) >= vals[1])
                    {//checa si hay que intetepolar
                        if (!jumpSat)
                        {
                            jumpSat = true;
                            continue;
                        }
                        //MessageBox.Show("Interpolar temperatura");
                        interpolatedVals = new DataTable();
                        interpolatedVals.Clear();
                        interpolatedVals = DoubleInterpolate(tabla, i, vals[1], index);
                        jumpSat = true;
                        displayTable7(0, interpolatedVals, index);
                        break;
                    }
                }  
            }
        }


        public void displayTable7(int row, DataTable tabla7)
        {
            int col = 0;
            foreach (TextBox display in Data)
            {
                if(col > 0)
                display.Text = tabla7.Rows[row][col].ToString();
                col++;
            }
        }
        public void displayTable7(int row, DataTable tabla, int colToJump)
        {//Sobrecarga cuando se interpoló
            for(int i  = 1; i < Data.Length; i++)
            {
                if (i == colToJump)
                    continue;
                Data[i].Text = tabla.Rows[row][i].ToString();
            }
        }

        /// <summary>
        /// Set de límites
        /// </summary>
        /// <param name="presion"></param>
        /// <param name="temperatura"></param>
        /// <param name="volumen"></param>
        /// <param name="energiaInterna"></param>
        /// <param name="entalpia"></param>
        /// <param name="entropia"></param>
        public void SetLimiteMaximo(double presion, double temperatura, double volumen, double energiaInterna, double entalpia, double entropia)
        {//Función para poner el límite maximo del renglon
            limiteMaximo[0] = presion;
            limiteMaximo[1] = temperatura;
            limiteMaximo[2] = volumen;
            limiteMaximo[3] = energiaInterna;
            limiteMaximo[4] = entalpia;
            limiteMaximo[5] = entropia;
        }
        public void SetLimiteMinimo(double presion, double temperatura, double volumen, double energiaInterna, double entalpia, double entropia)
        {//Función para poner el límite maximo del renglon
            limiteMinimo[0] = presion;
            limiteMinimo[1] = temperatura;
            limiteMinimo[2] = volumen;
            limiteMinimo[3] = energiaInterna;
            limiteMinimo[4] = entalpia;
            limiteMinimo[5] = entropia;
        }

        /// <summary>
        /// Chequeo de limites 
        /// </summary>
        /// <returns></returns>
        public bool CheckLimiteMaximo2Vals()
        {
            if ((vals[0] > limiteMaximo[0]) || (vals[1] > limiteMaximo[index]))
                return true;
            else
                return false;
        }

        public bool CheckLimiteMinimo2Vals()
        {
            if ((vals[0] < limiteMinimo[0]) || (vals[1] < limiteMinimo[index]))
                return true;
            else
                return false;
        }

        public bool CheckLimiteMaximo()
        {
            if ((vals[0] > limiteMaximo[0]))
                return true;
            else
                return false;
        }
        public bool CheckLimiteMinimo()
        {
            if ((vals[0] < limiteMinimo[0]))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Interpolaciones 
        /// </summary>
        /// <param name="localDataTable"></param>
        /// <param name="rowIndex"></param>
        /// <param name="valueSearch"></param>
        /// <returns></returns>
        public  DataTable InterpolateRow(DataTable localDataTable, int rowIndex, double valueSearch)
        {//Tabla donde interpolar, Fila dodne está el valor mayor, valor a interpolar, arreglo de los textos en el form)
            DataTable returnTable = new DataTable();
            returnTable.Clear();

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
                returnTable.Columns.Add();
            }

            DataRow row = returnTable.NewRow();
            for (int i = 0; i < localDataTable.Columns.Count; i++)
            {
                row[i] = interpolation[i];
            }
            returnTable.Rows.Add(row);
            return returnTable;
        }

        public DataTable InterpolateIn7(DataTable localDataTable, int rowIndex, double valueSearch, int colToJump)
        {//Tabla donde interpolar, Fila dodne está el valor mayor, valor a interpolar, arreglo de los textos en el form)
            DataTable returnDataTable = new DataTable();
            returnDataTable.Clear();

            double x1 = double.Parse(localDataTable.Rows[rowIndex - 1][colToJump].ToString());
            double x3 = double.Parse(localDataTable.Rows[rowIndex][colToJump].ToString());

            double y1, y3;

            double[] interpolation = new double[localDataTable.Columns.Count];
            interpolation[colToJump] = valueSearch;
            for (int i = 0; i < localDataTable.Columns.Count; i++)
            {
                if (i == colToJump)
                    continue;
                y1 = double.Parse(localDataTable.Rows[rowIndex - 1][i].ToString());
                y3 = double.Parse(localDataTable.Rows[rowIndex][i].ToString());

                interpolation[i] = ((valueSearch - x1) * (y3 - y1) / (x3 - x1)) + y1;
            }//Fin de la interpolación

            //Se agregan los daros a la data table
            for (int i = 0; i < localDataTable.Columns.Count; i++)
            {
                returnDataTable.Columns.Add();
            }

            DataRow row = returnDataTable.NewRow();
            for (int i = 0; i < localDataTable.Columns.Count; i++)
            {
                row[i] = interpolation[i];
            }
            returnDataTable.Rows.Add(row);
            return returnDataTable;
        }

        public DataTable IterpolatePress(DataTable TableToInterpolate, int rowIndex, double valueSearch, int colToJump)
        {
            TempDataTable.Clear();
            double x1 = 0;
            double x3 = double.Parse(TableToInterpolate.Rows[rowIndex][0].ToString());
            int lugaresRetrocedidos;
            for (lugaresRetrocedidos = 1; rowIndex - lugaresRetrocedidos > 0; lugaresRetrocedidos++)
            {
                if (double.Parse(TableToInterpolate.Rows[rowIndex][index].ToString()) == double.Parse(TableToInterpolate.Rows[rowIndex - lugaresRetrocedidos][1].ToString()))
                {
                    x1 = double.Parse(TableToInterpolate.Rows[rowIndex - lugaresRetrocedidos][0].ToString());
                    break;
                }
            }
            //Se obtienen los valores de ambas presiones

            double y1, y3;

            double[] interpolation = new double[TableToInterpolate.Columns.Count];
            //Areglo que aloja las presiones 
            interpolation[0] = valueSearch;
            for (int i = 1; i < TableToInterpolate.Columns.Count; i++)
            {
                y1 = double.Parse(TableToInterpolate.Rows[rowIndex-lugaresRetrocedidos][i].ToString());
                y3 = double.Parse(TableToInterpolate.Rows[rowIndex][i].ToString());

                interpolation[i] = ((valueSearch - x1) * (y3 - y1) / (x3 - x1)) + y1;
            }//Acaba la interpolación

            //Se pasan los datos a la data table
            for (int i = 0; i < TableToInterpolate.Columns.Count; i++)
            {
                TempDataTable.Columns.Add();
            }

            DataRow row = TempDataTable.NewRow();
            for (int i = 0; i < TableToInterpolate.Columns.Count; i++)
            {
                row[i] = interpolation[i];
            }
            TempDataTable.Rows.Add(row);

            return TempDataTable;
        }

        public DataTable DoubleInterpolate(DataTable TableToInterpolate, int rowIndex, double valueSearch, int colToJump)
        {
            DataTable returnDataTable = new DataTable();
            returnDataTable.Clear();
            int stepsBefore = 1;
            int rowIndex2 = 0;
            for (int i = rowIndex - stepsBefore; i > 0; stepsBefore++)
            {
                if (valueSearch > double.Parse(TableToInterpolate.Rows[rowIndex - stepsBefore][index].ToString()) && double.Parse(TableToInterpolate.Rows[rowIndex - stepsBefore][0].ToString()) != double.Parse(TableToInterpolate.Rows[rowIndex][0].ToString()))
                {

                    //MessageBox.Show("fila 1: " + rowIndex + "fila 2: " + (rowIndex - stepsBefore));
                    rowIndex2 = rowIndex - stepsBefore + 1;
                    break;
                }

            }

            DataTable RowDown = InterpolateIn7(TableToInterpolate, rowIndex, vals[1], index);
            DataTable RowUP = InterpolateIn7(TableToInterpolate, rowIndex2, vals[1], index);

            returnDataTable = TableToInterpolate.Clone();

            returnDataTable.Rows.Add(RowUP.Rows[0].ItemArray);
            returnDataTable.Rows.Add(RowDown.Rows[0].ItemArray);

            returnDataTable = InterpolateIn7(returnDataTable, 1, vals[0], 0);

            return returnDataTable;
        }

        /// <summary>
        /// Chequeo si es líquido
        /// </summary>
        /// <param name="tabla5"></param>
        /// <returns></returns>
        public bool IsLiquid(DataTable tabla5)
        {//Regresa si es líquida o no el agua que se metió
             try
             { 
                DataTable realValue = new DataTable();
                for (int i = 0; i < tabla5.Rows.Count; i++)
                {//REcorrre toda la tabla
                    if (double.Parse(tabla5.Rows[i][0].ToString()) == vals[0])
                    {//Checa si esta la presion en la tabla 5
                        if (double.Parse(tabla5.Rows[i][index].ToString()) >= vals[1])//Si el valor extra es menor o igla la de la tabla
                            return true;
                        else
                            break;
                    }
                    if (double.Parse(tabla5.Rows[i][0].ToString()) >= vals[0])
                    {//Checa si debe interpolar
                        realValue.Clear();
                        realValue = InterpolateRow(tabla5, i, vals[0]);
                        if (double.Parse(realValue.Rows[0][index].ToString()) >= vals[1])
                            return true;
                        else
                            break;
                    }
                }
             }
             catch { MessageBox.Show("Unable to know if it is water"); }
            return false;
        }

        /// <summary>
        /// Ver si es vapor
        /// </summary>
        /// <param name="nature"></param>

        public void ChangeEnable(bool nature)
        {
            foreach (TextBox i in Data)
            {
                i.Enabled = nature;
            }
            isCalculated = !nature;
        }

        /// <summary>
        /// Get punto de saturacion de vapor 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public double[] getVaporSaturado(DataTable tablaVapor)
        {
            double [] result = new double[6];
            DataTable interpolatedVal = new DataTable();
            for (int i = 0; i < tablaVapor.Rows.Count; i++)
            {//Recorrre toda la tabla
                if (double.Parse(tablaVapor.Rows[i][0].ToString()) == vals[0])
                {//Checa si esta la presion en la tabla 5
                    for (int col = 0; col < tablaVapor.Columns.Count; col++)
                    {
                        result[col] = double.Parse(tablaVapor.Rows[i][col].ToString());
                    }
                    break;
                }
                if (double.Parse(tablaVapor.Rows[i][0].ToString()) >= vals[0])
                {//Checa si debe interpolar
                    interpolatedVal.Clear();
                    interpolatedVal = InterpolateRow(tablaVapor, i, vals[0]);
                    for (int col = 0; col < interpolatedVal.Columns.Count; col++)
                    {
                        result[col] = double.Parse(interpolatedVal.Rows[0][col].ToString());
                    }
                    break;
                }
            }
            return result;
        }

        ///Get X
        ///
        public double GetX()
        {
            double x;
            double T = vals[1];
            double Tf = limiteMinimo[index];
            double deltaT = limiteMaximo[index] - limiteMinimo[index];
            x = (T - Tf) / deltaT;
            return x;
        }

        public void CalculateEachWithX(double X)
        {
            for (int i = 2; i < Data.Length; i++)
            {
                double Tf = limiteMinimo[i];
                double deltaT = limiteMaximo[i] - Tf;
                double T = Tf + (X/100) * (deltaT);
                Data[i].Text = T.ToString();
            }
        }

        public void clearItSelf()
        {
            foreach (TextBox item in Data)
            {
                item.Text = "";
                item.Enabled = true;
            }
            vals = new double[2];
            TempDataTable = new DataTable();
            interpolatedVals = new DataTable();
            isCalculated = false;
        }

        public void AutoFillWith(double [] vals)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i].Text = vals[i].ToString();
            }
        }
    }
}
