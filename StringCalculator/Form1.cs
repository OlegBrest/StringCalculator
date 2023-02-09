﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StringCalculator
{
    public partial class Form1 : Form
    {

        char[] opers = new char[] { '+', '-', '*', '/' };
        char[] prior = new char[] { '(', ')' };
        /// <summary>
        /// Разделитель дробной части
        /// </summary>
        string dec_sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        public Form1()
        {
            InitializeComponent();
        }

        private void calcButton_Click(object sender, EventArgs e)
        {
            string inp = inputTextBox.Text;
            inp = PriorCorrection(inp);
            if (corrected(inp))
            {
                outputTextBox.Text = StartCalulating(inp).ToString();
            }
        }

        private bool corrected(string txt)
        {
            int skobscount = 0;
            int textlenght = txt.Length;
            if (opers.Any(s => s == txt[textlenght - 1]))
            {
                outputTextBox.Text=("Перепроверь последний оператор");
                return false;
            }
            if (txt[textlenght - 1] == dec_sep[0])
            {
                outputTextBox.Text=("Перепроверь последнее дробное число");
                return false;
            }
            if (textlenght > 0)
            {
                for (int i = 0; i < textlenght; i++)
                {
                    if ((i + 1) < textlenght)
                    {
                        if ((opers.Any(s => s == txt[i + 1]) || (txt[i + 1] == ')')) && opers.Any(s => s == txt[i]))
                        {
                            outputTextBox.Text=("Перепроверь операторы");
                            return false;
                        }
                        if (((txt[i] == dec_sep[0]) && (!Char.IsDigit(txt[i + 1]))) || ((txt[i+1] == dec_sep[0]) && (!Char.IsDigit(txt[i]))))
                        {
                            outputTextBox.Text=("Перепроверь разделители дробной части");
                            return false;
                        }
                    }
                    if (txt[i] == '(') skobscount++;
                    if (txt[i] == ')') skobscount--;
                    if (skobscount < 0)
                    {
                        outputTextBox.Text=("Проверьте закрывающие скобки");
                        return false;
                    }
                }
                if (skobscount > 0)
                {
                    outputTextBox.Text=("Проверьте открывающие скобки");
                    return false;
                }
            }
            return true;
        }

        private string PriorCorrection(string inputString)
        {
            string tmp = "";
            string resval = "";
            tmp = inputString.Replace(")(", ")*(");
            int textlenght = tmp.Length;
            for (int i = 0; i < textlenght; i++)
            {
                resval += tmp[i];
                if ((i + 1) < textlenght)
                {
                    if (char.IsDigit(tmp[i]) && (tmp[i + 1] == '('))
                    {
                        resval += '*';
                    }
                    if (char.IsDigit(tmp[i+1]) && (tmp[i] == ')'))
                    {
                        resval += '*';
                    }
                }
            }
            return resval;
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
            if (double.TryParse(workingString, out result))
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
            if (workingString.Intersect(opers).Count() > 0)
            {
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
            }
            else
            {
                if (double.TryParse(workingString, out result))
                    return result;
                else
                {
                    result = double.NaN;
                }
            }
            return result;
        }

        /// <summary>
        /// Подсчёт содержимого в скобках и корректировка выходной строки
        /// </summary>
        /// <param name="inpText">входная строка</param>
        /// <returns></returns>
        private string PriorCalc(string inpText)
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

        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keypressed = e.KeyChar;
            if (!Char.IsDigit(keypressed) && !opers.Any(x => x == keypressed) && !prior.Any(x => x == keypressed) && (keypressed != dec_sep[0]) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }
    }
}
