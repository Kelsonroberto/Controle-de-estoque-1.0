using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Produto
{
    class Program
    {
        //Serializable faz a conversão de objetos para bytes para assim ser gravado na mémoria.
        [Serializable]
        struct dados
        {
            public string descricao;
            public decimal valor;
            public int estoque;
            public int EstMinimo;
            public int EstMaximo;
        }
        static void Main(string[] args)
        {
            dados[] prod = new dados[100];
            int opc;
            int quantidade = 0;

            LerDadosArquivo(prod, ref quantidade);
            do
            {
                opc = menu();
                switch (opc)
                {
                    case 1:
                        cadastrar(prod, ref quantidade);
                        break;
                    case 2:
                        vender(prod, ref quantidade);
                        break;
                    case 3:
                        inserir(prod, ref quantidade);
                        break;
                    case 4:
                        listar(prod, quantidade);
                        break;
                    case 5:
                        sugestao(prod, quantidade);
                        break;
                }
            } while (opc != 0);
            //No momento que o usuário escolhe a opção sair, todas modificações e inserções feitas serão gravadas.
            gravarDadosVetor(prod, quantidade);

            static int menu()
            {
                Console.Clear();
                Console.WriteLine("Seja bem-vindo ao Cadastro\n");
                Console.WriteLine("1 - Cadastrar");
                Console.WriteLine("2 - Vender");
                Console.WriteLine("3 - Inserir produtos");
                Console.WriteLine("4 - Listar Produtos");
                Console.WriteLine("5 - Sugestão");
                Console.WriteLine("0 - Sair");
                Console.Write("Opção: ");
                int num = int.Parse(Console.ReadLine());
                return num;
            }
        }

        static void cadastrar(dados[] prod, ref int quant)
        {
            Console.WriteLine("Digite a descrição do produto: ");
            prod[quant].descricao = Console.ReadLine().ToUpper();
            while(prod[quant].descricao == "")
            {
                Console.WriteLine("Por favor digite uma descrição para o produto: ");
                prod[quant].descricao = Console.ReadLine().ToUpper();

            }

            Console.WriteLine("Digite o valor do produto: ");
            prod[quant].valor = decimal.Parse(Console.ReadLine());            

            Console.WriteLine("Digite a quantidade de estoque mínimo: ");
            prod[quant].EstMinimo = int.Parse(Console.ReadLine());

            Console.WriteLine("Digite a quantidade de estoque máximo: ");
            prod[quant].EstMaximo = int.Parse(Console.ReadLine());
            while (prod[quant].EstMaximo <= prod[quant].EstMinimo)
            {
                Console.WriteLine("ATENÇÃO, O estoque máximo não pode ser menor ou igual ao estoque mínimo!\n digite a quantidade novamente: ");
                prod[quant].estoque = int.Parse(Console.ReadLine());
            }

            Console.WriteLine("Digite a quantidade em estoque: ");
            prod[quant].estoque = int.Parse(Console.ReadLine());
            while(prod[quant].estoque > prod[quant].EstMaximo)
            {
                Console.WriteLine("ATENÇÃO, você digitou uma quantidade de estoque superior a quantidade máxima definida!\n digite a quantidade novamente: ");
                prod[quant].estoque = int.Parse(Console.ReadLine());                
            }
            quant++;
        }
        
        static void vender(dados[] prod, ref int quant)
        {

            int qtdVender;
            Console.WriteLine("Digite a descrição do produto que deseja vender: \n");
            string desc = Convert.ToString(Console.ReadLine().ToUpper());            
            int b = busca_bin(prod, desc, quant);            
                if (b != -1)
                {
                    
                    Console.WriteLine("Digite a quantidade desse produto que deseja vender: ");
                    qtdVender = int.Parse(Console.ReadLine());

                    if (qtdVender > prod[b].estoque)
                    {
                        Console.WriteLine("Não há estoque suficiente para efetuar essa quantidade de venda!");                        
                        Console.ReadKey();
                        
                    }
                    else
                    {
                        prod[b].estoque -= qtdVender;
                        Console.ReadKey();
                        
                    }

                }
                else
                {
                    Console.WriteLine("A descrição informada não correspode a nenhum item. Tente novamente!");
                    Console.ReadKey();
                    
                }           
        }

        static void inserir(dados[] prod, ref int quant)
        {
            int qtdInserir;
            Console.WriteLine("Digite a descrição do produto que deseja inserir: \n");
            string desc = Convert.ToString(Console.ReadLine().ToUpper());
            int b = busca_bin(prod, desc, quant);
            
                if (b != -1)
                {

                    Console.WriteLine("Digite a quantidade desse produto que deseja complementar no estoque: ");
                    qtdInserir = int.Parse(Console.ReadLine());

                    if (qtdInserir + prod[b].estoque > prod[b].EstMaximo)
                    {
                        Console.WriteLine("Não é possível inserir uma quantidade, que somada ao estoque atual, seja superior ao estoque máximo!");
                        Console.ReadKey();
                        
                    }
                    else
                    {
                        prod[b].estoque += qtdInserir;
                        Console.ReadKey();                        
                    }
                }
                else
                {
                    Console.WriteLine("A descrição informada não correspode a nenhum item. Tente novamente!");
                    Console.ReadKey();

                }            
        }
        static void listar(dados[] prod, int quant)
        {
            Console.WriteLine("Os produtos existentes são: \n");
            Console.WriteLine("************************************************");
            InsertionSort(prod, quant);
            int num = 1;
            for (int i = 0; i < quant; i++)
            {
                //listagem com o valor do produto convertido para o modelo de moeda do Brasil.
                Console.WriteLine("Produto {0} - descrição: {1} com valor de {2} com quantidade em estoque: {3}, estoque mínimo: {4} e estoque máximo: {5}",
                num, prod[i].descricao, prod[i].valor.ToString("C"), prod[i].estoque, prod[i].EstMinimo, prod[i].EstMaximo);
                Console.WriteLine("************************************************");
                num++;
            }
            Console.ReadKey();
        }

        static void sugestao(dados[] prod, int quant)
        {
            StreamWriter ext = new StreamWriter("Extrato.txt", false);
            ext.WriteLine("Extrato de sugestão");
            ext.WriteLine("produtos com estoque menor que o valor mínimo: ");
            InsertionSort(prod, quant);
            for (int i = 0; i < quant; i++)
            {
                if (prod[i].estoque < prod[i].EstMinimo)
                {
                    Console.WriteLine("Os seguintes produtos estão abaixo do estoque mínimo: ");
                    ext.WriteLine(prod[i].descricao);
                    Console.Write(prod[i].descricao);                    
                }
                else
                {
                    Console.WriteLine("Não há sugestão!");
                    break;
                }

            }
            ext.Close();
            Console.ReadKey();
        }
        static void gravarDadosVetor(dados[] prod, int quant)
        {
            IFormatter formatter = new BinaryFormatter(); //Permite operação binária
            Stream wr = new FileStream("lancamentos.bin", FileMode.Create, FileAccess.Write);
            //Cria o arquivo e abre um fluxo
            for (int i = 0; i < quant; i++)
            {
                formatter.Serialize(wr, prod[i]); //grava os elementos do vetor p serializados
                                                  // no arquivo wr
            }
            wr.Close();
        }


        static int busca_bin(dados[] prod, string chave, int tam)
        {
            int inf = 0, sup = tam -1, pos = -1, meio;
            while (inf <= sup)
            {
                meio = (inf + sup) / 2;
                if (prod[meio].descricao.Equals(chave))
                {
                    pos = meio;
                    inf = sup + 1;
                }
                else if (prod[meio].descricao.CompareTo(chave) > 0) // < chave)
                    inf = meio + 1;
                else sup = meio - 1;
            }
            return pos;
        }

        static void InsertionSort(dados[] p, int quant)
        {
            //Deixar os produtos em ordem alfabética.
            dados chave;
            int i, j;
            for (j = 1; j < quant; j++)
            {
                chave = p[j];
                i = j - 1;
                while ((i >= 0) && (p[i].descricao.CompareTo(chave.descricao) > 0))
                {
                    p[i + 1] = p[i];
                    i -= 1;
                }
                p[i + 1] = chave;
            }
        }
        static void LerDadosArquivo(dados[] prod, ref int quant)
        {
            IFormatter formatter = new BinaryFormatter();
            if (File.Exists("lancamentos.bin"))
            {
                Stream rd = new FileStream("lancamentos.bin", FileMode.Open, FileAccess.Read);
                dados produtos;
                try
                {
                    while (true)
                    {
                        produtos = (dados)formatter.Deserialize(rd);
                        prod[quant] = produtos;
                        quant++;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Dados transferidos!\n");
                }
                rd.Close();
            }
        }
    }
}