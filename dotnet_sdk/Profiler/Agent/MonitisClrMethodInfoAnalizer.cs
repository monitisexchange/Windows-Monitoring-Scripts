using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Monitis;
using Newtonsoft.Json.Linq;
using log4net;

namespace AgentCore
{
    public class MonitisClrMethodInfoAnalizer : IInfoAnalizer
    {
        private readonly CustomMonitor _monitor;
        private readonly CustomUserAgent _agent;
        private DateTime? _lastAnalizeTime;
        private Dictionary<ClrMethodInfo, int> _entryCount = new Dictionary<ClrMethodInfo, int>();
        private ILog _log = LogManager.GetLogger(typeof(MonitisClrMethodInfoAnalizer));

        public MonitisClrMethodInfoAnalizer(Authentication authentification)
        {
            _monitor = new CustomMonitor();
            _monitor.SetAuthenticationParams(authentification);
            _agent = new CustomUserAgent();
            _agent.SetAuthenticationParams(authentification);
        }

        public void Analize(IEnumerable<ClrMethodOperation> operations, int timeSinceLastAnalizingInMs)
        {
            try
            {
                _log.Info("Start Analize: "+ operations.Count());
                var accum = AccumulateData(operations, timeSinceLastAnalizingInMs);
                _log.Info("Sending Info");
                SendingInfo(accum);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        private Dictionary<ClrMethodInfo, int> AccumulateData(IEnumerable<ClrMethodOperation> operations, int timeSinceLastAnalizingInMs)
        {
            var lastAnalizeTime = DateTime.Now;
            var _accum = new Dictionary<ClrMethodInfo, int>();
            foreach (var clrMethodOperation in operations.GroupBy(u => u.MethodInfo))
            {
                var method = clrMethodOperation.Key;
                double accumTransactionInMs = 0;

                var sorted = clrMethodOperation.OrderBy(u => u.Time).ToList();
                /*if (sorted.First().Action == MethodAction.End && _lastAnalizeTime != null)
                {
                    --_entryCount[method];
                    if (_entryCount[method] == 0)
                        _entryCount.Remove(method);
                    accumTransactionInMs += (sorted.FirstOrDefault().Time - _lastAnalizeTime).Value.TotalMilliseconds;
                }
*/

                for (int i = 0; i < sorted.Count(); i++)
                {
                    if (sorted[i].Action == MethodAction.End)
                    {
                        --_entryCount[method];
                        if (_entryCount[method] == 0)
                            _entryCount.Remove(method);
                        accumTransactionInMs += (sorted[i].Time - ((i == 0) ? (_lastAnalizeTime ?? sorted[i].Time) : sorted[i - 1].Time)).TotalMilliseconds;
                    }
                    else if (sorted[i].Action == MethodAction.Start)
                    {
                        if (!_entryCount.ContainsKey(method))
                            _entryCount.Add(method, 0);
                        ++_entryCount[method];
                        if (_entryCount[method] != 1)
                            accumTransactionInMs += (sorted[i].Time - ((i == 0)?(_lastAnalizeTime ?? sorted[i].Time) : sorted[i - 1].Time)).TotalMilliseconds;
                    }
                }

                if (sorted.Last().Action == MethodAction.Start && _lastAnalizeTime != null)
                {
                    accumTransactionInMs += (_lastAnalizeTime + new TimeSpan(0, 0, 0, 0, timeSinceLastAnalizingInMs) - sorted.Last().Time).
                            Value.TotalMilliseconds;
                }

                if (sorted.Last().Action == MethodAction.End && _lastAnalizeTime != null)
                {
                    if (_entryCount.ContainsKey(method) && _entryCount[method] > 0)
                    {
                        accumTransactionInMs += (_lastAnalizeTime + new TimeSpan(0, 0, 0, 0, timeSinceLastAnalizingInMs) - sorted.Last().Time).
                                Value.TotalMilliseconds;
                    }
                }

                _accum[method] = (int)accumTransactionInMs;
            }
            _lastAnalizeTime = lastAnalizeTime;
            return _accum;
        }

        private void SendingInfo(Dictionary<ClrMethodInfo, int> accum)
        {
            foreach (var i in accum)
            {
                try
                {
                    var monitorID = CreateMonitorIfNotExist(i.Key);
                    var param = new MonResult("Execution Time", i.Value.ToString(CultureInfo.InvariantCulture));
                    _log.Info(String.Format("Adding info to monitor {0}, value {1}", monitorID, i.Value));
                    _monitor.AddResult(monitorID, Helper.GetCurrentTime(), new List<MonResult>() { param });
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }

        private int CreateMonitorIfNotExist(ClrMethodInfo info)
        {
            var name = GetMonitorName(info);

            var monitors = _monitor.GetMonitors(OutputType.XML);
            var monitorsElement = System.Xml.Linq.XElement.Parse(monitors.Content);
            var monitorss = monitorsElement.Descendants("monitor").Select(u => new { ID = u.Elements("id").FirstOrDefault(), Name = u.Elements("name").FirstOrDefault(), Monitor = u });
            var monitor = monitorss.FirstOrDefault(u => u.Name != null && u.Name.Value == name);
            if (monitor == null)
            {
                var agents = _agent.GetAgents("profiler", true, true, OutputType.XML);
                var agentsElement = System.Xml.Linq.XElement.Parse(agents.Content);

                var agent = agentsElement.Descendants("agent").Select(u => new { ID = u.Elements("id").FirstOrDefault(), Name = u.Elements("name").FirstOrDefault(), Agent = u }).FirstOrDefault(u => u.Name != null && u.ID != null && u.Name.Value == "MonitisProfilerUserAgent");
                int agentID;
                if (agent == null)
                {
                    var result = _agent.AddAgent("MonitisProfilerUserAgent", "profiler", new JObject(), 100000, OutputType.JSON);
                    var res = JObject.Parse(result.Content);
                    if (res["status"].Value<string>() != "ok")
                        throw new InvalidOperationException("Cannot create agent!: " + res["status"]);
                    agentID = int.Parse(res["data"].Value<string>());
                }
                else
                {
                    agentID = int.Parse(agent.ID.Value);
                }

                var resParam = new MonResultParameter("Execution Time", "Execution Time", "Execution Time", DataType.Float);

                var param = new MonitorParameter("param1", "param1d", "val", DataType.String, false);

                var resAddParam = new MonResultParameter("MonAddResparam1", "MonAddResparam1d",
                                                                        "MonAddResval", DataType.String);
                _log.InfoFormat("Creating monitor {0}", name);
                var resMon = _monitor.AddMonitor(agentID, name, "profiler", "profiler",
                                          new List<MonitorParameter>() { param },
                                          new List<MonResultParameter>() { resParam },
                                          new List<MonResultParameter>() { resAddParam });

                var resJson = JObject.Parse(resMon.Content);
                if (resJson["status"].Value<string>() != "ok")
                    throw new InvalidOperationException("Cannot create custom monitor!: " + resJson["status"]);
                return int.Parse(resJson["data"].Value<string>());
            }
            else
            {
                _log.InfoFormat("Monitor exist {0}", name);
                return int.Parse(monitor.ID.Value);
            }
        }

        private string GetMonitorName(ClrMethodInfo info)
        {
            return info.UnitOfWork + "-" + info.MethodName;
        }
    }
}
