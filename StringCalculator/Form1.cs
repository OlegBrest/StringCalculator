using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StringCalculator
{
    public partial class Form1 : Form
    {

        char[] opers = new char[] { '+', '-', '*', '/' };
        char[] prior = new char[] { '(', ')' };
        public Form1()
        {
            InitializeComponent();
        }

        private void calcButton_Click(object sender, EventArgs e)
        {
            string inp = inputTextBox.Text;
            if (corrected(inp))
            {
                outputTextBox.Text = StartCalulating(inp).ToString();
            }
        }


        private bool corrected(string txt)
        {
            int skobscount = 0;
            int textlenght = txt.Length;
            if (textlenght > 0)
            {
                for (int i = 0; i < textlenght; i++)
                {
                    if ((i + 1) < textlenght && (opers.Any(s => s == txt[i + 1]) && opers.Any(s => s == txt[i])))
                    {
                        MessageBox.Show("Перепроверь операторы");
                        return false;
                    }
                    if (txt[i] == '(') skobscount++;
                    if (txt[i] == ')') skobscount--;
                    if (skobscount < 0)
                    {
                        MessageBox.Show("Проверьте закрывающие скобки");
                        return false;
                    }
                }
                if (skobscount > 0)
                {
                    MessageBox.Show("Проверьте открывающие скобки");
                    return false;
                }
            }
            if (opers.Any(s => s == txt[textlenght - 1]))
            {
                MessageBox.Show("Перепроверь операторы");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Начать считать строку
        /// </summary>
        /// <param name="inpText">входной текст</param>
        /// <returns></returns>
        private double StartCalulating(string inpText)
        {
            string workingString = inpText;
            bool isSplitted = false;
            double result = double.NaN;
            if (double.TryParse(inpText, out result))
                return result;
            else
            {
                result = double.NaN;
            }
            //работа со скобками
            if (inpText.Intersect(prior).Count() > 0)
            {
                workingString = PriorCalc(workingString);
            }
            // работа с операторами
            foreach (char oper in opers)
            {
                if (!isSplitted)
                {

                    string[] substrings = workingString.Split(oper);
                    int sslenght = substrings.Count();
                    if (sslenght > 1) isSplitted = true;
                    if ((sslenght > 1) && (result.Equals(double.NaN)))
                    {
                        result = StartCalulating(substrings[0]);
                    }
                    if (!result.Equals(double.NaN))
                    {
                        for (int sstr = 1; sstr < sslenght; sstr++)
                        {
                            string str = substrings[sstr];
                            if (oper == '+')
                            {
                                result += StartCalulating(str);
                            }
                            if (oper == '-')
                            {
                                result -= StartCalulating(str);
                            }
                            if (oper == '*')
                            {
                                result *= StartCalulating(str);
                            }
                            if (oper == '/')
                            {
                                result /= StartCalulating(str);
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Подсчёт содержимого в скобках и корректировка выходной строки
        /// </summary>
        /// <param name="inpText">входная строка</param>
        /// <returns></returns>
        private string PriorCalc (string inpText)
        {
            string retval = "";
            bool priorFinded = false;
            int textlenght = inpText.Length;
            int skobscount = 0;
            string priortext = "";
            if (textlenght > 0)
            {
                for (int i = 0; i < textlenght; i++)
                {
                    if (inpText[i] == '(')
                    {
                        if (!priorFinded) i++;
                        priorFinded = true;
                        skobscount++;
                    }
                    if (inpText[i] == ')') skobscount--;

                    if (!priorFinded) retval += inpText[i];
                    if (priorFinded)
                    {
                        if (skobscount > 0)
                        {
                            priortext += inpText[i];
                        }
                        else
                        {
                            retval += (StartCalulating(priortext).ToString());
                            priortext = "";
                            priorFinded = false;
                        }
                    }
                }
            }
            return retval;
        }
    }
}
