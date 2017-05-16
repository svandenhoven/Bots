using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Newtonsoft.Json;
using BotSample.MessageRouting;

namespace ControllerSample
{

    public class PartyAzureTable : TableEntity
    {
        public PartyAzureTable(string BotId, string ConversationId)
        {
            this.PartitionKey = BotId;
            this.RowKey = ConversationId;
        }

        public PartyAzureTable() { }

        public string Party { get; set; }

        public string Experation { get; set; }
    }

    class PartyManager
    {
        public static List<Party> GetParties()
        {
            var parties = new List<Party>();

            var azureTable = CreateTable("StorageConnectionString", "Conversations");
            if (null != azureTable)
            {
                IEnumerable<PartyAzureTable> query = (from conversations in azureTable.CreateQuery<PartyAzureTable>()
                                                      select conversations);

                foreach (PartyAzureTable partyAT in query)
                {
                    var party = JsonConvert.DeserializeObject<Party>(partyAT.Party);
                    parties.Add(party);
                }
            }
            return parties;
        }

        private static CloudTable CreateTable(string connection, string tableName)
        {
            var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            return table.Exists() ? table : null;
        }
    }


}
