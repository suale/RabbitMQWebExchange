using RabbitMQWebExchange.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RabbitMQWebExchange.Controllers
{
    public class QueueReceiverController : Controller
    {

        private static Consumer _consumer;
        
        public ActionResult ListClients()
        {
          
            Consumer.sayac++;

            if (Consumer.sayac == 1)
            {
                Thread t1 = new Thread(() =>
                {
                    _consumer = new Consumer();
                });
                t1.Start();
            }
           
          
            return View();
        }

        public JsonResult GetClientList()
        {            
            var clientNolar = Consumer.rabbitMessagesClientList
                              .GroupBy(o => new { o.ClientGUID })
                              .Select(o => o.LastOrDefault());
            var clientNos = clientNolar.ToList();

            return Json(clientNos, JsonRequestBehavior.AllowGet);
        }

    }
}