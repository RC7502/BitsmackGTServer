//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using BitsmackGTAPI;
//using BitsmackGTServer.Models;
////using Fitbit.Api;

//namespace BitsmackGTServer
//{
//    class PedometerService : BaseService
//    {
//        public PedometerService()
//        {
//            PedometerRecords = new List<Pedometer>();
//        }

//        private List<Pedometer> PedometerRecords { get; set; }
//        private APIKeys FitbitAPIKey { get; set; }
//        private const string APIKeyName = "api/Pedometer";

//        public async Task<bool> Update()
//        {
//            log.Debug("Starting Pedometer.Update");

//            try
//            {
//                await GetPedometerRecords();
//                if (FitbitAPIKey != null)
//                {
//                    await FillBlanks();
//                    await RefreshRecentSteps();
//                    await IntegrityCheck();
//                    await UpdateKey();
//                }
//            }
//            catch (Exception ex)
//            {
//                log.Error(ex.Message);
//                return false;
//            }

//            log.Debug(string.Format("Completed Pedometer.Update, {0} records in DB.", PedometerRecords.Count));
//            return true;
//        }

//        private async Task UpdateKey()
//        {
//            FitbitAPIKey.last_update = DateTime.UtcNow;
//            await WebAPIClient.Put("api/APIKeys", FitbitAPIKey.id, FitbitAPIKey);
//        }

//        private async Task IntegrityCheck()
//        {
//            var timePeriod = DateTime.Now.Date.AddMonths(-12);

//            var startDate = PedometerRecords.Where(x => x.lastupdateddate == null || x.lastupdateddate < timePeriod)
//                                            .OrderBy(x => x.lastupdateddate).FirstOrDefault();
//            if (startDate != null)
//            {
//                var newRecords = GetFitbitRecords(startDate.trandate, startDate.trandate.AddDays(10));
//                await UpdateOldRecords(newRecords);
//            }
//        }

//        private async Task UpdateOldRecords(IEnumerable<Pedometer> newRecords)
//        {
//            foreach (var newRecord in newRecords)
//            {
//                var oldRecord = PedometerRecords.FirstOrDefault(x => x.trandate == newRecord.trandate);
//                if (oldRecord != null)
//                {
//                    oldRecord.Copy(newRecord);
//                    await WebAPIClient.Put(APIKeyName, oldRecord.id, oldRecord);
//                }
//            }
//        }

//        private async Task RefreshRecentSteps()
//        {
//            var newRecords = GetFitbitRecords(DateTime.Now.Date.AddDays(-9), DateTime.Now.Date);
//            await UpdateOldRecords(newRecords);
//        }

//        private async Task FillBlanks()
//        {
//            try
//            {
//                for (var tDate = FitbitAPIKey.start_date; tDate <= DateTime.Now.Date; tDate = tDate.AddDays(1))
//                {
//                    if (PedometerRecords.All(x => x.trandate != tDate))
//                    {
//                        var newrec = new Pedometer
//                        {
//                            bodyfat = 0,
//                            createddate = DateTime.UtcNow,
//                            weight = 0,
//                            sleep = 0,
//                            steps = 0,
//                            calconsumed = null,
//                            id = 0,
//                            lastupdateddate = null,
//                            trandate = tDate
//                        };
//                        PedometerRecords.Add(newrec);
//                        await WebAPIClient.Post(APIKeyName, newrec);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                log.Error(string.Format("Error during FillBlanks: {0}", ex.Message));
//            }
//        }

//        private async Task GetPedometerRecords()
//        {
//            PedometerRecords = await WebAPIClient.Get<List<Pedometer>>(APIKeyName);
//            var keys = await WebAPIClient.Get<List<APIKeys>>("api/APIKeys");
//            FitbitAPIKey = keys.FirstOrDefault(x => x.service_name == "Fitbit");
//            if (FitbitAPIKey == null)
//            {
//                log.Error("Fitbit API key not found.");
//            }
//        }

//        private IEnumerable<Pedometer> GetFitbitRecords(DateTime startDate, DateTime endDate)
//        {
//            var list = new List<Pedometer>();

//            using (var fbClient = GetFitbitClient())
//            {
//                var weightlog = fbClient.GetWeight(startDate, endDate);
//                for (var d = startDate; d <= endDate; d = d.AddDays(1))
//                {
//                    var activity = fbClient.GetDayActivitySummary(d);
//                    var sleep = fbClient.GetSleep(d);
//                    var calLog = fbClient.GetFood(d);
//                    var newrec = new Pedometer
//                    {
//                        trandate = d,
//                        steps = activity.Steps,
//                        sleep = sleep.Summary.TotalMinutesAsleep,
//                        createddate = DateTime.UtcNow,
//                        calconsumed = (int?)calLog.Summary.Calories
//                    };
//                    var dayWeight = weightlog.Weights.FirstOrDefault(x => x.Date == d);
//                    newrec.weight = dayWeight != null ? dayWeight.Weight * 2.2046226 : 0;

//                    list.Add(newrec);
//                }
//            }

//            return list;
//        }

//        public IFitbitClient GetFitbitClient()
//        {
//            return new FitbitClient(FitbitAPIKey.consumer_key, FitbitAPIKey.consumer_secret, FitbitAPIKey.user_token, FitbitAPIKey.user_secret);

//        }
//    }
//}
