using Dapr;
using Dapr.Client;
using JsonTransformer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTransformer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JsonTransformerController : ControllerBase
    {
        private DaprClient daprClient;
        private ILogger<JsonTransformerController> logger;
        private ServerOptions serverOptions;

        public JsonTransformerController(DaprClient daprClient, ServerOptions serverOptions, ILogger<JsonTransformerController> logger)
        {
            this.daprClient = daprClient;
            this.serverOptions = serverOptions;
            this.logger = logger;
        }

        [HttpGet]
        [Route("Version")]
        public async Task<IActionResult> GetVersion()
        {
            return await Task.FromResult<IActionResult>(Ok(new { Message = $"Service is running version ({serverOptions.AppVersion})" }));
        }

        [HttpPost]
        [Route("Process")]
        [Topic("gateway-servicebus", "d2c-messages")]
        public async Task<ActionResult> ProcessMessage(dynamic d2cMessage)
        {
            var d2cMessageString = d2cMessage.ToString();
            var isValidMessage = IsMessageValid(d2cMessageString);
            
            if (isValidMessage)
            {
                try
                {
                    JObject message = JObject.Parse(d2cMessageString);
                    var transformedRows = new List<TransformedRow>();
                    if (message != null)
                    {
                        var transformedRow = new TransformedRow();
                        transformedRow.From = message["from"]?.ToString();
                        transformedRow.To = message["to"]?.ToString();
                        transformedRow.PlaceId = message.SelectToken("structure.id")?.Value<string>();
                        transformedRow.PlaceName = message.SelectToken("structure.name")?.Value<string>();
                        transformedRow.PlaceType = message.SelectToken("structure.kpi_type")?.Value<string>();
                        var kpiStructures = message.SelectToken("structure.kpi_structure")?.ToArray();
                        var kpiValues = message.SelectToken("kpi_data.[0].values");
                        foreach (var kpiStructure in kpiStructures)
                        {
                            transformedRow.KpiParentId = kpiStructure["id"]?.Value<string>();
                            transformedRow.KpiParentName = kpiStructure["name"]?.Value<string>();
                            transformedRow.KpiParentType = kpiStructure["kpi_type"]?.Value<string>();
                            var kpiSubStructures = kpiStructure.SelectToken("kpi_structure");
                            if (kpiSubStructures == null)
                            {
                                transformedRows.Add(transformedRow);
                                continue;
                            }

                            foreach(var kpiSubStructure in kpiSubStructures?.ToArray())
                            {
                                transformedRow.KpiId = kpiSubStructure["id"]?.Value<string>();
                                transformedRow.KpiName = kpiSubStructure["name"]?.Value<string>();
                                transformedRow.KpiType = kpiSubStructure["kpi_type"]?.Value<string>();
                                transformedRow.KpiUnit = kpiSubStructure["unit"]?.Value<string>();
                                transformedRow.KpiDataType = kpiSubStructure["data_type"]?.Value<string>();
                                var kpiValue = kpiValues.SelectToken(transformedRow.KpiId);
                                var kpiFirst = kpiValue?.First?.First?.Value<string>();
                                transformedRow.KpiValue = kpiValue?.First?.First?.Value<string>();
                                transformedRows.Add(transformedRow);
                            }
                        }
                    }

                    return new ObjectResult(transformedRows);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Transformer ERROR: with error ({ex.Message}||{ex.StackTrace}");
                    throw;
                }
            }
            else
            {
                //invalid messages handling here
                logger.LogError($"Translator ERROR: Incorrect format");
                throw new ArgumentException("Message is not in correct format");
            }
        }

        private bool IsMessageValid(string message)
        {
            //Simple validation (needs to be updated to reflect real validation
            return !string.IsNullOrEmpty(message);
        }
    }
}
