
using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RSA
{
    class Program
    {

        // Нахождение числа d.
        public static BigInteger GenerateD(BigInteger e, BigInteger phi)
        {
            BigInteger d = new BigInteger();

            for (BigInteger i = 1; i < phi; i = BigInteger.Add(i, 1))
            {
                d = BigInteger.DivRem(BigInteger.Add(BigInteger.Multiply(i, phi), 1), e, out BigInteger remainder);
                if (remainder == 0)
                {
                    break;
                }
            }
            return d;
        }
        
        // Китайская теорема об остатках.
        public static BigInteger ChineseTheoryDecryption(BigInteger d, BigInteger q, BigInteger p, BigInteger cryptedM){
            BigInteger mp = BigInteger.ModPow(cryptedM, d, p);
            BigInteger mq = BigInteger.ModPow(cryptedM, d, q);
            BigInteger M0 = p * q;
            BigInteger M1 = p;
            BigInteger M2 = q;

            int RM1 = (int)M1;
            int RRM1 = Int32.Parse(M1.ToString());
            int RM2 = (int)M2;
            int RRM2 = Int32.Parse(M2.ToString());
            int Rmp = (int)mp;
            int RRmp = Int32.Parse(mp.ToString());
            int Rmq = (int)mq;
            int RRmq = Int32.Parse(mq.ToString());
            

            // ax = b (mod n)
            BigInteger y1 = 0;
            BigInteger y2 = 0;
            BigInteger y3 = 0;
            ModularEquatation solver1 = new ModularEquatation(RRM1, RRmq, RRM2);

            ModularEquatation solver2 = new ModularEquatation(RRM2, RRmp, RRM1);
            
                foreach (var solution1 in solver1.GetSolutions()){
                    if (solution1.Value != 0){
                    Console.WriteLine("Китайская теорема об остатках: ");
                    Console.WriteLine("Первое уравнение: " + solution1);
                    y1 = solution1.Value;
                    Console.WriteLine("y1: " + y1);
                    break;
                    }
                }

                foreach (var solution2 in solver2.GetSolutions()){
                    if (solution2.Value != 0){
                    Console.WriteLine("Второе уравнение: " + solution2);
                    y2 = solution2.Value;
                    Console.WriteLine("y2: " + y2);
                    break;
                    }
                }

          
            BigInteger x1 = M1*y1 + (M2*y2) % M0;
            BigInteger x2 = x1 % M0;     
            return x2;
        }



        static void Main(string[] args)
        {
            BigInteger p;
            BigInteger q;

            //Взаимно простые числа — целые числа, не имеющие никаких общих делителей, кроме ±1. 
            //Равносильное определение: целые числа взаимно просты, если их наибольший общий делитель (НОД) равен 1.
            // Проверка phi(q) и phi(p) на взаимную простоту.

                while(true){
                    BigInteger a = 0;
                    Console.Write("Введите число p: ");
                    p = BigInteger.Parse(Console.ReadLine());
                    for (int i = 1; i<=p; i++){
                        if (p % i == 0){
                            a++;
                        }
                    }
                    if(a == 2){
                    break;
                    } 
                }
                
                while(true){ 
                    BigInteger b = 0;
                    Console.Write("Введите число q: ");
                    q = BigInteger.Parse(Console.ReadLine());
                    for (int i = 1; i<=q; i++){
                        if (q % i == 0){
                                b++;
                            }
                        }
                    if(b == 2){
                    break;
                    }
                }
                Console.WriteLine("q = " + q.ToString());
                Console.WriteLine("p = " + p.ToString());

            // Считаем число n.
            BigInteger n = p * q;
            Console.WriteLine("n = " + n.ToString());

            // Считаем число phi.
            BigInteger phi = (p - 1) * (q - 1);
            Console.WriteLine("phi = " + phi.ToString());

            // Проверка e на взаимную простоту с phi.
            BigInteger e;
            while (true)
            {
                Console.Write("Введите число e (1 < e < phi и взаимно простое с phi): ");
                e = BigInteger.Parse(Console.ReadLine());
                Console.WriteLine();
                if (BigInteger.GreatestCommonDivisor(e, phi) == BigInteger.One && e.ToString().Length > 2)
                {
                    break;
                }
            }
            Console.WriteLine("e = " + e.ToString());

            // Вычисляем число d.
            BigInteger d = GenerateD(e, phi);

            string openKeyText = $"Открытый ключ: (e = {e}, n = {n})";
            Console.WriteLine(openKeyText);
            string secretKeyText = $"Закрытый ключ: (d = {d}, n = {n})";
            Console.WriteLine(secretKeyText);

            // Шифруем и дешифруем сообщение.
            Console.Write("Введите число m (< n - 1), которое необходимо зашифровать: ");
            BigInteger m = BigInteger.Parse(Console.ReadLine());
            BigInteger cryptedM = BigInteger.ModPow(m, e, n);
            BigInteger decryptedM = BigInteger.ModPow(cryptedM, d, n);
            Console.WriteLine($"Зашифрованное сообщение: {cryptedM}");
            Console.WriteLine($"Расшифрованное сообщение: {decryptedM}");

            // Записываем ключи в файлы.
            using (StreamWriter fs = new StreamWriter("openKey.txt"))
            {
                fs.WriteLine(openKeyText);
            }
            using (StreamWriter fs = new StreamWriter("secretKey.txt"))
            {
                fs.WriteLine(secretKeyText);
            }


            // Использование китайской теорему об остатках для ускорения процесса дешифрования RSA.
            Console.WriteLine("Расшифрованное сообщение с использованием китайской теоремы об остатках = " +  ChineseTheoryDecryption(d,q,p,cryptedM));
            
            // Использование алгоритма быстрого возведения в степень для ускорения процесса шифрования RSA;
            static BigInteger GetPower(BigInteger value, BigInteger pow){
                BigInteger result = 1;
                while (pow > 0){
                    if(pow % 2 == 1)
                    result *= value;
                    value *= value;
                    pow /= 2;
                }
                return result;
            }
            BigInteger cryptedM_PowerAlgorithm = GetPower(m,e) % n;

            Console.WriteLine("Зашифрованное сообщение с помощью алгоритма быстрого возведения в степень = " + cryptedM_PowerAlgorithm);
    
        }
    }
}