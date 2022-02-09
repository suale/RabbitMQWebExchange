using RabbitMQWebExchange.ServiceModels;
using RabbitMQWebExchange.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RabbitMQWebExchange.Controllers
{
    public class ExClientChartController : Controller
    {
        
        private static List<TabState> TabStateList= new List<TabState>();

        [Route("ExClientChart/ClientChartAct/{GuidInCome}")]
        public ActionResult ClientChartAct(string GuidInCome)
        {
            ViewBag.value = GuidInCome;
            return View();
        }
        
        public JsonResult GetResult(string GuidInCome, string GuidTab)
        {

            List<RabbitMessage> chartMessage = new List<RabbitMessage>();

            TabState tabState = new TabState();

            tabState.ClientGuid = GuidInCome;
            tabState.TabGuid = GuidTab;

            bool tabExist = false;

            for (int i = 0; i < TabStateList.Count(); i++)
            {
                if (tabState.ClientGuid == TabStateList[i].ClientGuid && tabState.TabGuid == TabStateList[i].TabGuid)
                {
                    tabExist = true;
                    tabState = TabStateList[i];
                }
            }


            //foreach (var item in TabStateList)
            //{
            //    if (tabState.ClientGuid == item.ClientGuid && tabState.TabGuid == item.TabGuid)
            //    {
            //        tabExist = true;

            //    }
            //}

            if (!tabExist)
            {

                for (int i = 0; i < Consumer.rabbitMessages.Count; i++)
                {
                    if (Consumer.rabbitMessages[i].ClientGUID == GuidInCome)
                    {
                        chartMessage.Add(Consumer.rabbitMessages[i]);
                        tabState.LastDataNumber = Consumer.rabbitMessages[i].DataNumber;
                        Consumer.rabbitMessages.RemoveAt(i);
                    }

                }
                for (int i = 0; i < Consumer.rabbitMessages.Count; i++)
                {
                    if (Consumer.rabbitMessages[i].DataNumber < tabState.LastDataNumber)
                    {
                        Consumer.rabbitMessages.RemoveAt(i);
                    }
                }

                tabState.LastChartList = chartMessage.ToList(); ;
                TabStateList.Add(tabState);

            }
            else
            {
                foreach (var item in TabStateList)
                {
                    if (tabState.ClientGuid == item.ClientGuid && item.LastDataNumber > tabState.LastDataNumber)
                    {

                        tabState.LastChartList = item.LastChartList;
                        tabState.LastDataNumber = item.LastDataNumber;

                    }
                    else if(tabState.ClientGuid == item.ClientGuid && item.LastDataNumber <= tabState.LastDataNumber)
                    {
                        for (int i = 0; i < Consumer.rabbitMessages.Count; i++)
                        {
                            if (Consumer.rabbitMessages[i].ClientGUID == GuidInCome)
                            {
                                chartMessage.Add(Consumer.rabbitMessages[i]);
                                tabState.LastDataNumber = Consumer.rabbitMessages[i].DataNumber;
                                Consumer.rabbitMessages.RemoveAt(i);
                            }

                        }
                        for (int i = 0; i < Consumer.rabbitMessages.Count; i++)
                        {
                            if (Consumer.rabbitMessages[i].DataNumber < tabState.LastDataNumber)
                            {
                                Consumer.rabbitMessages.RemoveAt(i);
                            }
                        }

                        tabState.LastChartList = chartMessage.ToList();
                    }
                }
            }







            //int lastDataNumber = 0;
            //for (int i = 0; i < Consumer.rabbitMessages.Count; i++)
            //{
            //    if (Consumer.rabbitMessages[i].ClientGUID == GuidInCome)
            //    {
            //        chartMessage.Add(Consumer.rabbitMessages[i]);
            //        lastDataNumber = Consumer.rabbitMessages[i].DataNumber;
            //        Consumer.rabbitMessages.RemoveAt(i);
            //    }

            //}
            //for (int i = 0; i < Consumer.rabbitMessages.Count; i++)
            //{
            //    if (Consumer.rabbitMessages[i].DataNumber < lastDataNumber)
            //    {
            //        Consumer.rabbitMessages.RemoveAt(i);
            //    }

            //}





            return Json(tabState.LastChartList, JsonRequestBehavior.AllowGet);
        }
    }
}