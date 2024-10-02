namespace PersistenceLayerCosmosDBLib
{
    public interface IDatabaseListener
    {
        void SaveChangesFailed(Exception ex);
    }
}
