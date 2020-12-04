using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using ProjetoRedes.Services;

namespace ProjetoRedes
{
    public class Semaforo
    {
        public bool Ligado { get; set; }
        public Thread ThreadSemaforo { get; set; }
        public int[] RegistradoresInput { get; set; }
        public bool[] RegistradoresCoil { get; set; }
        public int[] RegistradoresHolding { get; set; }
        public int TempoLuzVermelha { get; set; }
        public int TempoLuzVerde { get; set; }
        public int TempoLuzAmarela { get; set; }
        public bool LuzVermelhaLigada { get; set; }
        public bool LuzVerdeLigada { get; set; }
        public bool LuzAmarelaLigada { get; set; }
        public int TempoRestanteLuzVermelha { get; set; }
        public int TempoRestanteLuzVerde { get; set; }
        public int TempoRestanteLuzAmarela { get; set; }

        ModbusService service = ModbusService.Instance;

        public Semaforo(int[] inputs)
        {
            ThreadSemaforo = new Thread(FuncionamentoSemaforo);
            this.RegistradoresInput = inputs;

            this.TempoLuzVermelha = inputs[1];
            this.TempoLuzAmarela = inputs[3];
            this.TempoLuzVerde = inputs[5];
        }

        public void FuncionamentoSemaforo()
        {
            while (Ligado)
            {
                this.RegistradoresCoil = service.ReadCoils();
                this.RegistradoresHolding = service.HoldingRegisters();
                this.RegistradoresInput = service.ReadInputRegisters();

                this.TempoLuzVermelha = RegistradoresInput[1];
                this.TempoLuzVerde = RegistradoresInput[3];
                this.TempoLuzAmarela = RegistradoresInput[5];

                this.Ligado = RegistradoresCoil[5];
                LuzVermelhaLigada = RegistradoresCoil[7];
                LuzAmarelaLigada = RegistradoresCoil[8];
                LuzVerdeLigada = RegistradoresCoil[6];

                TempoRestanteLuzVermelha = RegistradoresHolding[1] - RegistradoresHolding[2];
                TempoRestanteLuzVerde  = RegistradoresHolding[3] - RegistradoresHolding[4];
                TempoRestanteLuzAmarela = RegistradoresHolding[5] - RegistradoresHolding[6];
            }
        }
    }
}