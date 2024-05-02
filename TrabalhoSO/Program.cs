using System;
using System.Collections.Generic;
using System.Threading;

namespace TrabalhoSO
{
    public class Ambiente // classe do ambiente
    {
        public static int tempo_limite; // tempo limite (informada pelo usuario)
        public static int num_portas; // numero de portas disponiveis (informada pelo usuario)
        public static int cont_mov_portas; // numero de pessoas querendo sair pelas portas
        public static int ambiente_dim; // dimensao do ambiente (informada pelo usuario)
        public static int num_pessoas; // numero de pessoas (informada pelo usuario)
        public static int[,] ambiente; // matriz ambiente
        public static List<int> lista_espera; // lista de espera das pessoas

        public static void InicializarAmbiente()
        {
            ambiente = new int[ambiente_dim, ambiente_dim]; //inicializa o ambiente com as dimensoes passadas pelo usuario
            if (num_pessoas > num_portas)
                lista_espera = new List<int>(); // caso existam mais pessoas que portas, ira criar uma lista de espera
        }

        public static bool VerificarPosicaoValida(int x, int y) // verifica se a posicao existe no ambiente e se nao esta sendo ocupada
        {                                                       // por outra pessoa
            return (x >= 0 && x < ambiente_dim) && (y >= 0 && y < ambiente_dim) && ambiente[x, y] == 0;
        }

        public static bool LiberarPorta(int id) // controlar quais pessoas irao sair pelas portas em ordem de chegada
        {
            if (cont_mov_portas > num_portas) //se a quantidade de pessoas querendo sair pelas portas for maior que a
            {                                 //quantidade de portas, a pessoa ira esperar
                for (int i = 0; i < num_portas; i++) // se existir 5 portas, as 5 primeiras pessoas irao sair
                {
                    if (id == lista_espera[i]) // se o id for o primeiro na lista de espera, a pessoa ira sair
                        return false;                        
                }
                return true; // caso nao seja, ira continuar esperando
            }
            else
                return false; // quando a quantidade de pessoas querendo sair for menor que a quantidade de portas,a pessoa sai direto
        }

        public static void ImprimirAmbiente() //imprime o ambiente
        {
            for (int i = 0; i < ambiente.GetLength(0); i++)
            {
                for (int j = 0; j < ambiente.GetLength(1); j++)
                {
                    Console.Write($"{ambiente[i, j]}  ");
                }
                Console.WriteLine();
            }
        }
    }

    public class Threads // classe das threads
    {
        static Random rnd = new Random();
        public static void ThreadPessoa(int id) // define o comportamento de cada pessoa
        {
            int x = rnd.Next(0, Ambiente.ambiente_dim);
            int y = rnd.Next(0, Ambiente.ambiente_dim); // inicializa a pessoa com uma posicao aleatoria dentro do ambiente

            while (!Ambiente.VerificarPosicaoValida(x, y)) // se a posicao inicial for invalida, a posicao sera trocada ate ser valida
            {
                x = rnd.Next(0, Ambiente.ambiente_dim);
                y = rnd.Next(0, Ambiente.ambiente_dim);
            }

            Ambiente.ambiente[x, y] = id; // marca a posicao da pessoa no ambiente com o id dela

            for (int i = 0; i < Ambiente.tempo_limite; i++) // pessoa se movimentando durante o tempo limite definido pelo usuario
            {
                Thread.Sleep(1); // simula o tempo passando
                int mov_x = rnd.Next(-1, 2); // proximo movimento da pessoa, se ela andou 1 casa pra frente ou para tras ou nao andou no
                int mov_y = rnd.Next(-1, 2); // eixo x e y

                int nova_pos_x = x + mov_x; // define a nova posicao da pessoa
                int nova_pos_y = y + mov_y;

                if (Ambiente.VerificarPosicaoValida(nova_pos_x, nova_pos_y)) // verifica se a nova posicao da pessoa e valida
                {
                    Ambiente.ambiente[x, y] = 0; // libera a posicao atual
                    x = nova_pos_x;
                    y = nova_pos_y;
                    Ambiente.ambiente[x, y] = id; // marca a nova posicao da pessoa no ambiente com o id dela
                    Console.WriteLine($"Pessoa {id} moveu para ({x}, {y})");
                }
            }

            if (Ambiente.num_pessoas > Ambiente.num_portas) // se existir mais pessoas que portas, as pessoas irao verificar se as portas
            {                                               // estao liberadas
                Ambiente.cont_mov_portas++; // incrementa o contador de quantidade de pessoas querendo sair
                Ambiente.lista_espera.Add(id); // adiciona o id da pessoa a lista de espera

                while (Ambiente.LiberarPorta(id)) // se a quantidade de pessoas querendo sair pelas portas for maior que a
                {                                 // quantidade de portas, a pessoa ira esperar
                    Console.WriteLine($"Pessoa {id} esperando porta livre para sair");
                    Thread.Sleep(5);
                }

                Console.WriteLine($"Pessoa {id} saiu pela porta"); // quando uma porta for liberada, a pessoa ira sair
                Ambiente.cont_mov_portas--; // a pessoa saiu pela porta e outra pode utilizar a mesma porta 
                Ambiente.lista_espera.Remove(id); // quando a pessoa sair pela porta, ira sair da lista de espera
            }
            else
            {// caso exista mais portas que pessoas, as pessoas saem direto pelas portas, sem esperar
                Console.WriteLine($"Pessoa {id} saiu pela porta");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            List<Thread> thread_pessoas = new List<Thread>(); // cria uma lista de threads para armazenar as threads criadas
            // usuario digita os parametros
            Console.Write("Digite o numero de pessoas: ");
            Ambiente.num_pessoas = Convert.ToInt32(Console.ReadLine());
            Console.Write("Digite o numero de portas: ");
            Ambiente.num_portas = Convert.ToInt32(Console.ReadLine());
            Console.Write("Digite o tamanho do ambiente: ");
            Ambiente.ambiente_dim = Convert.ToInt32(Console.ReadLine());
            Console.Write("Digite o tempo limite: ");
            Ambiente.tempo_limite = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            Ambiente.InicializarAmbiente(); //inicializa o ambiente

            for (int i = 0; i < Ambiente.num_pessoas; i++) // criacao das pessoas
            {
                Thread pessoa = new Thread(() => Threads.ThreadPessoa(i)); // passando o i como id para as threads
                thread_pessoas.Add(pessoa); // inserindo a nova thread na lista de threads
                pessoa.Start(); // iniciando a thread pessoa
            }

            foreach (var thread in thread_pessoas) // aguarda todas as threads das pessoas terminarem
            {
                thread.Join();
            }

            Console.ReadKey();
        }
    }
}