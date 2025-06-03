using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static lab4.sintaxLR;

namespace lab4
{
    class Stack<T>
    {
        T[] mas;
        int top;

        public Stack()
        {
            mas = new T[100];
            top = 0;
        }
        public void Push(T a)
        {
            if (top < mas.Length - 1)
            {
                mas[++top] = a;
            }
            else
            {
                throw new InvalidOperationException("Ошибка: переполнение стека.");
            }
        }
        public T Read()
        {
            return mas[top];
        }
        public T Pop(int count)
        {
            top -= count;
            if (top >= 0)
            {
                return mas[top];
            }
            else
            {
                throw new InvalidOperationException("Ошибка: попытка извлечения из пустого стека.");
            }
        }

        public T Pop1()
        {
            if (top >= 0)
            {
                return mas[top--];
            }
            else
            {
                throw new InvalidOperationException("Ошибка: попытка извлечения из пустого стека.");
            }
        }

        public T Peek()
        {
            if (top < 0)
                throw new InvalidOperationException("Стек пуст.");
            return mas[top];
        }

        public bool isEmpty()
        {
            return top < 0;
        }
        public bool isFull()
        {
            return top >= mas.Length;
        }

        public int Count => top;
    }
    internal class sintaxLR
    {
        Stack<int> StackSost;
        Stack<Token> StackRazbor;
        public List<Token> Tokens;
        int index;
        private ListBox listBox;
        private ListBox listBox2;
        private ListBox listBox3;
        private ListBox listBox4;

        private Stack<string> LabelStack;
        private int labelCounter = 0;

        int id = 0;   //семантическая проверка на объявления
        int[] masType = new int[100];
        List<string> mas2 = analiz.Identifiers;

        public sintaxLR(List<Token> tokens, ListBox listBox = null, ListBox listBox2 = null, ListBox listBox3 = null, ListBox listBox4 = null)
        {
            StackSost = new Stack<int>();
            StackRazbor = new Stack<Token>();
            index = 0;
            this.listBox = listBox;
            this.listBox2 = listBox2;
            this.listBox3 = listBox3;
            this.listBox4 = listBox4;
            LabelStack = new Stack<string>();

            Tokens = tokens;
            for (int i = 0; i < masType.Length; i++)
            {
                masType[i] = -1;
            }
            int x = SinALR();
        }
        public void Sdvig()
        {
            StackRazbor.Push(Tokens[index++]);
        }
        public void Perehod(int perehod)
        {
            StackSost.Push(perehod);
        }
        public void Privedenie(int delete, N name)
        {
            Token temp = new Token("N", "", (int)name);
            StackSost.Pop(delete);
            StackRazbor.Pop(delete);
            StackRazbor.Push(temp);
        }

        private bool SintaxError = false;

        private void Error(int kod, int index, int sost)
        {
            if (kod != -1)
            {
                SintaxError = true;
            }

            string error = $"Синтаксическая ошибка №{kod} на позиции {index - 1} в состоянии {sost}: ";
            switch (kod)
            {
                case -1:
                    error = "Синтаксических ошибок нет.";
                    break;
                case 0:
                    error += "Ожидалось main";
                    break;
                case 1:
                    error += "Ожидалось (";
                    break;
                case 2:
                    error += "Ожидалось )";
                    break;
                case 3:
                    error += "Ожидалось {";
                    break;
                case 4:
                    error += "Ожидалось }";
                    break;
                case 5:
                    error += "Ожидалось ;";
                    break;
                case 6:
                    error += "Ожидалось I";
                    break;
                case 7:
                    error += "Ожидались =, ; или ,";
                    break;
                case 8:
                    error += "Ожидались =, --";
                    break;
                case 9:
                    error += "Ожидались I, L";
                    break;
                case 10:
                    error += "Ожидались ',' или ';' или арифметический оператор";
                    break;
                case 11:
                    error += "Ожидались !, (, I, L";
                    break;
                case 12:
                    error += "Ожидались ), &&";
                    break;
                case 13:
                    error += "Ожидались ;, {, char, long, I, if, --";
                    break;
                case 14:
                    error += "Ожидались }, char, long, I, if, --";
                    break;
                case 15:
                    error += "Ожидались ||, &&";
                    break;
                case 16:
                    error += "Ожидались ), &&, ||, оператор сравнения";
                    break;
                case 17:
                    error += "Ожидались ), &&, ||";
                    break;
                case 18:
                    error += "Ожидались I, L, (";
                    break;
                case 19:
                    error += "Ожидались char, long, I, if, --";
                    break;
                case 20:
                    error += "Ошибка приведения";
                    break;
                case 21:
                    error += "Ожидались ',' или ';'";
                    break;
                case 22:
                    error += "Ожидалось 'else'";
                    break;
                default:
                    error += "Неизвестная синтаксическая ошибка";
                    break;
            }
            index--;
            //Добавляем информацию о текущем токене
            if (index < Tokens.Count)
            {
                error += $". Обнаружено: {GetTokenType(Tokens[index].Type)}";
                if (!string.IsNullOrEmpty(Tokens[index].Value))
                {
                    error += $" '{Tokens[index].Value}'";
                }
            }
            else
            {
                error += " Достигнут конец входных данных";
            }
            //Выводим ошибку вListBox
            if (listBox != null)
            {
                listBox.Items.Add(error);
            }
            else
            {
                Console.WriteLine(error);
            }
        }

        private bool SemanticError = false;

        private void Error2(int kod, int index, string name = "")
        {
            if (kod != -1)
            {
                SemanticError = true;
            }

            string error = $"Семантическая ошибка №{kod} на позиции {index}: ";

            switch (kod)
            {
                case -1:
                    error = "Семантических ошибок нет.";
                    break;
                case 30:
                    error += $"Повторное объявление переменной {name}";
                    break;
                case 31:
                    error += $"Использование необъявленной переменной {name}";
                    break;
                default:
                    error += "Неизвестная семантическая ошибка";
                    break;
            }
            // Выводим ошибку в ListBox
            if (listBox2 != null)
            {
                listBox2.Items.Add(error);
            }
            else
            {
                Console.WriteLine(error);
            }
        }

        //объявление переменной
        private void Variable(Token typeToken, Token idToken)
        {
            //определние типа и имени переменной
            string name = idToken.Value;
            int type = typeToken.Number == 1 ? 1 : 2;
            
            // если переменная с таким именем существует в списке идентификаторов
            if (mas2.Contains(name))
            {
                // если переменная существует и её тип определен
                if (masType[mas2.IndexOf(name)] != -1)
                {
                    Error2(30, index, name);
                    return;
                }

                // если переменная существует, но её тип не определен
                masType[mas2.IndexOf(name)] = type;
                id++;
            }
            else
            {
                // если переменная не существует
                mas2.Add(name);
                masType[id++] = type;
            }
            GenerateVariableDeclaration(typeToken, idToken);
        }

        private void GenerateVariableDeclaration(Token typeToken, Token idToken)
        {
            string typeName = typeToken.Number == 1 ? "long" : "char";
            listBox4.Items.Add($"Объявление переменной: тип {typeName}, имя {idToken.Value}");
        }

        //использование переменной
        private void UsingVariable(Token idToken)
        {
            string name = idToken.Value;

            if (!mas2.Contains(name) || masType[mas2.IndexOf(name)] == -1)
            {
                Error2(31, index, name);
            }
        }

        private int temp = 0;

        private string GetTempVariable()
        {
            return $"D{temp++}";
        }

        private void TranslateExpression(string expression)
        {
            Stack<string> E = new Stack<string>(); // Стек для операндов
            Stack<string> O = new Stack<string>(); // Стек для операций
            List<(string Operation, string Operand1, string Operand2, string Result)> output = new List<(string, string, string, string)>(); // Список для вывода результатов

            int index = 0;
            while (index < expression.Length)
            {
                char current = expression[index];
                if (current == '(')
                {
                    O.Push(current.ToString());
                    index++;
                }
                else if (current == ')')
                {
                    while (O.Count > 0 && O.Peek() != "(")
                    {
                        string topOp = O.Pop1();
                        string operand2 = E.Pop1();
                        string operand1 = E.Pop1();
                        string result = GetTempVariable();
                        output.Add((topOp, operand1, operand2, result));
                        string operationName = GetOperationName(topOp);
                        listBox4.Items.Add($"{operationName} {operand1} и {operand2}, результат - {result}");
                        E.Push(result);
                    }
                    O.Pop1(); // Удаляем "(" из стека
                    index++;
                }
                else if (char.IsLetterOrDigit(current))
                {
                    // Если текущий элемент - операнд, помещаем его в стек E
                    string operand = ReadOperand(expression, ref index);
                    E.Push(operand);
                }
                else if (IsOperatorStart(current))
                {
                    string op = ReadOperator(expression, ref index);
                    while (O.Count > 0 && O.Peek() != "(" && Precedence(op) <= Precedence(O.Peek()))
                    {
                        string topOp = O.Pop1();
                        string operand2 = E.Pop1();
                        string operand1 = E.Pop1();
                        string result = GetTempVariable();
                        output.Add((topOp, operand1, operand2, result));
                        E.Push(result);
                    }
                    O.Push(op);
                }
                else
                {
                    index++;
                }
            }

            // После обработки всех символов выполняем оставшиеся операции в стеке O
            while (O.Count > 0)
            {
                string op = O.Pop1();
                string operand2 = E.Pop1();
                string operand1 = E.Pop1();
                string result = GetTempVariable();
                output.Add((op, operand1, operand2, result));
                E.Push(result);
            }

            // Вывод результатов в ListBox
            if (listBox3 != null)
            {
                foreach (var item in output)
                {
                    string line = $"{item.Operation}                              {item.Operand1}                         {item.Operand2}                        {item.Result}";
                    listBox3.Items.Add(line);
                }
            }
        }

        private string GetOperationName(string op)
        {
            switch (op)
            {
                case "+": return "Сложение";
                case "-": return "Вычитание";
                case "*": return "Умножение";
                case "/": return "Деление";
                case "%": return "Остаток от деления";
                case "&&": return "Логическое И";
                case "||": return "Логическое ИЛИ";
                case ">": return "Сравнение (больше)";
                case "<": return "Сравнение (меньше)";
                case ">=": return "Сравнение (больше или равно)";
                case "<=": return "Сравнение (меньше или равно)";
                case "==": return "Сравнение (равно)";
                case "!=": return "Сравнение (не равно)";
                default: return "Операция";
            }
        }

        // Чтение многосимвольного операнда
        private string ReadOperand(string expression, ref int index)
        {
            int start = index;
            while (index < expression.Length && (char.IsLetterOrDigit(expression[index])))
            {
                index++;
            }
            return expression.Substring(start, index - start);
        }

        // Чтение оператора
        private string ReadOperator(string expression, ref int index)
        {
            char firstChar = expression[index];
            if (index + 1 < expression.Length)
            {
                char secondChar = expression[index + 1];
                string potentialOp = $"{firstChar}{secondChar}";
                if (IsTwoCharOperator(potentialOp))
                {
                    index += 2;
                    return potentialOp;
                }
            }
            index++;
            return firstChar.ToString();
        }

        // Проверка, является ли символ началом оператора
        private bool IsOperatorStart(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '%' ||
                   c == '>' || c == '<' || c == '=' || c == '!' || c == '&' || c == '|';
        }

        // Проверка, является ли строка двухсимвольным оператором
        private bool IsTwoCharOperator(string op)
        {
            return op == "==" || op == "!=" || op == ">=" || op == "<=" ||
                   op == "&&" || op == "||" || op == "+=" || op == "-=" || op == "--";
        }

        //таблица приоритетов
        private int Precedence(string op)
        {
            switch (op)
            {
                case "!":
                    return 7;
                case "--":
                    return 6;
                case "*":
                case "/":
                case "%":
                    return 5;
                case "+":
                case "-":
                    return 4;
                case ">":
                case "<":
                case ">=":
                case "<=":
                    return 3;
                case "==":
                case "!=":
                    return 2;
                case "&&":
                    return 1;
                case "||":
                    return 0;
                default:
                    return -1;
            }
        }

        private string GetExpressionFromTokens()
        {
            StringBuilder sb = new StringBuilder();
            int i = index;
            while (i < Tokens.Count && Tokens[i].Value != ";")
            {
                sb.Append(Tokens[i].Value);
                i++;
            }
            return sb.ToString();
        }

        private string GetNewLabel()
        {
            return $"метка_{labelCounter++}";
        }

        private string GetOldLabel()
        {
            return $"метка_{labelCounter-2}";
        }

        public void PushLabel(string label)
        {
            LabelStack.Push(label);
        }

        private void GenerateIfCode(Token conditionToken)
        {
            string elseLabel = GetNewLabel();
            string endIfLabel = GetNewLabel();

            listBox4.Items.Add($"Начало условного оператора");
            
            listBox4.Items.Add($"Если условие ({conditionToken.Value}) ложно, перейти на {elseLabel},");
            listBox4.Items.Add($"иначе перейти на {endIfLabel}");
            listBox4.Items.Add($"Начало блока после условия ({endIfLabel})");
        }

        private void GenerateelseCode()
        {
            string elseLabel = GetOldLabel();
            listBox4.Items.Add($"Начало блока 'иначе' ({elseLabel})");
        }

        private void GenerateAssignment(Token idToken, Token expressionToken)
        {
            listBox4.Items.Add($"Присваивание: {idToken.Value} = {expressionToken.Value}");
        }

        private void GenerateDecrement(Token idToken, bool isPrefix)
        {
            if (isPrefix)
                listBox4.Items.Add($"Декремент префиксный: --{idToken.Value}");
            else
                listBox4.Items.Add($"Декремент постфиксный: {idToken.Value}--");
        }

        private string GetConditionFromTokens()
        {
            StringBuilder condition = new StringBuilder();
            int i = index;

            // Пропускаем сам токен 'if' и открывающую скобку
            i += 1;

            while (i < Tokens.Count && Tokens[i].Value != ")")
            {
                condition.Append(Tokens[i].Value);
                i++;
            }

            return condition.ToString();
        }

        private string GetTokenType(string type)
        {
            switch (type)
            {
                case "S":
                    return "служебное слово";
                case "R":
                    return "оператор";
                case "I":
                    return "идентификатор";
                case "L":
                    return "литерал";
                default:
                    return type;
            }
        }//              0        1      2     3    4      5        6       7       8   9   10       11      12   13  14 15 16 17   18    19     20
        public enum N { prog, spis_op, oper, opis, tip, spis_per, zhach, pr_arifm, vir, el, op, oper_decr, usl_op, A, B, C, D, F, block, op_sr, hvost }
        public int SinALR()
        {
            Token temp = new Token("", "", 0);
            while (true)
            {
                switch (StackSost.Read())
                {
                    case 0:
                        if (StackRazbor.isEmpty()) { Sdvig(); continue; }
                        temp = StackRazbor.Read();
                        if (index >= Tokens.Count && !SemanticError)
                        {
                            index++; Error(-1, index, 4); break;
                        }
                        else if (index >= Tokens.Count && SemanticError)
                        {
                            return -1;
                        }
                        Sdvig();
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.prog) return -1; //код ошибки успеха
                        if (temp.Type == "S" && temp.Number == 0) { Perehod(1); continue; } //main
                        Error(0, index, 0); break;
                    case 1:
                        temp = StackRazbor.Read();
                        if (temp.Type == "S" && temp.Number == 0) 
                        {
                            listBox4.Items.Add("Начало программы");
                            Sdvig(); 
                        }//main
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 0) { Perehod(2); continue; } //(
                        Error(1, index, 1); break; //ожидалась (
                    case 2:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 0) { Sdvig(); }//(
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 1) { Perehod(3); continue; } //)
                        Error(2, index, 2); break; //ожидалась )
                    case 3:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 1) { Sdvig(); }//)
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 2) { Perehod(4); continue; } //{
                        Error(3, index, 3); break; //ожидалась {
                    case 4:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 2) { Sdvig(); }//{
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.spis_op)
                        {
                            Perehod(80); continue;
                        } //spis_op
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Perehod(7); continue;
                        }//oper
                        if (temp.Type == "N" && temp.Number == (int)N.opis)
                        {
                            Perehod(8); continue;
                        }//opis
                        if (temp.Type == "N" && temp.Number == (int)N.oper_decr)
                        {
                            Perehod(10); continue;
                        }//oper_decr
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(12); continue;
                        }//pr_arifm
                        if (temp.Type == "N" && temp.Number == (int)N.usl_op)
                        {
                            Perehod(14); continue;
                        }//usl_op
                        if (temp.Type == "N" && temp.Number == (int)N.tip)
                        {
                            Perehod(27); continue;
                        }//tip
                        if (temp.Type == "S" && temp.Number == 1)
                        {
                            Perehod(5); continue;
                        } //char
                        if (temp.Type == "S" && temp.Number == 2)
                        {
                            Perehod(6); continue;
                        } //long
                        if (temp.Type == "I") { Perehod(15); continue; }//id
                        if (temp.Type == "S" && temp.Number == 3) { Perehod(39); continue; }//if
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Perehod(34); continue;
                        } //--
                        Error(19, index, 4); break; //ожидалось что-то из
                    case 5:
                        temp = StackRazbor.Read();
                        if (temp.Type == "S" && temp.Number == 1)
                        {
                            Privedenie(1, N.tip); continue;
                        } //char
                        Error(20, index, 5); break;// ошибка приведения
                    case 6:
                        temp = StackRazbor.Read();
                        if (temp.Type == "S" && temp.Number == 2)
                        {
                            Privedenie(1, N.tip); continue;
                        } //long
                        Error(20, index, 6); break;// ошибка приведения
                    case 7:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Privedenie(1, N.spis_op); continue;
                        } //oper
                        Error(20, index, 7); break;// ошибка приведения
                    case 8:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.opis)
                        { Sdvig(); }//opis
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 4)
                        {
                            Perehod(9); continue;
                        }//;
                        Error(5, index, 8); break; //ожидалась ;
                    case 9:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 4)
                        {
                            Privedenie(2, N.oper); continue;
                        }//;
                        Error(20, index, 9); break;// ошибка приведения
                    case 10:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.oper_decr)
                        { Sdvig(); }
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 4)
                        {
                            Perehod(11); continue;
                        }//;
                        Error(5, index, 10); break; //ожидалась ;
                    case 11:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 4)
                        {
                            Privedenie(2, N.oper); continue;
                        }//;
                        Error(20, index, 11); break; //ошибка приведения
                    case 12:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        { Sdvig(); }
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 4)
                        {
                            // Получаем выражение из токенов
                            string expression = GetExpressionFromTokens();
                            TranslateExpression(expression); // Анализируем выражение
                            Perehod(13); continue;
                        }//;
                        Error(5, index, 12); break; //ожидалась ;
                    case 13:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 4)
                        {
                            Privedenie(2, N.oper); continue;
                        }//;
                        Error(20, index, 13); break; //ошибка приведения
                    case 14:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.usl_op)
                        {
                            Privedenie(1, N.oper); continue;
                        }
                        Error(20, index, 14); break;// ошибка приведения
                    case 15:
                        temp = StackRazbor.Read();
                        if (temp.Type == "I") 
                        {
                            UsingVariable(temp); Sdvig(); 
                        } //id
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 13)
                        {
                            Perehod(16);
                            continue;
                        } //=
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Token id = Tokens[index-2];
                            GenerateDecrement(id, false);
                            Perehod(37); continue;
                        }//--
                        Error(8, index, 15); break;// ожидалось = или --
                    case 16:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 13) 
                        {
                            Token idToken = Tokens[index - 2];
                            string expr = GetExpressionFromTokens();
                            GenerateAssignment(idToken, new Token("", expr, 0));
                            Sdvig(); 
                        } //=
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.vir)
                        {
                            Perehod(35);
                            continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.el)
                        {
                            Perehod(19);
                            continue;
                        }
                        if (temp.Type == "I")
                        {
                            Perehod(17); continue;
                        }
                        if (temp.Type == "L")
                        {
                            Perehod(18); continue;
                        }
                        Error(9, index, 16); break;// ожидались id, lit
                    case 17:
                        temp = StackRazbor.Read();
                        if (temp.Type == "I") { UsingVariable(temp); Privedenie(1, N.el); continue; }//id
                        Error(20, index, 17); break; //ошибка приведения
                    case 18:
                        temp = StackRazbor.Read();
                        if (temp.Type == "L") { Privedenie(1, N.el); continue; } // L
                        Error(20, index, 18); break; //ошибка приведения
                    case 19:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.el)
                        {
                            if (Tokens[index].Value == ";" || Tokens[index].Value == ",")
                            {
                                Privedenie(1, N.vir); continue;
                            }
                        }
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.op)
                        {
                            Perehod(25); continue;
                        }
                        Sdvig();
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 6)
                        {
                            Perehod(20); continue;
                        }// +
                        if (temp.Type == "R" && temp.Number == 12)
                        {
                            Perehod(21); continue;
                        }//-
                        if (temp.Type == "R" && temp.Number == 9)
                        {
                            Perehod(22); continue;
                        }// %
                        if (temp.Type == "R" && temp.Number == 7)
                        {
                            Perehod(23); continue;
                        }// *
                        if (temp.Type == "R" && temp.Number == 8)
                        {
                            Perehod(24); continue;
                        }// /
                        Error(10, index, 19); break;// ожидались , или ; или арифм.оператор
                    case 20:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 6)
                        {
                            Privedenie(1, N.op); continue;
                        }//+
                        Error(20, index, 20); break; //ошибка приведения
                    case 21:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 12)
                        {
                            Privedenie(1, N.op); continue;
                        }//-
                        Error(20, index, 21); break; //ошибка приведения
                    case 22:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 9)
                        {
                            Privedenie(1, N.op); continue;
                        }//%
                        Error(20, index, 22); break; //ошибка приведения
                    case 23:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 7)
                        {
                            Privedenie(1, N.op); continue;
                        }//*
                        Error(20, index, 23); break; //ошибка приведения
                    case 24:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 8)
                        {
                            Privedenie(1, N.op); continue;
                        }// /
                        Error(20, index, 24); break; //ошибка приведения
                    case 25:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.op)
                        {
                            Sdvig();
                        } //op
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.el)
                        {
                            Perehod(26); continue;
                        }// el
                        if (temp.Type == "I") { Perehod(17); continue; }// id
                        if (temp.Type == "L") { Perehod(18); continue; }// lit
                        Error(9, index, 25); break;// id lit
                    case 26:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.el)
                        {
                            Privedenie(3, N.vir); continue;
                        } //el
                        Error(20, index, 26); break; //ошибка приведения
                    case 27:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.tip)
                        {
                            Sdvig();
                        }
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.spis_per)
                        {
                            Perehod(28); continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.zhach)
                        {
                            Perehod(31); continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(32); continue;
                        }
                        if (temp.Type == "I") 
                        {
                            Variable(new Token("", "", temp.Number-1), temp);
                            Perehod(33); continue; 
                        }// id
                        Error(6, index, 27); break;// id
                    case 28:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.spis_per)
                        {
                            if (Tokens[index].Value == ";")
                            {
                                Privedenie(2, N.opis); continue;
                            }
                        }
                        Sdvig();
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 5)
                        {
                            Perehod(29); continue;
                        }// ,
                        Error(21, index, 28); break;// , ;
                    case 29:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 5) { Sdvig(); } //,
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.zhach)
                        {
                            Perehod(30);
                            continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(32);
                            continue;
                        }
                        if (temp.Type == "I")
                        {
                            Variable(new Token("", "", temp.Number-1), temp);
                            Perehod(33); continue;
                        }
                        Error(6, index, 29); break;// ожидался id
                    case 30:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.zhach)
                        {
                            Privedenie(3, N.spis_per); continue;
                        }
                        Error(20, index, 30); break;// ошибка приведения
                    case 31:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.zhach)
                        {
                            Privedenie(1, N.spis_per); continue;
                        }
                        Error(20, index, 31); break;// ошибка приведения
                    case 32:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Privedenie(1, N.zhach); continue;
                        }
                        Error(20, index, 32); break; // ошибка приведения
                    case 33:
                        temp = StackRazbor.Read();
                        if (temp.Type == "I")
                        {
                            if (Tokens[index].Value == ";" || Tokens[index].Value == ",")
                            {
                                Privedenie(1, N.zhach); continue;
                            }
                        }
                        Sdvig();
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 13) //=
                        {
                            Perehod(16); continue;
                        }
                        Error(7, index, 33); break;// = ; ,
                    case 34:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 17) 
                        {
                            Token id = Tokens[index];
                            GenerateDecrement(id, true);
                            Sdvig(); 
                        } //--
                        temp = StackRazbor.Read();
                        if (temp.Type == "I") 
                        { 
                            UsingVariable(temp);
                            Perehod(36); continue; 
                        } //id
                        Error(6, index, 34); break;
                    case 35:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.vir)
                        {
                            Privedenie(3, N.pr_arifm); continue;
                        }
                        Error(20, index, 35); break;// ошибка приведения
                    case 36:
                        temp = StackRazbor.Read();
                        if (temp.Type == "I")
                        {
                            Privedenie(2, N.oper_decr); continue;
                        }
                        Error(20, index, 36); break;// ошибка приведения
                    case 37:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Privedenie(2, N.oper_decr); continue;
                        }
                        Error(20, index, 32); break; // ошибка приведения
                    case 39:
                        temp = StackRazbor.Read();
                        if (temp.Type == "S" && temp.Number == 3) 
                        {
                            string condition = GetConditionFromTokens();
                            GenerateIfCode(new Token("", condition, 0));
                            Sdvig(); 
                        } //if
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 0)
                        {
                            Perehod(40); continue;
                        }//(
                        Error(1, index, 39); break; // ожидалась (
                    case 40:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 0) { Sdvig(); }//(
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.A)
                        {
                            Perehod(41); continue;
                        } //A
                        if (temp.Type == "N" && temp.Number == (int)N.B)
                        {
                            Perehod(44); continue;
                        } //B
                        if (temp.Type == "N" && temp.Number == (int)N.C)
                        {
                            Perehod(47); continue;
                        } //C
                        if (temp.Type == "N" && temp.Number == (int)N.D)
                        {
                            Perehod(59); continue;
                        } //D
                        if (temp.Type == "N" && temp.Number == (int)N.F)
                        {
                            Perehod(56); continue;
                        } //F
                        if (temp.Type == "R" && temp.Number == 14)
                        {
                            Perehod(60);
                            continue;
                        } //!
                        if (temp.Type == "R" && temp.Number == 0)
                        {
                            Perehod(62); continue;
                        } //(
                        if (temp.Type == "I") { Perehod(57); continue; }//id
                        if (temp.Type == "L") { Perehod(58); continue; }// lit
                        Error(11, index, 40); break;// ожидались ! id lit (
                    case 41:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.A)
                        {
                            Sdvig();
                        }// A
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 1)
                        {
                            Perehod(65); continue;
                        } //)
                        if (temp.Type == "R" && temp.Number == 20)
                        {
                            Perehod(42);
                            continue;
                        } //&&
                        Error(12, index, 41); break;// ожидались ) &&
                    case 42:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 20) { Sdvig(); }//&&
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.B)
                        {
                            Perehod(43); continue;
                        } //B
                        if (temp.Type == "N" && temp.Number == (int)N.C)
                        {
                            Perehod(47); continue;
                        } //C
                        if (temp.Type == "N" && temp.Number == (int)N.D)
                        {
                            Perehod(59); continue;
                        } //D
                        if (temp.Type == "N" && temp.Number == (int)N.F)
                        {
                            Perehod(56); continue;
                        } //F
                        if (temp.Type == "R" && temp.Number == 14)
                        {
                            Perehod(60);
                            continue;
                        } //!
                        if (temp.Type == "R" && temp.Number == 0)
                        {
                            Perehod(62); continue;
                        } //(
                        if (temp.Type == "I") { Perehod(57); continue; }//id
                        if (temp.Type == "L") { Perehod(58); continue; }// lit
                        Error(11, index, 42); break;// ожидались ! id lit (
                    case 43:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.B)
                        {
                            if (Tokens[index].Value == ")" || Tokens[index].Value == "&&")
                            {
                                Privedenie(3, N.A); continue;
                            }
                        }
                        Sdvig();
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 21)
                        {
                            Perehod(45);
                            continue;
                        } //||
                        Error(17, index, 43); break; // ) || &&
                    case 44:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.B)
                        {
                            if (Tokens[index].Value == ")" || Tokens[index].Value == "&&")
                            {
                                Privedenie(1, N.A); continue;
                            }
                        }
                        Sdvig();
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 21)
                        {
                            Perehod(45);
                            continue;
                        } //||
                        Error(17, index, 44); break; // ) || &&
                    case 45:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 21) { Sdvig(); }//||
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.C)
                        {
                            // Получаем выражение из токенов
                            string expression = GetExpressionFromTokens();
                            TranslateExpression(expression); // Анализируем выражение
                            Perehod(46); continue;
                        } //C
                        if (temp.Type == "N" && temp.Number == (int)N.D)
                        {
                            Perehod(59); continue;
                        } //D
                        if (temp.Type == "N" && temp.Number == (int)N.F)
                        {
                            Perehod(56); continue;
                        } //F
                        if (temp.Type == "R" && temp.Number == 14)
                        {
                            Perehod(60);
                            continue;
                        } //!
                        if (temp.Type == "R" && temp.Number == 0)
                        {
                            Perehod(62); continue;
                        } //(
                        if (temp.Type == "I") { Perehod(57); continue; }//id
                        if (temp.Type == "L") { Perehod(58); continue; }// lit
                        Error(11, index, 45); break;// ожидались ! id lit (
                    case 46:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.C)
                        {
                            if (Tokens[index].Value == ")" || Tokens[index].Value == "&&" || Tokens[index].Value == "||")
                            {
                                Privedenie(3, N.B); continue;
                            }
                        }
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.op_sr)
                        {
                            Perehod(48); continue;
                        }
                        Sdvig();
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 11)
                        {
                            Perehod(50);
                            continue;
                        } //>
                        if (temp.Type == "R" && temp.Number == 10)
                        {
                            Perehod(51);
                            continue;
                        } //<
                        if (temp.Type == "R" && temp.Number == 16)
                        {
                            Perehod(52);
                            continue;
                        } //>=
                        if (temp.Type == "R" && temp.Number == 15)
                        {
                            Perehod(53);
                            continue;
                        } //<=
                        if (temp.Type == "R" && temp.Number == 18)
                        {
                            Perehod(54);
                            continue;
                        } //==
                        if (temp.Type == "R" && temp.Number == 19)
                        {
                            Perehod(55);
                            continue;
                        } //!=
                        Error(16, index, 46); break;// ) && || оп_ср
                    case 47:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.C)
                        {
                            if (Tokens[index].Value == ")" || Tokens[index].Value == "&&" || Tokens[index].Value == "||")
                            {
                                Privedenie(1, N.B); continue;
                            }
                        }
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.op_sr)
                        {
                            Perehod(48); continue;
                        }
                        Sdvig();
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 11)
                        {
                            Perehod(50);
                            continue;
                        } //>
                        if (temp.Type == "R" && temp.Number == 10)
                        {
                            Perehod(51);
                            continue;
                        } //<
                        if (temp.Type == "R" && temp.Number == 16)
                        {
                            Perehod(52);
                            continue;
                        } //>=
                        if (temp.Type == "R" && temp.Number == 15)
                        {
                            Perehod(53);
                            continue;
                        } //<=
                        if (temp.Type == "R" && temp.Number == 18)
                        {
                            Perehod(54);
                            continue;
                        } //==
                        if (temp.Type == "R" && temp.Number == 19)
                        {
                            Perehod(55);
                            continue;
                        } //!=
                        Error(16, index, 47); break;// ) && || оп_ср
                    case 48:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.op_sr) { Sdvig(); }
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.D)
                        {
                            Perehod(49); continue;
                        } //D
                        if (temp.Type == "N" && temp.Number == (int)N.F)
                        {
                            Perehod(56); continue;
                        } //F
                        if (temp.Type == "R" && temp.Number == 14)
                        {
                            Perehod(60);
                            continue;
                        } //!
                        if (temp.Type == "R" && temp.Number == 0)
                        {
                            Perehod(62); continue;
                        } //(
                        if (temp.Type == "I") { Perehod(57); continue; }//id
                        if (temp.Type == "L") { Perehod(58); continue; }// lit
                        Error(11, index, 48); break;// ожидались ! id lit (
                    case 49:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.D)
                        {
                            Privedenie(3, N.C); continue;
                        }
                        Error(20, index, 49); break;// ошибка приведения
                    case 50:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 11) //>
                        {
                            Privedenie(1, N.op_sr); continue;
                        }
                        Error(20, index, 50); break; //ошибка приведения
                    case 51:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 10) //<
                        {
                            Privedenie(1, N.op_sr); continue;
                        }
                        Error(20, index, 51); break; //ошибка приведения
                    case 52:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 16) //>=
                        {
                            Privedenie(1, N.op_sr); continue;
                        }
                        Error(20, index, 52); break; //ошибка приведения
                    case 53:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 15) //<=
                        {
                            Privedenie(1, N.op_sr); continue;
                        }
                        Error(20, index, 53); break; //ошибка приведения
                    case 54:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 18) //==
                        {
                            Privedenie(1, N.op_sr); continue;
                        }
                        Error(20, index, 54); break; //ошибка приведения
                    case 55:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 19) //!=
                        {
                            Privedenie(1, N.op_sr); continue;
                        }
                        Error(20, index, 55); break; //ошибка приведения
                    case 56:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.F)
                        {
                            Privedenie(1, N.D); continue;
                        }
                        Error(20, index, 56); break;// ошибка приведения
                    case 57:
                        temp = StackRazbor.Read();
                        if (temp.Type == "I")
                        {
                            Privedenie(1, N.F); continue;
                        }
                        Error(20, index, 57); break;// ошибка приведения
                    case 58:
                        temp = StackRazbor.Read();
                        if (temp.Type == "L")
                        {
                            Privedenie(1, N.F); continue;
                        }
                        Error(20, index, 58); break;// ошибка приведения
                    case 59:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.D)
                        {
                            Privedenie(1, N.C); continue;
                        }
                        Error(20, index, 59); break;// ошибка приведения
                    case 60:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 14) { Sdvig(); } //!
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.F)
                        {
                            Perehod(61); continue;
                        } //F
                        if (temp.Type == "R" && temp.Number == 0)
                        {
                            Perehod(62); continue;
                        } //(
                        if (temp.Type == "I") { Perehod(57); continue; }//id
                        if (temp.Type == "L") { Perehod(58); continue; }// lit
                        Error(11, index, 60); break;// ожидались ! id lit (
                    case 61:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.F)
                        {
                            Privedenie(2, N.D); continue;
                        }
                        Error(20, index, 61); break;// ошибка приведения
                    case 62:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 0) { Sdvig(); }//(
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.A)
                        {
                            Perehod(63); continue;
                        } //A
                        if (temp.Type == "N" && temp.Number == (int)N.B)
                        {
                            Perehod(44); continue;
                        } //B
                        if (temp.Type == "N" && temp.Number == (int)N.C)
                        {
                            Perehod(47); continue;
                        } //C
                        if (temp.Type == "N" && temp.Number == (int)N.D)
                        {
                            Perehod(59); continue;
                        } //D
                        if (temp.Type == "N" && temp.Number == (int)N.F)
                        {
                            Perehod(56); continue;
                        } //F
                        if (temp.Type == "R" && temp.Number == 14)
                        {
                            Perehod(60);
                            continue;
                        } //!
                        if (temp.Type == "R" && temp.Number == 0)
                        {
                            Perehod(62); continue;
                        } //(
                        if (temp.Type == "I") { Perehod(57); continue; }//id
                        if (temp.Type == "L") { Perehod(58); continue; }// lit
                        Error(11, index, 62); break;// ожидались ! id lit (
                    case 63:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.A)
                        {
                            Sdvig();
                        }// A
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 1)
                        {
                            Perehod(64); continue;
                        } //)
                        if (temp.Type == "R" && temp.Number == 20)
                        {
                            Perehod(42);
                            continue;
                        } //&&
                        Error(12, index, 63); break; // ) &&
                    case 64:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 1) //)
                        {
                            Privedenie(3, N.F); continue;
                        }
                        Error(20, index, 64); break;// ошибка приведения
                    case 65:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 1) { Sdvig(); }//)
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.block)
                        {
                            Perehod(70); continue;
                        }
                        if (temp.Type == "R" && temp.Number == 2) //{
                        {
                            Perehod(66); continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Perehod(67); continue;
                        }//oper
                        if (temp.Type == "N" && temp.Number == (int)N.opis)
                        {
                            Perehod(8); continue;
                        }//opis
                        if (temp.Type == "N" && temp.Number == (int)N.oper_decr)
                        {
                            Perehod(10); continue;
                        }//oper_decr
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(12); continue;
                        }//pr_arifm
                        if (temp.Type == "N" && temp.Number == (int)N.usl_op)
                        {
                            Perehod(14); continue;
                        }//usl_op
                        if (temp.Type == "N" && temp.Number == (int)N.tip)
                        {
                            Perehod(27); continue;
                        }//tip
                        if (temp.Type == "S" && temp.Number == 1)
                        {
                            Perehod(5); continue;
                        } //char
                        if (temp.Type == "S" && temp.Number == 2)
                        {
                            Perehod(6); continue;
                        } //long
                        if (temp.Type == "I") { Perehod(15); continue; }//id
                        if (temp.Type == "S" && temp.Number == 3) { Perehod(39); continue; }//if
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Perehod(34); continue;
                        } //--
                        Error(19, index, 65); break; //ожидалось что-то из
                    case 66:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 2) { Sdvig(); } //{
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.spis_op)
                        {
                            Perehod(68); continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Perehod(7); continue;
                        }//oper
                        if (temp.Type == "N" && temp.Number == (int)N.opis)
                        {
                            Perehod(8); continue;
                        }//opis
                        if (temp.Type == "N" && temp.Number == (int)N.oper_decr)
                        {
                            Perehod(10); continue;
                        }//oper_decr
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(12); continue;
                        }//pr_arifm
                        if (temp.Type == "N" && temp.Number == (int)N.usl_op)
                        {
                            Perehod(14); continue;
                        }//usl_op
                        if (temp.Type == "N" && temp.Number == (int)N.tip)
                        {
                            Perehod(27); continue;
                        }//tip
                        if (temp.Type == "S" && temp.Number == 1)
                        {
                            Perehod(5); continue;
                        } //char
                        if (temp.Type == "S" && temp.Number == 2)
                        {
                            Perehod(6); continue;
                        } //long
                        if (temp.Type == "I") { Perehod(15); continue; }//id
                        if (temp.Type == "S" && temp.Number == 3) { Perehod(39); continue; }//if
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Perehod(34); continue;
                        } //--
                        Error(19, index, 66); break; //ожидалось что-то из
                    case 67:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Privedenie(1, N.block); continue;
                        }
                        Error(20, index, 67); break;// ошибка приведения
                    case 68:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.spis_op) { Sdvig(); }
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 3) //}
                        {
                            Perehod(69); continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Perehod(73); continue;
                        }//oper
                        if (temp.Type == "N" && temp.Number == (int)N.opis)
                        {
                            Perehod(8); continue;
                        }//opis
                        if (temp.Type == "N" && temp.Number == (int)N.oper_decr)
                        {
                            Perehod(10); continue;
                        }//oper_decr
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(12); continue;
                        }//pr_arifm
                        if (temp.Type == "N" && temp.Number == (int)N.usl_op)
                        {
                            Perehod(14); continue;
                        }//usl_op
                        if (temp.Type == "N" && temp.Number == (int)N.tip)
                        {
                            Perehod(27); continue;
                        }//tip
                        if (temp.Type == "S" && temp.Number == 1)
                        {
                            Perehod(5); continue;
                        } //char
                        if (temp.Type == "S" && temp.Number == 2)
                        {
                            Perehod(6); continue;
                        } //long
                        if (temp.Type == "I") { Perehod(15); continue; }//id
                        if (temp.Type == "S" && temp.Number == 3) { Perehod(39); continue; }//if
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Perehod(34); continue;
                        } //--
                        Error(19, index, 68); break; //ожидалось что-то из
                    case 69:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 3) //}
                        {
                            Privedenie(3, N.block); continue;
                        }
                        Error(20, index, 69); break;// ошибка приведения
                    case 70:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.block) { Sdvig(); }
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.hvost)
                        {
                            Perehod(79); continue;
                        }
                        if (temp.Type == "S" && temp.Number == 4)
                        {
                            GenerateelseCode();
                            Perehod(71); continue;
                        }//else
                        if (StackRazbor.isEmpty()) { Perehod(74); continue; }
                        Error(22, index, 70); break;
                    case 71:
                        temp = StackRazbor.Read();
                        if (temp.Type == "S" && temp.Number == 4) { Sdvig(); } //else
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 2) //{
                        {
                            Perehod(72); continue;
                        }
                        Error(3, index, 71); break;
                    case 72:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 2) { Sdvig(); } //{
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.spis_op)
                        {
                            Perehod(75); continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Perehod(7); continue;
                        }//oper
                        if (temp.Type == "N" && temp.Number == (int)N.opis)
                        {
                            Perehod(8); continue;
                        }//opis
                        if (temp.Type == "N" && temp.Number == (int)N.oper_decr)
                        {
                            Perehod(10); continue;
                        }//oper_decr
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(12); continue;
                        }//pr_arifm
                        if (temp.Type == "N" && temp.Number == (int)N.usl_op)
                        {
                            Perehod(14); continue;
                        }//usl_op
                        if (temp.Type == "N" && temp.Number == (int)N.tip)
                        {
                            Perehod(27); continue;
                        }//tip
                        if (temp.Type == "S" && temp.Number == 1)
                        {
                            Perehod(5); continue;
                        } //char
                        if (temp.Type == "S" && temp.Number == 2)
                        {
                            Perehod(6); continue;
                        } //long
                        if (temp.Type == "I") { Perehod(15); continue; }//id
                        if (temp.Type == "S" && temp.Number == 3) { Perehod(39); continue; }//if
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Perehod(34); continue;
                        } //--
                        Error(19, index, 72); break; //ожидалось что-то из
                    case 73:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.oper) 
                        {
                            Privedenie(2, N.spis_op); continue;
                        }
                        Error(20, index, 73); break;// ошибка приведения
                    case 74:
                        temp = StackRazbor.Read();
                        if (StackRazbor.isEmpty())
                        {
                            Privedenie(1, N.hvost); continue;
                        }
                        Error(20, index, 74); break;// ошибка приведения
                    case 75:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.spis_op) { Sdvig(); }
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 3) //}
                        {
                            Perehod(77); continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Perehod(73); continue;
                        }//oper
                        if (temp.Type == "N" && temp.Number == (int)N.opis)
                        {
                            Perehod(8); continue;
                        }//opis
                        if (temp.Type == "N" && temp.Number == (int)N.oper_decr)
                        {
                            Perehod(10); continue;
                        }//oper_decr
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(12); continue;
                        }//pr_arifm
                        if (temp.Type == "N" && temp.Number == (int)N.usl_op)
                        {
                            Perehod(14); continue;
                        }//usl_op
                        if (temp.Type == "N" && temp.Number == (int)N.tip)
                        {
                            Perehod(27); continue;
                        }//tip
                        if (temp.Type == "S" && temp.Number == 1)
                        {
                            Perehod(5); continue;
                        } //char
                        if (temp.Type == "S" && temp.Number == 2)
                        {
                            Perehod(6); continue;
                        } //long
                        if (temp.Type == "I") { Perehod(15); continue; }//id
                        if (temp.Type == "S" && temp.Number == 3) { Perehod(39); continue; }//if
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Perehod(34); continue;
                        } //--
                        Error(19, index, 75); break; //ожидалось что-то из
                    case 77:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 3) //}
                        {
                            Privedenie(4, N.hvost); continue;
                        }
                        Error(20, index, 77); break;// ошибка приведения
                    case 79:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.hvost)
                        {
                            listBox4.Items.Add("Конец условного оператора");
                            Privedenie(6, N.usl_op); continue;
                        }
                        Error(20, index, 79); break;// ошибка приведения
                    case 80:
                        temp = StackRazbor.Read();
                        if (temp.Type == "N" && temp.Number == (int)N.spis_op) { Sdvig(); }
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 3) //}
                        {
                            Perehod(81); continue;
                        }
                        if (temp.Type == "N" && temp.Number == (int)N.oper)
                        {
                            Perehod(73); continue;
                        }//oper
                        if (temp.Type == "N" && temp.Number == (int)N.opis)
                        {
                            Perehod(8); continue;
                        }//opis
                        if (temp.Type == "N" && temp.Number == (int)N.oper_decr)
                        {
                            Perehod(10); continue;
                        }//oper_decr
                        if (temp.Type == "N" && temp.Number == (int)N.pr_arifm)
                        {
                            Perehod(12); continue;
                        }//pr_arifm
                        if (temp.Type == "N" && temp.Number == (int)N.usl_op)
                        {
                            Perehod(14); continue;
                        }//usl_op
                        if (temp.Type == "N" && temp.Number == (int)N.tip)
                        {
                            Perehod(27); continue;
                        }//tip
                        if (temp.Type == "S" && temp.Number == 1)
                        {
                            Perehod(5); continue;
                        } //char
                        if (temp.Type == "S" && temp.Number == 2)
                        {
                            Perehod(6); continue;
                        } //long
                        if (temp.Type == "I") { Perehod(15); continue; }//id
                        if (temp.Type == "S" && temp.Number == 3) { Perehod(39); continue; }//if
                        if (temp.Type == "R" && temp.Number == 17)
                        {
                            Perehod(34); continue;
                        } //--
                        Error(19, index, 80); break; //ожидалось что-то из
                    case 81:
                        temp = StackRazbor.Read();
                        if (temp.Type == "R" && temp.Number == 3) //}
                        {
                            listBox4.Items.Add("Конец программы");
                            Privedenie(6, N.prog); continue;
                        }
                        Error(20, index, 81); break;// ошибка приведения
                }
                break;
            }
            if (!SemanticError && !SintaxError)
            {
                Error2(-1, index);
            }
            return -1;
        }
    }
}
