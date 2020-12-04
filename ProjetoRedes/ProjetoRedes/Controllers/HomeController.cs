using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyModbus;
using ProjetoRedes.Services;

namespace ProjetoRedes.Controllers
{
    public class HomeController : Controller
    {
        public ModbusService service { get; set; }

        public HomeController()
        {
            service = ModbusService.Instance;
        }

        public ActionResult Index()
        {
            Semaforo semaforo = null;

            if(service.Semaforo != null)
            {
                semaforo = service.EstadoSemaforo();
                ViewBag.Ligado = semaforo.Ligado;

                ViewBag.TempoLuzVermelha = semaforo.TempoLuzVermelha;
                ViewBag.TempoLuzAmarela = semaforo.TempoLuzVerde;
                ViewBag.TempoLuzVerde = semaforo.TempoLuzAmarela;
                

                ViewBag.LuzVermelhaLigada = semaforo.LuzVermelhaLigada;
                ViewBag.LuzVerdeLigada = semaforo.LuzVerdeLigada;
                ViewBag.LuzAmarelaLigada = semaforo.LuzAmarelaLigada;

                ViewBag.TempoRestanteLuzVermelha = semaforo.TempoRestanteLuzVermelha;
                ViewBag.TempoRestanteLuzVerde = semaforo.TempoRestanteLuzVerde;
                ViewBag.TempoRestanteLuzAmarela = semaforo.TempoRestanteLuzAmarela;
            }
            
            return View();
        }

        public void Ligar()
        {
            var completed = service.CriarConexao();
            if (completed)
            {
                service.Iniciar();
            }
        }

        public void Desligar()
        {
            service.Desconectar();
            service.Semaforo.Ligado = false;
        }
        [HttpPost]
        public JsonResult AlterarTempoSemaforo(int? verde, int? amarelo, int? vermelho)
        {
            var result = new JsonResult();
            try
            {
                service.AlterarTempoSemaforo(verde.Value, amarelo.Value, vermelho.Value);
                result.Data = new { Success = true, Message = "Dados Alterados" };
            } catch (Exception e)
            {
                result.Data = new { Success = false, Message = "Ocorreu um erro: " +e };
            }

            return result;
        }
    }
}