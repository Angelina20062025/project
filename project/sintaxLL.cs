using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab4
{
    public class sintaxLL
    {
        List<Token> Tokens;
        int index;
        private ListBox listBox;
        public sintaxLL(List<Token> tokens, ListBox listBox = null)
        {
            this.listBox = listBox;
            Tokens = tokens;
            int x = prog();

        }

        public void Error(int kod, int index)
        {
            string error = $"Ошибка {kod} в {index}: ";

            switch (kod)
            {
                case -1:
                    error = "Ошибок нет";
                    break;
                case 0:
                    error += "Ожидалось main";
                    break;
                case 1:
                    error += "Ожидалось char";
                    break;
                case 2:
                    error += "Ожидалось long";
                    break;
                case 3:
                    error += "Ожидалось if";
                    break;
                case 4:
                    error += "Ожидалось else";
                    break;
                case 5:
                    error += "Ожидалось (";
                    break;
                case 6:
                    error += "Ожидалось )";
                    break;
                case 7:
                    error += "Ожидалось {";
                    break;
                case 8:
                    error += "Ожидалось }";
                    break;
                case 9:
                    error += "Ожидалось ;";
                    break;
                case 10:
                    error += "Ожидалось ,";
                    break;
                case 11:
                    error += "Ожидалось +";
                    break;
                case 12:
                    error += "Ожидалось -";
                    break;
                case 13:
                    error += "Ожидалось *";
                    break;
                case 14:
                    error += "Ожидалось /";
                    break;
                case 15:
                    error += "Ожидалось %";
                    break;
                case 16:
                    error += "Ожидалось >";
                    break;
                case 17:
                    error += "Ожидалось <";
                    break;
                case 18:
                    error += "Ожидалось =";
                    break;
                case 19:
                    error += "Ожидалось !";
                    break;
                case 20:
                    error += "Ожидалось <=";
                    break;
                case 21:
                    error += "Ожидалось >=";
                    break;
                case 22:
                    error += "Ожидалось --";
                    break;
                case 23:
                    error += "Ожидалось ==";
                    break;
                case 24:
                    error += "Ожидалось !=";
                    break;
                case 25:
                    error += "Ожидался идентификатор";
                    break;
                case 26:
                    error += "Ожидались индефикатор или литерал";
                    break;
                case 27:
                    error += "Ожидалось &&";
                    break;
                case 28:
                    error += "Ожидалось ||";
                    break;
                case 29:
                    error += "Ожидалось &";
                    break;
                case 30:
                    error += "Ожидалось |";
                    break;
                case 31:
                    error += "Ожидались char, long, if, --, индификатор";
                    break;
                case 32:
                    error += "Ожидалось служебное слово";
                    break;
                case 33:
                    error += "Ожидались арифметические операции";
                    break;
                case 34:
                    error += "Ожидались логические действия";
                    break;
                case 35:
                    error += "Ожидалось id lit ( !";
                    break;
                default:
                    error += "Неизвестная синтаксическая ошибка";
                    break;
            }

            //добавляем информацию о текущем токене
            if (index < Tokens.Count)
            {
                error += $". Обнаружено: {TokenType(Tokens[index].Type)}";
                if (!string.IsNullOrEmpty(Tokens[index].Value))
                {
                    error += $" '{Tokens[index].Value}'";
                }
            }
            else
            {
                error += ". Достигнут конец входных данных";
            }


            //выводим ошибку в ListBox
            if (listBox != null)
            {
                listBox.Items.Add(error);
            }
            else
            {
                Console.WriteLine(error);
            }
        }

        private string TokenType(string type)
        {
            switch (type)
            {
                case "S":
                    return "ключевое слово";
                case "R":
                    return "оператор";
                case "I":
                    return "идентификатор";
                case "O":
                    return "восьмеричное число";
                case "D":
                    return "десятичное число";
                default:
                    return type;
            }
        }

        public int prog()
        {
            int kod = -1;
            index = 0;

            if (!(Tokens[index].Type == "S" && Tokens[index].Number == 0)) //main
            {
                Error(0, index); return 0;
            }
            index++;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 0)) //(
            { Error(5, index); return 5; }
            index++;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 1)) //)
            { Error(6, index); return 6; }
            index++;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 2))//{
            {
                Error(7, index); return 7;
            }
            index++;

            kod = spis_oper(ref index);
            if (kod >= 0) return kod;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 3))//}
            {
                Error(8, index); return 8;
            }
            index++;
            Error(-1, index); return -1;
        }

        private int spis_oper(ref int index)
        {
            int kod = -1;
            kod = oper(ref index);
            if (kod >= 0)
                return kod;

            kod = X(ref index);
            return kod;
        }

        private int X(ref int index)
        {
            int kod = -1;
            //эпсилон
            if (Tokens[index].Type == "R" && Tokens[index].Number == 3) //}
                return -1;

            kod = spis_oper(ref index);
            if (kod >= 0) return kod;

            return -1;
        }

        private int oper(ref int index)
        {
            int kod = -1;
            //<опис>;
            if (Tokens[index].Type == "S" && (Tokens[index].Number == 1 || Tokens[index].Number == 2)) //char long
            {
                kod = opis(ref index);
                if (kod >= 0)
                    return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) //;
                {
                    Error(9, index); return 9;
                }
                index++;
                return -1;
            }
            // --id;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 17) //--
            {
                index++;
                if (!(Tokens[index].Type == "I")) //id
                {
                    Error(25, index); return 25;
                }
                index++;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) //;
                {
                    Error(9, index); return 9;
                }
                index++;
                return -1;
            }
            
            if (Tokens[index].Type == "I") //id
            {
                index++;
                if (Tokens[index].Type == "R" && Tokens[index].Number == 17) //--
                {
                    index++;
                    if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) //;
                    {
                        Error(9, index); return 9;
                    }
                    index++;
                    return -1;
                }
                else if (Tokens[index].Type == "R" && Tokens[index].Number == 13) //=
                {
                    index++;
                    kod = elem(ref index);
                    if (kod >= 0) return kod;

                    kod = Z(ref index);
                    if (kod >= 0) return kod;

                    if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) // ;
                    {
                        Error(9, index); return 9;
                    }
                    index++;
                    return -1;
                }
            }
            
            // if (<A>) <блок> <хвост>
            if (Tokens[index].Type == "S" && Tokens[index].Number == 3) //if
            {
                index++;
                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 0)) //(
                { Error(5, index); return 5; }
                index++;

                kod = A(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 1)) //)
                { Error(6, index); return 6; }
                index++;

                kod = block(ref index);
                if (kod >= 0) return kod;

                kod = hvost(ref index);
                if (kod >= 0) return kod;

                return -1;
            }

            Error(31, index); return 31;
        }

        private int opis(ref int index)
        {
            int kod = -1;
            kod = tip(ref index);
            if (kod >= 0)
                return kod;

            kod = spis_per(ref index);
            return kod;
        }

        private int tip(ref int index)
        {
            if (!(Tokens[index].Type == "S" && (Tokens[index].Number == 1 || Tokens[index].Number == 2))) //char long
            {
                Error(32, index); return 32;
            }
            index++;

            return -1;
        }

        private int spis_per(ref int index)
        {
            int kod = -1;
            kod = znach(ref index);
            if (kod >= 0)
                return kod;

            kod = Y(ref index);
            return kod;
        }

        private int Y(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 5) //,
            {
                kod = spis_per2(ref index);
                return kod;
            }   

            if (Tokens[index].Type == "R" && Tokens[index].Number == 4)//;
               return -1;

            Error(9, index); return 9;
        }

        //<спис_пер2> = ,<знач><Y>
        private int spis_per2(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 5) //,
            {
                index++;
                kod = znach(ref index);
                if (kod >= 0) return kod;

                kod = Y(ref index);
                return kod;
            }

            Error(10, index); return 10;
        }

        private int znach(ref int index)
        {
            int kod = -1;
            if (!(Tokens[index].Type == "I")) // id
            {
                Error(25, index); return 25;
            }
          
            if (Tokens[index+1].Type == "R" && Tokens[index+1].Number == 13) //=
            {
                kod = pr_arifm(ref index);
                if (kod >= 0) return kod;
            }

            index++;
            return -1;
        }

        //id = <элем><Z>
        private int pr_arifm(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "I") //id
            {
                index++;
                if (Tokens[index].Type == "R" && Tokens[index].Number == 13) //=
                {
                    index++;
                    kod = elem(ref index);
                    if (kod >= 0) return kod;

                    kod = Z(ref index);
                    return kod;

                    //if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) // ;
                    //{
                    //    Error(9, index); return 9;
                    //}
                    //index++;
                    //return -1;
                }
            }
            Error(25, index); return 25;
        }

        private int Z(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && (Tokens[index].Number == 6 || Tokens[index].Number == 12 || Tokens[index].Number == 8 || Tokens[index].Number == 9 || Tokens[index].Number == 7)) // + - % * /
            {
                kod = op(ref index);
                if (kod >= 0)
                    return kod;

                kod = elem(ref index);
                return kod;
            }

            if (Tokens[index].Type == "R" && (Tokens[index].Number == 4 || Tokens[index].Number == 5)) //, ;
                return -1;

            Error(10, index); return 10;
        }

        private int elem(ref int index)
        {
            if (!(Tokens[index].Type == "I" || Tokens[index].Type == "L")) // id или lit
            {
                Error(26, index); return 26;
            }
            index++;

            return -1;
        }

        private int op(ref int index)
        {
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 6)) // +
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 7)) // *
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 8)) // /
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 9)) // %
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 12)) // -
            {
                index++;
                return -1;
            }
            
            index++;
            Error(33, index); return 33;
        }

        //<A> = <B><W>
        private int A(ref int index)
        {
            int kod = -1;
            kod = B(ref index);
            if (kod >= 0)
                return kod;

            kod = W(ref index);
            return kod;
        }

        private int W(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 20) //&&
            {
                kod = A2(ref index);
                return kod;
            }

            if (Tokens[index].Type == "R" && Tokens[index].Number == 1)//)
                return -1;

            Error(6, index); return 6;
        }

        //<A2> = &&<B><W>
        private int A2(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 20) //&&
            {
                index++;
                kod = B(ref index);
                if (kod >= 0) return kod;

                kod = W(ref index);
                return kod;
            }

            Error(27, index); return 27;
        }

        //<B> = <C><V>
        private int B(ref int index)
        {
            int kod = -1;
            kod = C(ref index);
            if (kod >= 0)
                return kod;

            kod = V(ref index);
            return kod;
        }

        private int V(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 21) //||
            {
                kod = B2(ref index);
                return kod;
            }

            if (Tokens[index].Type == "R" && (Tokens[index].Number == 20 || Tokens[index].Number == 1))//&& )
                return -1;

            Error(27, index); return 27;
        }

        //<B2> = ||<C><V>
        private int B2(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 21) //||
            {
                index++;
                kod = C(ref index);
                if (kod >= 0) return kod;

                kod = V(ref index);
                return kod;
            }

            Error(28, index); return 28;
        }

        //<C> = <D><U>
        private int C(ref int index)
        {
            int kod = -1;
            kod = D(ref index);
            if (kod >= 0)
                return kod;

            kod = U(ref index);
            return kod;
        }

        private int U(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && (Tokens[index].Number == 11 || Tokens[index].Number == 10 || 
                Tokens[index].Number == 16 || Tokens[index].Number == 15 || Tokens[index].Number == 18 || 
                Tokens[index].Number == 19)) //> < >= <= == !=
            {
                kod = C2(ref index);
                return kod;
            }

            if (Tokens[index].Type == "R" && (Tokens[index].Number == 21 || Tokens[index].Number == 20 || 
                Tokens[index].Number == 1))//|| && )
                return -1;

            Error(28, index); return 28;
        }

        // <C2> = <оп_ср><D><U>
        private int C2(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && (Tokens[index].Number == 11 || Tokens[index].Number == 10 ||
                Tokens[index].Number == 16 || Tokens[index].Number == 15 || Tokens[index].Number == 18 ||
                Tokens[index].Number == 19)) //> < >= <= == !=
            {
                index++;
                kod = D(ref index);
                if (kod >= 0) return kod;

                kod = U(ref index);
                return kod;
            }

            Error(34, index); return 34;
        }

        private int D(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "I" || Tokens[index].Type == "L" || (Tokens[index].Type == "R" && Tokens[index].Number == 0)) //id lit (
            {
                kod = F(ref index);
                return kod;
            }

            if (Tokens[index].Type == "R" && Tokens[index].Number == 21) //!
            {
                kod = F(ref index);
                return kod;
            }

            Error(35, index); return 35;
        }

        private int F(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 0) // (
            {
                index++;
                kod = A(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 1)) // )
                {
                    Error(11, index); return 11;
                }
                index++;
                return -1;
            }

            if (!(Tokens[index].Type == "I" || Tokens[index].Type == "L")) // id или lit
            {
                Error(26, index); return 26;
            }
            index++;

            return -1;
        }

        private int op_sr(ref int index)
        {
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 11)) // >
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 10)) // <
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 16)) // >=
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 15)) // <=
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 18)) // ==
            {
                index++;
                return -1;
            }
            if ((Tokens[index].Type == "R" && Tokens[index].Number == 19)) // !=
            {
                index++;
                return -1;
            }

            index++;
            Error(34, index); return 34;

        }

        private int block(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 2) // {
            {
                index++;
                kod = spis_oper(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 3)) // }
                {
                    Error(8, index); return 8;
                }
                index++;
                return -1;
            }
            else
            {
                kod = oper(ref index);
                return kod;
            }
        }

        private int hvost(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "S" && Tokens[index].Number == 4) //else
            {
                index++;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 2)) // {
                {
                    Error(7, index); return 7; // Ожидалась {
                }

                index++;

                kod = spis_oper(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 3)) // }
                {
                    Error(8, index); return 8;
                }
                index++;
                return -1;
            }
            else if (Tokens[index].Type == "R" && Tokens[index].Number == 3) // }
            {
                return -1;
            }
            else
            {
                Error(4, index); return 4;
            }
        }
    }
}
