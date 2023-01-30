namespace Nike.Framework.Domain.EventSourcing
{
    public interface ISnapshot
    {
    }

    public interface IHasSnapshot
    {
    }

    public interface IHasSnapshot<TSnapshotModel> : IHasSnapshot where TSnapshotModel : class, ISnapshot
    {
        TSnapshotModel GetSnapshot();
        void SetSnapshot(TSnapshotModel snapshotModel);
    }
}