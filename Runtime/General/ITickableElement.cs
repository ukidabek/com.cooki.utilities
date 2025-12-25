namespace Utilities.General
{
    public interface ITickableElement
    {
        void Tick(float deltaTime, float timeScale);
    }
}