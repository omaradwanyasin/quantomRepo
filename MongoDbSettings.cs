namespace backend_api
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string NodesCollection { get; set; }
        public string AreaCollection { get; set; }
    }
}
