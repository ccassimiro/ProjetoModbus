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


        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}