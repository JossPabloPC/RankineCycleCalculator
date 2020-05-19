using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RankineCycle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        OpenFileDialog ofd = new OpenFileDialog();              //Manager de archivos. Con el Buscaremos la base de datos.
        string filePath = "";

        DataTable tabla3 = new DataTable();
        DataTable tabla7 = new DataTable();
        DataTable tabla6 = new DataTable();
        DataTable tabla4 = new DataTable();                     //Tablas guardan toda la info de las talbas de vapor

        Renglon paso1;
        Renglon paso2;
        Renglon paso3;
        Renglon paso4;
        Renglon resultados;

        double[] temp; //Para guardar temporalmente los límites de los pasos 

        private void SearchDatabseBtn_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)            //Sólo obtendra el pad de la base de datos si se hizo click en OK
            {
                filePath = ofd.FileName;                     //Guarda el Path de la base de datos
            }
            try //Lo hace dentro de un catch por si falla la conección
            {
                tabla7 = DataBaseConect.conect(filePath, 7);  //Se conecta a la Data Base con la base adecuada y asigna los datos a las tablas locales
                tabla6 = DataBaseConect.conect(filePath, 6);  //Se conecta a la Data Base con la base adecuada y asigna los datos a las tablas locales
                tabla4 = DataBaseConect.conect(filePath, 4);  //Se conecta a la Data Base con la base adecuada y asigna los datos a las tablas locales
                tabla3 = DataBaseConect.conect(filePath, 3);  //Se conecta a la Data Base con la base adecuada y asigna los datos a las tablas locales

                ConnectedTxt.Text = "Connected";
            }
            catch { }
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            paso1 = new Renglon(Presion1Txt, Temperatura1Txt, Volumen1Txt, EnergiaInterna1Txt, Entalpia1Txt, Entropia1Txt);
            paso2 = new Renglon(Presion2Txt, Temperatura2Txt, Volumen2Txt, EnergiaInterna2Txt, Entalpia2Txt, Entropia2Txt);

            paso3 = new Renglon(Presion3Txt, Temperatura3Txt, Volumen3Txt, EnergiaInterna3Txt, Entalpia3Txt, Entropia3Txt);
            paso4 = new Renglon(Presion4Txt, Temperatura4Txt, Volumen4Txt, EnergiaInterna4Txt, Entalpia4Txt, Entropia4Txt);
            set14Limits();
            set23Limits();

            resultados = new Renglon(WinTxt, QinTxt, WoutTxt, QoutTxt, WnetoTxt, EfficiencyTxt);

        }

        private void Paso12Btn_Click(object sender, EventArgs e)
        {
            if (Presion1Txt.Text != "")
            {//Chea si hay presión
                if (paso1.CheckValues() && paso1.index != 10)
                {//Si los valores son válidos y son dos
                    if (paso1.IsLiquid(tabla4))
                    {//Checa si es líquida el agua 
                        //MessageBox.Show("Es líquida");
                        paso1.SearchIn7(tabla7);//Busca el valor indicado
                        paso2.SetLimiteMinimo(double.Parse(Presion1Txt.Text), double.Parse(Temperatura1Txt.Text), double.Parse(Volumen1Txt.Text), double.Parse(EnergiaInterna1Txt.Text), double.Parse(Entalpia1Txt.Text), double.Parse(Entropia1Txt.Text));
                        //Cambia el límite mínimo del renglon 2

                        //Sets del renglon 4
                        paso4.Data[0].Text = paso1.Data[0].Text;    //Establece la presión en el renglon 4
                        paso4.Data[1].Text = paso1.Data[1].Text;    //Establece la temperatura en el renglon 4 
                        paso4.vals[0] = double.Parse(paso4.Data[0].Text);
                        paso4.vals[1] = double.Parse(paso4.Data[0].Text);
                        double[] vaporSaturado = paso4.getVaporSaturado(tabla3);
                        double[] aguaSaturada = paso4.getVaporSaturado(tabla4);
                        paso4.SetLimiteMaximo(vaporSaturado[0] + 1, vaporSaturado[1] + 1, vaporSaturado[2], vaporSaturado[3], vaporSaturado[4], vaporSaturado[5]);
                        paso4.SetLimiteMinimo(aguaSaturada[0] - 1, aguaSaturada[1] - 1, aguaSaturada[2], aguaSaturada[3], aguaSaturada[4], aguaSaturada[5]);

                        paso1.ChangeEnable(false);                  //Evita las modificaciones en el renglon 1
                    }
                    else
                        MessageBox.Show("Not liquid water");
                }
                if(paso1.CheckValues() && paso1.index == 10)
                {//Solo un valor 
                    //MessageBox.Show("Se recibió un solo valor");
                    if (paso2.isCalculated)
                    {//Si ya se calculó el renglon pareja.
                        //Calculo de entalpía
                        double p2 = (double.Parse(paso1.Data[0].Text));
                        double p1 = (double.Parse(paso2.Data[0].Text));
                        double vol = double.Parse(paso2.Data[2].Text);
                        double deltaH = (p1 - p2) * vol;
                        double H = (double.Parse(paso2.Data[4].Text)) - deltaH;
                        paso1.Data[4].Text = H.ToString();
                        paso1.index = 4;
                        paso1.vals[1] = H;
                        //MessageBox.Show(H.ToString());
                        if (paso1.IsLiquid(tabla4))
                        {//Checa si es líquida el agua 
                         //MessageBox.Show("Es líquida");
                            paso1.SearchIn7(tabla7);//Busca el valor indicado
                            paso2.SetLimiteMinimo(double.Parse(Presion1Txt.Text), double.Parse(Temperatura1Txt.Text), double.Parse(Volumen1Txt.Text), double.Parse(EnergiaInterna1Txt.Text), double.Parse(Entalpia1Txt.Text), double.Parse(Entropia1Txt.Text));
                            //Cambia el límite mínimo del renglon 2
                            
                            //Sets del renglon 4
                            paso4.Data[0].Text = paso1.Data[0].Text;    //Establece la presión en el renglon 4
                            paso4.Data[1].Text = paso1.Data[1].Text;    //Establece la temperatura en el renglon 4 
                            paso4.vals[0] = double.Parse(paso4.Data[0].Text);
                            paso4.vals[1] = double.Parse(paso4.Data[0].Text);
                            double[] vaporSaturado = paso4.getVaporSaturado(tabla3);
                            double[] aguaSaturada = paso4.getVaporSaturado(tabla4);
                            paso4.SetLimiteMaximo(vaporSaturado[0] + 1, vaporSaturado[1] + 1, vaporSaturado[2], vaporSaturado[3], vaporSaturado[4], vaporSaturado[5]);
                            paso4.SetLimiteMinimo(aguaSaturada[0] - 1, aguaSaturada[1] - 1, aguaSaturada[2], aguaSaturada[3], aguaSaturada[4], aguaSaturada[5]);
                           
                            paso1.ChangeEnable(false);                  //Evita las modificaciones en el renglon 1
                        }
                        else
                            MessageBox.Show("Not liquid water");
                    }
                    else
                    {
                        MessageBox.Show("Not enough values in step 2");
                    }
                }
            }
            else
            MessageBox.Show("Pressure needed");
        }

        private void Paso2Btn_Click(object sender, EventArgs e)
        {
            if (Presion2Txt.Text != "")
            {//Chea si hay presión
                if (paso2.CheckValues() && paso2.index != 10)
                {//Si los valores son válidos y son dos
                    if (paso2.IsLiquid(tabla4))
                    {//Checa si es líquida el agua 
                        //MessageBox.Show("Es líquida");
                        paso2.SearchIn7(tabla7);
                        paso1.SetLimiteMaximo(double.Parse(Presion2Txt.Text), double.Parse(Temperatura2Txt.Text), double.Parse(Volumen2Txt.Text), double.Parse(EnergiaInterna2Txt.Text), double.Parse(Entalpia2Txt.Text), double.Parse(Entropia2Txt.Text));
                        paso3.Data[0].Text = paso2.Data[0].Text;//Iguala la presión en el paso 3
                        paso2.ChangeEnable(false);                  //Evita las modificaciones en el renglon 2
                        double []temp = paso2.getVaporSaturado(tabla3);
                        paso3.SetLimiteMinimo(temp[0],temp[1],temp[2],temp[3],temp[4],temp[5]);
                    }
                    else
                        MessageBox.Show("Not liquid water");
                }
                if (paso2.CheckValues() && paso2.index == 10)
                {
                    //Solo un valor 
                    //MessageBox.Show("Se recibió un solo valor");
                    if (paso1.isCalculated)
                    {//Si ya se calculó el renglon pareja.
                        //Calculo de entalpía
                        double p2 = (double.Parse(paso1.Data[0].Text));
                        double p1 = (double.Parse(paso2.Data[0].Text));
                        double vol = double.Parse(paso1.Data[2].Text);
                        double deltaH = (p1 - p2) * vol;
                        double H = (double.Parse(paso1.Data[4].Text)) + deltaH;
                        paso2.Data[4].Text = H.ToString();
                        paso2.index = 4;
                        paso2.vals[1] = H;
                        //MessageBox.Show(H.ToString());
                        if (paso1.IsLiquid(tabla4))
                        {//Checa si es líquida el agua 
                         //MessageBox.Show("Es líquida");
                            paso2.SearchIn7(tabla7);
                            paso1.SetLimiteMaximo(double.Parse(Presion2Txt.Text), double.Parse(Temperatura2Txt.Text), double.Parse(Volumen2Txt.Text), double.Parse(EnergiaInterna2Txt.Text), double.Parse(Entalpia2Txt.Text), double.Parse(Entropia2Txt.Text));
                            paso3.Data[0].Text = paso2.Data[0].Text;//Iguala la presión en el paso 3
                            paso2.ChangeEnable(false);                  //Evita las modificaciones en el renglon 2
                            double[] temp = paso2.getVaporSaturado(tabla3);
                            paso3.SetLimiteMinimo(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]);
                        }
                        else
                            MessageBox.Show("Not liquid water");
                    }
                    else
                    {
                        MessageBox.Show("Not enough values in step 1");
                    }
                }
            }
            else
                MessageBox.Show("Pressure needed");
        }

        private void Paso3Btn_Click(object sender, EventArgs e)
        {
            if (Presion3Txt.Text != "")
            {//Checa si hay presión
                if (paso3.CheckValues() && paso3.index != 10)
                {//checa que haya dos valores
                    //MessageBox.Show("will calculate");
                    paso3.SearchIn6(tabla6);
                    paso3.ChangeEnable(false);

                }
                else
                 MessageBox.Show("Two values needed");
            }
            else
                MessageBox.Show("Pressure needed");
        }

        private void Paso4Btn_Click(object sender, EventArgs e)
        {
            if (paso1.isCalculated) {
                //Checa si hay presión
                if (paso4.GetThirdValue())
                {//Checa que no se haya puesto la X
                    //MessageBox.Show("will calculate");
                    Calidad4Txt.Text = (paso4.GetX() * 100).ToString();
                    paso4.CalculateEachWithX(paso4.GetX() * 100);
                    paso4.ChangeEnable(false);

                }
                else if (paso3.isCalculated)
                {
                    Entropia4Txt.Text = paso3.Data[5].Text;
                    paso4.vals[1] = double.Parse(Entropia4Txt.Text);
                    paso4.index = 5;
                    Calidad4Txt.Text = (paso4.GetX() * 100).ToString();
                    paso4.CalculateEachWithX(paso4.GetX() * 100);
                    paso4.ChangeEnable(false);
                }
                else if (Renglon.CheckSingleValue(Calidad4Txt))
                {//Checa si esta la X
                    //MessageBox.Show("Se recibió la X");
                    paso4.CalculateEachWithX(double.Parse(Calidad4Txt.Text));
                    paso4.ChangeEnable(false);
                }
                
                else
                {
                    MessageBox.Show("Step 3 needs to be calculated");
                }
            }
            else
                MessageBox.Show("Need the step one values first");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            paso1.clearItSelf();
            paso4.clearItSelf();
            Calidad4Txt.Text = "";
            set14Limits();
            paso4.Data[0].Enabled = false;
            paso4.Data[1].Enabled = false;

        }

        private void Clear23Btn_Click(object sender, EventArgs e)
        {
            paso2.clearItSelf();
            paso3.clearItSelf();
            set23Limits();
            paso3.Data[0].Enabled = false;
        }

        private void CalculateBtn_Click(object sender, EventArgs e)
        {
            if (paso1.isCalculated && paso2.isCalculated && paso3.isCalculated && paso4.isCalculated)
            {
                double[] results = Outputs.Calculate(Presion1Txt, Presion2Txt, Volumen1Txt, Entalpia1Txt, Entalpia2Txt, Entalpia3Txt, Entalpia4Txt);
                resultados.AutoFillWith(results);
            }
            double[] valores= Outputs.Fuel(mh2OTxt, EfCaldTxt, Entalpia1Txt, Entalpia2Txt, Entalpia3Txt, FuelCB, WnetoTxt);
            MassTxt.Text = valores[1].ToString();
            kWin.Text = valores[2].ToString();
            QinkW.Text = valores[3].ToString();
            textBox1.Text = valores[4].ToString();
        }
        public void set14Limits()
        {
            paso1.SetLimiteMaximo(6000, 260, 0.0012653, 1121.6, 1134.3, 2.8710);
            paso1.SetLimiteMinimo(34, 0, 0.0009977, 0.04, 5.03, 0.0001);
            paso4.SetLimiteMaximo(6000, 260, 0.0012653, 1121.6, 1134.3, 2.8710);
            paso4.SetLimiteMinimo(34, 0, 0.0009977, 0.04, 5.03, 0.0001);
        }

        public void set23Limits()
        {
            paso2.SetLimiteMaximo(6000, 260, 0.0012653, 1121.6, 1134.3, 2.8710);
            paso2.SetLimiteMinimo(34, 0, 0.0009977, 0.04, 5.03, 0.0001);
            paso3.SetLimiteMaximo(6000, 1300, 72.604, 4687.4, 5413.4, 11.5857);
            paso3.SetLimiteMinimo(10, 50, 14.67, 2437.2, 2583.9, 8.1488);
        }

        private void Confbtn_Click(object sender, EventArgs e)
        {
            if (FuelCB.SelectedItem == "" && mh2OTxt.Text == "" && EfCaldTxt.Text == "") 
                MessageBox.Show("Completa todos los datos");
        }

        private void Instructions_Click(object sender, EventArgs e)
        {
            instructions f1 = new instructions();
            f1.Show();
            this.Hide();
        }
    }
}
