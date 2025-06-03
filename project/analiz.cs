using lab4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace lab4
{
    public class Token
    {
        public string Type { get; set; } //тип
        public string Value { get; set; } //значение
        public int Number { get; set; } //номер
        public Token(string type, string value, int number)
        {
            Type = type;
            Value = value;
            Number = number;
        }
    }

    public class analiz
    {
        enum State { S, I, O, R} //состояния: S - начальное, I - идентификатор, O - восьмеричное число, R - одиночный разделитель

        //проверка на букву
        private static bool IsSymbol(char c)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                return true;
            }
            return false;
        }

        //проверка на восьмеричную цифру
        private static bool IsOctalDigit(char c)
        {
            if ((c >= '0') && (c <= '7'))
            {
                return true;
            }
            return false;
        }

        //проверка на букву или цифру
        private static bool IsOr(char c)
        {
            if (IsSymbol(c) || IsOctalDigit(c))
            {
                return true;
            }
            return false;
        }

        //проверка на пробел
        private static bool IsSpace(char c)
        {
            if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
            {
                return true;
            }
            return false;
        }

        //таблицы для хранения идентификаторов и литералов
        public static List<string> Identifiers = new List<string>();
        public static List<string> Literals = new List<string>();

                                            //  0        1       2      3      4
        private static string[] Terminals = { "main", "char", "long", "if", "else" }; //служебные слова S
                                              //  0    1    2    3    4    5    6    7    8    9
        private static string[] SingleRazdel = { "(", ")", "{", "}", ";", ",", "+", "*", "/", "%" }; //одиночные разделители
                                              //  10   11   12   13   14    15    16    17    18    19    20    21   22   23
        private static string[] ComplexRazdel = { "<", ">", "-", "=", "!", "<=", ">=", "--", "==", "!=", "&&", "||", "&", "|" }; //составные разделители

        public void Error(int kod, ListBox listBox, char? uc = null, string identifier = null)
        {
            string error = $"Лексическая ошибка №{kod}: ";

            switch (kod)
            {
                case -1:
                    error = "Лексических ошибок нет.";
                    break;
                case 50:
                    error += $"Неизвестный символ '{(uc.HasValue ? uc.Value.ToString() : " ")}'";
                    break;
                case 51:
                    error += $"Превышено количество символов в идентификаторе '{identifier}'. Максимальная длина - 10 символов";
                    break;
                case 52:
                    error += "Восьмеричное число должно быть 1-байтным (000-377) или 4-байтным (00000000000-77777777777)";
                    break;
                case 53:
                    error += "Восьмеричное число должно начинаться с 0";
                    break;
                default:
                    error += "Неизвестная лексическая ошибка";
                    break;
            }
            // Выводим ошибку
            if (listBox != null)
            {
                listBox.Items.Add(error);
            }
            else
            {
                Console.WriteLine(error);
            }
        }

        public static List<Token> CheckString(string str, ListBox listBox)
        {
            List<Token> Tokens = new List<Token>(); //список для хранения токенов
            
            var l = new analiz();
            int y = 0;

            State state = State.S; //начальное состояние
            string buffer = ""; //буфер для накопления символов
            int RepeatState = 1; //флаг для повторной обработки состояния
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (RepeatState == 1) RepeatState = 0;
                for (int j = -1; j < RepeatState; ++j)
                {
                    switch (state)
                    {
                        case State.S:
                            if (IsSymbol(c))
                            {
                                state = State.I; //состояние идентификатора
                                buffer += c;
                            }
                            else if (IsOctalDigit(c))
                            {
                                state = State.O; //восьмеричное число
                                buffer += c;
                            }
                            else if (IsSpace(c)) { }
                            else if (SingleRazdel.Contains(c.ToString()))
                            {
                                //добавление одиночного разделителя
                                Tokens.Add(new Token("R", c.ToString(), Array.IndexOf(SingleRazdel, c.ToString())));
                            }
                            //если разделитель парный
                            else if (ComplexRazdel.Contains(c.ToString()))
                            {
                                state = State.R;
                                buffer += c;
                            }
                            else
                            {
                                l.Error(50, listBox, c); // Передаем неизвестный символ в обработчик ошибок
                                y++;
                            }
                            break;

                        case State.I:
                            if (IsOr(c))
                            {
                                if (buffer.Length >= 10)
                                {
                                    l.Error(51, listBox, null, buffer);
                                    y++;
                                    continue;
                                }
                                buffer += c;
                            }
                            else
                            {
                                if (Terminals.Contains(buffer))
                                {
                                    Tokens.Add(new Token("S", buffer, Array.IndexOf(Terminals, buffer)));
                                }
                                else
                                {
                                    if (!Identifiers.Contains(buffer))
                                    {
                                        Identifiers.Add(buffer);
                                    }
                                    Tokens.Add(new Token("I", buffer, Identifiers.IndexOf(buffer)));
                                }
                                buffer = "";
                                state = State.S;
                                RepeatState = 1;
                            }
                            break;

                        case State.O:
                            if (IsOctalDigit(c)) { buffer += c; }
                            else
                            {
                                // Проверка размера восьмеричного числа
                                if (buffer.StartsWith("0"))
                                {
                                    string octalValue = buffer.Substring(1);

                                    // 4-байтное число (до 11 цифр: 00000000000–77777777777)
                                    if (octalValue.Length > 11)
                                    {
                                        l.Error(52, listBox); // Восьмеричное число слишком большое
                                        y++;
                                    }
                                    else if (octalValue.Length > 3 && octalValue.Length <= 11)
                                    {
                                        if (!Literals.Contains(buffer))
                                        {
                                            Literals.Add(buffer);
                                        }
                                        Tokens.Add(new Token("L", buffer, Literals.IndexOf(buffer)));
                                    }
                                    else if (octalValue.Length <= 3)
                                    {
                                        // 1-байтное число (000–377)
                                        if (!Literals.Contains(buffer))
                                        {
                                            Literals.Add(buffer);
                                        }
                                        Tokens.Add(new Token("L", buffer, Literals.IndexOf(buffer)));
                                    }
                                }
                                else
                                {
                                    l.Error(53, listBox); // Восьмеричное число должно начинаться с 0
                                    y++;
                                }
                                buffer = "";
                                state = State.S;
                                RepeatState = 1;
                            }
                            break;

                        case State.R:
                            if (ComplexRazdel.Contains(buffer + c))
                            {
                                buffer += c;
                            }
                            else
                            {
                                Tokens.Add(new Token("R", buffer, Array.IndexOf(ComplexRazdel, buffer) + 10));
                                buffer = "";
                                state = State.S;
                                RepeatState = 1;
                            }
                            break;
                    }
                }
            }

            if (buffer != "")
            {
                switch (state)
                {
                    case State.I:
                        if (Terminals.Contains(buffer))
                        {
                            Tokens.Add(new Token("S", buffer, Array.IndexOf(Terminals, buffer)));
                        }
                        else
                        {
                            if (!Identifiers.Contains(buffer))
                            {
                                Identifiers.Add(buffer);
                            }
                            Tokens.Add(new Token("I", buffer, Identifiers.IndexOf(buffer)));
                        }
                        break;
                    case State.O:
                        if (!Literals.Contains(buffer))
                        {
                            Literals.Add(buffer);
                        }
                        Tokens.Add(new Token("L", buffer, Literals.IndexOf(buffer)));
                        break;

                    case State.R:
                        if (buffer.Length > 0)
                            Tokens.Add(new Token("R", buffer, Array.IndexOf(ComplexRazdel, buffer) + 10));
                        else
                            Tokens.Add(new Token("R", buffer, Array.IndexOf(SingleRazdel, buffer)));
                        break;
                }
            }
            if (y == 0)
            {
                l.Error(-1, listBox);
            }
            return Tokens; //возвращаем список токенов
        }
    }  
}