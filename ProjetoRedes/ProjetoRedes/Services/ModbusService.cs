using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyModbus;

namespace ProjetoRedes.Services
{
    public sealed class ModbusService
    {
        public int Port { get; set; }
        public string Ip { get; set; }
        public int[] Inputs { get; set; }
        public int[] Holding { get; set; }
        public bool[] Coils { get; set; }
        public ModbusClient Client { get; set; }
        public Semaforo Semaforo { get; set; }
        public int init { get; set; }
        public int end { get; set; }

        private static ModbusService instance = null;
        private static readonly object locker = new Object();


        public static ModbusService Instance
        {
            get
            {
                lock (locker)
                {
                    if(instance == null)
                    {
                        instance = new ModbusService();
                    }
                    return instance;
                }
            }
        }

        ModbusService()
        {
            string ip = "127.0.0.1";
            int port = 502;
            this.init = 0;
            this.end = 24;

            Client = new ModbusClient(ip, port);

        }

        public bool CriarConexao()
        {
            try
            {
                this.Client.Connect();
                //this.Inputs = ReadInputRegisters();
                //this.Inputs[0] = 1;
                this.Holding = HoldingRegisters();
                this.Holding[0] = 1;
                
                this.Coils = ReadCoils();
                this.Coils[4] = true;
                WriteCoils(this.Coils);
                WriteRegisters(this.Holding);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Desconectar()
        {
            this.Semaforo.ThreadSemaforo.Interrupt();
            this.Semaforo.Ligado = false;
            this.Coils = ReadCoils();
            this.Coils[4] = false;
            WriteCoils(this.Coils);
            this.Holding = HoldingRegisters();
            this.Holding[0] = 0;
            WriteRegisters(this.Holding);

        }

        public void AlterarTempoSemaforo(int verde, int amarelo, int vermelho)
        {
            if (Semaforo != null && Semaforo.ThreadSemaforo.IsAlive)
            {
                Semaforo.ThreadSemaforo.Interrupt();
                this.Desconectar();
            }
            this.Client.WriteSingleRegister(1, vermelho);
            this.Client.WriteSingleRegister(3, verde);
            this.Client.WriteSingleRegister(5, amarelo);
            if (!Semaforo.ThreadSemaforo.IsAlive)
            {
                Semaforo.ThreadSemaforo.Start();
            }
        }


        public int[] ReadInputRegisters()
        {
            return this.Client.ReadInputRegisters(this.init, this.end);
        }

        public bool[] ReadCoils()
        {
            return this.Client.ReadCoils(this.init, this.end);
        }

        public void WriteCoils(bool[] coils)
        {
            this.Client.WriteMultipleCoils(this.init, coils);
        }

        public void WriteRegisters(int[] inputs)
        {
            this.Client.WriteMultipleRegisters(this.init, inputs);
        }

        public int[] HoldingRegisters()
        {
            return this.Client.ReadHoldingRegisters(this.init, this.end);
        }

        public void Iniciar()
        {
            var inputs = ReadInputRegisters();
            Semaforo = new Semaforo(inputs);
            Semaforo.Ligado = true;
            Semaforo.ThreadSemaforo.Start();
        }

        public Semaforo EstadoSemaforo()
        {
            return this.Semaforo;
        }


    }
}