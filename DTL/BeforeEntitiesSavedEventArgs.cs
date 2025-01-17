namespace EcfDotnet.DTL
{
    public class BeforeEntitiesSavedEventArgs<T> : EventArgs
    {
        public List<T> Entities { get; }

        public BeforeEntitiesSavedEventArgs(List<T> entities)
        {
            Entities = entities;
        }
    }
}