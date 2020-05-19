using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RankineCycle
{
    class Outputs
    {
        public static double[] Calculate(TextBox P1txt, TextBox P2txt, TextBox V1txt, TextBox H1txt, TextBox H2txt, TextBox H3txt, TextBox H4txt)
        {
            double[] results = new double[6];
            /*Win*/
            results[0] = (float.Parse(P2txt.Text) - float.Parse(P1txt.Text)) * float.Parse(V1txt.Text);
            /*Qin*/
            results[1] = float.Parse(H3txt.Text) - float.Parse(H2txt.Text);
            /*Wout*/
            results[2] = float.Parse(H4txt.Text) - float.Parse(H3txt.Text);
            /*Qout*/
            results[3] = float.Parse(H1txt.Text) - float.Parse(H4txt.Text);

            /*Wneto = Qin-Qout;*/
            results[4] = results[1] + results[3];
            /*eficiencia = Wneto / Qin;*/
            results[5] = (results[4] / results[1])*100;


            return results;
        }

        public static double[] Fuel(TextBox mass, TextBox effic, TextBox H1txt, TextBox H2txt, TextBox H3txt, ComboBox types, TextBox wneto)
        {
            float Pc;
            double[] Values = new double[5];

            //W in (1-2)
            Values[2] = (float.Parse(mass.Text)) * (float.Parse(H2txt.Text) - float.Parse(H1txt.Text));
            //Q in (2-3)
            Values[3] = (float.Parse(mass.Text)) * (float.Parse(H3txt.Text) - float.Parse(H2txt.Text));

            //Q out
            Values[4] = (float.Parse(mass.Text) * float.Parse(wneto.Text));

            switch (types.SelectedItem)
            {
                case "Carbón mineral":
                    Pc = 31400;
                    //Masa del combustible
                    Values[1] = (Values[3]) / (Pc * float.Parse(effic.Text));                 
                    break;
                case "Carbón vegetal":
                    Pc = 31820;
                    //Masa del combustible
                    Values[1] = (Values[3]) / (Pc * float.Parse(effic.Text));
                    break;
                case "Gasolina":
                    Pc = 45000;
                    //Masa del combustible
                    Values[1] = (Values[3]) / (Pc * float.Parse(effic.Text));
                    break;
                case "Gas LP":
                    Pc = 50242;
                    //Masa del combustible
                    Values[1] = (Values[3]) / (Pc * float.Parse(effic.Text));
                    break;
                case "Diesel":
                    Pc = 41868;
                    //Masa del combustible
                    Values[1] = (Values[3]) / (Pc * float.Parse(effic.Text));
                    break;
            }
            return Values;
        }
    }
}
