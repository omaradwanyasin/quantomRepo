using backend_api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend_api.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Node> _nodesCollection;
        private readonly IMongoCollection<Area> _areaCollection;
        private readonly IHubContext<UpdateHub> _hubContext;

        // Static dictionary to store nodes mapped by AreaId
        public static Dictionary<int, Node> nodesMap = new Dictionary<int, Node>();

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings, IMongoClient mongoClient, IHubContext<UpdateHub> hubContext)
        {
            var database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _nodesCollection = database.GetCollection<Node>(mongoDbSettings.Value.NodesCollection);
            _areaCollection = database.GetCollection<Area>(mongoDbSettings.Value.AreaCollection);
            _hubContext = hubContext;
        }

        // Function to get nodes by AreaId
        public async Task<List<Node>> GetNodesByAreaAsync(int areaId)
        {
            var filter = Builders<Node>.Filter.Eq(n => n.AreaId, areaId);
            return await _nodesCollection.Find(filter).ToListAsync();
        }

        // Function to check AreaStatus and fetch nodes for each active area
        public async Task CheckAreaStatusesAsync()
        {
            
            try
            {
                nodesMap.Clear();
                List<int> areaIds = new List<int> { 101, 102, 103, 104, 105, 106, 107, 108 };
                foreach (var areaId in areaIds)
                {
                    var filter = Builders<Area>.Filter.Eq(a => a.AreaId, areaId);
                    var area = await _areaCollection.Find(filter).FirstOrDefaultAsync();

                    if (area == null)
                    {
                        Console.WriteLine($"⚠️ No area found with AreaId = {areaId}");
                    }
                    else
                    {
                        if (area.AreaStatus)
                        {
                            Console.WriteLine($"✅ Area {area.AreaName} (ID: {area.AreaId}) is ACTIVE with Status: {area.AreaStatus}");
                            var nodes = await GetNodesByAreaAsync(area.AreaId);
                            foreach (var node in nodes)
                            {
                                nodesMap[node.NodeId] = node;
                            }
                            Console.WriteLine($"Stored {nodes.Count} nodes for Area {area.AreaId}");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Area {area.AreaName} (ID: {area.AreaId}) is INACTIVE with Status: {area.AreaStatus}");
                        }
                    }
                }
                Console.WriteLine("'''''''''''''''''''''''''");
                Console.WriteLine(nodesMap.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in CheckAreaStatusesAsync: {ex.Message}");
            }

        }
        //any change to this will call console and pritn 
        public async Task WatchCollection(CancellationToken cancellationToken)
        {
            using var cursor = await _areaCollection.WatchAsync(cancellationToken: cancellationToken);

            while (await cursor.MoveNextAsync(cancellationToken))
            {
                foreach (var change in cursor.Current)
                {
                    Console.WriteLine("🔄 Change detected in Nodes Collection: " + change.FullDocument);

                    // Notify Frontend via SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", change.FullDocument);
                }
            }
        }


    }
}
