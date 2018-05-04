using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    /// <summary>
    /// Provide the report data to dashboard and other public interfaces.
    /// GET: /api/custom/report/TargetStatusReport
    ///     get the target and task completion statistics report.
    ///     return: array of double, three elements:
    ///         0: Average Target Complete Progress.
    ///         1: Average task complete progress.
    ///  GET: /api/custom/report/YOYObjectUsageReport/{objectNameList}
    ///      get the YOY Object usage report.
    ///      objectNameList: object definition name collections, splited by ",".
    ///      return: array of int, 12 elements, from the month of last year to the current month, each of the value indicate the 
    ///      count of specific objects in that given month.
    /// </summary>
    [Authorize]
    public class ReportController : ApiController
    {
        IObjectService objectService = null;
        ILogger logger = LogManager.GetCurrentClassLogger(typeof(ReportController));
        public ReportController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        private int FindObjectDefinitionIdByName(string defName)
        {
            var objDefs = objectService.GetAllObjectDefinitions();
            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(defName, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

            return findObjDef.ObjectDefinitionID;
        }

        [HttpGet]
        [ResponseType(typeof(double[]))]
        public IHttpActionResult TargetStatusReport()
        {
            logger.Debug("Start TargetStatusReport");
            int targetDefId = FindObjectDefinitionIdByName("Target");
            var targetList = objectService.GetAllSerivceObjects(targetDefId, 
                new string[] { "targetStatus" },
                new ServiceRequestContext()
                {
                    IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                    RequestUser = RequestContext.Principal.Identity.Name,
                    UserSenstiveForSharedData = false
                });

            var targetPercentage = targetList.Count > 0 ? (from t in targetList select t)
                                                            .Average(s => 
                                                                    s.GetPropertyValue<PrimeObjectField>("targetStatus")
                                                                        .GetStrongTypeValue<int>()) 
                                                        : 0;

            //var targetCompletePercentage = targetList.Count > 0 ? (from t in targetList
            //                                                       select t).Average(s =>
            //                                                       s.GetPropertyValue<PrimeObjectField>("targetStatus")
            //                                                             .GetStrongTypeValue<int>() >= 100 ? 100 : 0) 
            //                                                    : 0;

            int taskDefId = FindObjectDefinitionIdByName("Task");
            var taskList = objectService.GetAllSerivceObjects(taskDefId, 
                        new string[] { "taskStatus" },
                        new ServiceRequestContext()
                        {
                            IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                            RequestUser = RequestContext.Principal.Identity.Name,
                            UserSenstiveForSharedData = false
                        });

            var taskPercentage = taskList.Count > 0 ? (from t in taskList select t)
                                                        .Average(s => s.GetPropertyValue<PrimeObjectField>("taskStatus")
                                                                        .GetStrongTypeValue<int>())
                                                    : 0;
            logger.Debug("taskPercentage : " + taskPercentage);
            logger.Debug("End TargetStatusReport");
            return this.Ok(new double[] { targetPercentage,
                taskPercentage });                                            
        }

        [HttpGet]
        [ResponseType(typeof(List<int[]>))]
        public IHttpActionResult YOYObjectUsageReport(string Id)
        {
            logger.Debug("Start YOYObjectUsageReport");
            List<int[]> seriesData = new List<int[]>();
            if (!string.IsNullOrEmpty(Id))
            {
                string[] objNames = Id.Split(new char[] { ',' });

                foreach (string objName in objNames)
                {
                    int[] data = GetYoYStatisticReportData(objName);
                    seriesData.Add(data);
                }
            }

            logger.Debug("End YOYObjectUsageReport");
            return this.Ok(seriesData);
        }

        private int[] GetYoYStatisticReportData(string objectName)
        {
            int objDefId = FindObjectDefinitionIdByName(objectName);
            var objList = objectService.GetAllSerivceObjects(objDefId, 
                null,
                new ServiceRequestContext()
                {
                    IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                    RequestUser = RequestContext.Principal.Identity.Name,
                    UserSenstiveForSharedData = false
                });

            DateTime dateOfLastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            //only take care of the data of recent years.
            var groupInfo = (from o in objList
                             where o.Created >= dateOfLastYear
                             group o.ObjectID by o.Created.ToString("MM-yy") into g
                             select new KeyValuePair<string, int>(g.Key, g.Count())).ToDictionary(ks => ks.Key);

            int[] data = new int[12];
            int idx = 0;
            for (; dateOfLastYear.Date <= DateTime.Now.Date; dateOfLastYear = dateOfLastYear.AddMonths(1))
            {
                var currentMonth = dateOfLastYear.ToString("MM-yy");
                data[idx++] = groupInfo != null && groupInfo.ContainsKey(currentMonth)
                                        ? groupInfo[currentMonth].Value : 0;
            }

            return data;
        }
    }
}
