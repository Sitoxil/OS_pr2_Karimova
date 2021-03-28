using System;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;


namespace prak2
{
  class Program
  {
    
    static int count = 0;
    static int Menu()
    {
      Console.WriteLine
      (
        "1. Ввести хэш-значения вручную.\n" +
        "2. Считать хэш-значения из файла."
      );
      string s;
      s = Console.ReadLine();
      while ((s != "2") && (s != "1"))
      {
        Console.WriteLine("Неверно! Повторите ввод:");
        s = Console.ReadLine();
      }
      return Int32.Parse(s);
    }

    static string[] SHA_read()
    {
      switch (Menu())
      {
        case 1:
          {
            string[] SHA = { "", "", "" };
            for (int i = 0; i < 3; i++)
            {
              Console.WriteLine($"Введите {i + 1} хэш-значение:");
              SHA[i] = Console.ReadLine();
            }
            return SHA;
          }
        case 2:
          {
            string name = "SHA.txt";
            if (!(File.Exists(name)))
            {
              FileStream fstream = new FileStream(name, FileMode.Create);
              string[] SHA_file = { "1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad", Environment.NewLine, "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b", Environment.NewLine, "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f" };
              foreach (string text in SHA_file)
              {
                byte[] array_file = System.Text.Encoding.Default.GetBytes(text);
                fstream.Write(array_file, 0, array_file.Length);
              }
              fstream.Close();
            }
            FileStream fs = new FileStream(name, FileMode.Open);
            string[] SHA = { "", "", "" };
            string copy;
            byte[] array = new byte[66 * 3];
            fs.Read(array, 0, array.Length);
            copy = System.Text.Encoding.Default.GetString(array);
            for (int i = 0; i < 3; i++)
            {
              SHA[i] = copy.Substring(66 * i, 64);
            }
            return SHA;
          }
        default:
          {
            string[] SHA = { "" };
            Console.WriteLine("Ошибка!");
            return SHA;
          }
      }
    }

    public static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
      byte[] data = hashAlgorithm.ComputeHash(System.Text.Encoding.ASCII.GetBytes(input));
      var sBuilder = new StringBuilder();
      for (int i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }
      return sBuilder.ToString();
    }

    public static void GetPassword(object obj)
    {
      Stopwatch timer = new Stopwatch();
      timer.Start();
      ThreadInfo inf_tread = (ThreadInfo)obj;
      string[] SHA_all = inf_tread.SHA_all;
      SHA256 SHA = SHA256.Create();
      string hash, password;
      string[] passwords_all = { "", "", "" }; /////////////////////////////////////////////
      char[] symbols = new char[5];
      for (int i = 0; i < inf_tread.elements; i++)
      {
        for (int j = 97; (j <= 122); j++)
        {
          for (int k = 97; (k <= 122); k++)
          {
            for (int l = 97; (l <= 122); l++)
            {
              for (int m = 97; (m <= 122); m++)
              {
                symbols[0] = Convert.ToChar(97 + inf_tread.number + i * count);
                symbols[1] = Convert.ToChar(j);
                symbols[2] = Convert.ToChar(k);
                symbols[3] = Convert.ToChar(l);
                symbols[4] = Convert.ToChar(m);
                password = new string(symbols);
                hash = GetHash(SHA, password);
                for (int h = 0; h < 3; h++)
                {
                  if (SHA_all[h] == hash)
                  {
                    passwords_all[h] = password;
                    Console.WriteLine(password + "   Время: " + timer.ElapsedMilliseconds + " мс");
                  }
                }
                Thread.Sleep(0);
              }
            }
          }
        }
      }
    }

    public class ThreadInfo
    {
      public int number;
      public int elements;
      public string[] SHA_all;
      public ThreadInfo() { }
    }

    static void Main()
    {
      List<Thread> threads = new List<Thread>();
      List<ThreadInfo> thrdinf = new List<ThreadInfo>();
      string[] SHA_all;
      SHA_all = SHA_read();
      Console.WriteLine("Введите количество потоков(до 26):");
      count = Int32.Parse(Console.ReadLine());
      int key1 = 26 / count, key2 = 26 % count;
      for (int a = 0; a < count; a++)
      {
        threads.Add(new Thread(new ParameterizedThreadStart(GetPassword)));
        thrdinf.Add(new ThreadInfo());
        thrdinf[a].SHA_all = SHA_all;
        thrdinf[a].number = a;
        if (key2 > 0)
        {
          thrdinf[a].elements = key1 + 1;
          key2--;
        }
        else { thrdinf[a].elements = key1;}
        threads[a].Start(thrdinf[a]);
      }
    }
  }
}