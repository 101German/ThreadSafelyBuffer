using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Writers_readers
{



    class ThreadSafelyBuffer<T>
    {
        public List<T> buffer;

     

        private int _readers_count = 0;
        private int _writers_count = 0;
        private bool _writers_working = false;
        

        public ThreadSafelyBuffer(List<T> buffer)
        {
            this.buffer = buffer;
        }

        public void Read(object index)
        {
            Monitor.Enter(buffer);
            while (_writers_count > 0) 
            Monitor.Wait(buffer);

            _readers_count++;


            ///////read
            Console.WriteLine($"read data at buffer [{index}]");


            _readers_count--;

            if (_readers_count == 0)
                Monitor.PulseAll(buffer);

            Monitor.Exit(buffer);


        }

        public void Write(object value)
        {

            Monitor.Enter(buffer);
            _writers_count++;
            while (_readers_count > 0 || _writers_working) 
            Monitor.Wait(buffer);

            _writers_working = true;



            buffer.Add((T)value);/////////// WRITE
            Console.WriteLine("write " + value);
          
            _writers_count--;
            _writers_working = false;
            
            Monitor.PulseAll(buffer);
            Monitor.Exit(buffer);



        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var thread_safely_buffer = new ThreadSafelyBuffer<int>(list);

            for (int i = 0; i < 5; i++)
            {             
                Thread thread = new Thread(new ParameterizedThreadStart(thread_safely_buffer.Write));
                thread.Name = $"Поток писатель  {i}";
                thread.Start(i);

                for (int j = 0; j < 5; j++)
                {
                    Thread threadj = new Thread(new ParameterizedThreadStart(thread_safely_buffer.Read));
                    threadj.Start(j);


                }

            }

            //for (int i = 0; i < 5; i++)
            //{
            //    Thread thread = new Thread(new ParameterizedThreadStart(thread_safely_buffer.Read));
            //    thread.Start(i);


            //}






            Console.ReadLine();
        }
    }
}
